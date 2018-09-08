using System;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Statistics;

namespace ScratchEVTCParser
{
	public class StatisticsCalculator
	{
		public LogStatistics GetStatistics(Log log)
		{
			var boss = log.Boss;
			var fightStartEvent = log.Events.OfType<AgentEnterCombatEvent>().First(x => x.Agent == boss);
			var fightEndEvent = log.Events.OfType<AgentDeadEvent>().FirstOrDefault(x => x.Agent == boss) ??
			                    log.Events.Last();

			long fightTimeMs = fightEndEvent.Time - fightStartEvent.Time;

			//var bossDamages = log.Events.OfType<DamageEvent>().Where(x => x.Defender == boss)
			//	.GroupBy(x => x.Attacker, (player, events) => (player, events.Sum(x => x.Damage)));

			int bossDamage = log.Events.OfType<DamageEvent>().Where(x => x.Defender == boss).Sum(x => x.Damage);
			int bossConditionDamage = log.Events.OfType<BuffDamageEvent>().Where(x => x.Defender == boss).Sum(x => x.Damage);
			int bossPhysicalDamage = log.Events.OfType<PhysicalDamageEvent>().Where(x => x.Defender == boss).Sum(x => x.Damage);

			float bossDps = bossDamage * 1000f / fightTimeMs;
			float bossConditionDps = bossConditionDamage * 1000f / fightTimeMs;
			float bossPhysicalDps = bossPhysicalDamage * 1000f / fightTimeMs;

			return new LogStatistics(fightTimeMs, bossDps, bossConditionDps, bossPhysicalDps);
		}
	}
}