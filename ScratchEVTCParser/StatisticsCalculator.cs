using System;
using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics;

namespace ScratchEVTCParser
{
	public class StatisticsCalculator
	{
		public LogStatistics GetStatistics(Log log)
		{
			var phaseStats = new List<PhaseStats>();
			foreach (var phase in log.Encounter.GetPhases())
			{
				var targetDamageData = new List<TargetDamageData>();

				long phaseDuration = phase.EndTime - phase.StartTime;
				foreach (var target in log.Encounter.ImportantAgents)
				{
					var physicalBossDamages = phase.Events
						.OfType<PhysicalDamageEvent>()
						.Where(x => x.Defender == target)
						.GroupBy(x => x.Attacker, (attacker, events) => (attacker, events.Sum(x => x.Damage)));

					var conditionBossDamages = phase.Events
						.OfType<BuffDamageEvent>()
						.Where(x => x.Defender == target)
						.GroupBy(x => x.Attacker, (attacker, events) => (attacker, events.Sum(x => x.Damage)));

					var damageDataByAttacker = new Dictionary<Agent, DamageData>();

					foreach ((var attacker, int damage) in physicalBossDamages)
					{
						if (attacker == null)
						{
							continue; // TODO: Save as unknown damage
						}

						var mainMaster = attacker;
						while (mainMaster.Master != null)
						{
							mainMaster = attacker.Master;
						}

						if (!damageDataByAttacker.ContainsKey(mainMaster))
						{
							damageDataByAttacker[mainMaster] = new DamageData(mainMaster, phaseDuration, 0, 0);
						}

						damageDataByAttacker[mainMaster] += new DamageData(mainMaster, phaseDuration, damage, 0);
					}

					foreach ((var attacker, int damage) in conditionBossDamages)
					{
						if (attacker == null)
						{
							continue; // TODO: Save as unknown damage
						}

						var mainMaster = attacker;
						while (mainMaster.Master != null)
						{
							mainMaster = attacker.Master;
						}

						if (!damageDataByAttacker.ContainsKey(mainMaster))
						{
							damageDataByAttacker[mainMaster] = new DamageData(mainMaster, phaseDuration, 0, 0);
						}

						damageDataByAttacker[mainMaster] += new DamageData(mainMaster, phaseDuration, 0, damage);
					}

					targetDamageData.Add(new TargetDamageData(target, phaseDuration, damageDataByAttacker.Values));
				}

				phaseStats.Add(new PhaseStats(phase.Name, phase.StartTime, phase.EndTime, targetDamageData));
			}

			var eventCounts = new Dictionary<Type, int>();
			foreach (var e in log.Events)
			{
				var type = e.GetType();
				if (!eventCounts.ContainsKey(type))
				{
					eventCounts[type] = 0;
				}

				eventCounts[type]++;
			}

			var eventCountsByName =
				eventCounts.Select(x => (x.Key.Name, x.Value)).ToDictionary(x => x.Item1, x => x.Item2);

			return new LogStatistics(phaseStats, log.Encounter.GetResult(), log.Encounter.GetName(), log.EVTCVersion,
				eventCountsByName, log.Agents);
		}
	}
}