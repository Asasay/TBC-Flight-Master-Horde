using System;
using robotManager.Helpful;

// Token: 0x02000003 RID: 3
public class FlightMasterDB
{
	// Token: 0x0600001A RID: 26 RVA: 0x000020E7 File Offset: 0x000002E7
	public FlightMasterDB(string name, int NPCId, Vector3 position, int continent, bool alreadyDiscovered)
	{
		this.name = name;
		this.NPCId = NPCId;
		this.position = position;
		this.continent = continent;
		this.alreadyDiscovered = alreadyDiscovered;
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x0600001B RID: 27 RVA: 0x0000211B File Offset: 0x0000031B
	// (set) Token: 0x0600001C RID: 28 RVA: 0x00002123 File Offset: 0x00000323
	public int NPCId { get; set; }

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x0600001D RID: 29 RVA: 0x0000212C File Offset: 0x0000032C
	// (set) Token: 0x0600001E RID: 30 RVA: 0x00002134 File Offset: 0x00000334
	public Vector3 position { get; set; }

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x0600001F RID: 31 RVA: 0x0000213D File Offset: 0x0000033D
	// (set) Token: 0x06000020 RID: 32 RVA: 0x00002145 File Offset: 0x00000345
	public string name { get; set; }

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000021 RID: 33 RVA: 0x0000214E File Offset: 0x0000034E
	// (set) Token: 0x06000022 RID: 34 RVA: 0x00002156 File Offset: 0x00000356
	public int continent { get; set; }

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000023 RID: 35 RVA: 0x0000215F File Offset: 0x0000035F
	// (set) Token: 0x06000024 RID: 36 RVA: 0x00002167 File Offset: 0x00000367
	public bool alreadyDiscovered { get; set; }
}
