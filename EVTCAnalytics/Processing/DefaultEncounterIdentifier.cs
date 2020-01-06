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
			var encounter = IdentifyEncounter(mainTarget, agents);

			if (encounter == Encounter.ValeGuardian)
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
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(
						new StartTrigger(new PhaseDefinition("Phase 1", mainTarget)),
						new BuffAddTrigger(mainTarget, SkillIds.Invulnerability,
							new PhaseDefinition("Split 1", split1Guardians)),
						new BuffRemoveTrigger(mainTarget, SkillIds.Invulnerability, new PhaseDefinition("Phase 2", mainTarget)),
						new BuffAddTrigger(mainTarget, SkillIds.Invulnerability,
							new PhaseDefinition("Split 2", split2Guardians)),
						new BuffRemoveTrigger(mainTarget, SkillIds.Invulnerability, new PhaseDefinition("Phase 3", mainTarget))
					),
					new AgentDeadDeterminer(mainTarget)
				);
			}

			if (encounter == Encounter.TwinLargos)
			{
				var nikare = GetTargetBySpeciesId(agents, SpeciesIds.Nikare);
				var kenut = GetTargetBySpeciesId(agents, SpeciesIds.Kenut);

				var bosses = new List<NPC>();
				if (nikare != null) bosses.Add(nikare);
				if (kenut != null) bosses.Add(kenut);

				return new BaseEncounterData(
					encounter,
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

			if (encounter == Encounter.Gorseval)
			{
				var chargedSouls = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.ChargedSoul)
					.ToArray();

				var split1Souls = chargedSouls.Length >= 4 ? chargedSouls.Take(4).ToArray() : new Agent[0];
				var split2Souls = chargedSouls.Length >= 8 ? chargedSouls.Skip(4).Take(4).ToArray() : new Agent[0];

				return new BaseEncounterData(
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(
						new StartTrigger(new PhaseDefinition("Phase 1", mainTarget)),
						new BuffAddTrigger(mainTarget, SkillIds.GorsevalInvulnerability,
							new PhaseDefinition("Split 1", split1Souls)),
						new BuffRemoveTrigger(mainTarget, SkillIds.GorsevalInvulnerability,
							new PhaseDefinition("Phase 2", mainTarget)),
						new BuffAddTrigger(mainTarget, SkillIds.GorsevalInvulnerability,
							new PhaseDefinition("Split 2", split2Souls)),
						new BuffRemoveTrigger(mainTarget, SkillIds.GorsevalInvulnerability,
							new PhaseDefinition("Phase 3", mainTarget))
					),
					new AgentDeadDeterminer(mainTarget)
				);
			}

			if (encounter == Encounter.Sabetha)
			{
				var kernan = GetTargetBySpeciesId(agents, SpeciesIds.Kernan);
				var knuckles = GetTargetBySpeciesId(agents, SpeciesIds.Knuckles);
				var karde = GetTargetBySpeciesId(agents, SpeciesIds.Karde);

				return new BaseEncounterData(
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(
						new StartTrigger(new PhaseDefinition("Phase 1", mainTarget)),
						new BuffAddTrigger(mainTarget, SkillIds.Invulnerability,
							new PhaseDefinition("Kernan", kernan != null ? new Agent[] {kernan} : new Agent[0])),
						new BuffRemoveTrigger(mainTarget, SkillIds.Invulnerability, new PhaseDefinition("Phase 2", mainTarget)),
						new BuffAddTrigger(mainTarget, SkillIds.Invulnerability,
							new PhaseDefinition("Knuckles",
								knuckles != null ? new Agent[] {knuckles} : new Agent[0])),
						new BuffRemoveTrigger(mainTarget, SkillIds.Invulnerability, new PhaseDefinition("Phase 3", mainTarget)),
						new BuffAddTrigger(mainTarget, SkillIds.Invulnerability,
							new PhaseDefinition("Karde", karde != null ? new Agent[] {karde} : new Agent[0])),
						new BuffRemoveTrigger(mainTarget, SkillIds.Invulnerability, new PhaseDefinition("Phase 4", mainTarget))
					),
					new AgentDeadDeterminer(mainTarget)
				);
			}

			if (encounter == Encounter.Qadim)
			{
				var hydra = GetTargetBySpeciesId(agents, SpeciesIds.AncientInvokedHydra);
				var destroyer = GetTargetBySpeciesId(agents, SpeciesIds.ApocalypseBringer);
				var matriarch = GetTargetBySpeciesId(agents, SpeciesIds.WyvernMatriarch);
				var patriarch = GetTargetBySpeciesId(agents, SpeciesIds.WyvernPatriarch);

				return new BaseEncounterData(
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(
						new AgentEventTrigger<TeamChangeEvent>(mainTarget,
							new PhaseDefinition("Hydra phase", hydra != null ? new Agent[] {hydra} : new Agent[0])),
						new BuffRemoveTrigger(mainTarget, SkillIds.QadimFlameArmor,
							new PhaseDefinition("Qadim 100-66%", mainTarget)),
						new BuffAddTrigger(mainTarget, SkillIds.QadimFlameArmor,
							new PhaseDefinition("Destroyer phase",
								destroyer != null ? new Agent[] {destroyer} : new Agent[0])),
						new BuffRemoveTrigger(mainTarget, SkillIds.QadimFlameArmor,
							new PhaseDefinition("Qadim 66-33%", mainTarget)),
						new BuffAddTrigger(mainTarget, SkillIds.QadimFlameArmor,
							new PhaseDefinition("Wyvern phase",
								matriarch != null && patriarch != null
									? new Agent[] {matriarch, patriarch}
									: new Agent[0])),
						new MultipleAgentsDeadTrigger(
							new PhaseDefinition("Jumping puzzle"),
							matriarch != null && patriarch != null
								? new Agent[] {matriarch, patriarch}
								: new Agent[0]),
						new BuffRemoveTrigger(mainTarget, SkillIds.QadimFlameArmor,
							new PhaseDefinition("Qadim 33-0%", mainTarget))),
					new AgentCombatExitDeterminer(mainTarget)
				);
			}

			if (encounter == Encounter.BanditTrio)
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

				var targets = new[] {berg, zane, narella}.Where(x => x != null).ToArray();
				return new BaseEncounterData(
					encounter,
					targets,
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", targets))),
					resultDeterminer
				);
			}

			if (encounter == Encounter.Xera)
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
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
					resultDeterminer
				);
			}

			if (encounter == Encounter.SoullessHorror)
			{
				return new BaseEncounterData(
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
					new AgentBuffGainedDeterminer(mainTarget, SkillIds.SoullessHorrorDetermined)
				);
			}

			if (encounter == Encounter.RiverOfSouls)
			{
				long startTime = events.FirstOrDefault()?.Time ?? -1;
				return new BaseEncounterData(
					Encounter.RiverOfSouls,
					new[] {mainTarget},
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
					// At the end of the event, 8 of the rifts become untargetable
					new GroupedEventDeterminer<TargetableChangeEvent>(
						e => e.Time - startTime > 3000 && e.IsTargetable == false, 8, 1000)
				);
			}

			if (encounter == Encounter.Eyes)
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

				var targets = new[] {fate, judgment}.Where(x => x != null).ToArray();
				return new BaseEncounterData(
					encounter,
					targets,
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", targets))),
					resultDeterminer
				);
			}

			if (encounter == Encounter.Deimos)
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
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
					resultDeterminer
				);
			}

			if (encounter == Encounter.Artsariiv)
			{
				return new BaseEncounterData(
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
					new AgentKillingBlowDeterminer(mainTarget)
				);
			}

			if (encounter == Encounter.Arkk)
			{
				return new BaseEncounterData(
					Encounter.Arkk,
					new[] {mainTarget},
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
					new AgentKillingBlowDeterminer(mainTarget)
				);
			}

			// TODO: Check if these are all the possible kitty golem IDs
			// TODO: Improve the success detection if possible
			if (encounter == Encounter.StandardKittyGolem
			    || encounter == Encounter.MediumKittyGolem
			    || encounter == Encounter.LargeKittyGolem
			    || encounter == Encounter.MassiveKittyGolem)
			{
				return new BaseEncounterData(
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
					new TransformResultDeterminer(
						new AgentKillingBlowDeterminer(mainTarget),
						result => result == EncounterResult.Failure ? EncounterResult.Unknown : result
					)
				);
			}

			if (encounter == Encounter.Freezie)
			{
				return new BaseEncounterData(
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
					new AgentBuffGainedDeterminer(mainTarget, SkillIds.Determined)
				);
			}

			if (encounter == Encounter.ConjuredAmalgamate)
			{
				return new BaseEncounterData(
					encounter,
					new[] {mainTarget},
					new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
					new NPCSpawnDeterminer(SpeciesIds.RoleplayZommoros)
				);
			}

			return GetDefaultEncounterData(encounter, mainTarget);
		}

		public Encounter IdentifyEncounter(Agent mainTarget, IReadOnlyList<Agent> agents)
		{
			if (mainTarget is NPC boss)
			{
				switch (boss.SpeciesId)
				{
					case SpeciesIds.ValeGuardian:
						return Encounter.ValeGuardian;
					case SpeciesIds.Gorseval:
						return Encounter.Gorseval;
					case SpeciesIds.Sabetha:
						return Encounter.Sabetha;
					case SpeciesIds.Slothasor:
						return Encounter.Slothasor;
					case SpeciesIds.Berg:
					case SpeciesIds.Zane:
					case SpeciesIds.Narella:
						return Encounter.BanditTrio;
					case SpeciesIds.MatthiasGabrel:
						return Encounter.Matthias;
					case SpeciesIds.KeepConstruct:
						return Encounter.KeepConstruct;
					case SpeciesIds.HauntingStatue:
						return Encounter.TwistedCastle;
					case SpeciesIds.Xera:
						return Encounter.Xera;
					case SpeciesIds.CairnTheIndomitable:
						return Encounter.Cairn;
					case SpeciesIds.MursaatOverseer:
						return Encounter.MursaatOverseer;
					case SpeciesIds.Samarog:
						return Encounter.Samarog;
					case SpeciesIds.Deimos:
						return Encounter.Deimos;
					case SpeciesIds.SoullessHorror:
						return Encounter.SoullessHorror;
					case SpeciesIds.Desmina:
						return Encounter.RiverOfSouls;
					case SpeciesIds.EyeOfJudgment:
					case SpeciesIds.EyeOfFate:
						return Encounter.Eyes;
					case SpeciesIds.Dhuum:
						// Eyes logs sometimes get Dhuum as the main target when the player is too close to him
						if (agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.EyeOfFate))
						{
							return Encounter.Eyes;
						}

						return Encounter.Dhuum;
					case SpeciesIds.Nikare:
					case SpeciesIds.Kenut:
						return Encounter.TwinLargos;
					case SpeciesIds.Qadim:
						return Encounter.Qadim;
					case SpeciesIds.CardinalAdina:
						return Encounter.Adina;
					case SpeciesIds.CadinalSabir:
						return Encounter.Sabir;
					case SpeciesIds.QadimThePeerless:
						return Encounter.QadimThePeerless;
					case SpeciesIds.StandardKittyGolem:
						return Encounter.StandardKittyGolem;
					case SpeciesIds.MediumKittyGolem:
						return Encounter.MediumKittyGolem;
					case SpeciesIds.LargeKittyGolem:
						return Encounter.LargeKittyGolem;
					case SpeciesIds.MassiveKittyGolem:
						return Encounter.MassiveKittyGolem;
					case SpeciesIds.MAMA:
						return Encounter.MAMA;
					case SpeciesIds.SiaxTheCorrupted:
						return Encounter.SiaxTheCorrupted;
					case SpeciesIds.EnsolyssOfTheEndlessTorment:
						return Encounter.EnsolyssOfTheEndlessTorment;
					case SpeciesIds.Skorvald:
						return Encounter.Skorvald;
					case SpeciesIds.Artsariiv:
						return Encounter.Artsariiv;
					case SpeciesIds.Arkk:
						return Encounter.Arkk;
					case SpeciesIds.Freezie:
						return Encounter.Freezie;
					case SpeciesIds.IcebroodConstruct:
						return Encounter.ShiverpeaksPass;
					case SpeciesIds.VoiceOfTheFallen:
					case SpeciesIds.ClawOfTheFallen:
						return Encounter.VoiceAndClawOfTheFallen;
					case SpeciesIds.FraenirOfJormag:
						return Encounter.FraenirOfJormag;
					case SpeciesIds.Boneskinner:
						return Encounter.Boneskinner;
				}
			}
			else if (mainTarget is Gadget gadgetBoss)
			{
				switch (gadgetBoss.VolatileId)
				{
					case GadgetIds.ConjuredAmalgamate:
						return Encounter.ConjuredAmalgamate;
				}
			}

			return Encounter.Other;
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