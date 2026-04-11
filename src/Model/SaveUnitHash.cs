namespace ii.Min.Model;

public class SaveUnitHash
{
	public ushort HashSize { get; set; }
	public List<SaveUnitInfo>[] Buckets { get; set; } = [];
}