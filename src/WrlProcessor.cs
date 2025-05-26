namespace iiMin;

public class WrlProcessor
{
    public WrlFile Read(string filename)
    {
        const int TileDimension = 64;
        const int PaletteLength = 768;

        var result = new WrlFile();

        try
        {
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs);

            result.Signature = new string(br.ReadChars(3));
            if (result.Signature != "WRL")
            {
                //Console.WriteLine("File does not appear a WRL file");
                return result;
            }

            result.Version = br.ReadInt16();
            if (result.Version != 1)
            {
                //Console.WriteLine("File does not appear a supported WRL file version");
                return result;
            }

            result.X = br.ReadInt16();
            result.Y = br.ReadInt16();

            result.Unknown1 = br.ReadByte(); // 1 byte null marker to separate contents
            result.Unknown2 = br.ReadBytes(12544); // This seems a fixed size in all WRL files

            result.Unknown3 = br.ReadByte(); // 1 byte null marker to separate contents
            result.Unknown4 = br.ReadBytes(25085); // This seems a fixed size in all WRL files

            result.Unknown5 = br.ReadByte(); // 1 byte null marker to separate contents
            result.TileCount = br.ReadInt16();

            var tileDataSize = result.TileCount * TileDimension * TileDimension;
            result.TileData = br.ReadBytes(tileDataSize);

            result.Palette = br.ReadBytes(PaletteLength);
            //File.WriteAllBytes(Path.Combine(outputDirectory, "palette.pal"), paletteBytes);

            result.TerrainInfo = br.ReadBytes(result.TileCount);
        }
        catch
        {
            Console.WriteLine("An unexpected error occured");
        }
        return result;
    }

    public class WrlFile
    {
        public string Signature { get; set; }
        public Int16 Version { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public byte Unknown1 { get; set; }
        public byte[] Unknown2 { get; set; }
        public byte Unknown3 { get; set; }
        public byte[] Unknown4 { get; set; }
        public byte Unknown5 { get; set; }
        public int TileCount { get; set; }
        public byte[] TileData { get; set; }
        public byte[] Palette { get; set; }
        public byte[] TerrainInfo { get; set; } //  0 - land, 1 - water, 2 - shoreline, 3 - mountain
    }
}