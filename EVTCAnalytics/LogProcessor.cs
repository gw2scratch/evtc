using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Exceptions;
using GW2Scratch.EVTCAnalytics.IO;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;

namespace GW2Scratch.EVTCAnalytics
{
	public class LogProcessor
	{
		private static readonly Profession[] Professions =
		{
			Profession.Guardian,
			Profession.Warrior,
			Profession.Engineer,
			Profession.Ranger,
			Profession.Thief,
			Profession.Elementalist,
			Profession.Mesmer,
			Profession.Necromancer,
			Profession.Revenant,
		};

		private static readonly Dictionary<Profession, EliteSpecialization> HeartOfThornsSpecializationsByProfession =
			new Dictionary<Profession, EliteSpecialization>
			{
				{Profession.Guardian, EliteSpecialization.Dragonhunter},
				{Profession.Warrior, EliteSpecialization.Berserker},
				{Profession.Engineer, EliteSpecialization.Scrapper},
				{Profession.Ranger, EliteSpecialization.Druid},
				{Profession.Thief, EliteSpecialization.Daredevil},
				{Profession.Elementalist, EliteSpecialization.Tempest},
				{Profession.Mesmer, EliteSpecialization.Chronomancer},
				{Profession.Necromancer, EliteSpecialization.Reaper},
				{Profession.Revenant, EliteSpecialization.Herald}
			};

		private static readonly Dictionary<uint, EliteSpecialization> SpecializationsById =
			new Dictionary<uint, EliteSpecialization>()
			{
				{5, EliteSpecialization.Druid},
				{7, EliteSpecialization.Daredevil},
				{18, EliteSpecialization.Berserker},
				{27, EliteSpecialization.Dragonhunter},
				{34, EliteSpecialization.Reaper},
				{40, EliteSpecialization.Chronomancer},
				{43, EliteSpecialization.Scrapper},
				{48, EliteSpecialization.Tempest},
				{52, EliteSpecialization.Herald},
				{55, EliteSpecialization.Soulbeast},
				{56, EliteSpecialization.Weaver},
				{57, EliteSpecialization.Holosmith},
				{58, EliteSpecialization.Deadeye},
				{59, EliteSpecialization.Mirage},
				{60, EliteSpecialization.Scourge},
				{61, EliteSpecialization.Spellbreaker},
				{62, EliteSpecialization.Firebrand},
				{63, EliteSpecialization.Renegade}
			};

		public Log GetProcessedLog(ParsedLog log)
		{
			var agents = GetAgents(log).ToList();
			var skills = GetSkills(log).ToArray();
			var combatItemData = GetDataFromCombatItems(agents, skills, log);
			var events = combatItemData.Events.ToArray();

			Agent mainTarget = null;
			LogType logType = LogType.PvE;
			if (log.ParsedBossData.ID != 1)
			{
				foreach (var agent in agents)
				{
					if (agent is NPC npc && npc.SpeciesId == log.ParsedBossData.ID ||
					    agent is Gadget gadget && gadget.VolatileId == log.ParsedBossData.ID)
					{
						mainTarget = agent;
						break;
					}
				}
			}
			else
			{
				logType = LogType.WorldVersusWorld;
			}

			SetAgentAwareTimes(events);
			AssignAgentMasters(log, agents); // Needs to be done after setting aware times

			return new Log(mainTarget, logType, events, agents, skills, log.LogVersion.BuildVersion,
				combatItemData.LogStartTime, combatItemData.LogEndTime, combatItemData.PointOfView,
				combatItemData.Language, combatItemData.GameBuild, combatItemData.GameShardId, combatItemData.MapId);
		}


		private IEnumerable<Skill> GetSkills(ParsedLog log)
		{
			foreach (var skill in log.ParsedSkills)
			{
				var name = skill.Name.Trim('\0');
				uint skillId = checked((uint) skill.SkillId);
				yield return new Skill(skillId, name);
			}
		}

