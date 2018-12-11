using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.GameData;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;
using ScratchEVTCParser.Model.Encounters.Phases;
using ScratchEVTCParser.Model.Skills;
using ScratchEVTCParser.Parsed;
using ScratchEVTCParser.Parsed.Enums;

namespace ScratchEVTCParser
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
			var events = GetEvents(agents, skills, log).ToArray();

			var boss = agents.OfType<NPC>().First(a => a.SpeciesId == log.ParsedBossData.ID);

			SetAgentAwareTimes(events);
			AssignAgentMasters(log, agents); // Needs to be done after setting aware times

			var encounter = GetEncounter(boss, agents, skills, events);

			return new Log(encounter, events, agents, skills, log.LogVersion.BuildVersion);
		}

		private IEncounter GetEncounter(NPC boss, ICollection<Agent> agents, ICollection<Skill> skills,
			ICollection<Event> events)
		{
			IEncounter encounter = new DefaultEncounter(boss, events);

			if (boss.SpeciesId == SpeciesIds.ValeGuardian)
			{
				var invulnerability = skills.First(x => x.Id == SkillIds.Invulnerability);

				var redGuardians = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.RedGuardian).ToArray();
				var greenGuardians = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.GreenGuardian).ToArray();
				var blueGuardians = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.BlueGuardian).ToArray();

				bool firstGuardians =
					redGuardians.Length >= 1 && greenGuardians.Length >= 1 && blueGuardians.Length >= 1;
				bool secondGuardians =
					redGuardians.Length >= 2 && greenGuardians.Length >= 2 && blueGuardians.Length >= 2;

				var split1Guardians = firstGuardians
					? new[] {blueGuardians[0], greenGuardians[0], redGuardians[0]}
					: new Agent[0];
				var split2Guardians = secondGuardians
					? new[] {blueGuardians[1], greenGuardians[1], redGuardians[1]}
					: new Agent[0];

				encounter = new BaseEncounter(
					new[] {boss},
					events,
					new PhaseSplitter(
						new StartTrigger(new PhaseDefinition("Phase 1", boss)),
						new BuffAddTrigger(boss, invulnerability, new PhaseDefinition("Split 1", split1Guardians)),
						new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 2", boss)),
						new BuffAddTrigger(boss, invulnerability, new PhaseDefinition("Split 2", split2Guardians)),
						new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 3", boss))
					),
					new AgentDeathResultDeterminer(boss),
					new AgentNameEncounterNameProvider(boss));
			}
			else if (boss.SpeciesId == SpeciesIds.Nikare || boss.SpeciesId == SpeciesIds.Kenut)
			{
				var determined = skills.First(x => x.Id == SkillIds.Determined);
				var nikare = agents.OfType<NPC>().First(x => x.SpeciesId == SpeciesIds.Nikare);
				var kenut = agents.OfType<NPC>().First(x => x.SpeciesId == SpeciesIds.Kenut);

				encounter = new BaseEncounter(
					new[] {nikare, kenut},
					events,
					new PhaseSplitter(
						new StartTrigger(new PhaseDefinition("Nikare's platform", nikare)),
						new BuffAddTrigger(nikare, determined, new PhaseDefinition("Kenut's platform", kenut)),
						new BuffAddTrigger(kenut, determined, new PhaseDefinition("Split phase", nikare, kenut))
					),
					new CombinedResultDeterminer(
						new AgentDeathResultDeterminer(nikare),
						new AgentDeathResultDeterminer(kenut)
					),
					new ConstantEncounterNameProvider("Twin Largos")
				);
			}
			else if (boss.SpeciesId == SpeciesIds.Gorseval)
			{
				var invulnerability = skills.First(x => x.Id == SkillIds.GorsevalInvulnerability);

				var chargedSouls = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.ChargedSoul).ToArray();

				var split1Souls = chargedSouls.Length >= 4 ? chargedSouls.Take(4).ToArray() : new Agent[0];
				var split2Souls = chargedSouls.Length >= 8 ? chargedSouls.Skip(4).Take(4).ToArray() : new Agent[0];

				encounter = new BaseEncounter(
					new[] {boss},
					events,
					new PhaseSplitter(
						new StartTrigger(new PhaseDefinition("Phase 1", boss)),
						new BuffAddTrigger(boss, invulnerability, new PhaseDefinition("Split 1", split1Souls)),
						new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 2", boss)),
						new BuffAddTrigger(boss, invulnerability, new PhaseDefinition("Split 2", split2Souls)),
						new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 3", boss))
					),
					new AgentDeathResultDeterminer(boss),
					new AgentNameEncounterNameProvider(boss));
			}
			else if (boss.SpeciesId == SpeciesIds.Sabetha)
			{
				var invulnerability = skills.First(x => x.Id == SkillIds.Invulnerability);

				var kernan = agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Kernan);
				var knuckles = agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Knuckles);
				var karde = agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Karde);

				encounter = new BaseEncounter(
					new[] {boss},
					events,
					new PhaseSplitter(
						new StartTrigger(new PhaseDefinition("Phase 1", boss)),
						new BuffAddTrigger(boss, invulnerability,
							new PhaseDefinition("Kernan", kernan != null ? new Agent[] {kernan} : new Agent[0])),
						new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 2", boss)),
						new BuffAddTrigger(boss, invulnerability,
							new PhaseDefinition("Knuckles", knuckles != null ? new Agent[] {knuckles} : new Agent[0])),
						new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 3", boss)),
						new BuffAddTrigger(boss, invulnerability,
							new PhaseDefinition("Karde", karde != null ? new Agent[] {karde} : new Agent[0])),
						new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 4", boss))
					),
					new AgentDeathResultDeterminer(boss),
					new AgentNameEncounterNameProvider(boss)
				);
			}
			else if (boss.SpeciesId == SpeciesIds.Qadim)
			{
				var flameArmor = skills.First(x => x.Id == SkillIds.QadimFlameArmor);

				var hydra = agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.AncientInvokedHydra);
				var destroyer = agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.ApocalypseBringer);
				var matriarch = agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.WyvernMatriarch);
				var patriarch = agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.WyvernPatriarch);

				encounter = new BaseEncounter(
					new[] {boss},
					events,
					new PhaseSplitter(
						new AgentEventTrigger<TeamChangeEvent>(boss,
							new PhaseDefinition("Hydra phase", hydra != null ? new Agent[] {hydra} : new Agent[0])),
						new BuffRemoveTrigger(boss, flameArmor, new PhaseDefinition("Qadim 100-66%", boss)),
						new BuffAddTrigger(boss, flameArmor,
							new PhaseDefinition("Destroyer phase",
								destroyer != null ? new Agent[] {destroyer} : new Agent[0])),
						new BuffRemoveTrigger(boss, flameArmor, new PhaseDefinition("Qadim 66-33%", boss)),
						new BuffAddTrigger(boss, flameArmor,
							new PhaseDefinition("Wyvern phase",
								matriarch != null && patriarch != null
									? new Agent[] {matriarch, patriarch}
									: new Agent[0])),
						new MultipleAgentsDeadTrigger(new PhaseDefinition("Jumping puzzle"), // TODO: Add Pyre Guardians
							matriarch != null && patriarch != null
								? new Agent[] {matriarch, patriarch}
								: new Agent[0]),
						new BuffRemoveTrigger(boss, flameArmor, new PhaseDefinition("Qadim 33-0%", boss))
					),
					new AgentExitCombatDeterminer(boss),
					new AgentNameEncounterNameProvider(boss)
				);
			}

			return encounter;
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
					else
					{
						specialization = SpecializationsById[agent.IsElite];
					}

					if (!idsByAddress.TryGetValue(agent.Address, out int id))
					{
						id = -1;
					}

					var nameParts = agent.Name.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
					string characterName = nameParts[0];
					string accountName = nameParts[1];
					string subgroupLiteral = nameParts[2];
					if (!int.TryParse(subgroupLiteral, out int subgroup))
					{
						subgroup = -2;
					}

					subgroup++; // Recorded subgroups are one less than ingame.

					yield return new Player(agent.Address, id, characterName, agent.Toughness, agent.Concentration,
						agent.Healing, agent.Condition, agent.HitboxWidth, agent.HitboxHeight, accountName, profession,
						specialization, subgroup);
				}
				else
				{
					if (agent.Prof >> 16 == 0xFFFF)
					{
						// Gadget
						int volatileId = (int) (agent.Prof & 0xFFFF);
						string name = agent.Name.Trim('\0');

						yield return new Gadget(agent.Address, volatileId, name, agent.HitboxWidth, agent.HitboxHeight);
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
						foreach (var agent in agents)
						{
							if (agent.Id == combatItem.SrcAgentId && agent.IsWithinAwareTime(combatItem.Time))
							{
								minion = agent;
								break;
							}
						}

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

						if (minion != null && master != null && minion.Master == null)
						{
							minion.Master = master;
							master.MinionList.Add(minion);
						}
					}
				}
			}
		}

		private IEnumerable<Event> GetEvents(IEnumerable<Agent> agents, IEnumerable<Skill> skills, ParsedLog log)
		{
			var agentsByAddress = agents.ToDictionary(x => x.Address);
			var skillsById = skills.ToDictionary(x => x.Id);

			var events = new List<Event>();
			foreach (var item in log.ParsedCombatItems)
			{
				events.Add(GetEvent(agentsByAddress, skillsById, item));
			}

			return events;
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
					case StateChange.LogStart:
					{
						var serverTime = DateTimeOffset.FromUnixTimeSeconds(item.Value);
						var localTime = DateTimeOffset.FromUnixTimeSeconds(item.BuffDmg);
						return new LogStartEvent(item.Time, serverTime, localTime);
					}
					case StateChange.LogEnd:
					{
						var serverTime = DateTimeOffset.FromUnixTimeSeconds(item.Value);
						var localTime = DateTimeOffset.FromUnixTimeSeconds(item.BuffDmg);
						return new LogEndEvent(item.Time, serverTime, localTime);
					}
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
					case StateChange.PointOfView:
						return new PointOfViewEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.Language:
						return new LanguageEvent(item.Time, (int) item.SrcAgent);
					case StateChange.GWBuild:
						return new GameBuildEvent(item.Time, (int) item.SrcAgent);
					case StateChange.ShardId:
						return new GameShardEvent(item.Time, item.SrcAgent);
					case StateChange.Reward:
						return new RewardEvent(item.Time, item.DstAgent, item.Value);
					case StateChange.BuffInitial:
						return new InitialBuffEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId));
					case StateChange.Position:
					{
						float x = BitConversions.ToSingle((uint) (item.DstAgent & 0xFFFFFFFF));
						float y = BitConversions.ToSingle((uint) (item.DstAgent >> 32 & 0xFFFFFFFF));
						float z = BitConversions.ToSingle((item.Value));

						return new PositionChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y, z);
					}
					case StateChange.Velocity:
					{
						float x = BitConversions.ToSingle((uint) (item.DstAgent & 0xFFFFFFFF));
						float y = BitConversions.ToSingle((uint) (item.DstAgent >> 32 & 0xFFFFFFFF));
						float z = BitConversions.ToSingle((item.Value));

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
						return new SingleStackRemovedBuffEvent(item.Time, agent, buff, cleansingAgent,
							remainingDuration, remainingIntensity, stacksRemoved);
					case BuffRemove.Manual:
						return new ManualSingleStackRemovedBuffEvent(item.Time, agent, buff, cleansingAgent,
							remainingDuration, remainingIntensity, stacksRemoved);
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