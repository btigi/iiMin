namespace ii.Min.Model;

public class SaveMapHashCell
{
	public ushort X { get; set; }
	public ushort Y { get; set; }
	public List<SaveUnitInfo> Units { get; } = [];
}