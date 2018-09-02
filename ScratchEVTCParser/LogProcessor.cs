using System;
using System.Collections.Generic;
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
			return new Log(GetEvents(log), GetAgents(log));
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

					int id;
					if (!idsByAddress.TryGetValue(agent.Address, out id))
					{
						id = -1;
					}

					var nameParts = agent.Name.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
					string characterName = nameParts[0];
					string accountName = nameParts[1];

					yield return new Player(id, characterName, agent.Toughness, agent.Concentration, agent.Healing,
						agent.Condition, agent.HitboxWidth, agent.HitboxHeight, accountName, profession,
						specialization);
				}
				else
				{
					if (agent.Prof >> 16 == 0xFFFF)
					{
						// Gadget
					}
					else
					{
						// NPC
					}
				}
			}
		}

		private IEnumerable<Event> GetEvents(ParsedLog log)
		{
			foreach (var item in log.ParsedCombatItems)
			{
				if (item.IsStateChange != StateChange.Normal)
				{
					switch (item.IsStateChange)
					{
						case StateChange.EnterCombat:
							yield return new AgentEnterCombatEvent(item.Time, item.SrcAgentId, (int) item.DstAgent);
							break;
						case StateChange.ExitCombat:
							yield return new AgentExitCombatEvent(item.Time, item.SrcAgentId);
							break;
						case StateChange.ChangeUp:
							yield return new AgentRevivedEvent(item.Time, item.SrcAgentId);
							break;
						case StateChange.ChangeDead:
							yield return new AgentDeadEvent(item.Time, item.SrcAgentId);
							break;
						case StateChange.ChangeDown:
							yield return new AgentDownedEvent(item.Time, item.SrcAgentId);
							break;
						case StateChange.Spawn:
							yield return new AgentSpawnEvent(item.Time, item.SrcAgentId);
							break;
						case StateChange.Despawn:
							yield return new AgentDespawnEvent(item.Time, item.SrcAgentId);
							break;
						case StateChange.HealthUpdate:
							var healthFraction = item.DstAgent / 10000f;
							yield return new AgentHealthUpdateEvent(item.Time, item.SrcAgentId, healthFraction);
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

							yield return new AgentWeaponSwapEvent(item.Time, item.SrcAgentId, newWeaponSet);
							break;
						case StateChange.MaxHealthUpdate:
							yield return new AgentMaxHealthUpdateEvent(item.Time, item.SrcAgentId, item.DstAgent);
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
							yield return new InitialBuffEvent(item.Time, item.DstAgentId, item.SkillId);
							break;
						case StateChange.Position:
						{
							// TODO: Check results
							float x = (float) (item.DstAgent & 0xFFFFFFFF);
							float y = (float) ((item.DstAgent << 32) & 0xFFFFFFFF);
							float z = (float) (item.Value & 0xFFFFFFFF);
							yield return new PositionChangeEvent(item.Time, item.SrcAgentId, x, y, z);
							break;
						}
						case StateChange.Velocity:
						{
							// TODO: Check results
							float x = (float) (item.DstAgent & 0xFFFFFFFF);
							float y = (float) ((item.DstAgent << 32) & 0xFFFFFFFF);
							float z = (float) (item.Value & 0xFFFFFFFF);
							yield return new VelocityChangeEvent(item.Time, item.SrcAgentId, x, y, z);
							break;
						}
						case StateChange.Rotation:
						{
							// TODO: Check results
							float x = (float) (item.DstAgent & 0xFFFFFFFF);
							float y = (float) ((item.DstAgent << 32) & 0xFFFFFFFF);
							yield return new FacingChangeEvent(item.Time, item.SrcAgentId, x, y);
							break;
						}
						case StateChange.TeamChange:
							yield return new TeamChangeEvent(item.Time, item.SrcAgentId, item.DstAgent);
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
							yield return new CancelledSkillCastEvent(item.Time, item.SrcAgentId, item.SkillId,
								item.Value);
							break;
						case Activation.Normal:
							yield return new SuccessfulSkillCastEvent(item.Time, item.SrcAgentId, item.SkillId,
								item.Value, SuccessfulSkillCastEvent.SkillCastType.Normal);
							break;
						case Activation.Quickness:
							yield return new SuccessfulSkillCastEvent(item.Time, item.SrcAgentId, item.SkillId,
								item.Value, SuccessfulSkillCastEvent.SkillCastType.WithQuickness);
							break;
						case Activation.Reset:
							yield return new ResetSkillCastEvent(item.Time, item.SrcAgentId, item.SkillId, item.Value);
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