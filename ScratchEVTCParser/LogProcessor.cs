using System;
using System.Collections.Generic;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Parsed;
using ScratchEVTCParser.Parsed.Enums;

namespace ScratchEVTCParser
{
	public class LogProcessor
	{
		public IEnumerable<Event> GetEvents(ParsedLog log)
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
				else
				{
					yield return new UnknownEvent(item.Time, item);
				}
			}
		}
	}
}