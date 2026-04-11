namespace ii.Min.Model;

public class SaveTeamUnits
{
	public short Gold { get; set; }
	public SaveUnitValues?[] BaseUnitValues { get; set; } = new SaveUnitValues?[Constants.UnitCount];
	public SaveUnitValues?[] CurrentUnitValues { get; set; } = new SaveUnitValues?[Constants.UnitCount];
	public List<SaveComplex> Complexes { get; } = [];
}