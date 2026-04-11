using ii.Min.Model;

namespace ii.Min;

public class WrlProcessor
{
	private const string Signature = "WRL";
	private const int TileDimension = 64;
	private const int PaletteLength = 768;

	public WrlFile Read(string filename)
	{
		var fileData = File.ReadAllBytes(filename);
		return Read(fileData);
	}

	public WrlFile Read(byte[] fileData)
	{
		using var stream = new MemoryStream(fileData);
		using var reader = new BinaryReader(stream);

		var result = new WrlFile();

		var signature = new string(reader.ReadChars(3));
		if (signature != Signature)
		{
			throw new InvalidDataException($"Invalid WRL file. Expected signature 'WRL', got 0x{signature:X8}.");
		}

		result.Version = reader.ReadInt16();
		result.Width = reader.ReadInt16();
		result.Height = reader.ReadInt16();

		var cellCount = result.Width * result.Height;
		result.Minimap = reader.ReadBytes(cellCount);

		result.TileLookup = new ushort[cellCount];
		for (var i = 0; i < cellCount; i++)
		{
			result.TileLookup[i] = reader.ReadUInt16();
		}

		result.TileCount = reader.ReadUInt16();
		result.TileData = reader.ReadBytes(result.TileCount * TileDimension * TileDimension);
		result.Palette = reader.ReadBytes(PaletteLength);
		result.TerrainInfo = reader.ReadBytes(result.TileCount);
		return result;
	}

	public void Write(string filename, WrlFile wrlFile)
	{
		using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
		using var bw = new BinaryWriter(fs);

		bw.Write("WRL".ToCharArray());
		bw.Write(wrlFile.Version);
		bw.Write((short)wrlFile.Width);
		bw.Write((short)wrlFile.Height);
		bw.Write(wrlFile.Minimap);
		foreach (var index in wrlFile.TileLookup)
		{ 
			bw.Write(index);
		}
		bw.Write((ushort)wrlFile.TileCount);
		bw.Write(wrlFile.TileData);
		bw.Write(wrlFile.Palette);
		bw.Write(wrlFile.TerrainInfo);
	}
}