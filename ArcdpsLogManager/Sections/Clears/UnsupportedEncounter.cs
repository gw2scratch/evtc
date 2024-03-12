using GW2Scratch.ArcdpsLogManager.Logs;
using System;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Sections.Clears;

public class UnsupportedEncounter(string name, Category category) : IFinishableEncounter
{
	public EncounterAvailability GetNormalModeAvailability(DateOnly resetDate) => EncounterAvailability.NotLogged;
	public EncounterAvailability GetChallengeModeAvailability(DateOnly resetDate) => EncounterAvailability.NotLogged;

	public string Name { get; } = name;
	public Category Category { get; } = category;

	public bool IsSatisfiedBy(IEnumerable<LogData> logs) => false;
	public bool IsChallengeModeSatisfiedBy(IEnumerable<LogData> logs) => false;
}