using System;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class ResetWeek
{
	public ResetWeek(DateOnly reset)
	{
		Reset = reset;
		foreach (var category in Enum.GetValues<Category>())
		{
			FinishedNormalModesByCategory[category] = 0;
			FinishedChallengeModesByCategory[category] = 0;
		}
	}

	public DateOnly Reset { get; }
	public Dictionary<Category, int> FinishedNormalModesByCategory { get; } = new Dictionary<Category, int>();
	public Dictionary<Category, int> FinishedChallengeModesByCategory { get; } = new Dictionary<Category, int>();
}