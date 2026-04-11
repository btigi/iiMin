namespace ii.Min.Model;

public class SaveFile
{
	public ushort Version { get; set; }
	public SaveGameType SaveGameType { get; set; }
	public string SaveGameName { get; set; } = string.Empty;
	public PlanetType PlanetType { get; set; }
	public ushort MissionIndex { get; set; }
	public string[] TeamNames { get; set; } = new string[4];
	// Red, green, blue, gray, alien
	public TeamType[] TeamTypes { get; set; } = new TeamType[5];
	// Red, green, blue, gray, alien
	public TeamClan[] TeamClans { get; set; } = new TeamClan[5];
	public uint RngSeed { get; set; }
	public OpponentType Opponent { get; set; }
	public ushort TurnTimerSetting { get; set; }
	public ushort EndTurnSetting { get; set; }
	public PlayMode PlayMode { get; set; }
	public SaveIniOptions Options { get; set; } = new();
	public byte[] SurfaceMap { get; set; } = [];
	public short[] ResourceMap { get; set; } = [];
	public SaveTeamInfo[] Teams { get; set; } = [new(), new(), new(), new()];
	public TeamIndex ActiveTurnTeam { get; set; }
	public TeamIndex PlayerTeam { get; set; }
	public int TurnCounter { get; set; }
	public ushort GameState { get; set; }
	public ushort MenuTurnTimer { get; set; }
	public SaveIniPreferences Preferences { get; set; } = new();
	public SaveTeamUnits[] TeamUnits { get; set; } = [new(), new(), new(), new()];
	public List<SaveUnitInfo> GroundCoverUnits { get; } = [];
	public List<SaveUnitInfo> MobileLandSeaUnits { get; } = [];
	public List<SaveUnitInfo> StationaryUnits { get; } = [];
	public List<SaveUnitInfo> MobileAirUnits { get; } = [];
	public List<SaveUnitInfo> ParticleUnits { get; } = [];
	public SaveUnitHash UnitHash { get; set; } = new();
	public SaveMapHash MapHash { get; set; } = new();
	public SaveTeamHeatMaps?[] HeatMaps { get; set; } = new SaveTeamHeatMaps?[4];
	public List<SaveMessageLogEntry>[] MessageLogs { get; set; } = [[], [], [], []];
	public SaveAiPlayer?[] AiPlayers { get; set; } = new SaveAiPlayer?[4];
	// Multiplayer specific info
	public byte[] OpaqueTailAfterMessages { get; set; } = [];
}