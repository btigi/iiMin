namespace ii.Min.Model;

public class SaveUnitInfo : IV70Serializable
{
	public ushort UnitType { get; set; }
	public ushort Id { get; set; }
	public uint Flags { get; set; }
	public ushort X { get; set; }
	public ushort Y { get; set; }
	public short GridX { get; set; }
	public short GridY { get; set; }
	public string Name { get; set; } = string.Empty;
	public short ShadowOffsetX { get; set; }
	public short ShadowOffsetY { get; set; }
	public TeamIndex Team { get; set; }
	public byte UnitId { get; set; }
	public byte Brightness { get; set; }
	public byte Angle { get; set; }
	public byte[] VisibleToTeam { get; } = new byte[5];
	public byte[] SpottedByTeam { get; } = new byte[5];
	public byte MaxVelocity { get; set; }
	public byte Velocity { get; set; }
	public byte Sound { get; set; }
	public byte ScalerAdjust { get; set; }
	public int SpriteUlx { get; set; }
	public int SpriteUly { get; set; }
	public int SpriteLrx { get; set; }
	public int SpriteLry { get; set; }
	public int ShadowUlx { get; set; }
	public int ShadowUly { get; set; }
	public int ShadowLrx { get; set; }
	public int ShadowLry { get; set; }
	public byte TurretAngle { get; set; }
	public sbyte TurretOffsetX { get; set; }
	public sbyte TurretOffsetY { get; set; }
	public short TotalImages { get; set; }
	public short ImageBase { get; set; }
	public short TurretImageBase { get; set; }
	public short FiringImageBase { get; set; }
	public short ConnectorImageBase { get; set; }
	public short ImageIndex { get; set; }
	public short TurretImageIndex { get; set; }
	public short ImageIndexMax { get; set; }
	public OrderType Orders { get; set; }
	public OrderStateType State { get; set; }
	public OrderType PriorOrders { get; set; }
	public byte PriorState { get; set; }
	public byte LayingState { get; set; }
	public short TargetGridX { get; set; }
	public short TargetGridY { get; set; }
	public byte BuildTime { get; set; }
	public byte TotalMining { get; set; }
	public byte RawMining { get; set; }
	public byte FuelMining { get; set; }
	public byte GoldMining { get; set; }
	public byte RawMiningMax { get; set; }
	public byte GoldMiningMax { get; set; }
	public byte FuelMiningMax { get; set; }
	public byte Hits { get; set; }
	public byte Speed { get; set; }
	public byte Shots { get; set; }
	public byte MoveAndFire { get; set; }
	public short Storage { get; set; }
	public byte Ammo { get; set; }
	public byte TargetingMode { get; set; }
	public byte EnterMode { get; set; }
	public byte Cursor { get; set; }
	public sbyte RecoilDelay { get; set; }
	public byte DelayedReaction { get; set; }
	public bool DamagedThisTurn { get; set; }
	public byte ResearchTopic { get; set; }
	public byte Moved { get; set; }
	public bool Bobbed { get; set; }
	public byte ShakeEffectState { get; set; }
	public byte Engine { get; set; }
	public byte Weapon { get; set; }
	public byte Comm { get; set; }
	public byte FuelDistance { get; set; }
	public byte MoveFraction { get; set; }
	public bool Energized { get; set; }
	public byte RepeatBuild { get; set; }
	public ushort BuildRate { get; set; }
	public bool DisabledReactionFire { get; set; }
	public bool AutoSurvey { get; set; }
	public uint Unknown { get; set; }
	public SaveUnitPath? Path { get; set; }
	public ushort Connectors { get; set; }
	public SaveUnitValues? BaseValues { get; set; }
	public SaveComplex? Complex { get; set; }
	public SaveUnitInfo? ParentUnit { get; set; }
	public SaveUnitInfo? EnemyUnit { get; set; }
	public List<ushort> BuildList { get; } = [];

