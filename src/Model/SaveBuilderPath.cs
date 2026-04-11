namespace ii.Min.Model;

public class SaveBuilderPath : SaveUnitPath
{
	public short X { get; set; }
	public short Y { get; set; }

	public override void Deserialize(V70Loader l)
	{
		X = l.Reader.ReadInt16();
		Y = l.Reader.ReadInt16();
	}
}
