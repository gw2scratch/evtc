using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;

namespace ScratchEVTCParser.Statistics
{
	public class LogStatistics
	{
		public string EncounterName { get; }

		public long FightTimeMs { get; }
		public float BossDps { get; }
		public float BossConditionDps { get; }
		public float BossPhysicalDps { get; }
		public IReadOnlyDictionary<Agent, TargetDamageData> BossDamageByAgent { get; }
		public EncounterResult EncounterResult { get; }

		public LogStatistics(long fightTimeMs, float bossDps, float bossConditionDps, float bossPhysicalDps,
			Dictionary<Agent, TargetDamageData> bossDamageByAgent, EncounterResult encounterResult)
		{
			FightTimeMs = fightTimeMs;
			BossDps = bossDps;
			BossConditionDps = bossConditionDps;
			BossPhysicalDps = bossPhysicalDps;
			BossDamageByAgent = bossDamageByAgent;
			EncounterResult = encounterResult;
		}
	}
}