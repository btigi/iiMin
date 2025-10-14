namespace ii.Min;

public class WrlProcessor
{
    public WrlFile Read(string filename)
    {
        const int TileDimension = 64;
        const int PaletteLength = 768;

        var result = new WrlFile();
        using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);

        result.Signature = new string(br.ReadChars(3));
        if (result.Signature != "WRL")
        {
            return result;
        }

        result.Version = br.ReadInt16();
        if (result.Version != 1)
        {
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

        result.TerrainInfo = br.ReadBytes(result.TileCount);
        return result;
    }

    public void Write(string filename, WrlFile wrlFile)
    {
        using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        using var bw = new BinaryWriter(fs);

        // Write signature (3 chars)
        bw.Write(wrlFile.Signature.ToCharArray());

        // Write version
        bw.Write(wrlFile.Version);

        // Write coordinates
        bw.Write((short)wrlFile.X);
        bw.Write((short)wrlFile.Y);

        // Write first section
        bw.Write(wrlFile.Unknown1);
        bw.Write(wrlFile.Unknown2);

        // Write second section
        bw.Write(wrlFile.Unknown3);
        bw.Write(wrlFile.Unknown4);

        // Write tile information
        bw.Write(wrlFile.Unknown5);
        bw.Write((short)wrlFile.TileCount);

        // Write tile data
        bw.Write(wrlFile.TileData);

        // Write palette
        bw.Write(wrlFile.Palette);

        // Write terrain info
        bw.Write(wrlFile.TerrainInfo);
    }
}