		private IEnumerable<Agent> GetAgents(ParsedLog log)
		{
			var idsByAddress = new Dictionary<ulong, int>();
			foreach (var combatItem in log.ParsedCombatItems)
			{
				if (combatItem.IsStateChange == StateChange.Normal)
				{
					idsByAddress[combatItem.SrcAgent] = combatItem.SrcAgentId;
				}
			}

			var playerAddresses = new HashSet<ulong>();

			foreach (var agent in log.ParsedAgents)
			{
				if (agent.IsElite != 0xFFFFFFFF)
				{
					// Player
					var profession = Professions[agent.Prof - 1];
					EliteSpecialization specialization;
					if (agent.IsElite == 0)
					{
						specialization = EliteSpecialization.None;
					}
					else if (agent.IsElite == 1)
					{
						specialization = HeartOfThornsSpecializationsByProfession[profession];
					}
					else
					{
						specialization = SpecializationsById[agent.IsElite];
					}

					if (!idsByAddress.TryGetValue(agent.Address, out int id))
					{
						id = -1;
					}

					// All parts of the name of the player might not be available, most commonly in WvW logs.
					var nameParts = agent.Name.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
					string characterName = nameParts[0];
					string accountName = nameParts.Length > 1 ? nameParts[1] : ":Unknown";
					string subgroupLiteral = nameParts.Length > 2 ? nameParts[2] : "";
					bool identified = nameParts.Length >= 3;

					if (!int.TryParse(subgroupLiteral, out int subgroup))
					{
						subgroup = -1;
					}

					if (playerAddresses.Contains(agent.Address))
					{
						// If there is already an agent with this address, this is the same player after changing
						// characters. For now we simply merge everything into the first instance, even though that
						// may result in a player using skills of a different profession, a better solution will be
						// needed for the future.
						continue;
					}

					playerAddresses.Add(agent.Address);

					yield return new Player(agent.Address, id, characterName, agent.Toughness, agent.Concentration,
						agent.Healing, agent.Condition, agent.HitboxWidth, agent.HitboxHeight, accountName, profession,
						specialization, subgroup, identified);
				}
				else
				{
					if (agent.Prof >> 16 == 0xFFFF)
					{
						// Gadget or Attack Target
						int volatileId = (int) (agent.Prof & 0xFFFF);
						string name = agent.Name.Trim('\0');

						if (name.StartsWith("at"))
						{
							// The attack target name is structured as "at[MasterAddress]-[GadgetId]-[MasterId]"
							// Preferably, the AttackTarget statechange would be used to detect if this is an
							// attack target to not rely on the name which could change in the future
							yield return new AttackTarget(agent.Address, volatileId, name, agent.HitboxWidth,
								agent.HitboxHeight);
						}
						else
						{
							yield return new Gadget(agent.Address, volatileId, name, agent.HitboxWidth,
								agent.HitboxHeight);
						}
					}
					else
					{
						// NPC
						if (!idsByAddress.TryGetValue(agent.Address, out int id))
						{
							id = -1;
						}

						string name = agent.Name.Trim('\0');
						int speciesId = (int) (agent.Prof & 0xFFFF);

						yield return new NPC(agent.Address, id, name, speciesId, agent.Toughness, agent.Concentration,
							agent.Healing, agent.Condition, agent.HitboxWidth, agent.HitboxHeight);
					}
				}
			}
		}

		private void SetAgentAwareTimes(IReadOnlyCollection<Event> events)
		{
			foreach (var ev in events)
			{
				if (ev is AgentEvent agentEvent)
				{
					if (agentEvent.Agent == null) continue;

					if (agentEvent.Agent.FirstAwareTime == 0 || ev.Time < agentEvent.Agent.FirstAwareTime)
					{
						agentEvent.Agent.FirstAwareTime = ev.Time;
					}

					if (agentEvent.Agent.LastAwareTime == long.MaxValue || ev.Time > agentEvent.Agent.LastAwareTime)
					{
						agentEvent.Agent.LastAwareTime = ev.Time;
					}
				}
				else if (ev is DamageEvent damageEvent)
				{
					if (damageEvent.Attacker != null)
					{
						if (damageEvent.Attacker.FirstAwareTime == 0 || ev.Time < damageEvent.Attacker.FirstAwareTime)
						{
							damageEvent.Attacker.FirstAwareTime = ev.Time;
						}

						if (damageEvent.Attacker.LastAwareTime == long.MaxValue ||
						    ev.Time > damageEvent.Attacker.LastAwareTime)
						{
							damageEvent.Attacker.LastAwareTime = ev.Time;
						}
					}

					if (damageEvent.Defender != null)
					{
						if (damageEvent.Defender.FirstAwareTime == 0 || ev.Time < damageEvent.Defender.FirstAwareTime)
						{
							damageEvent.Defender.FirstAwareTime = ev.Time;
						}

						if (damageEvent.Defender.LastAwareTime == long.MaxValue ||
						    ev.Time > damageEvent.Defender.LastAwareTime)
						{
							damageEvent.Defender.LastAwareTime = ev.Time;
						}
					}
				}
			}
		}

