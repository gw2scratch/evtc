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
            // TODO: Fix all .First calls
			if (log.MainTarget is NPC boss)
			{
				if (boss.Id == SpeciesIds.ValeGuardian)
				{
					var invulnerability = log.Skills.First(x => x.Id == SkillIds.Invulnerability);

					var redGuardians = log.Agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.RedGuardian).ToArray();
					var greenGuardians = log.Agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.GreenGuardian).ToArray();
					var blueGuardians = log.Agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.BlueGuardian).ToArray();

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
							new BuffAddTrigger(boss, invulnerability, new PhaseDefinition("Split 1", split1Guardians)),
							new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 2", boss)),
							new BuffAddTrigger(boss, invulnerability, new PhaseDefinition("Split 2", split2Guardians)),
							new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 3", boss))
						),
						new AgentDeathResultDeterminer(boss),
						new AgentNameEncounterNameProvider(boss));
				}

				if (boss.SpeciesId == SpeciesIds.Nikare || boss.SpeciesId == SpeciesIds.Kenut)
				{
					var determined = log.Skills.First(x => x.Id == SkillIds.Determined);
					var nikare = log.Agents.OfType<NPC>().First(x => x.SpeciesId == SpeciesIds.Nikare);
					var kenut = log.Agents.OfType<NPC>().First(x => x.SpeciesId == SpeciesIds.Kenut);

					return new BaseEncounter(
						new[] {nikare, kenut},
						log.Events,
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

				if (boss.SpeciesId == SpeciesIds.Gorseval)
				{
					var invulnerability = log.Skills.First(x => x.Id == SkillIds.GorsevalInvulnerability);

					var chargedSouls = log.Agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.ChargedSoul).ToArray();

					var split1Souls = chargedSouls.Length >= 4 ? chargedSouls.Take(4).ToArray() : new Agent[0];
					var split2Souls = chargedSouls.Length >= 8 ? chargedSouls.Skip(4).Take(4).ToArray() : new Agent[0];

					return new BaseEncounter(
						new[] {boss},
						log.Events,
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

				if (boss.SpeciesId == SpeciesIds.Sabetha)
				{
					var invulnerability = log.Skills.First(x => x.Id == SkillIds.Invulnerability);

					var kernan = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Kernan);
					var knuckles = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Knuckles);
					var karde = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.Karde);

					return new BaseEncounter(
						new[] {boss},
						log.Events,
						new PhaseSplitter(
							new StartTrigger(new PhaseDefinition("Phase 1", boss)),
							new BuffAddTrigger(boss, invulnerability,
								new PhaseDefinition("Kernan", kernan != null ? new Agent[] {kernan} : new Agent[0])),
							new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 2", boss)),
							new BuffAddTrigger(boss, invulnerability,
								new PhaseDefinition("Knuckles",
									knuckles != null ? new Agent[] {knuckles} : new Agent[0])),
							new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 3", boss)),
							new BuffAddTrigger(boss, invulnerability,
								new PhaseDefinition("Karde", karde != null ? new Agent[] {karde} : new Agent[0])),
							new BuffRemoveTrigger(boss, invulnerability, new PhaseDefinition("Phase 4", boss))
						),
						new AgentDeathResultDeterminer(boss),
						new AgentNameEncounterNameProvider(boss)
					);
				}

				if (boss.SpeciesId == SpeciesIds.Qadim)
				{
					var flameArmor = log.Skills.First(x => x.Id == SkillIds.QadimFlameArmor);

					var hydra = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.AncientInvokedHydra);
					var destroyer = log.Agents.OfType<NPC>()
						.FirstOrDefault(x => x.SpeciesId == SpeciesIds.ApocalypseBringer);
					var matriarch = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.WyvernMatriarch);
					var patriarch = log.Agents.OfType<NPC>().FirstOrDefault(x => x.SpeciesId == SpeciesIds.WyvernPatriarch);

					return new BaseEncounter(
						new[] {boss},
						log.Events,
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
							new MultipleAgentsDeadTrigger(
								new PhaseDefinition("Jumping puzzle"), // TODO: Add Pyre Guardians
								matriarch != null && patriarch != null
									? new Agent[] {matriarch, patriarch}
									: new Agent[0]),
							new BuffRemoveTrigger(boss, flameArmor, new PhaseDefinition("Qadim 33-0%", boss))
						),
						new AgentExitCombatDeterminer(boss),
						new AgentNameEncounterNameProvider(boss)
					);
				}
			}

			return new DefaultEncounter(log.MainTarget, log.Events);
		}
	}
}