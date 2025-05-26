namespace iiMin;

public class ResProcessor
{
    public List<FileInfo> Read(string filename)
    {
        const int FilenameLength = 8;

        var result = new List<FileInfo>();

        try
        {
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs);

            var signature = br.ReadChars(4);
            if (String.Join("", signature) != "RES0")
            {
                //Console.WriteLine("File does not appear a RES file");
                return result;
            }

            var fileListingOffset = br.ReadInt32();
            var fileListingCount = br.ReadInt32() / 16;

            br.BaseStream.Seek(fileListingOffset, SeekOrigin.Begin);
            for (int i = 0; i < fileListingCount; i++)
            {
                var fileInfo = new FileInfo();
                fileInfo.Name = String.Join("", br.ReadChars(FilenameLength));
                fileInfo.Name = fileInfo.Name.Trim('\0');
                fileInfo.Offset = br.ReadInt32();
                fileInfo.Length = br.ReadInt32();
                result.Add(fileInfo);
            }

            for (int i = 0; i < result.Count; i++)
            {
                var fileInfo = result[i];
                br.BaseStream.Seek(fileInfo.Offset, SeekOrigin.Begin);
                var bytes = br.ReadBytes(fileInfo.Length);
                fileInfo.Content = bytes;
            }
        }
        catch
        {
            Console.WriteLine("An unexpected error occured");
        }
        return result;
    }

    public class FileInfo
    {
        public string Name { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        public byte[] Content { get; set; }
    }
}