		// Requires aware times to be set first
		private void AssignAgentMasters(ParsedLog log, IReadOnlyCollection<Agent> agents)
		{
			foreach (var combatItem in log.ParsedCombatItems)
			{
				if (combatItem.IsStateChange == StateChange.Normal)
				{
					if (combatItem.SrcMasterId != 0)
					{
						Agent minion = null;

						// TODO: Look up by id with a dictionary first
						foreach (var agent in agents)
						{
							if (agent.Id == combatItem.SrcAgentId && agent.IsWithinAwareTime(combatItem.Time))
							{
								minion = agent;
								break;
							}
						}

						if (minion != null && minion.Master == null)
						{
							Agent master = null;
							foreach (var agent in agents)
							{
								if (!(agent is Gadget) && agent.Id == combatItem.SrcMasterId &&
								    agent.IsWithinAwareTime(combatItem.Time))
								{
									master = agent;
									break;
								}
							}

							if (master != null)
							{
								bool inCycle = false;
								var masterParent = master;
								while (masterParent != null)
								{
									if (masterParent == minion)
									{
										// A cycle is present in the minion hierarchy, this minion would end up as
										// a transitive minion of itself, which could cause infinite looping.
										// This is common in very old logs where minion data is somewhat weird.
										inCycle = true;
										break;
									}
									masterParent = masterParent.Master;
								}

								if (!inCycle)
								{
									minion.Master = master;
									master.MinionList.Add(minion);
								}
							}
						}
					}
				}

				if (combatItem.IsStateChange == StateChange.AttackTarget)
				{
					ulong attackTargetAddress = combatItem.SrcAgent;
					ulong masterGadgetAddress = combatItem.DstAgent;

					AttackTarget target = null;
					Gadget master = null;
					foreach (var agent in agents)
					{
						switch (agent)
						{
							case Gadget gadget when gadget.Address == masterGadgetAddress:
								master = gadget;
								break;
							case AttackTarget attackTarget when attackTarget.Address == attackTargetAddress:
								target = attackTarget;
								break;
						}
					}

					if (master != null && target != null)
					{
						master.AddAttackTarget(target);
						target.Gadget = master;
					}
				}
			}
		}

		private class CombatItemData
		{
			public IEnumerable<Event> Events { get; }
			public LogTime LogStartTime { get; }
			public LogTime LogEndTime { get; }
			public Player PointOfView { get; }
			public int? Language { get; }
			public int? GameBuild { get; }
			public int? GameShardId { get; }
			public int? MapId { get; }

			public CombatItemData(IEnumerable<Event> events, LogTime logStartTime, LogTime logEndTime,
				Player pointOfView, int? language, int? gameBuild, int? gameShardId, int? mapId)
			{
				Events = events;
				LogStartTime = logStartTime;
				LogEndTime = logEndTime;
				PointOfView = pointOfView;
				Language = language;
				GameBuild = gameBuild;
				GameShardId = gameShardId;
				MapId = mapId;
			}
		}

