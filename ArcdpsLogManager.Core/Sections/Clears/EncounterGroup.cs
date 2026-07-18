using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class EncounterGroup(EncounterCategory category, string name, IReadOnlyList<EncounterRow> encounters)
{
	public EncounterCategory Category { get; } = category;
	public string Name { get; } = name;
	public IReadOnlyList<EncounterRow> Rows { get; } = encounters;
}