namespace ii.Min;

public partial class ResProcessor
{
    public List<(string filename, byte[] bytes)> Read(string filename)
    {
        const int FilenameLength = 8;

        var result = new List<(string filename, byte[] bytes)>();
        using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);

        var signature = br.ReadChars(4);
        if (String.Join("", signature) != "RES0")
        {
            throw new InvalidDataException("Invalid RES file signature");
        }

        var fileListingOffset = br.ReadInt32();
        var fileListingCount = br.ReadInt32() / 16;

        var fileInfos = new List<FileInfo>();
        br.BaseStream.Seek(fileListingOffset, SeekOrigin.Begin);
        for (int i = 0; i < fileListingCount; i++)
        {
            var fileInfo = new FileInfo();
            fileInfo.Name = String.Join("", br.ReadChars(FilenameLength));
            fileInfo.Name = fileInfo.Name.Trim('\0');
            fileInfo.Offset = br.ReadInt32();
            fileInfo.Length = br.ReadInt32();
            fileInfos.Add(fileInfo);
        }

        foreach (var fileInfo in fileInfos)
        {
            br.BaseStream.Seek(fileInfo.Offset, SeekOrigin.Begin);
            var bytes = br.ReadBytes(fileInfo.Length);
            result.Add((fileInfo.Name, bytes));
        }

        return result;
    }

    public void Write(string filename, List<(string filename, byte[] bytes)> files)
    {
        const int FilenameLength = 8;
        const int HeaderSize = 12; // Signature (4) + FileListingOffset (4) + FileListingSize (4)
        const int FileInfoSize = 16; // Name (8) + Offset (4) + Length (4)

        using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        using var bw = new BinaryWriter(fs);

        bw.Write("RES0".ToCharArray());

        var currentOffset = HeaderSize;
        var fileInfos = new List<(string name, int offset, int length)>();

        foreach (var file in files)
        {
            fileInfos.Add((file.filename, currentOffset, file.bytes.Length));
            currentOffset += file.bytes.Length;
        }

        var fileListingOffset = currentOffset;
        var fileListingSize = fileInfos.Count * FileInfoSize;

        bw.Write(fileListingOffset);
        bw.Write(fileListingSize);

        foreach (var file in files)
        {
            bw.Write(file.bytes);
        }

        foreach (var fileInfo in fileInfos)
        {
            var nameChars = fileInfo.name.PadRight(FilenameLength, '\0').Substring(0, FilenameLength).ToCharArray();
            bw.Write(nameChars);
            bw.Write(fileInfo.offset);
            bw.Write(fileInfo.length);
        }
    }
}