		private CombatItemData GetDataFromCombatItems(IEnumerable<Agent> agents, IEnumerable<Skill> skills,
			ParsedLog log)
		{
			var agentsByAddress = agents.ToDictionary(x => x.Address);
			var skillsById = skills.ToDictionary(x => x.Id);

			var events = new List<Event>();
			LogTime startTime = null;
			LogTime endTime = null;
			Player pointOfView = null;
			int? languageId = null;
			int? gameBuild = null;
			int? gameShardId = null;
			int? mapId = null;
			foreach (var item in log.ParsedCombatItems)
			{
				if (item.IsStateChange == StateChange.LogStart)
				{
					if (startTime != null)
					{
						throw new LogProcessingException("Multiple log start combat items found");
					}

					var serverTime = DateTimeOffset.FromUnixTimeSeconds(item.Value);
					var localTime = DateTimeOffset.FromUnixTimeSeconds(item.BuffDmg);

					startTime = new LogTime(localTime, serverTime, item.Time);
					continue;
				}

				if (item.IsStateChange == StateChange.LogEnd)
				{
					if (endTime != null)
					{
						throw new LogProcessingException("Multiple log end combat items found");
					}

					var serverTime = DateTimeOffset.FromUnixTimeSeconds(item.Value);
					var localTime = DateTimeOffset.FromUnixTimeSeconds(item.BuffDmg);

					endTime = new LogTime(localTime, serverTime, item.Time);
					continue;
				}

				if (item.IsStateChange == StateChange.PointOfView)
				{
					if (agentsByAddress.TryGetValue(item.SrcAgent, out var agent))
					{
						pointOfView = agent as Player ??
						              throw new LogProcessingException("The point of view agent is not a player");
					}

					continue;
				}

				if (item.IsStateChange == StateChange.Language)
				{
					languageId = (int) item.SrcAgent;
					continue;
				}

				if (item.IsStateChange == StateChange.GWBuild)
				{
					gameBuild = (int) item.SrcAgent;
					continue;
				}

				if (item.IsStateChange == StateChange.ShardId)
				{
					gameShardId = (int) item.SrcAgent;
					continue;
				}

				if (item.IsStateChange == StateChange.MapId)
				{
					mapId = (int) item.SrcAgent;
					continue;
				}

				if (item.IsStateChange == StateChange.Guild)
				{
					if (agentsByAddress.TryGetValue(item.SrcAgent, out Agent agent))
					{
						var player = (Player) agent;
						var guid = new byte[16];

						// It is unclear how the arcdps values would be stored on a big-endian platform
						Debug.Assert(BitConverter.IsLittleEndian);

						var dstBytes = BitConverter.GetBytes(item.DstAgent);
						var valueBytes = BitConverter.GetBytes(item.Value);
						var buffDamageBytes = BitConverter.GetBytes(item.BuffDmg);

						guid[0] = dstBytes[3];
						guid[1] = dstBytes[2];
						guid[2] = dstBytes[1];
						guid[3] = dstBytes[0];
						guid[4] = dstBytes[5];
						guid[5] = dstBytes[4];
						guid[6] = dstBytes[7];
						guid[7] = dstBytes[6];
						guid[8] = valueBytes[0];
						guid[9] = valueBytes[1];
						guid[10] = valueBytes[2];
						guid[11] = valueBytes[3];
						guid[12] = buffDamageBytes[0];
						guid[13] = buffDamageBytes[1];
						guid[14] = buffDamageBytes[2];
						guid[15] = buffDamageBytes[3];

						player.GuildGuid = guid;
					}

					continue;
				}

				if (item.IsStateChange == StateChange.AttackTarget)
				{
					// Only used for master assignment
					// Contains if the attack target is targetable as the value.
					continue;
				}

				events.Add(GetEvent(agentsByAddress, skillsById, item));
			}

			return new CombatItemData(events, startTime, endTime, pointOfView, languageId, gameBuild, gameShardId,
				mapId);
		}

