namespace ii.Min.Model;

public class SaveTeamInfo
{
	public short[,] Markers { get; set; } = new short[10, 2];
	public TeamType TeamType { get; set; }
	public bool FinishedTurn { get; set; }
	public TeamClan TeamClan { get; set; }
	public SaveResearchTopic[] ResearchTopics { get; set; } = [.. Enumerable.Range(0, 8).Select(_ => new SaveResearchTopic())];
	public uint TeamPoints { get; set; }
	public ushort NumberOfObjectsCreated { get; set; }
	public byte[] UnitCounters { get; set; } = new byte[Constants.UnitCount];
	public sbyte[,] CameraPositions { get; set; } = new sbyte[6, 2];
	public short[] ScoreGraph { get; set; } = new short[50];
	public ushort SelectedUnitId { get; set; }
	public ushort ZoomLevel { get; set; }
	public short CameraX { get; set; }
	public short CameraY { get; set; }
	public sbyte DisplayButtonRange { get; set; }
	public sbyte DisplayButtonScan { get; set; }
	public sbyte DisplayButtonStatus { get; set; }
	public sbyte DisplayButtonColors { get; set; }
	public sbyte DisplayButtonHits { get; set; }
	public sbyte DisplayButtonAmmo { get; set; }
	public sbyte DisplayButtonNames { get; set; }
	public sbyte DisplayButtonMinimap2x { get; set; }
	public sbyte DisplayButtonMinimapTnt { get; set; }
	public sbyte DisplayButtonGrid { get; set; }
	public sbyte DisplayButtonSurvey { get; set; }
	public short StatsFactoriesBuilt { get; set; }
	public short StatsMinesBuilt { get; set; }
	public short StatsBuildingsBuilt { get; set; }
	public short StatsUnitsBuilt { get; set; }
	public ushort[] Casualties { get; set; } = new ushort[Constants.UnitCount];
	public short StatsGoldSpentOnUpgrades { get; set; }
}
