using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using robotManager.Helpful;
using robotManager.Products;
using wManager;
using wManager.Events;
using wManager.Plugin;
using wManager.Wow;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

// Token: 0x02000002 RID: 2
public class Main : IPlugin
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000001 RID: 1
	// (set) Token: 0x06000002 RID: 2
	public static bool _copySettings { get; set; }

	// Token: 0x06000003 RID: 3
	public void Initialize()
	{
		Logging.Write("[tbcFlightMaster]: Flight Master initialized - " + this.version);
		Main._isLaunched = true;
		Main.inProcessing = false;
		Main._copySettings = true;
		Main._runScan = true;
		Main._updateNodes = false;
		Main.cancelCheckPathThread = false;
		Main.ingameSettings();
		this.watchForEvents();
		FNVFlightMasterSettings.Load();
		Main.applyDefaultNodes();
		MovementEvents.OnMovementPulse += Main.MovementEventsOnOnMovementPulse;
		MovementEvents.OnSeemStuck += Main.MovementEventsOnOnSeemStuck;
		this.scanNearbyTaxi.Start();
		this.flightMasterLoop();
	}

	// Token: 0x06000004 RID: 4
	public void Dispose()
	{
		Main._runScan = false;
		Main.cancelCheckPathThread = true;
		Main._isLaunched = false;
		Main._updateNodes = false;
		MovementEvents.OnMovementPulse -= Main.MovementEventsOnOnMovementPulse;
		MovementEvents.OnSeemStuck -= Main.MovementEventsOnOnSeemStuck;
		FNVFlightMasterSettings.CurrentSettings.Save();
		Logging.Write("[tbcFlightMaster]: Flight Master disposed");
	}

	// Token: 0x06000005 RID: 5
	public void Settings()
	{
		FNVFlightMasterSettings.Load();
		FNVFlightMasterSettings.CurrentSettings.ToForm();
		FNVFlightMasterSettings.CurrentSettings.Save();
	}

	// Token: 0x06000006 RID: 6
	public static void ingameSettings()
	{
		if (wManagerSetting.CurrentSetting.FlightMasterTaxiUse)
		{
			Logging.Write("[tbcFlightMaster]: WRobots Taxi is enabled, going to disable it...");
			wManagerSetting.CurrentSetting.FlightMasterTaxiUse = false;
		}
	}

	// Token: 0x06000007 RID: 7
	public static void applyDefaultNodes()
	{
		if (ObjectManager.Me.PlayerRace == PlayerFactions.Orc || ObjectManager.Me.PlayerRace == PlayerFactions.Troll)
		{
			FNVFlightMasterSettings.CurrentSettings.Orgrimmar = true;
		}
		if (ObjectManager.Me.PlayerRace == PlayerFactions.Tauren)
		{
			FNVFlightMasterSettings.CurrentSettings.Mulgore = true;
		}
		if (ObjectManager.Me.PlayerRace == PlayerFactions.Undead)
		{
			FNVFlightMasterSettings.CurrentSettings.Undercity = true;
		}
	}

	// Token: 0x06000008 RID: 8
	private void flightMasterLoop()
	{
		while (Products.IsStarted && Main._isLaunched)
		{
			if (Products.InPause || !Main._takenTaxi)
			{
				if (!Main._timer)
				{
					goto IL_83;
				}
			}
			while (ObjectManager.Me.IsOnTaxi)
			{
				Thread.Sleep(1000);
			}
			int num = FNVFlightMasterSettings.CurrentSettings.pauseTaxiTime;
			while (num > 0 && Main._timer)
			{
				Thread.Sleep(1000);
				num -= 1000;
			}
			if (!this.scanNearbyTaxi.IsAlive)
			{
				Logging.Write("Taxi scan not running, restarting...");
				this.scanNearbyTaxi.Start();
			}
			Main.resetTaxi();
			IL_83:
			Thread.Sleep(5000);
		}
		this.Dispose();
	}

	// Token: 0x06000009 RID: 9
	private static void resetTaxi()
	{
		while (ObjectManager.Me.IsOnTaxi)
		{
			Thread.Sleep(5000);
		}
		Thread.Sleep(Usefuls.Latency * 3 + 1500);
		Logging.Write("[tbcFlightMaster]: Reset taxi");
		Main._takenTaxi = false;
		Main.from = null;
		Main.to = null;
		Main._timer = false;
		Main.checkPath = true;
		Main.checkPathActive = false;
		Main.checkPathActiveFM = null;
	}

	// Token: 0x0600000A RID: 10
	private void watchForEvents()
	{
		EventsLua.AttachEventLua(LuaEventsId.TAXIMAP_OPENED, delegate(object m)
		{
			Main.g__CHAT_MSG_PET_INFO();
		});
	}

	// Token: 0x0600000B RID: 11
	private static void MovementEventsOnOnSeemStuck()
	{
		Vector3 vector = new Vector3(-6033.529f, -2490.157f, 310.9456f, "None");
		if ((Usefuls.MapZoneName.Contains("Loch Modan") || Usefuls.MapZoneName.Contains("Searing Gorge")) && ObjectManager.Me.Position.DistanceTo2D(vector) < 50f && FNVFlightMasterSettings.CurrentSettings.pauseSearingGorge)
		{
			Main.stuckCounter++;
			if (Main.stuckCounter >= 5)
			{
				Logging.Write("[tbcFlightMaster]: Repeated stucks detected at the locked gate between Loch Modan and Searing Gorge. Going to stop bot, to prevent getting caught");
				Main.stuckCounter = 0;
				Products.ProductStop();
			}
		}
		else
		{
			Main.stuckCounter = 0;
		}
		if (Main._timer || Main._takenTaxi)
		{
			Logging.Write("[tbcFlightMaster]: SeemStuck detected, reset taxi to help solving it");
			Main.resetTaxi();
		}
	}

	// Token: 0x0600000C RID: 12
	private static void MovementEventsOnOnMovementPulse(List<Vector3> points, CancelEventArgs cancelable)
	{
		Main.statusDiscover = Logging.Status;
		if (Main._taxiToDiscover && !Main.discoverTaxiNode.Equals(null) && !Main._discoverInProessing && !Main._updateNodes && !Main.statusDiscover.Contains("Boat") && !Main.statusDiscover.Contains("Ship"))
		{
			Main._discoverInProessing = true;
			Thread.Sleep(Usefuls.Latency + 500);
			cancelable.Cancel = true;
			Main.checkPathActive = true;
			Main.checkPathActiveFM = Main.discoverTaxiNode;
			Main.discoverTaxi(Main.discoverTaxiNode);
			Thread.Sleep(Usefuls.Latency * 3);
			cancelable.Cancel = false;
			Main.checkPathActive = false;
		}
		if (Main.changer && !Main._updateNodes && !Main.inProcessing && ObjectManager.Me.IsAlive)
		{
			Main.changer = false;
			if (!Main._taxiToDiscover && !Main._timer && !Main._takenTaxi && ObjectManager.Me.Position.DistanceTo(points.Last<Vector3>()) > (float)FNVFlightMasterSettings.CurrentSettings.taxiTriggerDistance)
			{
				Main.status = Logging.Status;
				if (FNVFlightMasterSettings.CurrentSettings.skipIfFollowPath && Main.status.Contains("Follow Path") && !Main.status.Contains("Resurrect") && Main.calculateRealDistance(ObjectManager.Me.Position, points.Last<Vector3>()) < FNVFlightMasterSettings.CurrentSettings.skipIfFollowPathDistance)
				{
					Logging.Write(string.Concat(new object[]
					{
						"[tbcFlightMaster]: Currently following path or distance to start (",
						Main.calculateRealDistance(ObjectManager.Me.Position, points.Last<Vector3>()),
						" yards) is smaller than setting value (",
						FNVFlightMasterSettings.CurrentSettings.skipIfFollowPathDistance,
						" yards)"
					}));
					Thread.Sleep(1000);
					cancelable.Cancel = false;
					Main.inProcessing = false;
					Main.checkPathActive = true;
					Main.changer = true;
					Main._timer = true;
					return;
				}
				Main.destinationVector = points.Last<Vector3>();
				Main.saveDistance = Main.calculateRealDistance(ObjectManager.Me.Position, points.Last<Vector3>());
				Thread.Sleep(Usefuls.Latency + 500);
				cancelable.Cancel = true;
				if (!Main.inProcessing)
				{
					Main.from = Main.getClosestFlightMasterFrom();
					Main.to = Main.getClosestFlightMasterTo();
				}
				Thread.Sleep(1000);
				if (!Main.from.name.Contains(Main.to.name) && !Main.to.name.Contains("null") && !Main.to.name.Contains("FlightMaster") && !Main.from.name.Contains("null") && !Main.from.Equals(Main.to) && Main.calculateRealDistance(ObjectManager.Me.Position, Main.from.position) + Main.calculateRealDistance(Main.to.position, Main.destinationVector) + (float)FNVFlightMasterSettings.CurrentSettings.shorterMinDistance <= Main.saveDistance)
				{
					Logging.Write("[tbcFlightMaster]: Shorter path detected, taking Taxi from " + Main.from.name + " to " + Main.to.name);
					Main.inProcessing = true;
					Main.checkPathActive = true;
					Main.checkPathActiveFM = Main.from;
					Main.takeTaxi(Main.from, Main.to);
					Thread.Sleep(1000);
					cancelable.Cancel = false;
					Main.inProcessing = false;
					Main.checkPathActive = true;
				}
				else
				{
					Logging.Write("[tbcFlightMaster]: No shorter path available, skip flying");
					cancelable.Cancel = false;
					Main._timer = true;
					Main.inProcessing = false;
				}
			}
			Main.changer = true;
		}
	}

	// Token: 0x0600000D RID: 13
	public static bool inCombat()
	{
		return Lua.LuaDoString<bool>("return UnitAffectingCombat('player');", "");
	}

	// Token: 0x0600000E RID: 14
	public static bool inCombatPet()
	{
		return Lua.LuaDoString<bool>("return UnitAffectingCombat('pet');", "");
	}

	// Token: 0x0600000F RID: 15
	private static async void Reenable()
	{
		await Task.Run(delegate()
		{
			Products.InPause = true;
			if (ObjectManager.Me.WowClass == WoWClass.Hunter)
			{
				Lua.LuaDoString("RotaOn = false", false);
			}
			MovementManager.StopMove();
			MovementManager.CurrentPath.Clear();
			MovementManager.CurrentPathOrigine.Clear();
			Thread.Sleep(5000);
			Products.InPause = false;
			if (ObjectManager.Me.WowClass == WoWClass.Hunter)
			{
				Lua.LuaDoString("RotaOn = true", false);
			}
			Logging.Write("[tbcFlightMaster]: Resetting pathing");
		});
	}

	// Token: 0x06000010 RID: 16
	private static float calculateRealDistance(Vector3 startVector, Vector3 destinationVector)
	{
		float num = 0f;
		List<Vector3> list = new List<Vector3>();
		list = PathFinder.FindPath(startVector, destinationVector);
		for (int i = 0; i < list.Count - 1; i++)
		{
			num += list[i].DistanceTo2D(list[i + 1]);
		}
		return num;
	}

	// Token: 0x06000011 RID: 17
	public static FlightMasterDB getClosestFlightMasterFrom()
	{
		List<FlightMasterDB> list = Main.fillDB();
		float num = 99999f;
		FlightMasterDB result = new FlightMasterDB("null", 0, new Vector3(0f, 0f, 0f, "None"), 0, false);
		foreach (FlightMasterDB flightMasterDB in list)
		{
			if (flightMasterDB.alreadyDiscovered && flightMasterDB.position.DistanceTo(ObjectManager.Me.Position) < num && flightMasterDB.continent == Main.checkContinent())
			{
				num = flightMasterDB.position.DistanceTo(ObjectManager.Me.Position);
				result = flightMasterDB;
			}
		}
		return result;
	}

	// Token: 0x06000012 RID: 18
	public static FlightMasterDB getClosestFlightMasterTo()
	{
		List<FlightMasterDB> list = Main.fillDB();
		float num = 99999f;
		FlightMasterDB result = new FlightMasterDB("null", 0, new Vector3(0f, 0f, 0f, "None"), 0, false);
		foreach (FlightMasterDB flightMasterDB in list)
		{
			if (flightMasterDB.alreadyDiscovered && flightMasterDB.position.DistanceTo(Main.destinationVector) < num && flightMasterDB.continent == Main.checkContinent())
			{
				num = flightMasterDB.position.DistanceTo(Main.destinationVector);
				result = flightMasterDB;
			}
		}
		return result;
	}

	// Token: 0x06000013 RID: 19
	public static int checkContinent()
	{
		return Usefuls.ContinentId;
	}

	// Token: 0x06000014 RID: 20
	public static void waitFlying(string destinationFlightMaster)
	{
		while (ObjectManager.Me.IsOnTaxi)
		{
			Logging.Write("[tbcFlightMaster]: On taxi, waiting");
			Thread.Sleep(30000);
		}
		Main._takenTaxi = true;
		Main.inProcessing = false;
		Thread.Sleep(5000);
		Main.Reenable();
		Logging.Write("[tbcFlightMaster]: Arrived at destination " + destinationFlightMaster + " , finished waiting");
	}

	// Token: 0x06000015 RID: 21
	public static List<FlightMasterDB> fillDB()
	{
		return new List<FlightMasterDB>
		{
			new FlightMasterDB("Grom'gol", 1387, new Vector3(-12417.5f, 144.474f, 3.36881f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.StranglethornGromgol),
			new FlightMasterDB("Booty Bay", 2858, new Vector3(-14448.6f, 506.129f, 26.3565f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.StranglethornBootyBay),
			new FlightMasterDB("Silverpine Forest", 2226, new Vector3(473.939f, 1533.95f, 131.96f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.SilverpineForest),
			new FlightMasterDB("Hillsbrad Foothills", 2389, new Vector3(2.67557f, -857.919f, 58.889f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.HillsbradFoothills),
			new FlightMasterDB("Arathi Highlands", 2851, new Vector3(-917.658, -3496.94f, 70.4505f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.ArathiHighlands),
			new FlightMasterDB("Badlands", 2861, new Vector3(-6632.22f, -2178.42f, 244.227, "None"), 2, FNVFlightMasterSettings.CurrentSettings.Badlands),
			new FlightMasterDB("Mulgore", 2995, new Vector3(-1196.75f, 26.0777f, 177.033f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Mulgore),
			new FlightMasterDB("Searing Gorge", 3305, new Vector3(-6559.26f, -1100.23f, 310.353f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.SearingGorge),
			new FlightMasterDB("Orgrimmar", 3310, new Vector3(1676.25f, -4313.45f, 61.7176f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Orgrimmar),
			new FlightMasterDB("Crossroads", 3615, new Vector3(-437.137f, -2596f, 95.8708f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.BarrensCrossroads),
			new FlightMasterDB("Camp Taurajo", 10378, new Vector3(-2384.08f, -1880.94f, 95.9336f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.BarrensTaurajo),
			new FlightMasterDB("Ratchet", 16227, new Vector3(-898.246f, -3769.65f, 11.7932f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.BarrensRatchet),
			new FlightMasterDB("Sun Rock Retreat", 4312, new Vector3(968.077f, 1042.29f, 104.563f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.StonetalonSunRockRetreat),
			new FlightMasterDB("The Hinterlands", 4314, new Vector3(-631.736f, -4720.6f, 5.48226f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.TheHinterlands),
			new FlightMasterDB("Thousand Needles", 4317, new Vector3(-5407.12f, -2419.61f, 89.7094f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.ThousandNeedles),
			new FlightMasterDB("Undercity", 4551, new Vector3(1567.12f, 266.345f, -43.0194f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.Undercity),
			new FlightMasterDB("Swamp of Sorrows", 6026, new Vector3(-10459.2f, -3279.76f, 21.5445f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.SwampofSorrows),
			new FlightMasterDB("Desolace", 6726, new Vector3(-1770.37f, 3262.19f, 5.10852f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Desolace),
			new FlightMasterDB("Tanaris", 7824, new Vector3(-7045.24f, -3779.4f, 10.3158f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Tanaris),
			new FlightMasterDB("Feralas", 8020, new Vector3(-4421.94f, 198.146f, 25.1863f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Feralas),
			new FlightMasterDB("Azshara", 8610, new Vector3(3664.02f, -4390.45f, 113.169f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Azshara),
			new FlightMasterDB("Winterspring", 11139, new Vector3(6815.12f, -4610.12f, 710.759f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Winterspring),
			new FlightMasterDB("Dustwallow Marsh", 11899, new Vector3(-3149.14f, -2842.13f, 34.6649f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.DustwallowMarsh),
			new FlightMasterDB("Felwood", 11900, new Vector3(5064.72f, -338.845f, 367.463f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Felwood),
			new FlightMasterDB("Zoram'gar Outpost", 11901, new Vector3(3373.69f, 994.351f, 5.36158f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.AshenvaleZoramgar),
			new FlightMasterDB("Splintertree Post", 12616, new Vector3(2305.64f, -2520.15f, 103.893f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.AshenvaleSplintertree),
			new FlightMasterDB("Moonglade", 12740, new Vector3(7466.15f, -2122.08f, 492.427f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Moonglade),
			new FlightMasterDB("Burning Steppes", 13177, new Vector3(-7504.06f, -2190.77f, 165.302f, "None"), 2, FNVFlightMasterSettings.CurrentSettings.BurningSteppes),
			new FlightMasterDB("Silithus", 15178, new Vector3(-6810.2f, 841.704f, 49.7481f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.Silithus),
			new FlightMasterDB("Un'Goro Crater", 10583, new Vector3(-6110.54f, -1140.35f, -186.866f, "None"), 1, FNVFlightMasterSettings.CurrentSettings.UngoroCrater),

			new FlightMasterDB("Thrallmar", 16587, new Vector3(228.5f, 2633.57f, 87.67f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.Thrallmar),
			new FlightMasterDB("Falcon Watch", 18942, new Vector3(-587.41f, 4101.01f, 91.37f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.FalconWatch),
			new FlightMasterDB("Zabra'jin", 18791, new Vector3(219.45f, 7816f, 22.72f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.Zabrajin),
			new FlightMasterDB("Garadar", 18808, new Vector3(-1261.09f, 7133.39f, 57.34f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.Garadar),
			new FlightMasterDB("Area 52", 18938, new Vector3(3082.31f, 3596.11f, 144.02f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.Area52),
			new FlightMasterDB("Shadowmoon Village", 19317, new Vector3(-3018.62f, 2557.09f, 79.09f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.ShadowmoonVillage),
			new FlightMasterDB("Stonebreaker Hold", 18807, new Vector3(-2567.33f, 4423.83f, 39.33f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.StonebreakerHold),
			new FlightMasterDB("Thunderlord Stronghold", 18953, new Vector3(2446.37f, 6020.93f, 154.34f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.ThunderlordStronghold),
			new FlightMasterDB("Shattrath City", 18940, new Vector3(-1837.23f, 5301.9f, -12.43f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.Shattrath),
			new FlightMasterDB("The Stormspire", 19583, new Vector3(4157.58f, 2959.69f, 352.08f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.TheStormspire),
			new FlightMasterDB("Altar of Sha'tar", 19581, new Vector3(-3065.6f, 749.42f, -10.1f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.AltarofShatar),
			new FlightMasterDB("Cosmowrench", 20515, new Vector3(2974.95f, 1848.24f, 141.28f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.Cosmowrench),
			new FlightMasterDB("Sanctum of the Stars", 21766, new Vector3(-4073.17f, 1123.61f, 42.47f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.SanctumOfTheStars),
			new FlightMasterDB("Spinebreaker Ridge", 19558, new Vector3(-1316.84f, 2358.62f, 88.96f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.SpinebreakerPost),
			new FlightMasterDB("Mok'Nathal Village", 22455, new Vector3(2028.79f, 4705.27f, 150.51f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.MokNathalVillage),
			new FlightMasterDB("Evergrove", 22216, new Vector3(2976.01f, 5501.13f, 143.67f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.Evergrove),
			new FlightMasterDB("Swamprat Post", 20762, new Vector3(91.67f, 5214.92f, 23.1f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.SwampratPost),
			new FlightMasterDB("The Dark Portal", 18930, new Vector3(-178.09f, 1026.72f, 54.19f, "None"), 530, FNVFlightMasterSettings.CurrentSettings.TheDarkPortal)

		};
	}

	// Token: 0x06000016 RID: 22
	private static void takeTaxi(FlightMasterDB from, FlightMasterDB to)
	{
		if (GoToTask.ToPosition(from.position, 3.5f, false, (object context) => Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Conditions.IsAttackedAndCannotIgnore))
		{
			if (GoToTask.ToPositionAndIntecractWithNpc(from.position, from.NPCId, -1, false, (object context) => Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Conditions.IsAttackedAndCannotIgnore, false))
			{
				while (!ObjectManager.Me.IsOnTaxi)
				{
					if (ObjectManager.Me.IsMounted)
					{
						MountTask.DismountMount(false, false, 100);
					}
					Usefuls.SelectGossipOption(GossipOptionsType.taxi);
					Thread.Sleep(Usefuls.Latency + 1500);
					while (Main._updateNodes)
					{
						Logging.Write("[tbcFlightMaster]: Taxi node update in progress, waiting...");
						Thread.Sleep(10000);
					}
					int num = Lua.LuaDoString<int>("for i=0,30 do if string.find(TaxiNodeName(i),\'" + to.name.Replace("'", "\\'") + "\') then return i end end", "");
					Lua.LuaDoString("TakeTaxiNode(" + num + ")", false);
					Logging.Write("[tbcFlightMaster]: Taking Taxi from " + from.name + " to " + to.name);
					Thread.Sleep(Usefuls.Latency + 500);
					Keyboard.DownKey(Memory.WowMemory.Memory.WindowHandle, Keys.Escape);
					Thread.Sleep(Usefuls.Latency + 2500);
					if (!ObjectManager.Me.IsOnTaxi)
					{
						GoToTask.ToPositionAndIntecractWithNpc(from.position, from.NPCId, -1, false, (object context) => Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Conditions.IsAttackedAndCannotIgnore, false);
					}
				}
				if (ObjectManager.Me.IsOnTaxi)
				{
					Main.waitFlying(to.name);
				}
			}
		}
	}

	// Token: 0x06000017 RID: 23
	private static void discoverTaxi(FlightMasterDB flightMasterToDiscover)
	{
		FNVFlightMasterSettings.Load();
		Main.fillDB();
		if (GoToTask.ToPosition(flightMasterToDiscover.position, 3.5f, false, (object context) => Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Conditions.IsAttackedAndCannotIgnore))
		{
			GoToTask.ToPosition(flightMasterToDiscover.position, 3.5f, false, (object context) => Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Conditions.IsAttackedAndCannotIgnore);
			if (GoToTask.ToPositionAndIntecractWithNpc(flightMasterToDiscover.position, flightMasterToDiscover.NPCId, -1, false, (object context) => Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Conditions.IsAttackedAndCannotIgnore, false))
			{
				wManagerSetting.ClearBlacklistOfCurrentProductSession();
				GoToTask.ToPositionAndIntecractWithNpc(flightMasterToDiscover.position, flightMasterToDiscover.NPCId, -1, false, (object context) => Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Conditions.IsAttackedAndCannotIgnore, false);
				if (ObjectManager.Me.IsMounted)
				{
					MountTask.DismountMount(false, false, 100);
				}
				Usefuls.SelectGossipOption(GossipOptionsType.taxi);
				Thread.Sleep(Usefuls.Latency + 1500);
				while (Main._updateNodes)
				{
					Logging.Write("[tbcFlightMaster]: Taxi node update in progress...");
					Thread.Sleep(10000);
				}
				Logging.Write("[tbcFlightMaster]: Flight Master " + flightMasterToDiscover.name + " discovered");
				flightMasterToDiscover.alreadyDiscovered = true;
				FNVFlightMasterSettings.flightMasterSaveChanges(flightMasterToDiscover, true);
				Thread.Sleep(Usefuls.Latency * 5);
				Main.timer = 0;
				Main.discoverTaxiNode = null;
				Main._taxiToDiscover = false;
				Main._discoverInProessing = false;
				Main._discoverTaxiTimer = true;
				Main.Reenable();
				return;
			}
		}
		Main._discoverInProessing = false;
	}

	internal static void g__CHAT_MSG_PET_INFO()
	{
		if (FNVFlightMasterSettings.CurrentSettings.updateTaxi && !Main._updateNodes)
		{
			Main._updateNodes = true;
			foreach (FlightMasterDB flightMasterDB in Main.fillDB())
			{
				if (flightMasterDB.continent.Equals(Main.checkContinent()))
				{
					int num = Lua.LuaDoString<int>("for i=0,48 do if string.find(TaxiNodeName(i),\'" + to.name.Replace("'", "\\'") + "\') then return i end end return -1", "");
					if (num == -1 && flightMasterDB.alreadyDiscovered)
					{
						Logging.Write("[tbcFlightMaster]: Taxi node " + flightMasterDB.name + " has not been discovered so far");
						flightMasterDB.alreadyDiscovered = false;
						FNVFlightMasterSettings.flightMasterSaveChanges(flightMasterDB, false);
					}
					else if (num != -1 && !flightMasterDB.alreadyDiscovered)
					{
						Logging.Write("[tbcFlightMaster]: Taxi node " + flightMasterDB.name + " has already been discovered");
						flightMasterDB.alreadyDiscovered = true;
						FNVFlightMasterSettings.flightMasterSaveChanges(flightMasterDB, true);
					}
				}
			}
			Main._updateNodes = false;
			Thread.Sleep(Usefuls.Latency * 5 + 5000);
		}
	}

	// Token: 0x04000001 RID: 1
	private string version = "1.7";

	// Token: 0x04000002 RID: 2
	public static int timer = 0;

	// Token: 0x04000003 RID: 3
	public static bool _isLaunched;

	// Token: 0x04000004 RID: 4
	private static float saveDistance;

	// Token: 0x04000005 RID: 5
	public static Vector3 destinationVector = new Vector3(0f, 0f, 0f, "None");

	// Token: 0x04000006 RID: 6
	public static bool inProcessing = false;

	// Token: 0x04000007 RID: 7
	public static bool _takenTaxi = false;

	// Token: 0x04000008 RID: 8
	private static FlightMasterDB from = null;

	// Token: 0x04000009 RID: 9
	private static FlightMasterDB to = null;

	// Token: 0x0400000A RID: 10
	private static FlightMasterDB discoverTaxiNode = null;

	// Token: 0x0400000B RID: 11
	public static bool _timer = false;

	// Token: 0x0400000C RID: 12
	public static bool _discoverTaxiTimer = false;

	// Token: 0x0400000D RID: 13
	public static bool changer = true;

	// Token: 0x0400000E RID: 14
	public static bool _updateNodes;

	// Token: 0x0400000F RID: 15
	public static bool checkPath = true;

	// Token: 0x04000010 RID: 16
	public static bool checkPathActive = false;

	// Token: 0x04000011 RID: 17
	public static FlightMasterDB checkPathActiveFM = null;

	// Token: 0x04000012 RID: 18
	public static bool cancelCheckPathThread = false;

	// Token: 0x04000013 RID: 19
	public static bool pauseCheckPathThread = false;

	// Token: 0x04000014 RID: 20
	public static string status = "";

	// Token: 0x04000015 RID: 21
	public static string statusDiscover = "";

	// Token: 0x04000017 RID: 23
	public static bool _runScan = false;

	// Token: 0x04000018 RID: 24
	public static FlightMasterDB taxiToDiscover = null;

	// Token: 0x04000019 RID: 25
	public static bool _taxiToDiscover = false;

	// Token: 0x0400001A RID: 26
	public static bool _discoverInProessing = false;

	// Token: 0x0400001B RID: 27
	public static int stuckCounter = 0;

	// Token: 0x0400001C RID: 28
	private Thread scanNearbyTaxi = new Thread(delegate()
	{
		int millisecondsTimeout = 10000;
		List<FlightMasterDB> list = Main.fillDB();
		Logging.Write("[tbcFlightMaster]: Taxi scan started");
		while (Products.IsStarted)
		{
			if (Main._discoverTaxiTimer || Main._discoverInProessing)
			{
				Logging.Write("[tbcFlightMaster]: Discover in processing or scan for nearby nodes paused");
				for (int i = FNVFlightMasterSettings.CurrentSettings.pauseTaxiTime; i > 0; i -= 1000)
				{
					Thread.Sleep(1000);
				}
				Main._discoverTaxiTimer = false;
			}
			while (Main.inCombat() || Main.inCombatPet())
			{
				Thread.Sleep(5000);
			}
			string text = Logging.Status;
			while (text.Contains("First Aid") && Usefuls.MapZoneName.Contains("Teldrassil"))
			{
				Logging.Write("[tbcFlightMaster]: HumanMasterPlugin trying to train First Aid. Pausing undiscovered node scan for five minutes to avoid conflicts");
				Thread.Sleep(300000);
			}
			if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Main._taxiToDiscover && !ObjectManager.Me.IsOnTaxi)
			{
				foreach (FlightMasterDB flightMasterDB in list)
				{
					if (Main.checkContinent() == flightMasterDB.continent && !flightMasterDB.alreadyDiscovered && ObjectManager.Me.Position.DistanceTo(flightMasterDB.position) < (float)FNVFlightMasterSettings.CurrentSettings.detectTaxiDistance)
					{
						Main.taxiToDiscover = flightMasterDB;
						Main.discoverTaxiNode = flightMasterDB;
						Main._taxiToDiscover = true;
						Logging.Write("[tbcFlightMaster]: Near undiscovered Taxi node found: " + flightMasterDB.name);
						Thread.Sleep(1000 + Usefuls.Latency);
						while (!MovementManager.InMovement)
						{
							Thread.Sleep(100);
						}
						Main.Reenable();
					}
				}
			}
			Thread.Sleep(Usefuls.Latency * 10);
			list = Main.fillDB();
			Thread.Sleep(millisecondsTimeout);
		}
	});
}
