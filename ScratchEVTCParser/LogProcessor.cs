using System;
using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Parsed;
using ScratchEVTCParser.Parsed.Enums;

namespace ScratchEVTCParser
{
	public class LogProcessor
	{
		private static Profession[] professions =
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

		private static Dictionary<uint, EliteSpecialization> specializationsById =
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
			return new Log(GetEvents(agents, log), agents);
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

			foreach (var agent in log.ParsedAgents)
			{
				if (agent.IsElite != 0xFFFFFFFF)
				{
					// Player
					var profession = professions[agent.Prof - 1];
					EliteSpecialization specialization;
					if (agent.IsElite == 0)
					{
						specialization = EliteSpecialization.None;
					}
					else
					{
						specialization = specializationsById[agent.IsElite];
					}

					if (!idsByAddress.TryGetValue(agent.Address, out int id))
					{
						id = -1;
					}

					var nameParts = agent.Name.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
					string characterName = nameParts[0];
					string accountName = nameParts[1];

					yield return new Player(agent.Address, id, characterName, agent.Toughness, agent.Concentration,
						agent.Healing,
						agent.Condition, agent.HitboxWidth, agent.HitboxHeight, accountName, profession,
						specialization);
				}
				else
				{
					if (agent.Prof >> 16 == 0xFFFF)
					{
						// Gadget
						// TODO: Implement
					}
					else
					{
						// NPC
						if (!idsByAddress.TryGetValue(agent.Address, out int id))
						{
							id = -1;
						}

						string name = agent.Name.Trim('\0');

						yield return new NPC(agent.Address, id, name, agent.Toughness, agent.Concentration,
							agent.Healing,
							agent.Condition, agent.HitboxWidth, agent.HitboxHeight);
					}
				}
			}
		}

