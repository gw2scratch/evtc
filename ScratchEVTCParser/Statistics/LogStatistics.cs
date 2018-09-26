using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;

namespace ScratchEVTCParser.Statistics
{
	public class LogStatistics
	{
		public IEnumerable<PhaseStats> PhaseStats { get; }
		public string EncounterName { get; }
		public EncounterResult EncounterResult { get; }

		public long FightTimeMs { get; }
		public string LogVersion { get; }

		public IReadOnlyDictionary<string, int> EventCounts { get; }
		public IEnumerable<Agent> Agents { get; }

		public SquadDamageData FullFightSquadDamageData { get; }
		public IEnumerable<TargetSquadDamageData> FullFightTargetDamageData { get; }

		public LogStatistics(IEnumerable<PhaseStats> phaseStats, SquadDamageData fullFightSquadDamageData,
			IEnumerable<TargetSquadDamageData> fullFightTargetDamageData, EncounterResult encounterResult,
			string encounterName, string logVersion, IReadOnlyDictionary<string, int> eventCounts,
			IEnumerable<Agent> agents)
		{
			EncounterName = encounterName;
			LogVersion = logVersion;
			EncounterResult = encounterResult;
			EventCounts = eventCounts;
			FullFightSquadDamageData = fullFightSquadDamageData;
			FullFightTargetDamageData = fullFightTargetDamageData.ToArray();

			PhaseStats = phaseStats as PhaseStats[] ?? phaseStats.ToArray();
			FightTimeMs = PhaseStats.Sum(x => x.PhaseDuration);
			Agents = agents as Agent[] ?? agents.ToArray();
		}
	}
}