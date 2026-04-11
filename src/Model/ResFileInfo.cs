namespace ii.Min.Model;

internal class ResFileInfo
{
    public string Name { get; set; } = string.Empty;
    public int Offset { get; set; }
    public int Length { get; set; }
    public byte[] Content { get; set; } = [];
}