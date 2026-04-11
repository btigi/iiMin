namespace ii.Min.Model;

public class SaveAiPlayer
{
	public ushort PlayerTeam { get; set; }
	public byte Strategy { get; set; }
	public short Field3 { get; set; }
	public short Field5 { get; set; }
	public short Field7 { get; set; }
	public short TargetTeam { get; set; }
	public List<SaveSpottedUnit> SpottedUnits { get; } = [];
	public byte[,]? InfoMap { get; set; }
	public sbyte[,]? MineMap { get; set; }
	public short TargetLocationX { get; set; }
	public short TargetLocationY { get; set; }
}
