namespace ii.Min.Model;

public class SaveGroundPath : SaveUnitPath
{
	public short XEnd { get; set; }
	public short YEnd { get; set; }
	public uint Index { get; set; }
	public List<(sbyte X, sbyte Y)> Steps { get; } = [];

	public override void Deserialize(V70Loader l)
	{
		XEnd = l.Reader.ReadInt16();
		YEnd = l.Reader.ReadInt16();
		Index = l.Reader.ReadUInt16();
		var count = l.ReadObjectCount();
		for (var i = 0; i < count; i++)
		{
			var sx = (sbyte)l.Reader.ReadByte();
			var sy = (sbyte)l.Reader.ReadByte();
			Steps.Add((sx, sy));
		}
	}
}