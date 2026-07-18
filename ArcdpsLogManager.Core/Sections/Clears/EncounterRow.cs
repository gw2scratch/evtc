using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class EncounterRow(string name, List<IFinishableEncounter> encounters)
{
	public string Name { get; } = name;
	public List<IFinishableEncounter> Encounters { get; } = encounters;
}