namespace ii.Min.Model;

public class SaveComplex : IV70Serializable
{
	public short Material { get; set; }
	public short Fuel { get; set; }
	public short Gold { get; set; }
	public short Power { get; set; }
	public short Workers { get; set; }
	public short Buildings { get; set; }
	public short Id { get; set; }

	public void Deserialize(V70Loader l)
	{
		Material = l.Reader.ReadInt16();
		Fuel = l.Reader.ReadInt16();
		Gold = l.Reader.ReadInt16();
		Power = l.Reader.ReadInt16();
		Workers = l.Reader.ReadInt16();
		Buildings = l.Reader.ReadInt16();
		Id = l.Reader.ReadInt16();
	}
}
