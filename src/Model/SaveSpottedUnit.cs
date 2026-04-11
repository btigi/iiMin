namespace ii.Min.Model;

public class SaveSpottedUnit
{
	public SaveUnitInfo? Unit { get; set; }
	public ushort Team { get; set; }
	public bool VisibleToTeam { get; set; }
	public short LastX { get; set; }
	public short LastY { get; set; }
}
