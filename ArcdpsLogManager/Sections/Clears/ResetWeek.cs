using System;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class ResetWeek(DateOnly reset)
{
	public DateOnly Reset { get; set; } = reset;
	public int FinishedNormalEncounters { get; set; } = 0;
	public int FinishedChallengeModeEncounters { get; set; } = 0;
}