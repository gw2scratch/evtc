using GW2Scratch.ArcdpsLogManager.Logs;
using System;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public interface IFinishableEncounter
{
	bool IsSatisfiedBy(IEnumerable<LogData> logs);
	bool IsChallengeModeSatisfiedBy(IEnumerable<LogData> logs);
	EncounterAvailability GetNormalModeAvailability(DateOnly resetDate);
	EncounterAvailability GetChallengeModeAvailability(DateOnly resetDate);
}