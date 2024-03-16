using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class EncounterGroup(EncounterGroupId id, string name, IReadOnlyList<EncounterRow> encounters)
{
	public EncounterGroupId Id { get; } = id;
	public string Name { get; } = name;
	public IReadOnlyList<EncounterRow> Rows { get; } = encounters;
}