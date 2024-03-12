using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class MultipartEncounter : IFinishableEncounter
{
	private readonly DateOnly? normalModeSince;
	private readonly DateOnly? challengeModeSince;

	public MultipartEncounter(string name, IEnumerable<Encounter> encounters, Category category, DateOnly? normalModeSince, DateOnly? challengeModeSince)
	{
		if (normalModeSince.HasValue && normalModeSince.Value.DayOfWeek != DayOfWeek.Monday)
		{
			throw new ArgumentException("Normal mode reset date must be a Monday.", nameof(normalModeSince));
		}

		if (challengeModeSince.HasValue && challengeModeSince.Value.DayOfWeek != DayOfWeek.Monday)
		{
			throw new ArgumentException("Challenge mode reset date must be a Monday.", nameof(challengeModeSince));
		}

		this.normalModeSince = normalModeSince;
		this.challengeModeSince = challengeModeSince;
		Name = name;
		Encounters = encounters;
		Category = category;
	}

	public string Name { get; }
	private IEnumerable<Encounter> Encounters { get; }
	public Category Category { get; }

	public EncounterAvailability GetNormalModeAvailability(DateOnly resetDate)
	{
		if (normalModeSince == null) return EncounterAvailability.NotLogged;
		return resetDate < normalModeSince ? EncounterAvailability.DoesNotExist : EncounterAvailability.Available;
	}

	public EncounterAvailability GetChallengeModeAvailability(DateOnly resetDate)
	{
		if (challengeModeSince == null) return EncounterAvailability.NotLogged;
		return resetDate < challengeModeSince ? EncounterAvailability.DoesNotExist : EncounterAvailability.Available;
	}

	public bool IsSatisfiedBy(IEnumerable<LogData> logs)
	{
		return Encounters.All(encounter => logs.Any(log =>
			log.ParsingStatus == ParsingStatus.Parsed && log.EncounterResult == EncounterResult.Success && log.Encounter == encounter));
	}

	public bool IsChallengeModeSatisfiedBy(IEnumerable<LogData> logs)
	{
		return Encounters.All(encounter => logs.Any(log =>
			log.ParsingStatus == ParsingStatus.Parsed && log.EncounterResult == EncounterResult.Success && log.Encounter == encounter &&
			log.EncounterMode == EncounterMode.Challenge));
	}
}