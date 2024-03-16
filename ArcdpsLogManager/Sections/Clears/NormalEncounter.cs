using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class NormalEncounter : IFinishableEncounter
{
	private readonly DateOnly? normalModeSince;
	private readonly DateOnly? challengeModeSince;
	private readonly DateOnly? logsSince;

	public NormalEncounter(Encounter encounter, DateOnly? normalModeSince, DateOnly? challengeModeSince, DateOnly? logsSince = null)
	{
		if (normalModeSince.HasValue && normalModeSince.Value.DayOfWeek != DayOfWeek.Monday)
		{
			throw new ArgumentException("Normal mode reset date must be a Monday.", nameof(normalModeSince));
		}

		if (challengeModeSince.HasValue && challengeModeSince.Value.DayOfWeek != DayOfWeek.Monday)
		{
			throw new ArgumentException("Challenge mode reset date must be a Monday.", nameof(challengeModeSince));
		}

		if (logsSince.HasValue && logsSince.Value.DayOfWeek != DayOfWeek.Monday)
		{
			throw new ArgumentException("Logs since date must be a Monday.", nameof(logsSince));
		}

		this.normalModeSince = normalModeSince;
		this.challengeModeSince = challengeModeSince;
		this.logsSince = logsSince;
		Encounter = encounter;
	}

	public Encounter Encounter { get; }

	public EncounterAvailability GetNormalModeAvailability(DateOnly resetDate)
	{
		if (normalModeSince == null) return EncounterAvailability.DoesNotExist;
		if (resetDate < normalModeSince)
		{
			return EncounterAvailability.DoesNotExist;
		}

		return resetDate < logsSince ? EncounterAvailability.NotLogged : EncounterAvailability.Available;
	}

	public EncounterAvailability GetChallengeModeAvailability(DateOnly resetDate)
	{
		if (challengeModeSince == null) return EncounterAvailability.DoesNotExist;
		if (resetDate < challengeModeSince)
		{
			return EncounterAvailability.DoesNotExist;
		}
		return resetDate < logsSince ? EncounterAvailability.NotLogged : EncounterAvailability.Available;
	}

	public bool IsSatisfiedBy(IEnumerable<LogData> logs)
	{
		return logs.Any(log => log.ParsingStatus == ParsingStatus.Parsed && log.EncounterResult == EncounterResult.Success && log.Encounter == Encounter);
	}

	public bool IsChallengeModeSatisfiedBy(IEnumerable<LogData> logs)
	{
		return logs.Any(log => log.ParsingStatus == ParsingStatus.Parsed && log.EncounterResult == EncounterResult.Success && log.Encounter == Encounter &&
		                       log.EncounterMode == EncounterMode.Challenge);
	}
}