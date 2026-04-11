using ii.Min.Model;

namespace ii.Min;

public class SaveProcessor
{
	public const ushort SaveFormatVersionV70 = 70;
	public const int MapWidth = 112;
	public const int MapHeight = 112;

	private static int MapCellCount => MapWidth * MapHeight;

	public SaveFile Read(string filename)
	{
		using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		using var br = new BinaryReader(fs);

		var result = new SaveFile();
		result.Version = br.ReadUInt16();
		if (result.Version != SaveFormatVersionV70)
		{
			throw new InvalidDataException($"Expected save format V{SaveFormatVersionV70}, got V{result.Version}.");
		}

		result.SaveGameType = (SaveGameType)br.ReadByte();
		result.SaveGameName = ReadFixedString(br, 30);
		result.PlanetType = (PlanetType)br.ReadByte();
		result.MissionIndex = br.ReadUInt16();

		for (var i = 0; i < 4; i++)
		{
			result.TeamNames[i] = ReadFixedString(br, 30);
		}

		for (var i = 0; i < 5; i++)
		{
			result.TeamTypes[i] = (TeamType)br.ReadByte();
		}

		for (var i = 0; i < 5; i++)
		{
			result.TeamClans[i] = (TeamClan)br.ReadByte();
		}

		result.RngSeed = br.ReadUInt32();
		result.Opponent = (OpponentType)br.ReadByte();
		result.TurnTimerSetting = br.ReadUInt16();
		result.EndTurnSetting = br.ReadUInt16();
		result.PlayMode = (PlayMode)br.ReadByte();

		ReadIniOptions(br, result.Options);

		result.SurfaceMap = br.ReadBytes(MapCellCount); //TODO:Expose the constituant bits

		var resourceBytes = br.ReadBytes(MapCellCount * 2);
		result.ResourceMap = new short[MapCellCount];
		Buffer.BlockCopy(resourceBytes, 0, result.ResourceMap, 0, resourceBytes.Length);

		for (var i = 0; i < 4; i++)
		{ 
			ReadTeamInfo(br, result.Teams[i]);
		}

		result.ActiveTurnTeam = (TeamIndex)br.ReadByte();
		result.PlayerTeam = (TeamIndex)br.ReadByte();
		result.TurnCounter = br.ReadInt32();
		result.GameState = br.ReadUInt16();
		result.MenuTurnTimer = br.ReadUInt16();

		ReadIniPreferences(br, result.Preferences);

		var loader = new V70Loader(br);
		for (var i = 0; i < 4; i++)
		{ 
			ReadTeamUnits(loader, result.TeamUnits[i]);
		}

		ReadUnitList(loader, result.GroundCoverUnits);
		ReadUnitList(loader, result.MobileLandSeaUnits);
		ReadUnitList(loader, result.StationaryUnits);
		ReadUnitList(loader, result.MobileAirUnits);
		ReadUnitList(loader, result.ParticleUnits);

		ReadUnitHash(loader, result.UnitHash);
		ReadMapHash(loader, result.MapHash);

		for (var team = 0; team < 4; team++)
		{
			if (result.Teams[team].TeamType == TeamType.TEAM_TYPE_NONE)
				continue;

			var heat = new SaveTeamHeatMaps
			{
				Complete = br.ReadBytes(MapCellCount),
				StealthSea = br.ReadBytes(MapCellCount),
				StealthLand = br.ReadBytes(MapCellCount)
			};
			result.HeatMaps[team] = heat;
		}

		ReadMessageLogs(loader, result.MessageLogs);

		if (result.SaveGameType == SaveGameType.Multi_scenario)
		{
			var rest = (int)(fs.Length - fs.Position);
			if (rest > 0)
			{ 
				result.OpaqueTailAfterMessages = br.ReadBytes(rest);
			}
		}
		else
		{
			ReadAiPlayers(loader, result.Teams, result.AiPlayers);
		}

		return result;
	}

