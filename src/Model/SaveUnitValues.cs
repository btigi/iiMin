namespace ii.Min.Model;

public class SaveUnitValues : IV70Serializable
{
	public ushort Turns { get; set; }
	public ushort Hits { get; set; }
	public ushort Armor { get; set; }
	public ushort Attack { get; set; }
	public ushort Speed { get; set; }
	public ushort Range { get; set; }
	public ushort Rounds { get; set; }
	public byte MoveAndFire { get; set; }
	public ushort Scan { get; set; }
	public ushort Storage { get; set; }
	public ushort Ammo { get; set; }
	public ushort AttackRadius { get; set; }
	public ushort AgentAdjust { get; set; }
	public ushort Version { get; set; }
	public byte UnitsBuilt { get; set; }

	public void Deserialize(V70Loader l)
	{
		Turns = l.Reader.ReadUInt16();
		Hits = l.Reader.ReadUInt16();
		Armor = l.Reader.ReadUInt16();
		Attack = l.Reader.ReadUInt16();
		Speed = l.Reader.ReadUInt16();
		Range = l.Reader.ReadUInt16();
		Rounds = l.Reader.ReadUInt16();
		MoveAndFire = l.Reader.ReadByte();
		Scan = l.Reader.ReadUInt16();
		Storage = l.Reader.ReadUInt16();
		Ammo = l.Reader.ReadUInt16();
		AttackRadius = l.Reader.ReadUInt16();
		AgentAdjust = l.Reader.ReadUInt16();
		Version = l.Reader.ReadUInt16();
		UnitsBuilt = l.Reader.ReadByte();
	}
}