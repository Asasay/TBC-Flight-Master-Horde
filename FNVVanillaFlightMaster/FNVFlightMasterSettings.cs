using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Threading;
using robotManager.Helpful;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

// Token: 0x02000004 RID: 4
[Serializable]
public class FNVFlightMasterSettings : Settings
{
	// Token: 0x06000025 RID: 37 RVA: 0x00003C80 File Offset: 0x00001E80
	public FNVFlightMasterSettings()
	{
		this.taxiTriggerDistance = 1000;
		this.pauseTaxiTime = 50000;
		this.detectTaxiDistance = 50;
		this.shorterMinDistance = 1000;
		this.skipIfFollowPath = true;
		this.updateTaxi = true;
		this.skipIfFollowPathDistance = 5000f;
		this.pauseSearingGorge = true;

		this.StranglethornGromgol = false;
		this.StranglethornBootyBay = false;
		this.SilverpineForest = false;
		this.HillsbradFoothills = false;
		this.ArathiHighlands = false;
		this.Badlands = false;
		this.Mulgore = false;
		this.SearingGorge = false;
		this.Orgrimmar = false;
		this.BarrensCrossroads = false;
		this.BarrensRatchet = false;
		this.BarrensTaurajo = false;
		this.StonetalonSunRockRetreat = false;
		this.TheHinterlands = false;
		this.ThousandNeedles = false;
		this.Undercity = false;
		this.SwampofSorrows = false;
		this.Desolace = false;
		this.Tanaris = false;
		this.Feralas = false;
		this.Azshara = false;
		this.Winterspring = false;
		this.DustwallowMarsh = false;
		this.Felwood = false;
		this.AshenvaleSplintertree = false;
		this.AshenvaleZoramgar = false;
		this.UngoroCrater = false;
		this.Silithus = false;
		this.Moonglade = false;
		this.BurningSteppes = false;

		this.Thrallmar = false;
		this.FalconWatch = false;
		this.Zabrajin = false;
		this.Garadar = false;
		this.Area52 = false;
		this.Cosmowrench = false;
		this.Evergrove = false;
		this.SanctumOfTheStars = false;
		this.ShadowmoonVillage = false;
		this.StonebreakerHold = false;
		this.Shattrath = false;
		this.ThunderlordStronghold = false;
		this.TheStormspire = false;
		this.AltarofShatar = false;
		this.SpinebreakerPost = false;
		this.MokNathalVillage = false;
		this.SwampratPost = false;
		this.TheDarkPortal = false;

	}

