using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	public class DefaultEncounterIdentifier : IEncounterIdentifier
	{
		public IEncounterData GetEncounterData(Agent mainTarget, IReadOnlyList<Event> events, IReadOnlyList<Agent> agents, int? gameBuild, LogType logType)
		{
			switch (logType)
			{
				case LogType.PvE:
					return GetPvEEncounterData(mainTarget, events, agents, gameBuild);
				case LogType.WorldVersusWorld:
					return GetWvWEncounterData(agents);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private IEncounterData GetWvWEncounterData(IReadOnlyList<Agent> agents)
		{
			return new WorldVersusWorldEncounterData(
				agents.OfType<Player>().Where(x => x.Subgroup == -1)
			);
		}

		private IEncounterData GetPvEEncounterData(Agent mainTarget, IReadOnlyList<Event> events, IReadOnlyList<Agent> agents, int? gameBuild)
		{
			if (mainTarget is NPC boss)
			{
				if (boss.Id == SpeciesIds.ValeGuardian)
				{
					var redGuardians = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.RedGuardian)
						.ToArray();
					var greenGuardians = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.GreenGuardian)
						.ToArray();
					var blueGuardians = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.BlueGuardian)
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

					return new BaseEncounterData(
						Encounter.ValeGuardian,
						new[] {boss},
						new PhaseSplitter(
							new StartTrigger(new PhaseDefinition("Phase 1", boss)),
							new BuffAddTrigger(boss, SkillIds.Invulnerability,
								new PhaseDefinition("Split 1", split1Guardians)),
							new BuffRemoveTrigger(boss, SkillIds.Invulnerability, new PhaseDefinition("Phase 2", boss)),
							new BuffAddTrigger(boss, SkillIds.Invulnerability,
								new PhaseDefinition("Split 2", split2Guardians)),
							new BuffRemoveTrigger(boss, SkillIds.Invulnerability, new PhaseDefinition("Phase 3", boss))
						),
						new AgentDeadDeterminer(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Nikare || boss.SpeciesId == SpeciesIds.Kenut)
				{
					var nikare = GetTargetBySpeciesId(agents, SpeciesIds.Nikare);
					var kenut = GetTargetBySpeciesId(agents, SpeciesIds.Kenut);

					var bosses = new List<NPC>();
					if (nikare != null) bosses.Add(nikare);
					if (kenut != null) bosses.Add(kenut);

					return new BaseEncounterData(
						Encounter.TwinLargos,
						bosses,
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
						)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Gorseval)
				{
					var chargedSouls = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.ChargedSoul)
						.ToArray();

					var split1Souls = chargedSouls.Length >= 4 ? chargedSouls.Take(4).ToArray() : new Agent[0];
					var split2Souls = chargedSouls.Length >= 8 ? chargedSouls.Skip(4).Take(4).ToArray() : new Agent[0];

					return new BaseEncounterData(
						Encounter.Gorseval,
						new[] {boss},
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
						new AgentDeadDeterminer(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Sabetha)
				{
					var kernan = GetTargetBySpeciesId(agents, SpeciesIds.Kernan);
					var knuckles = GetTargetBySpeciesId(agents, SpeciesIds.Knuckles);
					var karde = GetTargetBySpeciesId(agents, SpeciesIds.Karde);

					return new BaseEncounterData(
						Encounter.Sabetha,
						new[] {boss},
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
						new AgentDeadDeterminer(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Qadim)
				{
					var hydra = GetTargetBySpeciesId(agents, SpeciesIds.AncientInvokedHydra);
					var destroyer = GetTargetBySpeciesId(agents, SpeciesIds.ApocalypseBringer);
					var matriarch = GetTargetBySpeciesId(agents, SpeciesIds.WyvernMatriarch);
					var patriarch = GetTargetBySpeciesId(agents, SpeciesIds.WyvernPatriarch);

					return new BaseEncounterData(
						Encounter.Qadim,
						new[] {boss},
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
						new AgentCombatExitDeterminer(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Berg ||
				    boss.SpeciesId == SpeciesIds.Zane ||
				    boss.SpeciesId == SpeciesIds.Narella)
				{
					var berg = GetTargetBySpeciesId(agents, SpeciesIds.Berg);
					var zane = GetTargetBySpeciesId(agents, SpeciesIds.Zane);
					var narella = GetTargetBySpeciesId(agents, SpeciesIds.Narella);
					var prisoner = GetTargetBySpeciesId(agents, SpeciesIds.TrioCagePrisoner);

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

					return new BaseEncounterData(
						Encounter.BanditTrio,
						new[] {berg, zane, narella}.Where(x => x != null),
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						resultDeterminer
					);
				}

				if (boss.SpeciesId == SpeciesIds.Xera)
				{
					var secondPhaseXera = GetTargetBySpeciesId(agents, SpeciesIds.XeraSecondPhase);

					IResultDeterminer resultDeterminer;
					if (secondPhaseXera == null)
					{
						resultDeterminer = new ConstantResultDeterminer(EncounterResult.Failure);
					}
					else
					{
						resultDeterminer = new AgentCombatExitDeterminer(secondPhaseXera);
					}

					return new BaseEncounterData(
						Encounter.Xera,
						new[] {boss},
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						resultDeterminer
					);
				}

				if (boss.SpeciesId == SpeciesIds.SoullessHorror)
				{
					return new BaseEncounterData(
						Encounter.SoullessHorror,
						new[] {boss},
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						new AgentBuffGainedDeterminer(boss, SkillIds.SoullessHorrorDetermined)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Desmina)
				{
					long startTime = events.FirstOrDefault()?.Time ?? -1;
					return new BaseEncounterData(
						Encounter.RiverOfSouls,
						new[] {boss},
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						// At the end of the event, 8 of the rifts become untargetable
						new GroupedEventDeterminer<TargetableChangeEvent>(
							e => e.Time - startTime > 3000 && e.IsTargetable == false, 8, 1000)
					);
				}

				// Eyes logs sometimes get Dhuum as the boss when the player is too close to him
				if (boss.SpeciesId == SpeciesIds.EyeOfJudgment ||
				    boss.SpeciesId == SpeciesIds.EyeOfFate ||
				    boss.SpeciesId == SpeciesIds.Dhuum &&
				    agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.EyeOfFate))
				{
					var fate = GetTargetBySpeciesId(agents, SpeciesIds.EyeOfFate);
					var judgment = GetTargetBySpeciesId(agents, SpeciesIds.EyeOfJudgment);

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

					return new BaseEncounterData(
						Encounter.Eyes,
						new[] {fate, judgment}.Where(x => x != null),
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						resultDeterminer
					);
				}

				if (boss.SpeciesId == SpeciesIds.Deimos)
				{
					IResultDeterminer resultDeterminer;
					if (gameBuild != null && gameBuild < GameBuilds.AhdashimRelease)
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
						foreach (var gadget in agents.OfType<Gadget>().Where(x => x.AttackTargets.Count == 1))
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


					return new BaseEncounterData(
						Encounter.Deimos,
						new[] {boss},
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						resultDeterminer
					);
				}

				if (boss.SpeciesId == SpeciesIds.Artsariiv)
				{
					return new BaseEncounterData(
						Encounter.Artsariiv,
						new[] {boss},
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						new AgentKillingBlowDeterminer(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Arkk)
				{
					return new BaseEncounterData(
						Encounter.Arkk,
						new[] {boss},
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						new AgentKillingBlowDeterminer(boss)
					);
				}

				// TODO: Check if these are all the possible kitty golem IDs
				// TODO: Improve the detection if possible
				if (boss.SpeciesId == SpeciesIds.StandardKittyGolem || boss.SpeciesId == SpeciesIds.MediumKittyGolem ||
				    boss.SpeciesId == SpeciesIds.LargeKittyGolem || boss.SpeciesId == SpeciesIds.MassiveKittyGolem)
				{
					var encounter = boss.SpeciesId switch
					{
						SpeciesIds.StandardKittyGolem => Encounter.StandardKittyGolem,
						SpeciesIds.MediumKittyGolem => Encounter.MediumKittyGolem,
						SpeciesIds.LargeKittyGolem => Encounter.LargeKittyGolem,
						SpeciesIds.MassiveKittyGolem => Encounter.MassiveKittyGolem,
						_ => throw new ArgumentOutOfRangeException()
					};

					return new BaseEncounterData(
						encounter,
						new[] {boss},
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						new TransformResultDeterminer(
							new AgentKillingBlowDeterminer(boss),
							result => result == EncounterResult.Failure ? EncounterResult.Unknown : result
						)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Freezie)
				{
					return new BaseEncounterData(
						Encounter.Freezie,
						new[] {boss},
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", boss))),
						new AgentBuffGainedDeterminer(boss, SkillIds.Determined)
					);
				}
			}
			else if (mainTarget is Gadget gadgetBoss)
			{
				if (gadgetBoss.VolatileId == GadgetIds.ConjuredAmalgamate)
				{
					return new BaseEncounterData(
						Encounter.ConjuredAmalgamate,
						new[] {gadgetBoss},
						new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", gadgetBoss))),
						new NPCSpawnDeterminer(SpeciesIds.RoleplayZommoros)
					);
				}
			}

			return GetDefaultEncounterData(Encounter.Other, mainTarget);
		}

		private static IEncounterData GetDefaultEncounterData(Encounter encounter, Agent mainTarget)
		{
			return new BaseEncounterData(
				encounter,
				new[] {mainTarget},
				new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
				new AgentDeadDeterminer(mainTarget));
		}

		private NPC GetTargetBySpeciesId(IEnumerable<Agent> agents, int speciesId)
		{
			// When leaving the detection range and returning, another copy of the NPC may appear in the log.
			// As this is rather rare, we take the one that had events spanning the longest time
			long maxAwareTime = int.MinValue;
			NPC target = null;
			foreach (var agent in agents)
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