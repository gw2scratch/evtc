using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.Statistics
{
	public class EncounterFinder : IEncounterFinder
	{
		public IEncounter GetEncounter(Log log)
		{
			switch (log.LogType)
			{
				case LogType.PvE:
					return GetPvEEncounter(log);
				case LogType.WorldVersusWorld:
					return GetWvWEncounter(log);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private IEncounter GetWvWEncounter(Log log)
		{
			return new WorldVersusWorldEncounter(log.Agents.OfType<Player>().Where(x => x.Subgroup == -1), log.Events);
		}

		private IEncounter GetPvEEncounter(Log log)
		{
			if (log.MainTarget is NPC boss)
			{
				if (boss.Id == SpeciesIds.ValeGuardian)
				{
					var redGuardians = log.Agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.RedGuardian)
						.ToArray();
					var greenGuardians = log.Agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.GreenGuardian)
						.ToArray();
					var blueGuardians = log.Agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.BlueGuardian)
						.ToArray();

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

					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(
							new StartTrigger(new PhaseDefinition("Phase 1", boss)),
							new BuffAddTrigger(boss, SkillIds.Invulnerability,
								new PhaseDefinition("Split 1", split1Guardians)),
							new BuffRemoveTrigger(boss, SkillIds.Invulnerability, new PhaseDefinition("Phase 2", boss)),
							new BuffAddTrigger(boss, SkillIds.Invulnerability,
								new PhaseDefinition("Split 2", split2Guardians)),
							new BuffRemoveTrigger(boss, SkillIds.Invulnerability, new PhaseDefinition("Phase 3", boss))
						),
						new AgentDeadDeterminer(boss),
						new AgentNameEncounterNameProvider(boss));
				}

				if (boss.SpeciesId == SpeciesIds.Nikare || boss.SpeciesId == SpeciesIds.Kenut)
				{
					var nikare = GetTargetBySpeciesId(log, SpeciesIds.Nikare);
					var kenut = GetTargetBySpeciesId(log, SpeciesIds.Kenut);

					var bosses = new List<NPC>();
					if (nikare != null) bosses.Add(nikare);
					if (kenut != null) bosses.Add(kenut);

					return new BaseEncounter(
						bosses,
						log.Events,
						new PhaseSplitter(
							new StartTrigger(new PhaseDefinition("Nikare's platform", nikare)),
							new BuffAddTrigger(nikare, SkillIds.Determined,
								new PhaseDefinition("Kenut's platform", kenut)),
							new BuffAddTrigger(kenut, SkillIds.Determined,
								new PhaseDefinition("Split phase", nikare, kenut))
						),
						new AllCombinedResultDeterminer(
							new AgentDeadDeterminer(nikare),
							new AgentDeadDeterminer(kenut)
						),
						new ConstantEncounterNameProvider("Twin Largos")
					);
				}

				if (boss.SpeciesId == SpeciesIds.Gorseval)
				{
					var chargedSouls = log.Agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.ChargedSoul)
						.ToArray();

					var split1Souls = chargedSouls.Length >= 4 ? chargedSouls.Take(4).ToArray() : new Agent[0];
					var split2Souls = chargedSouls.Length >= 8 ? chargedSouls.Skip(4).Take(4).ToArray() : new Agent[0];

					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(
							new StartTrigger(new PhaseDefinition("Phase 1", boss)),
							new BuffAddTrigger(boss, SkillIds.GorsevalInvulnerability,
								new PhaseDefinition("Split 1", split1Souls)),
							new BuffRemoveTrigger(boss, SkillIds.GorsevalInvulnerability,
								new PhaseDefinition("Phase 2", boss)),
							new BuffAddTrigger(boss, SkillIds.GorsevalInvulnerability,
								new PhaseDefinition("Split 2", split2Souls)),
							new BuffRemoveTrigger(boss, SkillIds.GorsevalInvulnerability,
								new PhaseDefinition("Phase 3", boss))
						),
						new AgentDeadDeterminer(boss),
						new AgentNameEncounterNameProvider(boss));
				}

				if (boss.SpeciesId == SpeciesIds.Sabetha)
				{
					var kernan = GetTargetBySpeciesId(log, SpeciesIds.Kernan);
					var knuckles = GetTargetBySpeciesId(log, SpeciesIds.Knuckles);
					var karde = GetTargetBySpeciesId(log, SpeciesIds.Karde);

					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(
							new StartTrigger(new PhaseDefinition("Phase 1", boss)),
							new BuffAddTrigger(boss, SkillIds.Invulnerability,
								new PhaseDefinition("Kernan", kernan != null ? new Agent[] {kernan} : new Agent[0])),
							new BuffRemoveTrigger(boss, SkillIds.Invulnerability, new PhaseDefinition("Phase 2", boss)),
							new BuffAddTrigger(boss, SkillIds.Invulnerability,
								new PhaseDefinition("Knuckles",
									knuckles != null ? new Agent[] {knuckles} : new Agent[0])),
							new BuffRemoveTrigger(boss, SkillIds.Invulnerability, new PhaseDefinition("Phase 3", boss)),
							new BuffAddTrigger(boss, SkillIds.Invulnerability,
								new PhaseDefinition("Karde", karde != null ? new Agent[] {karde} : new Agent[0])),
							new BuffRemoveTrigger(boss, SkillIds.Invulnerability, new PhaseDefinition("Phase 4", boss))
						),
						new AgentDeadDeterminer(boss),
						new AgentNameEncounterNameProvider(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Qadim)
				{
					var hydra = GetTargetBySpeciesId(log, SpeciesIds.AncientInvokedHydra);
					var destroyer = GetTargetBySpeciesId(log, SpeciesIds.ApocalypseBringer);
					var matriarch = GetTargetBySpeciesId(log, SpeciesIds.WyvernMatriarch);
					var patriarch = GetTargetBySpeciesId(log, SpeciesIds.WyvernPatriarch);

					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(
							new AgentEventTrigger<TeamChangeEvent>(boss,
								new PhaseDefinition("Hydra phase", hydra != null ? new Agent[] {hydra} : new Agent[0])),
							new BuffRemoveTrigger(boss, SkillIds.QadimFlameArmor,
								new PhaseDefinition("Qadim 100-66%", boss)),
							new BuffAddTrigger(boss, SkillIds.QadimFlameArmor,
								new PhaseDefinition("Destroyer phase",
									destroyer != null ? new Agent[] {destroyer} : new Agent[0])),
							new BuffRemoveTrigger(boss, SkillIds.QadimFlameArmor,
								new PhaseDefinition("Qadim 66-33%", boss)),
							new BuffAddTrigger(boss, SkillIds.QadimFlameArmor,
								new PhaseDefinition("Wyvern phase",
									matriarch != null && patriarch != null
										? new Agent[] {matriarch, patriarch}
										: new Agent[0])),
							new MultipleAgentsDeadTrigger(
								new PhaseDefinition("Jumping puzzle"),
								matriarch != null && patriarch != null
									? new Agent[] {matriarch, patriarch}
									: new Agent[0]),
							new BuffRemoveTrigger(boss, SkillIds.QadimFlameArmor,
								new PhaseDefinition("Qadim 33-0%", boss))),
						new AgentCombatExitDeterminer(boss),
						new AgentNameEncounterNameProvider(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Berg ||
				    boss.SpeciesId == SpeciesIds.Zane ||
				    boss.SpeciesId == SpeciesIds.Narella)
				{
					var berg = GetTargetBySpeciesId(log, SpeciesIds.Berg);
					var zane = GetTargetBySpeciesId(log, SpeciesIds.Zane);
					var narella = GetTargetBySpeciesId(log, SpeciesIds.Narella);
					var prisoner = GetTargetBySpeciesId(log, SpeciesIds.TrioCagePrisoner);

					IResultDeterminer resultDeterminer;
					if (berg != null && zane != null && narella != null && prisoner != null)
					{
						resultDeterminer = new AllCombinedResultDeterminer(
							new AgentDeadDeterminer(berg), // Berg has to die
							new AgentDeadDeterminer(zane), // So does Zane
							new AgentAliveDeterminer(prisoner), // The prisoner in the cage must survive
							new AnyCombinedResultDeterminer( // And finally, Narella has to perish as well
								new AgentKillingBlowDeterminer(narella),
								new AgentDeadDeterminer(narella)
							)
						);
					}
					else
					{
						resultDeterminer = new ConstantResultDeterminer(EncounterResult.Unknown);
					}

					return new BaseEncounter(
						new[] {berg, zane, narella}.Where(x => x != null),
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						resultDeterminer,
						new ConstantEncounterNameProvider("Bandit Trio")
					);
				}

				if (boss.SpeciesId == SpeciesIds.Xera)
				{
					var secondPhaseXera = GetTargetBySpeciesId(log, SpeciesIds.XeraSecondPhase);

					IResultDeterminer resultDeterminer;
					if (secondPhaseXera == null)
					{
						resultDeterminer = new ConstantResultDeterminer(EncounterResult.Failure);
					}
					else
					{
						resultDeterminer = new AgentCombatExitDeterminer(secondPhaseXera);
					}

					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						resultDeterminer,
						new AgentNameEncounterNameProvider(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.SoullessHorror)
				{
					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						new AgentBuffGainedDeterminer(boss, SkillIds.SoullessHorrorDetermined),
						new AgentNameEncounterNameProvider(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Desmina)
				{
					long startTime = log.Events.FirstOrDefault()?.Time ?? -1;
					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						// At the end of the event, 8 of the rifts become untargetable
						new GroupedEventDeterminer<TargetableChangeEvent>(
							e => e.Time - startTime > 3000 && e.IsTargetable == false, 8, 1000),
						new ConstantEncounterNameProvider("River of Souls")
					);
				}

				// Eyes logs sometimes get Dhuum as the boss when the player is too close to him
				if (boss.SpeciesId == SpeciesIds.EyeOfJudgment ||
				    boss.SpeciesId == SpeciesIds.EyeOfFate ||
				    boss.SpeciesId == SpeciesIds.Dhuum &&
				    log.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.EyeOfFate))
				{
					var fate = GetTargetBySpeciesId(log, SpeciesIds.EyeOfFate);
					var judgment = GetTargetBySpeciesId(log, SpeciesIds.EyeOfJudgment);

					IResultDeterminer resultDeterminer;
					if (fate == null || judgment == null)
					{
						resultDeterminer = new ConstantResultDeterminer(EncounterResult.Unknown);
					}
					else
					{
						resultDeterminer = new AnyCombinedResultDeterminer(
							new AgentDeadDeterminer(judgment),
							new AgentDeadDeterminer(fate)
						);
					}

					return new BaseEncounter(
						new[] {fate, judgment}.Where(x => x != null),
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						resultDeterminer,
						new ConstantEncounterNameProvider("Statue of Darkness")
					);
				}

				if (boss.SpeciesId == SpeciesIds.Deimos)
				{
					IResultDeterminer resultDeterminer;
					if (log.GameBuild < GameBuilds.AhdashimRelease)
					{
						// This release reworked rewards, currently a reward event is not present if an encounter was
						// finished a second time within a week. Before that, we can safely just check for the
						// presence of such a reward.

						// We cannot use the targetable detection method before they were introduced, and there was
						// a long period of time when logs did not contain the main gadget so we need to rely on this.
						resultDeterminer = new RewardDeterminer(525);
					}
					else
					{
						long maxAwareTime = -1;
						Gadget mainGadget = null;
						foreach (var gadget in log.Agents.OfType<Gadget>().Where(x => x.AttackTargets.Count == 1))
						{
							long aware = gadget.LastAwareTime - gadget.FirstAwareTime;
							if (aware > maxAwareTime)
							{
								maxAwareTime = aware;
								mainGadget = gadget;
							}
						}

						if (mainGadget != null)
						{
							resultDeterminer = new TargetableDeterminer(mainGadget.AttackTargets.Single(), false);
						}
						else
						{
							resultDeterminer = new ConstantResultDeterminer(EncounterResult.Unknown);
						}
					}


					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						resultDeterminer,
						new AgentNameEncounterNameProvider(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Arkk || boss.SpeciesId == SpeciesIds.Artsariiv)
				{
					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						new AgentKillingBlowDeterminer(boss),
						new AgentNameEncounterNameProvider(boss)
					);
				}

				// TODO: Check if these are all the possible kitty golem IDs
				// TODO: Improve the detection if possible
				if (boss.SpeciesId == SpeciesIds.StandardKittyGolem || boss.SpeciesId == SpeciesIds.MediumKittyGolem ||
				    boss.SpeciesId == SpeciesIds.LargeKittyGolem || boss.SpeciesId == SpeciesIds.MassiveKittyGolem)
				{
					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						new TransformResultDeterminer(
							new AgentKillingBlowDeterminer(boss),
							result => result == EncounterResult.Failure ? EncounterResult.Unknown : result
						),
						new AgentNameEncounterNameProvider(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Freezie)
				{
					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						new AgentBuffGainedDeterminer(boss, SkillIds.Determined),
						new AgentNameEncounterNameProvider(boss)
					);
				}
			}
			else if (log.MainTarget is Gadget gadgetBoss)
			{
				if (gadgetBoss.VolatileId == GadgetIds.ConjuredAmalgamate)
				{
					return new BaseEncounter(
						new[] {gadgetBoss},
						log.Events,
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", gadgetBoss))),
						new NPCSpawnDeterminer(SpeciesIds.RoleplayZommoros),
						new AgentNameEncounterNameProvider(gadgetBoss)
					);
				}
			}

			return new DefaultEncounter(log.MainTarget, log.Events);
		}

		private NPC GetTargetBySpeciesId(Log log, int speciesId)
		{
			// When leaving the detection range and returning, another copy of the NPC may appear in the log.
			// As this is rather rare, we take the one that had events spanning the longest time
			long maxAwareTime = int.MinValue;
			NPC target = null;
			foreach (var agent in log.Agents)
			{
				if (agent is NPC npc && npc.SpeciesId == speciesId)
				{
					long awareTime = npc.LastAwareTime - npc.FirstAwareTime;
					if (awareTime > maxAwareTime)
					{
						maxAwareTime = awareTime;
						target = npc;
					}
				}
			}

			return target;
		}
	}
}