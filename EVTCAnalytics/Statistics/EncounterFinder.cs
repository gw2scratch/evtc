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
						new AgentDeathResultDeterminer(boss),
						new AgentNameEncounterNameProvider(boss));
				}

				if (boss.SpeciesId == SpeciesIds.Nikare || boss.SpeciesId == SpeciesIds.Kenut)
				{
					var nikare = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Nikare);
					var kenut = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Kenut);

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
							new AgentDeathResultDeterminer(nikare),
							new AgentDeathResultDeterminer(kenut)
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
						new AgentDeathResultDeterminer(boss),
						new AgentNameEncounterNameProvider(boss));
				}

				if (boss.SpeciesId == SpeciesIds.Sabetha)
				{
					var kernan = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Kernan);
					var knuckles = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Knuckles);
					var karde = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Karde);

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
						new AgentDeathResultDeterminer(boss),
						new AgentNameEncounterNameProvider(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Qadim)
				{
					var hydra = log.Agents.OfType<NPC>()
						.FirstOrDefault(x => x.SpeciesId == SpeciesIds.AncientInvokedHydra);
					var destroyer = log.Agents.OfType<NPC>()
						.FirstOrDefault(x => x.SpeciesId == SpeciesIds.ApocalypseBringer);
					var matriarch = log.Agents.OfType<NPC>()
						.FirstOrDefault(x => x.SpeciesId == SpeciesIds.WyvernMatriarch);
					var patriarch = log.Agents.OfType<NPC>()
						.FirstOrDefault(x => x.SpeciesId == SpeciesIds.WyvernPatriarch);

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
						new AgentExitCombatDeterminer(boss),
						new AgentNameEncounterNameProvider(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Berg ||
				    boss.SpeciesId == SpeciesIds.Zane ||
				    boss.SpeciesId == SpeciesIds.Narella)
				{
					var berg = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Berg);
					var zane = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Zane);
					var narella = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Narella);
					var prisoner = log.Agents.OfType<NPC>()
						.FirstOrDefault(x => x.SpeciesId == SpeciesIds.TrioCagePrisoner);

					IResultDeterminer resultDeterminer;
					if (berg != null && zane != null && narella != null && prisoner != null)
					{
						resultDeterminer = new AllCombinedResultDeterminer(
							new AgentDeathResultDeterminer(berg), // Berg has to die
							new AgentDeathResultDeterminer(zane), // So does Zane
							new AgentAliveDeterminer(prisoner), // The prisoner in the cage must survive
							new AnyCombinedResultDeterminer(
								// Narella has to be below 3% health
								new AgentBelowHealthFractionDeterminer(narella, 0.03f),
								// Or killed with a physical killing blow above 3% (a Warg can do that)
								new AgentKillingBlowDeterminer(narella)
							),
							new AnyCombinedResultDeterminer(
								new AgentExitCombatDeterminer(narella), // Narella typically doesn't die; exits combat
								new AgentDeathResultDeterminer(narella) // Or she can seldom die
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
					var fate = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.EyeOfFate);
					var judgment = log.Agents.OfType<NPC>()
						.FirstOrDefault(x => x.SpeciesId == SpeciesIds.EyeOfJudgment);

					IResultDeterminer resultDeterminer;
					if (fate == null || judgment == null)
					{
						resultDeterminer = new ConstantResultDeterminer(EncounterResult.Unknown);
					}
					else
					{
						resultDeterminer = new AnyCombinedResultDeterminer(
							new AgentDeathResultDeterminer(judgment),
							new AgentDeathResultDeterminer(fate)
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
			}

			return new DefaultEncounter(log.MainTarget, log.Events);
		}
	}
}