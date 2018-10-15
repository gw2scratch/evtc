using System;
using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;
using ScratchEVTCParser.Statistics;
using ScratchEVTCParser.Statistics.Buffs;

namespace ScratchEVTCParser
{
	public class StatisticsCalculator
	{
		public BuffSimulator BuffSimulator { get; set; } = new BuffSimulator();

		public LogStatistics GetStatistics(Log log)
		{
			var phaseStats = new List<PhaseStats>();
			foreach (var phase in log.Encounter.GetPhases())
			{
				var targetDamageData = new List<TargetSquadDamageData>();
				long phaseDuration = phase.EndTime - phase.StartTime;
				foreach (var target in phase.ImportantEnemies)
				{
					var damageData = GetDamageData(
						log.Agents,
						phase.Events
							.OfType<DamageEvent>()
							.Where(x => x.Defender == target)
							.ToArray(),
						phaseDuration);
					targetDamageData.Add(new TargetSquadDamageData(target, phaseDuration, damageData.Values));
				}

				var totalDamageData = new SquadDamageData(phaseDuration,
					GetDamageData(
						log.Agents,
						phase.Events.OfType<DamageEvent>().ToArray(),
						phaseDuration).Values
				);

				phaseStats.Add(new PhaseStats(phase.Name, phase.StartTime, phase.EndTime, targetDamageData,
					totalDamageData));
			}

			var fightTime = log.Encounter.GetPhases().Sum(x => x.EndTime - x.StartTime);
			var fullFightSquadDamageData = new SquadDamageData(fightTime,
				GetDamageData(log.Agents, log.Events.OfType<DamageEvent>().ToArray(), fightTime).Values);

			var fullFightTargetDamageData = new List<TargetSquadDamageData>();
			foreach (var target in log.Encounter.ImportantEnemies)
			{
				var damageData = GetDamageData(
					log.Agents,
					log.Events
						.OfType<DamageEvent>()
						.Where(x => x.Defender == target)
						.ToArray(),
					fightTime);
				fullFightTargetDamageData.Add(new TargetSquadDamageData(target, fightTime, damageData.Values));
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

			var logAuthor = log.Events.OfType<PointOfViewEvent>().First().RecordingAgent as Player;
			var startTime = log.Events.OfType<LogStartEvent>().First().ServerTime;

			var might = log.Skills.FirstOrDefault(x => x.Id == SkillIds.Might);
			var vulnerability = log.Skills.FirstOrDefault(x => x.Id == SkillIds.Vulnerability);
			if (might != null) BuffSimulator.TrackBuff(might, BuffSimulationType.Intensity, 25);
			if (vulnerability != null) BuffSimulator.TrackBuff(vulnerability, BuffSimulationType.Intensity, 25);

			var buffData = BuffSimulator.SimulateBuffs(log.Agents, log.Events.OfType<BuffEvent>(),
				log.Encounter.GetPhases().Last().EndTime);

			return new LogStatistics(startTime, logAuthor, phaseStats, fullFightSquadDamageData,
				fullFightTargetDamageData, buffData, log.Encounter.GetResult(), log.Encounter.GetName(),
				log.EVTCVersion, eventCountsByName, log.Agents, log.Skills);
		}

		private Dictionary<Agent, DamageData> GetDamageData(IEnumerable<Agent> agents, ICollection<DamageEvent> events,
			long phaseDuration)
		{
			var physicalBossDamages = events.OfType<PhysicalDamageEvent>();
			var conditionBossDamages = events.OfType<BuffDamageEvent>();

			var damageDataByAttacker = new Dictionary<Agent, DamageData>();

			// Ensure all players are always in the damage data, even if they did no damage.
			foreach (var player in agents.OfType<Player>())
			{
				if (!damageDataByAttacker.ContainsKey(player))
				{
					damageDataByAttacker[player] = new DamageData(player, phaseDuration, 0, 0);
				}
			}

			foreach (var damageEvent in physicalBossDamages)
			{
				var attacker = damageEvent.Attacker;
				long damage = damageEvent.Damage;
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

			foreach (var damageEvent in conditionBossDamages)
			{
				var attacker = damageEvent.Attacker;
				long damage = damageEvent.Damage;
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

			return damageDataByAttacker;
		}
	}
}