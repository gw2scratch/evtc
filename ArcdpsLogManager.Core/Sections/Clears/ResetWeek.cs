using System;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class ResetWeek
{
	public ResetWeek(DateOnly reset)
	{
		Reset = reset;
		foreach (var category in Enum.GetValues<EncounterCategory>())
		{
			FinishedNormalModesByCategory[category] = 0;
			FinishedChallengeModesByCategory[category] = 0;
		}
	}

	public DateOnly Reset { get; }
	public Dictionary<EncounterCategory, int> FinishedNormalModesByCategory { get; } = new Dictionary<EncounterCategory, int>();
	public Dictionary<EncounterCategory, int> FinishedChallengeModesByCategory { get; } = new Dictionary<EncounterCategory, int>();
}