		private IEnumerable<Event> GetEvents(List<Agent> agents, ParsedLog log)
		{
			var agentsByAddress = agents.ToDictionary(x => x.Address);

			Agent GetAgentByAddress(ulong address)
			{
				if (agentsByAddress.TryGetValue(address, out Agent agent))
				{
					return agent;
				}

				return null;
			}

			foreach (var item in log.ParsedCombatItems)
			{
				if (item.IsStateChange != StateChange.Normal)
				{
					switch (item.IsStateChange)
					{
						case StateChange.EnterCombat:
							yield return new AgentEnterCombatEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								(int) item.DstAgent);
							break;
						case StateChange.ExitCombat:
							yield return new AgentExitCombatEvent(item.Time, GetAgentByAddress(item.SrcAgent));
							break;
						case StateChange.ChangeUp:
							yield return new AgentRevivedEvent(item.Time, GetAgentByAddress(item.SrcAgent));
							break;
						case StateChange.ChangeDead:
							yield return new AgentDeadEvent(item.Time, GetAgentByAddress(item.SrcAgent));
							break;
						case StateChange.ChangeDown:
							yield return new AgentDownedEvent(item.Time, GetAgentByAddress(item.SrcAgent));
							break;
						case StateChange.Spawn:
							yield return new AgentSpawnEvent(item.Time, GetAgentByAddress(item.SrcAgent));
							break;
						case StateChange.Despawn:
							yield return new AgentDespawnEvent(item.Time, GetAgentByAddress(item.SrcAgent));
							break;
						case StateChange.HealthUpdate:
							var healthFraction = item.DstAgent / 10000f;
							yield return new AgentHealthUpdateEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								healthFraction);
							break;
						case StateChange.LogStart:
						{
							var serverTime = DateTimeOffset.FromUnixTimeSeconds(item.Value);
							var localTime = DateTimeOffset.FromUnixTimeSeconds(item.BuffDmg);
							yield return new LogStartEvent(item.Time, serverTime, localTime);
							break;
						}
						case StateChange.LogEnd:
						{
							var serverTime = DateTimeOffset.FromUnixTimeSeconds(item.Value);
							var localTime = DateTimeOffset.FromUnixTimeSeconds(item.BuffDmg);
							yield return new LogEndEvent(item.Time, serverTime, localTime);
							break;
						}
						case StateChange.WeaponSwap:
							AgentWeaponSwapEvent.WeaponSet newWeaponSet;
							switch (item.DstAgent)
							{
								case 0:
									newWeaponSet = AgentWeaponSwapEvent.WeaponSet.Water1;
									break;
								case 1:
									newWeaponSet = AgentWeaponSwapEvent.WeaponSet.Water2;
									break;
								case 4:
									newWeaponSet = AgentWeaponSwapEvent.WeaponSet.Land1;
									break;
								case 5:
									newWeaponSet = AgentWeaponSwapEvent.WeaponSet.Land2;
									break;
								default:
									newWeaponSet = AgentWeaponSwapEvent.WeaponSet.Unknown;
									break;
							}

							yield return new AgentWeaponSwapEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								newWeaponSet);
							break;
						case StateChange.MaxHealthUpdate:
							yield return new AgentMaxHealthUpdateEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								item.DstAgent);
							break;
						case StateChange.PointOfView:
							yield return new PointOfViewEvent(item.Time, item.SrcAgent);
							break;
						case StateChange.Language:
							yield return new LanguageEvent(item.Time, (int) item.SrcAgent);
							break;
						case StateChange.GWBuild:
							yield return new GameBuildEvent(item.Time, (int) item.SrcAgent);
							break;
						case StateChange.ShardId:
							yield return new GameShardEvent(item.Time, item.SrcAgent);
							break;
						case StateChange.Reward:
							yield return new RewardEvent(item.Time, item.DstAgent, item.Value);
							break;
						case StateChange.BuffInitial:
							yield return new InitialBuffEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								item.SkillId);
							break;
						case StateChange.Position:
						{
							// TODO: Check results
							float x = (float) (item.DstAgent & 0xFFFFFFFF);
							float y = (float) ((item.DstAgent << 32) & 0xFFFFFFFF);
							float z = (float) (item.Value & 0xFFFFFFFF);
							yield return new PositionChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y, z);
							break;
						}
						case StateChange.Velocity:
						{
							// TODO: Check results
							float x = (float) (item.DstAgent & 0xFFFFFFFF);
							float y = (float) ((item.DstAgent << 32) & 0xFFFFFFFF);
							float z = (float) (item.Value & 0xFFFFFFFF);
							yield return new VelocityChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y, z);
							break;
						}
						case StateChange.Rotation:
						{
							// TODO: Check results
							float x = (float) (item.DstAgent & 0xFFFFFFFF);
							float y = (float) ((item.DstAgent << 32) & 0xFFFFFFFF);
							yield return new FacingChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y);
							break;
						}
						case StateChange.TeamChange:
							yield return new TeamChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								item.DstAgent);
							break;
						case StateChange.Unknown:
							yield return new UnknownEvent(item.Time, item);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else if (item.IsActivation != Activation.None)
				{
					switch (item.IsActivation)
					{
						// TODO: Are both of these cancelled attacks? The EVTC documentation only mentions CancelFire occurs when reaching channel time
						case Activation.CancelCancel:
						case Activation.CancelFire:
							yield return new CancelledSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								item.SkillId, item.Value);
							break;
						case Activation.Normal:
							yield return new SuccessfulSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								item.SkillId, item.Value, SuccessfulSkillCastEvent.SkillCastType.Normal);
							break;
						case Activation.Quickness:
							yield return new SuccessfulSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								item.SkillId, item.Value, SuccessfulSkillCastEvent.SkillCastType.WithQuickness);
							break;
						case Activation.Reset:
							yield return new ResetSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
								item.SkillId, item.Value);
							break;
						case Activation.Unknown:
							yield return new UnknownEvent(item.Time, item);
							break;
					}
				}
				else
				{
					yield return new UnknownEvent(item.Time, item);
				}
			}
		}
	}
}