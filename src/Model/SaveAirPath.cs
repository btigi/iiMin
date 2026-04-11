namespace ii.Min.Model;

public class SaveAirPath : SaveUnitPath
{
	public short Length { get; set; }
	public byte Angle { get; set; }
	public short PixelXStart { get; set; }
	public short PixelYStart { get; set; }
	public short XEnd { get; set; }
	public short YEnd { get; set; }
	public int XStep { get; set; }
	public int YStep { get; set; }
	public int DeltaX { get; set; }
	public int DeltaY { get; set; }

	public override void Deserialize(V70Loader l)
	{
		Length = l.Reader.ReadInt16();
		Angle = l.Reader.ReadByte();
		PixelXStart = l.Reader.ReadInt16();
		PixelYStart = l.Reader.ReadInt16();
		XEnd = l.Reader.ReadInt16();
		YEnd = l.Reader.ReadInt16();
		XStep = l.Reader.ReadInt32();
		YStep = l.Reader.ReadInt32();
		DeltaX = l.Reader.ReadInt32();
		DeltaY = l.Reader.ReadInt32();
	}
}
