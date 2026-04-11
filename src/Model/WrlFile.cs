namespace ii.Min.Model;

public class WrlFile
{
	public short Version { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public int TileCount { get; set; }
	public byte[] TileData { get; set; } = [];
	public byte[] Palette { get; set; } = [];
	public byte[] TerrainInfo { get; set; } = [];
	public byte[] Minimap { get; set; } = [];
	public ushort[] TileLookup { get; set; } = [];
}