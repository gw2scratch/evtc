using System;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCInspector
{
	public class Statistics
	{
		public DateTimeOffset FightStart { get; }
		public Player LogAuthor { get; }

		public Encounter Encounter { get; }
		public EncounterResult EncounterResult { get; }
		public EncounterMode EncounterMode { get; }

		public TimeSpan EncounterDuration { get; }
		public string LogVersion { get; }
		
		public List<LogError> LogErrors { get; }

		public Statistics(DateTimeOffset fightStart, Player logAuthor, EncounterResult encounterResult, EncounterMode encounterMode, Encounter encounter,
			string logVersion, TimeSpan encounterDuration, IEnumerable<LogError> logErrors)
		{
			Encounter = encounter;
			LogVersion = logVersion;
			EncounterResult = encounterResult;
			EncounterMode = encounterMode;
			FightStart = fightStart;
			LogAuthor = logAuthor;
			EncounterDuration = encounterDuration;
			LogErrors = logErrors.ToList();
		}
	}
}