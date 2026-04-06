using GW2Scratch.EVTCAnalytics.GameData;
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
		public int? GameBuild { get; }
		public GameLanguage GameLanguage { get; }
		public int? GameShardId { get; }
		public int? UpperShardId { get; }
		public int? UserWorldId0 { get; }
		public int? UserWorldId1 { get; }
		public int? MapId { get; }
		public int? FractalScale { get; }
		public string ArcdpsBuild { get; }

		public Statistics(DateTimeOffset fightStart, Player logAuthor, EncounterResult encounterResult,
			EncounterMode encounterMode, Encounter encounter, string logVersion, TimeSpan encounterDuration,
			int? gameBuild, GameLanguage language,
			int? gameShardId, int? upperShardId, int? userWorldId0, int? userWorldId1, int? mapId,
			int? fractalScale, IEnumerable<LogError> logErrors, string arcDpsBuild)
		{
			Encounter = encounter;
			LogVersion = logVersion;
			EncounterResult = encounterResult;
			EncounterMode = encounterMode;
			FightStart = fightStart;
			LogAuthor = logAuthor;
			EncounterDuration = encounterDuration;
			LogErrors = logErrors.ToList();
			GameBuild = gameBuild;
			GameLanguage = language;
			GameShardId = gameShardId;
			UpperShardId = upperShardId;
			UserWorldId0 = userWorldId0;
			UserWorldId1 = userWorldId1;
			MapId = mapId;
			FractalScale = fractalScale;
			ArcdpsBuild = arcDpsBuild;
		}
	}
}