	public void Deserialize(V70Loader l)
	{
		UnitType = l.Reader.ReadUInt16();
		Id = l.Reader.ReadUInt16();
		Flags = l.Reader.ReadUInt32();
		X = l.Reader.ReadUInt16();
		Y = l.Reader.ReadUInt16();
		GridX = l.Reader.ReadInt16();
		GridY = l.Reader.ReadInt16();

		var nameLen = l.Reader.ReadUInt16();
		if (nameLen > 0)
		{
			var raw = l.Reader.ReadBytes(nameLen);
			Name = V70Loader.DecodeName(raw);
		}
		else
		{
			Name = string.Empty;
		}

		ShadowOffsetX = l.Reader.ReadInt16();
		ShadowOffsetY = l.Reader.ReadInt16();
		Team = (TeamIndex)l.Reader.ReadByte();
		UnitId = l.Reader.ReadByte();
		Brightness = l.Reader.ReadByte();
		Angle = l.Reader.ReadByte();
		for (var i = 0; i < 5; i++)
		{ 
			VisibleToTeam[i] = l.Reader.ReadByte();
		}
		for (var i = 0; i < 5; i++)
		{ 
			SpottedByTeam[i] = l.Reader.ReadByte();
		}
		MaxVelocity = l.Reader.ReadByte();
		Velocity = l.Reader.ReadByte();
		Sound = l.Reader.ReadByte();
		ScalerAdjust = l.Reader.ReadByte();
		SpriteUlx = l.Reader.ReadInt32();
		SpriteUly = l.Reader.ReadInt32();
		SpriteLrx = l.Reader.ReadInt32();
		SpriteLry = l.Reader.ReadInt32();
		ShadowUlx = l.Reader.ReadInt32();
		ShadowUly = l.Reader.ReadInt32();
		ShadowLrx = l.Reader.ReadInt32();
		ShadowLry = l.Reader.ReadInt32();
		TurretAngle = l.Reader.ReadByte();
		TurretOffsetX = l.Reader.ReadSByte();
		TurretOffsetY = l.Reader.ReadSByte();
		TotalImages = l.Reader.ReadInt16();
		ImageBase = l.Reader.ReadInt16();
		TurretImageBase = l.Reader.ReadInt16();
		FiringImageBase = l.Reader.ReadInt16();
		ConnectorImageBase = l.Reader.ReadInt16();
		ImageIndex = l.Reader.ReadInt16();
		TurretImageIndex = l.Reader.ReadInt16();
		ImageIndexMax = l.Reader.ReadInt16();
		Orders = (OrderType)l.Reader.ReadByte();
		State = (OrderStateType)l.Reader.ReadByte();
		PriorOrders = (OrderType)l.Reader.ReadByte();
		PriorState = l.Reader.ReadByte();
		LayingState = l.Reader.ReadByte();
		TargetGridX = l.Reader.ReadInt16();
		TargetGridY = l.Reader.ReadInt16();
		BuildTime = l.Reader.ReadByte();
		TotalMining = l.Reader.ReadByte();
		RawMining = l.Reader.ReadByte();
		FuelMining = l.Reader.ReadByte();
		GoldMining = l.Reader.ReadByte();
		RawMiningMax = l.Reader.ReadByte();
		GoldMiningMax = l.Reader.ReadByte();
		FuelMiningMax = l.Reader.ReadByte();
		Hits = l.Reader.ReadByte();
		Speed = l.Reader.ReadByte();
		Shots = l.Reader.ReadByte();
		MoveAndFire = l.Reader.ReadByte();
		Storage = l.Reader.ReadInt16();
		Ammo = l.Reader.ReadByte();
		TargetingMode = l.Reader.ReadByte();
		EnterMode = l.Reader.ReadByte();
		Cursor = l.Reader.ReadByte();
		RecoilDelay = l.Reader.ReadSByte();
		DelayedReaction = l.Reader.ReadByte();
		DamagedThisTurn = l.Reader.ReadBoolean();
		ResearchTopic = l.Reader.ReadByte();
		Moved = l.Reader.ReadByte();
		Bobbed = l.Reader.ReadBoolean();
		ShakeEffectState = l.Reader.ReadByte();
		Engine = l.Reader.ReadByte();
		Weapon = l.Reader.ReadByte();
		Comm = l.Reader.ReadByte();
		FuelDistance = l.Reader.ReadByte();
		MoveFraction = l.Reader.ReadByte();
		Energized = l.Reader.ReadBoolean();
		RepeatBuild = l.Reader.ReadByte();
		BuildRate = l.Reader.ReadUInt16();
		DisabledReactionFire = l.Reader.ReadBoolean();
		AutoSurvey = l.Reader.ReadBoolean();
		Unknown = l.Reader.ReadUInt32();
		Path = l.ReadUnitPath();
		Connectors = l.Reader.ReadUInt16();
		BaseValues = l.ReadObject<SaveUnitValues>();
		Complex = l.ReadObject<SaveComplex>();
		ParentUnit = l.ReadObject<SaveUnitInfo>();
		EnemyUnit = l.ReadObject<SaveUnitInfo>();

		var bc = l.ReadObjectCount();
		for (var i = 0; i < bc; i++)
		{ 
			BuildList.Add(l.Reader.ReadUInt16());
		}
	}
}