	// Token: 0x06000026 RID: 38 RVA: 0x00003DE8 File Offset: 0x00001FE8
	public static void flightMasterSaveChanges(FlightMasterDB needToChange, bool value)
	{
		if (needToChange.name.Contains("Base")) FNVFlightMasterSettings.CurrentSettings.StranglethornGromgol = value;
		if (needToChange.name.Contains("Booty")) FNVFlightMasterSettings.CurrentSettings.StranglethornBootyBay = value;
		if (needToChange.name.Contains("Silverpine")) FNVFlightMasterSettings.CurrentSettings.SilverpineForest = value;
		if (needToChange.name.Contains("Hillsbrad")) FNVFlightMasterSettings.CurrentSettings.HillsbradFoothills = value;
		if (needToChange.name.Contains("Arathi")) FNVFlightMasterSettings.CurrentSettings.ArathiHighlands = value;
		if (needToChange.name.Contains("Badlands")) FNVFlightMasterSettings.CurrentSettings.Badlands = value;
		if (needToChange.name.Contains("Mulgore")) FNVFlightMasterSettings.CurrentSettings.Mulgore = value;
		if (needToChange.name.Contains("Searing")) FNVFlightMasterSettings.CurrentSettings.SearingGorge = value;
		if (needToChange.name.Contains("Orgrimmar")) FNVFlightMasterSettings.CurrentSettings.Orgrimmar = value;
		if (needToChange.name.Contains("Crossroads")) FNVFlightMasterSettings.CurrentSettings.BarrensCrossroads = value;
		if (needToChange.name.Contains("Ratchet")) FNVFlightMasterSettings.CurrentSettings.BarrensRatchet = value;
		if (needToChange.name.Contains("Taurajo")) FNVFlightMasterSettings.CurrentSettings.BarrensTaurajo = value;
		if (needToChange.name.Contains("Retreat")) FNVFlightMasterSettings.CurrentSettings.StonetalonSunRockRetreat = value;
		if (needToChange.name.Contains("Hinterlands")) FNVFlightMasterSettings.CurrentSettings.TheHinterlands = value;
		if (needToChange.name.Contains("Thousand")) FNVFlightMasterSettings.CurrentSettings.ThousandNeedles = value;
		if (needToChange.name.Contains("Undercity")) FNVFlightMasterSettings.CurrentSettings.Undercity = value;
		if (needToChange.name.Contains("Swamp")) FNVFlightMasterSettings.CurrentSettings.SwampofSorrows = value;
		if (needToChange.name.Contains("Desolace")) FNVFlightMasterSettings.CurrentSettings.Desolace = value;
		if (needToChange.name.Contains("Tanaris")) FNVFlightMasterSettings.CurrentSettings.Tanaris = value;
		if (needToChange.name.Contains("Feralas")) FNVFlightMasterSettings.CurrentSettings.Feralas = value;
		if (needToChange.name.Contains("Azshara")) FNVFlightMasterSettings.CurrentSettings.Azshara = value;
		if (needToChange.name.Contains("Winterspring")) FNVFlightMasterSettings.CurrentSettings.Winterspring = value;
		if (needToChange.name.Contains("Dustwallow")) FNVFlightMasterSettings.CurrentSettings.DustwallowMarsh = value;
		if (needToChange.name.Contains("Felwood")) FNVFlightMasterSettings.CurrentSettings.Felwood = value;
		if (needToChange.name.Contains("Outpost")) FNVFlightMasterSettings.CurrentSettings.AshenvaleZoramgar = value;
		if (needToChange.name.Contains("Splintertree")) FNVFlightMasterSettings.CurrentSettings.AshenvaleSplintertree = value;
		if (needToChange.name.Contains("Moonglade")) FNVFlightMasterSettings.CurrentSettings.Moonglade = value;
		if (needToChange.name.Contains("Burning")) FNVFlightMasterSettings.CurrentSettings.BurningSteppes = value;
		if (needToChange.name.Contains("Silithus")) FNVFlightMasterSettings.CurrentSettings.Silithus = value;
		if (needToChange.name.Contains("Crater")) FNVFlightMasterSettings.CurrentSettings.UngoroCrater = value;

		if (needToChange.name.Contains("Thrallmar")) FNVFlightMasterSettings.CurrentSettings.Thrallmar = value;
		if (needToChange.name.Contains("Falcon")) FNVFlightMasterSettings.CurrentSettings.FalconWatch = value;
		if (needToChange.name.Contains("Zabra'jin")) FNVFlightMasterSettings.CurrentSettings.Zabrajin = value;
		if (needToChange.name.Contains("Garadar")) FNVFlightMasterSettings.CurrentSettings.Garadar = value;
		if (needToChange.name.Contains("Area")) FNVFlightMasterSettings.CurrentSettings.Area52 = value;
		if (needToChange.name.Contains("Shadowmoon")) FNVFlightMasterSettings.CurrentSettings.ShadowmoonVillage = value;
		if (needToChange.name.Contains("Stonebreaker")) FNVFlightMasterSettings.CurrentSettings.StonebreakerHold = value;
		if (needToChange.name.Contains("Thunderlord")) FNVFlightMasterSettings.CurrentSettings.ThunderlordStronghold = value;
		if (needToChange.name.Contains("Shattrath")) FNVFlightMasterSettings.CurrentSettings.Shattrath = value;
		if (needToChange.name.Contains("Stormspire")) FNVFlightMasterSettings.CurrentSettings.TheStormspire = value;
		if (needToChange.name.Contains("Altar")) FNVFlightMasterSettings.CurrentSettings.AltarofShatar = value;
		if (needToChange.name.Contains("Cosmowrench")) FNVFlightMasterSettings.CurrentSettings.Cosmowrench = value;
		if (needToChange.name.Contains("Sanctum")) FNVFlightMasterSettings.CurrentSettings.SanctumOfTheStars = value;
		if (needToChange.name.Contains("Spinebreaker")) FNVFlightMasterSettings.CurrentSettings.SpinebreakerPost = value;
		if (needToChange.name.Contains("Mok'Nathal")) FNVFlightMasterSettings.CurrentSettings.MokNathalVillage = value;
		if (needToChange.name.Contains("Evergrove")) FNVFlightMasterSettings.CurrentSettings.Evergrove = value;
		if (needToChange.name.Contains("Swamprat")) FNVFlightMasterSettings.CurrentSettings.SwampratPost = value;
		if (needToChange.name.Contains("Portal")) FNVFlightMasterSettings.CurrentSettings.TheDarkPortal = value;
		

		FNVFlightMasterSettings.CurrentSettings.Save();
		Thread.Sleep(2500);

		try
		{
			FNVFlightMasterSettings.CurrentSettings = Settings.Load<FNVFlightMasterSettings>(Settings.AdviserFilePathAndName("tbcFlightMaster_DB", ObjectManager.Me.Name + "." + Usefuls.RealmName));
		}
		catch (Exception arg)
		{
			Logging.Write("[tbcFlightMaster]: Error when trying to reload DB file -> " + arg);
		}
		Logging.Write("[tbcFlightMaster]: Settings saved of Flight Master " + needToChange.name);
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000027 RID: 39 RVA: 0x00002170 File Offset: 0x00000370
	// (set) Token: 0x06000028 RID: 40 RVA: 0x00002177 File Offset: 0x00000377
	public static FNVFlightMasterSettings CurrentSettings { get; set; }

	// Token: 0x06000029 RID: 41 RVA: 0x00004294 File Offset: 0x00002494
	public bool Save()
	{
		bool result;
		try
		{
			result = base.Save(Settings.AdviserFilePathAndName("tbcFlightMaster_DB", ObjectManager.Me.Name + "." + Usefuls.RealmName));
		}
		catch (Exception arg)
		{
			Logging.WriteDebug("tbcFlightMaster_DB => Save(): " + arg);
			result = false;
		}
		return result;
	}

	// Token: 0x0600002A RID: 42 RVA: 0x000042F8 File Offset: 0x000024F8
	public static bool Load()
	{
		try
		{
			bool flag = File.Exists(Settings.AdviserFilePathAndName("tbcFlightMaster_DB", ObjectManager.Me.Name + "." + Usefuls.RealmName));
			if (flag)
			{
				FNVFlightMasterSettings.CurrentSettings = Settings.Load<FNVFlightMasterSettings>(Settings.AdviserFilePathAndName("tbcFlightMaster_DB", ObjectManager.Me.Name + "." + Usefuls.RealmName));
				return true;
			}
			FNVFlightMasterSettings.CurrentSettings = new FNVFlightMasterSettings();
		}
		catch (Exception arg)
		{
			Logging.WriteDebug("tbcFlightMaster_DB => Load(): " + arg);
		}
		return false;
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x0600002B RID: 43 RVA: 0x0000217F File Offset: 0x0000037F
	// (set) Token: 0x0600002C RID: 44 RVA: 0x00002187 File Offset: 0x00000387
	[Setting]
	[DefaultValue(1000)]
	[Category("1 - Main")]
	[DisplayName("Trigger Distance")]
	[Description("Sets how long your distance to your destination has to be, to trigger use of taxi")]
	public int taxiTriggerDistance { get; set; }

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600002D RID: 45 RVA: 0x00002190 File Offset: 0x00000390
	// (set) Token: 0x0600002E RID: 46 RVA: 0x00002198 File Offset: 0x00000398
	[Setting]
	[DefaultValue(50000)]
	[Category("1 - Main")]
	[DisplayName("Pause Taxi Time")]
	[Description("Sets how long taxi is paused after use, to avoid loops. Only change it, if you experience issues")]
	public int pauseTaxiTime { get; set; }

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x0600002F RID: 47 RVA: 0x000021A1 File Offset: 0x000003A1
	// (set) Token: 0x06000030 RID: 48 RVA: 0x000021A9 File Offset: 0x000003A9
	[Setting]
	[DefaultValue(50)]
	[Category("1 - Main")]
	[DisplayName("Discover Distance")]
	[Description("Min distance to discover an undiscovered taxi node")]
	public int detectTaxiDistance { get; set; }

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000031 RID: 49 RVA: 0x000021B2 File Offset: 0x000003B2
	// (set) Token: 0x06000032 RID: 50 RVA: 0x000021BA File Offset: 0x000003BA
	[Setting]
	[DefaultValue(1000)]
	[Category("1 - Main")]
	[DisplayName("Shorter Path Min")]
	[Description("Sets how much shorter a path has to be, to trigger taxi")]
	public int shorterMinDistance { get; set; }

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000033 RID: 51 RVA: 0x000021C3 File Offset: 0x000003C3
	// (set) Token: 0x06000034 RID: 52 RVA: 0x000021CB File Offset: 0x000003CB
	[Setting]
	[DefaultValue(true)]
	[Category("2 - Useful")]
	[DisplayName("1. Skip if Follow Path / Boat step")]
	[Description("Skips take taxi, if currently executing a Follow Path or Boat Quester step. When running a profile with dedicated paths")]
	public bool skipIfFollowPath { get; set; }

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000035 RID: 53 RVA: 0x000021D4 File Offset: 0x000003D4
	// (set) Token: 0x06000036 RID: 54 RVA: 0x000021DC File Offset: 0x000003DC
	[Setting]
	[DefaultValue(true)]
	[Category("2 - Useful")]
	[DisplayName("2. Update taxi nodes")]
	[Description("Scans and updates all entries on the taxi map of the current continent, if they have already been discovered. Triggers, when the taxi map is opened")]
	public bool updateTaxi { get; set; }

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x06000037 RID: 55 RVA: 0x000021E5 File Offset: 0x000003E5
	// (set) Token: 0x06000038 RID: 56 RVA: 0x000021ED File Offset: 0x000003ED
	[Setting]
	[DefaultValue(5000)]
	[Category("2 - Useful")]
	[DisplayName("1.1 Skip if ... min distance")]
	[Description("Won't skip taxi min distance to destination")]
	public float skipIfFollowPathDistance { get; set; }

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000039 RID: 57 RVA: 0x000021F6 File Offset: 0x000003F6
	// (set) Token: 0x0600003A RID: 58 RVA: 0x000021FE File Offset: 0x000003FE
	[Setting]
	[DefaultValue(true)]
	[Category("2 - Useful")]
	[DisplayName("3. Stop bot at Searing Gorge gate")]
	[Description("Stops the bot, to prevent it from running into the Searing Gorge gate from Loch Modan and getting stuck over and over again")]
	public bool pauseSearingGorge { get; set; }

	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool StranglethornGromgol { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool StranglethornBootyBay { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool SilverpineForest { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool HillsbradFoothills { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool ArathiHighlands { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Badlands { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Mulgore { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool SearingGorge { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Orgrimmar { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool BarrensCrossroads { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool BarrensTaurajo { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool BarrensRatchet { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool StonetalonSunRockRetreat { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool TheHinterlands { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool ThousandNeedles { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Undercity { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool SwampofSorrows { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Desolace { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Tanaris { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Feralas { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Azshara { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Winterspring { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool DustwallowMarsh { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Felwood { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool AshenvaleZoramgar { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool AshenvaleSplintertree { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Moonglade { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool BurningSteppes { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool Silithus { get; set; }
	[Setting]
	[Category("Azeroth Discovered Nodes")]
	public bool UngoroCrater { get; set; }

	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool Thrallmar { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool FalconWatch { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool Zabrajin { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool Garadar { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool Area52 { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool ShadowmoonVillage { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool StonebreakerHold { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool Shattrath { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool ThunderlordStronghold{ get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool TheStormspire { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool AltarofShatar { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool SpinebreakerPost { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool MokNathalVillage { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool SwampratPost { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool TheDarkPortal { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool Evergrove { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool Cosmowrench { get; set; }
	[Setting]
	[Category("Outland Discovered Nodes")]
	public bool SanctumOfTheStars { get; set; }

}
