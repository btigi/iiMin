namespace ii.Min;

public class WrlFile
{
    public string Signature { get; set; } = string.Empty;
    public Int16 Version { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public byte Unknown1 { get; set; }
    public byte[] Unknown2 { get; set; } = Array.Empty<byte>();
    public byte Unknown3 { get; set; }
    public byte[] Unknown4 { get; set; } = Array.Empty<byte>();
    public byte Unknown5 { get; set; }
    public int TileCount { get; set; }
    public byte[] TileData { get; set; } = Array.Empty<byte>();
    public byte[] Palette { get; set; } = Array.Empty<byte>();
    public byte[] TerrainInfo { get; set; } = Array.Empty<byte>(); //  0 - land, 1 - water, 2 - shoreline, 3 - mountain
}