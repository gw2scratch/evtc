using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using GW2Scratch.EVTCAnalytics.Statistics;
using GW2Scratch.EVTCAnalytics.Statistics.Buffs;
using GW2Scratch.EVTCAnalytics.Statistics.PlayerDataParts;

namespace GW2Scratch.EVTCAnalytics
{
	/// <summary>
	/// The Log Analyzer calculates potentially performance-heavy statistics.
	/// All calculations are cached within the object and not repeated.
	/// </summary>
	public class LogAnalyzer
	{
		/// <summary>
		/// The simulator used to produce time series data of applied buff stacks.
		/// If replaced after buff statistics were calculated, the resulting data will not be updated.
		/// </summary>
		public BuffSimulator BuffSimulator { get; set; } = new BuffSimulator();

		/// <summary>
		/// Weapon skill data used for determining which skills a player is using.
		/// If replaced after used, the resulting statistics will not be updated.
		/// </summary>
		public WeaponSkillData WeaponSkillData { get; set; } = new WeaponSkillData();

		/// <summary>
		/// Definitions of skill detections used to determine which skills a player was using.
		/// If replaced after skills were detected, they will not be updated.
		/// </summary>
		public SkillDetections SkillDetections { get; set; } = new SkillDetections();

		/// <summary>
		/// The rotation calculator that will be used for determining player skill rotations.
		/// If replaced after rotations were cached, they will not be updated.
		/// </summary>
		public IRotationCalculator RotationCalculator { get; set; } = new RotationCalculator();

		/// <summary>
		/// Data from the GW2 API, may be null, some statistics won't be calculated.
		/// </summary>
		public GW2ApiData ApiData { get; set; } = null;

		private readonly Log log;
		private IReadOnlyList<Player> logPlayers = null;
		private IReadOnlyList<PlayerData> logPlayerData = null;
		private EncounterResult? logResult = null;
		private EncounterMode? encounterMode = null;
		private long? logEncounterStart = null;
		private long? logEncounterEnd = null;

		/// <summary>
		/// Creates a new instance of a log analyzer for a provided log.
		/// </summary>
		/// <param name="log">The processed log that will be analyzed.</param>
		/// <param name="apiData">Data from the GW2 API, may be null, some statistics won't be calculated.</param>
		public LogAnalyzer(Log log, GW2ApiData apiData = null)
		{
			this.log = log;
			ApiData = apiData;
		}

		public TimeSpan GetEncounterDuration()
		{
			return new TimeSpan(0, 0, 0, 0, (int)(GetEncounterEnd() - GetEncounterStart()));
		}

		public long GetEncounterEnd()
		{
			logEncounterEnd ??= log.EndTime?.TimeMilliseconds ?? log.Events.Last().Time;
			return logEncounterEnd.Value;
		}

		public long GetEncounterStart()
		{
			logEncounterStart ??= log.StartTime?.TimeMilliseconds ?? log.Events.First().Time;
			return logEncounterStart.Value;
		}

		public IReadOnlyList<Player> GetPlayers()
		{
			logPlayers ??= log.Agents.OfType<Player>().ToList();
			return logPlayers;
		}

		public EncounterResult GetResult()
		{
			logResult ??= log.EncounterData.ResultDeterminer.GetResult(log.Events);
			return logResult.Value;
		}

		public EncounterMode GetMode()
		{
			encounterMode ??= log.EncounterData.ModeDeterminer.GetMode(log);
			return encounterMode.Value;
		}

		public Encounter GetEncounter()
		{
			return log.EncounterData.Encounter;
		}

		public IReadOnlyList<PlayerData> GetPlayerData()
		{
			if (logPlayerData == null)
			{
				CalculatePlayerData();
			}

			return logPlayerData;
		}

		private void CalculatePlayerData()
		{
			var players = GetPlayers();

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
				if (ApiData != null)
				{
					utilitySkills = new HashSet<SkillData>();
					healingSkills = new HashSet<SkillData>();
					eliteSkills = new HashSet<SkillData>();
					foreach (var usedSkill in usedSkills[player])
					{
						var skillData = ApiData.GetSkillData(usedSkill);

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
				if (ApiData != null)
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
							skillData = ApiData.GetSkillData(castEvent.Skill);
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

					land1WeaponSkills = WeaponSkillData
						.GetWeaponSkillIds(player.Profession, land1Weapon1, land1Weapon2)
						.Select(x => x == -1 ? null : ApiData.GetSkillData(x));
					land2WeaponSkills = WeaponSkillData
						.GetWeaponSkillIds(player.Profession, land2Weapon1, land2Weapon2)
						.Select(x => x == -1 ? null : ApiData.GetSkillData(x));
				}

				if (ApiData != null)
				{
					var skillDetections = SkillDetections.GetSkillDetections(player.Profession).ToArray();
					foreach (var e in log.Events)
					{
						foreach (var detection in skillDetections)
						{
							if (detection.Detection.IsDetected(player, e))
							{
								var skill = ApiData.GetSkillData(detection.SkillId);
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

				var rotation = RotationCalculator.GetRotation(log, player);

				var data = new PlayerData(player, downCounts[player], deathCounts[player], rotation, usedSkills[player],
					healingSkills, utilitySkills, eliteSkills, land1Weapon1, land1Weapon2, land2Weapon1, land2Weapon2,
					land1WeaponSkills, land2WeaponSkills);

				playerData.Add(data);
			}

			logPlayerData = playerData;
		}

		private static Dictionary<Agent, DamageData> GetDamageData(IEnumerable<Player> players,
			IEnumerable<DamageEvent> damageEvents, IEnumerable<SkillCastEvent> skillCastEvents,
			BuffData buffData, IReadOnlyList<Skill> skills, long phaseDuration)
		{
			// Multiple enumeration is needed
			var damageEventArray = damageEvents as DamageEvent[] ?? damageEvents.ToArray();

			// Could not be present at all
			var might = skills.FirstOrDefault(x => x.Id == SkillIds.Might);
			var vulnerability = skills.FirstOrDefault(x => x.Id == SkillIds.Vulnerability);

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
					mainMaster = mainMaster.Master;
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
					mainMaster = mainMaster.Master;
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