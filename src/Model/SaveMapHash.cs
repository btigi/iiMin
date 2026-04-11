namespace ii.Min.Model;

public class SaveMapHash
{
	public ushort HashSize { get; set; }
	public short XShift { get; set; }
	public List<SaveMapHashCell>[] Buckets { get; set; } = [];
}