	private static void ReadFixedTeamInfoFields(BinaryReader br, SaveTeamInfo team)
	{
		for (var i = 0; i < 10; i++)
		{
			team.Markers[i, 0] = br.ReadInt16();
			team.Markers[i, 1] = br.ReadInt16();
		}

		team.TeamType = (TeamType)br.ReadByte();
		team.FinishedTurn = br.ReadBoolean();
		team.TeamClan = (TeamClan)br.ReadByte();

		for (var i = 0; i < 8; i++)
		{
			team.ResearchTopics[i].ResearchLevel = br.ReadInt32();
			team.ResearchTopics[i].TurnsToComplete = br.ReadInt32();
			team.ResearchTopics[i].Allocation = br.ReadInt32();
		}

		team.TeamPoints = br.ReadUInt32();
		team.NumberOfObjectsCreated = br.ReadUInt16();

		for (var i = 0; i < Constants.UnitCount; i++)
		{ 
			team.UnitCounters[i] = br.ReadByte();
		}

		for (var i = 0; i < 6; i++)
		{
			team.CameraPositions[i, 0] = br.ReadSByte();
			team.CameraPositions[i, 1] = br.ReadSByte();
		}

		for (var i = 0; i < 50; i++)
		{ 
			team.ScoreGraph[i] = br.ReadInt16();
		}

		team.SelectedUnitId = br.ReadUInt16();
		team.ZoomLevel = br.ReadUInt16();
		team.CameraX = br.ReadInt16();
		team.CameraY = br.ReadInt16();

		team.DisplayButtonRange = br.ReadSByte();
		team.DisplayButtonScan = br.ReadSByte();
		team.DisplayButtonStatus = br.ReadSByte();
		team.DisplayButtonColors = br.ReadSByte();
		team.DisplayButtonHits = br.ReadSByte();
		team.DisplayButtonAmmo = br.ReadSByte();
		team.DisplayButtonNames = br.ReadSByte();
		team.DisplayButtonMinimap2x = br.ReadSByte();
		team.DisplayButtonMinimapTnt = br.ReadSByte();
		team.DisplayButtonGrid = br.ReadSByte();
		team.DisplayButtonSurvey = br.ReadSByte();

		team.StatsFactoriesBuilt = br.ReadInt16();
		team.StatsMinesBuilt = br.ReadInt16();
		team.StatsBuildingsBuilt = br.ReadInt16();
		team.StatsUnitsBuilt = br.ReadInt16();

		for (var i = 0; i < Constants.UnitCount; i++)
		{ 
			team.Casualties[i] = br.ReadUInt16();
		}

		team.StatsGoldSpentOnUpgrades = br.ReadInt16();
	}

	private static void ReadTeamInfo(BinaryReader br, SaveTeamInfo team) => ReadFixedTeamInfoFields(br, team);

	private static void ReadIniOptions(BinaryReader br, SaveIniOptions o)
	{
		o.World = br.ReadInt32();
		o.TurnTimer = br.ReadInt32();
		o.EndTurn = br.ReadInt32();
		o.StartGold = br.ReadInt32();
		o.PlayMode = br.ReadInt32();
		o.VictoryType = br.ReadInt32();
		o.VictoryLimit = br.ReadInt32();
		o.Opponent = br.ReadInt32();
		o.RawResource = br.ReadInt32(); // 0,1,2
		o.FuelResource = br.ReadInt32(); // 0,1,2
		o.GoldResource = br.ReadInt32(); // 0,1,2
		o.AlienDerelicts = br.ReadInt32(); // 0,1,2
	}

	private static void ReadIniPreferences(BinaryReader br, SaveIniPreferences p)
	{
		p.Effects = br.ReadInt32();
		p.ClickScroll = br.ReadInt32();
		p.QuickScroll = br.ReadInt32();
		p.FastMovement = br.ReadInt32();
		p.FollowUnit = br.ReadInt32();
		p.AutoSelect = br.ReadInt32();
		p.EnemyHalt = br.ReadInt32();
	}

	private static string ReadFixedString(BinaryReader br, int length)
	{
		var b = br.ReadBytes(length);
		var end = Array.IndexOf(b, (byte)0);
		if (end < 0)
			end = length;
		return System.Text.Encoding.UTF8.GetString(b, 0, end);
	}

	private static void ReadTeamUnits(V70Loader L, SaveTeamUnits tu)
	{
		tu.Gold = L.Reader.ReadInt16();

		for (var i = 0; i < Constants.UnitCount; i++)
		{ 
			tu.BaseUnitValues[i] = L.ReadObject<SaveUnitValues>();
		}

		for (var i = 0; i < Constants.UnitCount; i++)
		{ 
			tu.CurrentUnitValues[i] = L.ReadObject<SaveUnitValues>();
		}

		var objectCount = L.ReadObjectCount();
		for (var i = 0; i < objectCount; i++)
		{
			var complex = L.ReadObject<SaveComplex>() ?? throw new InvalidDataException("Null complex in team units.");
			tu.Complexes.Add(complex);
		}
	}

