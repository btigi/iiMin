namespace ii.Min.Model;

public class ImageData
{
	public short Width { get; set; }
	public short Height { get; set; }
	public short HotspotX { get; set; }
	public short HotspotY { get; set; }
	public byte[] Data { get; set; } = null!;
	public byte[] Alpha { get; set; } = null!;
}