using System;
using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Statistics
{
	public class LogStatistics
	{
		public DateTimeOffset FightStart { get; }
		public Player LogAuthor { get; }

		public IEnumerable<PhaseStats> PhaseStats { get; }
		public string EncounterName { get; }
		public EncounterResult EncounterResult { get; }

		public long FightTimeMs { get; }
		public string LogVersion { get; }

		public IReadOnlyDictionary<string, int> EventCounts { get; }
		public IEnumerable<Agent> Agents { get; }
		public IEnumerable<Skill> Skills { get; }

		public SquadDamageData FullFightSquadDamageData { get; }
		public IEnumerable<TargetSquadDamageData> FullFightTargetDamageData { get; }

		public LogStatistics(DateTimeOffset fightStart, Player logAuthor, IEnumerable<PhaseStats> phaseStats,
			SquadDamageData fullFightSquadDamageData, IEnumerable<TargetSquadDamageData> fullFightTargetDamageData,
			EncounterResult encounterResult, string encounterName, string logVersion,
			IReadOnlyDictionary<string, int> eventCounts, IEnumerable<Agent> agents, IEnumerable<Skill> skills)
		{
			EncounterName = encounterName;
			LogVersion = logVersion;
			EncounterResult = encounterResult;
			EventCounts = eventCounts;
			FightStart = fightStart;
			LogAuthor = logAuthor;
			FullFightSquadDamageData = fullFightSquadDamageData;
			FullFightTargetDamageData = fullFightTargetDamageData.ToArray();

			PhaseStats = phaseStats as PhaseStats[] ?? phaseStats.ToArray();
			FightTimeMs = PhaseStats.Sum(x => x.PhaseDuration);
			Agents = agents as Agent[] ?? agents.ToArray();
			Skills = skills as Skill[] ?? skills.ToArray();
		}
	}
}