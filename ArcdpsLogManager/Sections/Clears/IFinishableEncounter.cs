using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public interface IFinishableEncounter
{
	bool IsSatisfiedBy(IEnumerable<LogData> logs);
	bool IsChallengeModeSatisfiedBy(IEnumerable<LogData> logs);
	bool HasNormalMode { get; }
	bool HasChallengeMode { get; }
	Category Category { get; }
}

public class NormalEncounter(Encounter encounter, bool hasChallengeMode, Category category) : IFinishableEncounter
{
	public Encounter Encounter { get; } = encounter;
	public bool HasNormalMode => true;
	public bool HasChallengeMode { get; } = hasChallengeMode;
	public Category Category { get; } = category;

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

public class UnsupportedEncounter(string name, Category category) : IFinishableEncounter
{
	public string Name { get; } = name;
	public bool HasNormalMode => false;
	public bool HasChallengeMode => false;
	public Category Category { get; } = category;

	public bool IsSatisfiedBy(IEnumerable<LogData> logs)
	{
		throw new NotSupportedException();
	}

	public bool IsChallengeModeSatisfiedBy(IEnumerable<LogData> logs)
	{
		throw new NotSupportedException();
	}
}

public class MultipartEncounter(string name, IEnumerable<Encounter> encounters, bool hasChallengeMode, Category category) : IFinishableEncounter
{
	public string Name { get; } = name;
	private IEnumerable<Encounter> Encounters { get; } = encounters;
	public bool HasNormalMode => true;
	public bool HasChallengeMode { get; } = hasChallengeMode;
	public Category Category { get; } = category;

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