using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class EncounterColumn(string name, List<EncounterRow> rows)
{
	public string Name { get; } = name;
	public List<EncounterRow> Rows { get; } = rows;
}