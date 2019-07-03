using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Statistics;
using GW2Scratch.EVTCAnalytics.Statistics.Buffs;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters;
using GW2Scratch.EVTCAnalytics.Statistics.PlayerDataParts;
using SkillSlot = GW2Scratch.EVTCAnalytics.Model.Skills.SkillSlot;

namespace GW2Scratch.EVTCAnalytics
{
	public class StatisticsCalculator
	{
		public BuffSimulator BuffSimulator { get; set; } = new BuffSimulator();
		public GameSkillDataRepository GameSkillDataRepository { get; set; } = new GameSkillDataRepository();
		public SpecializationDetections SpecializationDetections { get; set; } = new SpecializationDetections();
		public SkillDetections SkillDetections { get; set; } = new SkillDetections();
		public IEncounterFinder EncounterFinder { get; set; } = new EncounterFinder();
		public IRotationCalculator RotationCalculator { get; set; } = new RotationCalculator();

		/// <summary>
		/// Calculates statistics for an encounter, such as damage done...
		/// </summary>
		/// <param name="log">The processed log.</param>
		/// <param name="apiData">Data from the GW2 API, may be null, some statistics won't be calculated.</param>
		public LogStatistics GetStatistics(Log log, GW2ApiData apiData)
		{
			var encounter = GetEncounter(log);

			var phaseStats = new List<PhaseStats>();

			var might = log.Skills.FirstOrDefault(x => x.Id == SkillIds.Might);
			var vulnerability = log.Skills.FirstOrDefault(x => x.Id == SkillIds.Vulnerability);
			if (might != null) BuffSimulator.TrackBuff(might, BuffSimulationType.Intensity, 25);
			if (vulnerability != null) BuffSimulator.TrackBuff(vulnerability, BuffSimulationType.Intensity, 25);

			var buffData = BuffSimulator.SimulateBuffs(log.Agents, log.Events.OfType<BuffEvent>(),
				encounter.GetPhases().Last().EndTime);

			var players = GetPlayers(log).ToArray();

			foreach (var phase in encounter.GetPhases())
			{
				var targetDamageData = new List<TargetSquadDamageData>();
				long phaseDuration = phase.EndTime - phase.StartTime;
				foreach (var target in phase.ImportantEnemies)
				{
					var damageData = GetDamageData(
						players,
						phase.Events
							.OfType<DamageEvent>()
							.Where(x => x.Defender == target),
						phase.Events.OfType<SkillCastEvent>(),
						buffData,
						log.Skills,
						phaseDuration);
					targetDamageData.Add(new TargetSquadDamageData(target, phaseDuration, damageData.Values));
				}

				var totalDamageData = new SquadDamageData(phaseDuration,
					GetDamageData(
						players,
						phase.Events.OfType<DamageEvent>(),
						phase.Events.OfType<SkillCastEvent>(),
						buffData,
						log.Skills,
						phaseDuration).Values
				);

				phaseStats.Add(new PhaseStats(phase.Name, phase.StartTime, phase.EndTime, targetDamageData,
					totalDamageData));
			}

			var fightTime = encounter.GetPhases().Sum(x => x.EndTime - x.StartTime);
			var fullFightSquadDamageData = new SquadDamageData(fightTime,
				GetDamageData(players, log.Events.OfType<DamageEvent>(),
					log.Events.OfType<SkillCastEvent>(), buffData, log.Skills, fightTime).Values);

			var fullFightTargetDamageData = new List<TargetSquadDamageData>();
			foreach (var target in encounter.ImportantEnemies)
			{
				var damageData = GetDamageData(
					players,
					log.Events
						.OfType<DamageEvent>()
						.Where(x => x.Defender == target),
					log.Events.OfType<SkillCastEvent>(),
					buffData,
					log.Skills,
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

			var playerData = GetPlayerData(log, apiData);

			return new LogStatistics(log.StartTime.ServerTime, log.PointOfView, playerData, phaseStats, fullFightSquadDamageData,
				fullFightTargetDamageData, buffData, encounter.GetResult(), encounter.GetName(),
				log.EVTCVersion, eventCountsByName, log.Agents, log.Skills);
		}

		public TimeSpan GetEncounterDuration(IEncounter encounter)
		{
			// TODO: This should be a method on Encounter
			var phases = encounter.GetPhases().ToArray();
			var start = phases.Min(x => x.StartTime);
			var end = phases.Max(x => x.EndTime);

			return new TimeSpan(0, 0, 0, 0, (int)(end - start));
		}

		public IEncounter GetEncounter(Log log)
		{
			return EncounterFinder.GetEncounter(log);
		}

		public IEnumerable<Player> GetPlayers(Log log)
		{
			return log.Agents.OfType<Player>();
		}

		private IEnumerable<PlayerData> GetPlayerData(Log log, GW2ApiData apiData)
		{
			var players = log.Agents.OfType<Player>().ToArray();

			var deathCounts = players.ToDictionary(x => x, x => 0);
			var downCounts = players.ToDictionary(x => x, x => 0);
			var usedSkills = players.ToDictionary(x => x, x => new HashSet<Skill>());

			foreach (var deadEvent in log.Events.OfType<AgentDeadEvent>().Where(x => x.Agent is Player))
			{
				var player = (Player) deadEvent.Agent;
				deathCounts[player]++;
			}

			foreach (var downEvent in log.Events.OfType<AgentDownedEvent>().Where(x => x.Agent is Player))
			{
				var player = (Player) downEvent.Agent;
				downCounts[player]++;
			}

			// Buff damage events only tell us which conditions/buffs, not what skill actually applied them
			foreach (var damageEvent in log.Events.OfType<PhysicalDamageEvent>().Where(x => x.Attacker is Player))
			{
				var player = (Player) damageEvent.Attacker;
				usedSkills[player].Add(damageEvent.Skill);
			}

			foreach (var activationEvent in log.Events.OfType<SkillCastEvent>().Where(x => x.Agent is Player))
			{
				var player = (Player) activationEvent.Agent;
				usedSkills[player].Add(activationEvent.Skill);
			}

			var playerData = new List<PlayerData>();
			foreach (var player in players)
			{
				HashSet<SkillData> utilitySkills = null;
				HashSet<SkillData> healingSkills = null;
				HashSet<SkillData> eliteSkills = null;
				if (apiData != null)
				{
					utilitySkills = new HashSet<SkillData>();
					healingSkills = new HashSet<SkillData>();
					eliteSkills = new HashSet<SkillData>();
					foreach (var usedSkill in usedSkills[player])
					{
						var skillData = apiData.GetSkillData(usedSkill);

						// Skills may be also registered as used if they affect other players and do damage through them
						if (skillData != null && skillData.Professions.Contains(player.Profession))
						{
							if (skillData.Slot == SkillSlot.Elite)
							{
								eliteSkills.Add(skillData);
							}
							else if (skillData.Slot == SkillSlot.Utility)
							{
								utilitySkills.Add(skillData);
							}
							else if (skillData.Slot == SkillSlot.Heal)
							{
								healingSkills.Add(skillData);
							}
						}
					}
				}

				WeaponType land1Weapon1 = WeaponType.Other;
				WeaponType land1Weapon2 = WeaponType.Other;
				WeaponType land2Weapon1 = WeaponType.Other;
				WeaponType land2Weapon2 = WeaponType.Other;
				IEnumerable<SkillData> land1WeaponSkills = null;
				IEnumerable<SkillData> land2WeaponSkills = null;

				// TODO: Dual wield skill handling for Thieves
				if (apiData != null)
				{
					WeaponSet currentWeaponSet = WeaponSet.Unknown;
					// We are only interested in land weapons. This may be imperfect if started on an underwater set.
					var firstWeaponSwap = log.Events.OfType<AgentWeaponSwapEvent>().FirstOrDefault(x =>
						x.NewWeaponSet == WeaponSet.Land1 || x.NewWeaponSet == WeaponSet.Land2);

					if (firstWeaponSwap == null)
					{
						currentWeaponSet = WeaponSet.Land1;
					}
					else
					{
						// First weapon set is the other one than the first swap swaps to (unless it was an underwater one)
						currentWeaponSet = firstWeaponSwap.NewWeaponSet == WeaponSet.Land1
							? WeaponSet.Land2
							: WeaponSet.Land1;
					}

					foreach (var logEvent in log.Events)
					{
						if (logEvent is AgentWeaponSwapEvent weaponSwapEvent && weaponSwapEvent.Agent == player)
						{
							currentWeaponSet = weaponSwapEvent.NewWeaponSet;
							continue;
						}

						SkillData skillData = null;
						if (logEvent is StartSkillCastEvent castEvent && castEvent.Agent == player)
						{
							skillData = apiData.GetSkillData(castEvent.Skill);
						}

						if (skillData != null)
						{
							if (skillData.Professions.Contains(player.Profession) && skillData.Type == SkillType.Weapon)
							{
								if (skillData.WeaponType.IsTwoHanded() || skillData.Slot == SkillSlot.Weapon1 ||
								    skillData.Slot == SkillSlot.Weapon2 || skillData.Slot == SkillSlot.Weapon3)
								{
									if (currentWeaponSet == WeaponSet.Land1)
									{
										land1Weapon1 = skillData.WeaponType;
									}
									else if (currentWeaponSet == WeaponSet.Land2)
									{
										land2Weapon1 = skillData.WeaponType;
									}
								}

								if (skillData.WeaponType.IsTwoHanded() || skillData.Slot == SkillSlot.Weapon4 ||
								    skillData.Slot == SkillSlot.Weapon5)
								{
									if (currentWeaponSet == WeaponSet.Land1)
									{
										land1Weapon2 = skillData.WeaponType;
									}
									else if (currentWeaponSet == WeaponSet.Land2)
									{
										land2Weapon2 = skillData.WeaponType;
									}
								}
							}
						}
					}

					land1WeaponSkills = GameSkillDataRepository
						.GetWeaponSkillIds(player.Profession, land1Weapon1, land1Weapon2)
						.Select(x => x == -1 ? null : apiData.GetSkillData(x));
					land2WeaponSkills = GameSkillDataRepository
						.GetWeaponSkillIds(player.Profession, land2Weapon1, land2Weapon2)
						.Select(x => x == -1 ? null : apiData.GetSkillData(x));
				}

				if (apiData != null)
				{
					var skillDetections = SkillDetections.GetSkillDetections(player.Profession).ToArray();
					foreach (var e in log.Events)
					{
						foreach (var detection in skillDetections)
						{
							if (detection.Detection.IsDetected(player, e))
							{
								var skill = apiData.GetSkillData(detection.SkillId);
								if (detection.Slot == SkillSlot.Utility)
								{
									utilitySkills.Add(skill);
								}
								else if (detection.Slot == SkillSlot.Heal)
								{
									healingSkills.Add(skill);
								}
								else if (detection.Slot == SkillSlot.Elite)
								{
									eliteSkills.Add(skill);
								}
							}
						}
					}
				}

				var ignoredSkills = SkillDetections.GetIgnoredSkillIds(player.Profession);
				healingSkills?.RemoveWhere(x => ignoredSkills.Contains(x.Id));
				utilitySkills?.RemoveWhere(x => ignoredSkills.Contains(x.Id));
				eliteSkills?.RemoveWhere(x => ignoredSkills.Contains(x.Id));

				var specializationDetections =
					SpecializationDetections.GetSpecializationDetections(player.Profession).ToArray();
				var badges = new List<PlayerBadge>();

				var specializations = new HashSet<CoreSpecialization>();
				foreach (var e in log.Events)
				{
					foreach (var detection in specializationDetections)
					{
						if (detection.Detection.IsDetected(player, e))
						{
							specializations.Add(detection.Specialization);
						}
					}
				}

				foreach (var spec in specializations.OrderBy(x => x.ToString()))
				{
					badges.Add(new PlayerBadge(spec.ToString(), BadgeType.Specialization));
				}

				var rotation = RotationCalculator.GetRotation(log, player);

				var data = new PlayerData(player, downCounts[player], deathCounts[player], rotation, usedSkills[player],
					healingSkills, utilitySkills, eliteSkills, land1Weapon1, land1Weapon2, land2Weapon1, land2Weapon2,
					land1WeaponSkills, land2WeaponSkills, badges);

				playerData.Add(data);
			}

			return playerData;
		}

		private Dictionary<Agent, DamageData> GetDamageData(IEnumerable<Player> players,
			IEnumerable<DamageEvent> damageEvents, IEnumerable<SkillCastEvent> skillCastEvents,
			BuffData buffData, IEnumerable<Skill> skills, long phaseDuration)
		{
			// Multiple enumeration is needed
			var skillArray = skills as Skill[] ?? skills.ToArray();
			var damageEventArray = damageEvents as DamageEvent[] ?? damageEvents.ToArray();

			// Could not be present at all
			var might = skillArray.FirstOrDefault(x => x.Id == SkillIds.Might);
			var vulnerability = skillArray.FirstOrDefault(x => x.Id == SkillIds.Vulnerability);

			var physicalBossDamages = damageEventArray.OfType<PhysicalDamageEvent>();
			var conditionBossDamages = damageEventArray.OfType<BuffDamageEvent>();
			var skillCastStarts = skillCastEvents.OfType<StartSkillCastEvent>();

			var damageDataByAttacker = new Dictionary<Agent, DamageData>();

			void EnsureDamageDataExists(Agent agent)
			{
				if (!damageDataByAttacker.ContainsKey(agent))
				{
					damageDataByAttacker[agent] = new DamageData(agent, phaseDuration);

					if (buffData.AgentBuffData.ContainsKey(agent))
					{
						var buffDataBySkill = buffData.AgentBuffData[agent].StackCollectionsBySkills;
						if (might != null)
						{
							bool mightDataAvailable = buffDataBySkill.ContainsKey(might);
							damageDataByAttacker[agent].MightDataAvailable = mightDataAvailable;
						}
						else
						{
							damageDataByAttacker[agent].MightDataAvailable = false;
						}

						// Vulnerability has to be checked on targets.
						bool vulnerabilityDataAvailable = vulnerability != null;
						damageDataByAttacker[agent].VulnerabilityDataAvailable = vulnerabilityDataAvailable;
					}
				}
			}

			// Ensure all players are always in the damage data, even if they did no damage.
			foreach (var player in players)
			{
				EnsureDamageDataExists(player);
			}

			foreach (var skillCastEvent in skillCastStarts)
			{
				if (skillCastEvent.Agent == null) continue;

				EnsureDamageDataExists(skillCastEvent.Agent);

				damageDataByAttacker[skillCastEvent.Agent]
					.AddSkillCast(skillCastEvent.CastType == StartSkillCastEvent.SkillCastType.WithQuickness);
			}

			foreach (var damageEvent in physicalBossDamages)
			{
				var attacker = damageEvent.Attacker;
				var defender = damageEvent.Defender;

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

				EnsureDamageDataExists(mainMaster);

				var damageData = damageDataByAttacker[mainMaster];
				int mightStacks = 0;
				if (damageData.MightDataAvailable)
				{
					mightStacks = buffData.AgentBuffData[mainMaster].StackCollectionsBySkills[might]
						.GetStackCount(damageEvent.Time);
				}

				int vulnerabilityStacks = 0;
				if (damageData.VulnerabilityDataAvailable)
				{
					if (defender != null)
					{
						if (buffData.AgentBuffData.TryGetValue(defender, out var defenderBuffs))
						{
							if (defenderBuffs.StackCollectionsBySkills.TryGetValue(vulnerability,
								out var vulnerabilityBuffStacks))
							{
								vulnerabilityStacks = vulnerabilityBuffStacks.GetStackCount(damageEvent.Time);
							}
							else
							{
								damageData.VulnerabilityDataAvailable = false;
							}
						}
						else
						{
							damageData.VulnerabilityDataAvailable = false;
						}
					}
					else
					{
						damageData.VulnerabilityDataAvailable = false;
					}
				}

				damageData.AddPhysicalDamage(damage, mightStacks, vulnerabilityStacks);
			}

			foreach (var damageEvent in conditionBossDamages)
			{
				var attacker = damageEvent.Attacker;
				var defender = damageEvent.Defender;

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

				EnsureDamageDataExists(mainMaster);

				var damageData = damageDataByAttacker[mainMaster];
				int mightStacks = 0;
				if (damageData.MightDataAvailable)
				{
					mightStacks = buffData.AgentBuffData[mainMaster].StackCollectionsBySkills[might]
						.GetStackCount(damageEvent.Time);
				}

				int vulnerabilityStacks = 0;
				if (damageData.VulnerabilityDataAvailable)
				{
					if (defender != null)
					{
						if (buffData.AgentBuffData.TryGetValue(defender, out var defenderBuffs))
						{
							if (defenderBuffs.StackCollectionsBySkills.TryGetValue(vulnerability,
								out var vulnerabilityBuffStacks))
							{
								vulnerabilityStacks = vulnerabilityBuffStacks.GetStackCount(damageEvent.Time);
							}
							else
							{
								damageData.VulnerabilityDataAvailable = false;
							}
						}
						else
						{
							damageData.VulnerabilityDataAvailable = false;
						}
					}
					else
					{
						damageData.VulnerabilityDataAvailable = false;
					}
				}

				damageData.AddConditionDamage(damage, mightStacks, vulnerabilityStacks);
			}

			return damageDataByAttacker;
		}
	}
}