		private Event GetEvent(IReadOnlyDictionary<ulong, Agent> agentsByAddress,
			IReadOnlyDictionary<uint, Skill> skillsById, ParsedCombatItem item)
		{
			Agent GetAgentByAddress(ulong address)
			{
				if (agentsByAddress.TryGetValue(address, out Agent agent))
				{
					return agent;
				}

				return null;
			}

			Skill GetSkillById(uint id)
			{
				if (skillsById.TryGetValue(id, out Skill skill))
				{
					return skill;
				}

				return null;
			}

			if (item.IsStateChange != StateChange.Normal)
			{
				switch (item.IsStateChange)
				{
					case StateChange.EnterCombat:
						return new AgentEnterCombatEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							(int) item.DstAgent);
					case StateChange.ExitCombat:
						return new AgentExitCombatEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.ChangeUp:
						return new AgentRevivedEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.ChangeDead:
						return new AgentDeadEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.ChangeDown:
						return new AgentDownedEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.Spawn:
						return new AgentSpawnEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.Despawn:
						return new AgentDespawnEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.HealthUpdate:
						var healthFraction = item.DstAgent / 10000f;
						return new AgentHealthUpdateEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							healthFraction);
					case StateChange.WeaponSwap:
						WeaponSet newWeaponSet;
						switch (item.DstAgent)
						{
							case 0:
								newWeaponSet = WeaponSet.Water1;
								break;
							case 1:
								newWeaponSet = WeaponSet.Water2;
								break;
							case 4:
								newWeaponSet = WeaponSet.Land1;
								break;
							case 5:
								newWeaponSet = WeaponSet.Land2;
								break;
							default:
								newWeaponSet = WeaponSet.Unknown;
								break;
						}

						return new AgentWeaponSwapEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							newWeaponSet);
					case StateChange.MaxHealthUpdate:
						return new AgentMaxHealthUpdateEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							item.DstAgent);
					case StateChange.Reward:
						return new RewardEvent(item.Time, item.DstAgent, item.Value);
					case StateChange.BuffInitial:
						return new InitialBuffEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId));
					case StateChange.Position:
					{
						float x = BitConversions.ToSingle((uint) (item.DstAgent & 0xFFFFFFFF));
						float y = BitConversions.ToSingle((uint) (item.DstAgent >> 32 & 0xFFFFFFFF));
						float z = BitConversions.ToSingle(item.Value);

						return new PositionChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y, z);
					}
					case StateChange.Velocity:
					{
						float x = BitConversions.ToSingle((uint) (item.DstAgent & 0xFFFFFFFF));
						float y = BitConversions.ToSingle((uint) (item.DstAgent >> 32 & 0xFFFFFFFF));
						float z = BitConversions.ToSingle(item.Value);

						return new VelocityChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y, z);
					}
					case StateChange.Rotation:
					{
						float x = BitConversions.ToSingle((uint) (item.DstAgent & 0xFFFFFFFF));
						float y = BitConversions.ToSingle((uint) (item.DstAgent >> 32 & 0xFFFFFFFF));

						return new FacingChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y);
					}
					case StateChange.TeamChange:
						return new TeamChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							item.DstAgent);
					case StateChange.Targetable:
					{
						var agent = GetAgentByAddress(item.SrcAgent);
						if (agent is AttackTarget target)
						{
							return new TargetableChangeEvent(item.Time, target, item.DstAgent != 0);
						}
						else
						{
							return new UnknownEvent(item.Time, item);
						}
					}
					case StateChange.ReplInfo:
						return new UnknownEvent(item.Time, item);
					case StateChange.StackActive:
						return new ActiveBuffStackEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							(uint) item.DstAgent);
					case StateChange.StackReset:
						return new ResetBuffStackEvent(item.Time, GetAgentByAddress(item.SrcAgent), item.Padding,
							item.Value);
					case StateChange.Unknown:
						return new UnknownEvent(item.Time, item);
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (item.IsActivation != Activation.None)
			{
				switch (item.IsActivation)
				{
					case Activation.CancelCancel:
						return new EndSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value, EndSkillCastEvent.SkillEndType.Cancel);
					case Activation.CancelFire:
						return new EndSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value, EndSkillCastEvent.SkillEndType.Fire);
					case Activation.Normal:
						return new StartSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value, StartSkillCastEvent.SkillCastType.Normal);
					case Activation.Quickness:
						return new StartSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value,
							StartSkillCastEvent.SkillCastType.WithQuickness);
					case Activation.Reset:
						return new ResetSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value);
					case Activation.Unknown:
						return new UnknownEvent(item.Time, item);
				}
			}
			else if (item.Buff != 0 && item.IsBuffRemove != BuffRemove.None)
			{
				Skill buff = GetSkillById(item.SkillId);
				int remainingDuration = item.Value;
				int remainingIntensity = item.BuffDmg;
				int stacksRemoved = (int) item.Result;
				var cleansingAgent = GetAgentByAddress(item.DstAgent);
				var agent = GetAgentByAddress(item.SrcAgent);
				switch (item.IsBuffRemove)
				{
					case BuffRemove.All:
						return new AllStacksRemovedBuffEvent(item.Time, agent, buff, cleansingAgent,
							stacksRemoved);
					case BuffRemove.Single:
						uint stackId = item.Padding;
						return new SingleStackRemovedBuffEvent(item.Time, agent, buff, cleansingAgent,
							remainingDuration, remainingIntensity, stackId);
					case BuffRemove.Manual:
						return new ManualStackRemovedBuffEvent(item.Time, agent, buff, cleansingAgent,
							remainingDuration, remainingIntensity);
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (item.Buff > 0 && item.BuffDmg == 0)
			{
				Skill buff = GetSkillById(item.SkillId);
				int durationApplied = item.Value;
				uint durationOfRemovedStack = item.OverstackValue;
				var agent = GetAgentByAddress(item.DstAgent);
				var sourceAgent = GetAgentByAddress(item.SrcAgent);
				return new BuffApplyEvent(item.Time, agent, buff, sourceAgent, durationApplied,
					durationOfRemovedStack);
			}
			else if (item.Buff > 0 && item.Value == 0)
			{
				Skill buff = GetSkillById(item.SkillId);
				int buffDamage = item.BuffDmg;
				bool isOffCycle = item.IsOffCycle > 0;
				Agent attacker = GetAgentByAddress(item.SrcAgent);
				Agent defender = GetAgentByAddress(item.DstAgent);
				bool isMoving = item.IsMoving > 0;
				bool isNinety = item.IsNinety > 0;
				bool isFlanking = item.IsFlanking > 0;
				bool isIgnored = item.Result != 0;

				if (isIgnored)
				{
					var reason = item.Result == (Result) 1
						? IgnoredBuffDamageEvent.Reason.InvulnerableBuff
						: IgnoredBuffDamageEvent.Reason.InvulnerableSkill;

					return new IgnoredBuffDamageEvent(item.Time, attacker, defender, buff, buffDamage,
						isMoving, isNinety, isFlanking, reason);
				}
				else
				{
					if (isOffCycle)
					{
						return new OffCycleBuffDamageEvent(item.Time, attacker, defender, buff, buffDamage,
							isMoving, isNinety, isFlanking);
					}
					else
					{
						return new BuffDamageEvent(item.Time, attacker, defender, buff, buffDamage, isMoving,
							isNinety, isFlanking);
					}
				}
			}
			else if (item.Buff == 0)
			{
				int damage = item.Value;
				uint shieldDamage = item.OverstackValue;
				Agent attacker = GetAgentByAddress(item.SrcAgent);
				Agent defender = GetAgentByAddress(item.DstAgent);
				Skill skill = GetSkillById(item.SkillId);
				bool isMoving = item.IsMoving > 0;
				bool isNinety = item.IsNinety > 0;
				bool isFlanking = item.IsFlanking > 0;

				// TODO: Rewrite
				bool ignored = false;
				var hitResult = PhysicalDamageEvent.Result.Normal;
				var ignoreReason = IgnoredPhysicalDamageEvent.Reason.Absorbed;
				switch (item.Result)
				{
					case Result.Normal:
						hitResult = PhysicalDamageEvent.Result.Normal;
						break;
					case Result.Critical:
						hitResult = PhysicalDamageEvent.Result.Critical;
						break;
					case Result.Glance:
						hitResult = PhysicalDamageEvent.Result.Glance;
						break;
					case Result.Block:
						ignored = true;
						ignoreReason = IgnoredPhysicalDamageEvent.Reason.Blocked;
						break;
					case Result.Evade:
						ignored = true;
						ignoreReason = IgnoredPhysicalDamageEvent.Reason.Evaded;
						break;
					case Result.Interrupt:
						hitResult = PhysicalDamageEvent.Result.Interrupt;
						break;
					case Result.Absorb:
						ignored = true;
						ignoreReason = IgnoredPhysicalDamageEvent.Reason.Absorbed;
						break;
					case Result.Blind:
						ignored = true;
						ignoreReason = IgnoredPhysicalDamageEvent.Reason.Missed;
						break;
					case Result.KillingBlow:
						hitResult = PhysicalDamageEvent.Result.KillingBlow;
						break;
					case Result.Downed:
						hitResult = PhysicalDamageEvent.Result.DowningBlow;
						break;
					case Result.Unknown:
						return new UnknownEvent(item.Time, item);
					default:
						return new UnknownEvent(item.Time, item);
				}

				if (!ignored)
				{
					return new PhysicalDamageEvent(item.Time, attacker, defender, skill, damage, isMoving,
						isNinety, isFlanking, shieldDamage, hitResult);
				}
				else
				{
					return new IgnoredPhysicalDamageEvent(item.Time, attacker, defender, skill, damage,
						isMoving, isNinety, isFlanking, shieldDamage, ignoreReason);
				}
			}

			return new UnknownEvent(item.Time, item);
		}
	}
}