	private static void ReadUnitList(V70Loader L, List<SaveUnitInfo> list)
	{
		var n = L.ReadObjectCount();
		for (var i = 0; i < n; i++)
		{
			var u = L.ReadObject<SaveUnitInfo>() ?? throw new InvalidDataException("Null unit in list.");
			list.Add(u);
		}
	}

	private static void ReadUnitHash(V70Loader L, SaveUnitHash h)
	{
		h.HashSize = L.Reader.ReadUInt16();
		h.Buckets = new List<SaveUnitInfo>[h.HashSize];
		for (var i = 0; i < h.HashSize; i++)
		{
			h.Buckets[i] = [];
			ReadUnitList(L, h.Buckets[i]);
		}
	}

	private static void ReadMapHash(V70Loader L, SaveMapHash h)
	{
		h.HashSize = L.Reader.ReadUInt16();
		h.XShift = L.Reader.ReadInt16();
		h.Buckets = new List<SaveMapHashCell>[h.HashSize];
		for (var i = 0; i < h.HashSize; i++)
		{
			h.Buckets[i] = [];
			var cnt = L.ReadObjectCount();
			for (var j = 0; j < cnt; j++)
			{
				var cell = new SaveMapHashCell
				{
					X = L.Reader.ReadUInt16(),
					Y = L.Reader.ReadUInt16()
				};
				ReadUnitList(L, cell.Units);
				h.Buckets[i].Add(cell);
			}
		}
	}

	private static void ReadMessageLogs(V70Loader L, List<SaveMessageLogEntry>[] logs)
	{
		for (var i = 0; i < 4; i++)
		{
			var cnt = L.ReadObjectCount();
			for (var j = 0; j < cnt; j++)
			{
				var textLen = L.Reader.ReadUInt16();
				var textBytes = L.Reader.ReadBytes(textLen);
				var entry = new SaveMessageLogEntry
				{
					Text = V70Loader.DecodeName(textBytes),
					Unit = L.ReadObject<SaveUnitInfo>(),
					PointX = L.Reader.ReadInt16(),
					PointY = L.Reader.ReadInt16(),
					IsAlertMessage = L.Reader.ReadBoolean(),
					ResourceId = L.Reader.ReadUInt16()
				};
				logs[i].Add(entry);
			}
		}
	}

	private static void ReadAiPlayers(V70Loader L, SaveTeamInfo[] teams, SaveAiPlayer?[] aiSlots)
	{
		for (var team = 0; team < 4; team++)
		{
			if (teams[team].TeamType != TeamType.TEAM_TYPE_COMPUTER)
				continue;

			var ai = new SaveAiPlayer();
			ai.PlayerTeam = L.Reader.ReadUInt16();
			ai.Strategy = L.Reader.ReadByte();
			ai.Field3 = L.Reader.ReadInt16();
			ai.Field5 = L.Reader.ReadInt16();
			ai.Field7 = L.Reader.ReadInt16();
			ai.TargetTeam = L.Reader.ReadInt16();

			var spottedCount = L.ReadObjectCount();
			for (var i = 0; i < spottedCount; i++)
			{
				var su = new SaveSpottedUnit
				{
					Unit = L.ReadObject<SaveUnitInfo>(),
					Team = L.Reader.ReadUInt16(),
					VisibleToTeam = L.Reader.ReadBoolean(),
					LastX = L.Reader.ReadInt16(),
					LastY = L.Reader.ReadInt16()
				};
				ai.SpottedUnits.Add(su);
			}

			var infoFlag = L.ReadObjectCount();
			if (infoFlag != 0)
			{
				var map = new byte[MapWidth, MapHeight];
				for (var x = 0; x < MapWidth; x++)
				{
					var col = L.Reader.ReadBytes(MapHeight);
					for (var y = 0; y < MapHeight; y++)
						map[x, y] = col[y];
				}

				ai.InfoMap = map;
			}

			var mineFlag = L.ReadObjectCount();
			if (mineFlag != 0)
			{
				var mine = new sbyte[MapWidth, MapHeight];
				for (var x = 0; x < MapWidth; x++)
				{
					var col = L.Reader.ReadBytes(MapHeight);
					for (var y = 0; y < MapHeight; y++)
						mine[x, y] = (sbyte)col[y];
				}

				ai.MineMap = mine;
			}

			ai.TargetLocationX = L.Reader.ReadInt16();
			ai.TargetLocationY = L.Reader.ReadInt16();
			aiSlots[team] = ai;
		}
	}
}