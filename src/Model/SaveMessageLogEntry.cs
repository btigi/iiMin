namespace ii.Min.Model;

public class SaveMessageLogEntry
{
	public string Text { get; set; } = string.Empty;
	public SaveUnitInfo? Unit { get; set; }
	public short PointX { get; set; }
	public short PointY { get; set; }
	public bool IsAlertMessage { get; set; }
	public ushort ResourceId { get; set; }
}