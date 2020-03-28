using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using GW2Scratch.EVTCAnalytics.Processing.Steps;

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

			switch (encounter)
			{
				// Raids - Wing 1
				case Encounter.ValeGuardian:
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

					return GetDefaultBuilder(encounter, mainTarget)
						.WithPhases(new PhaseSplitter(
							new StartTrigger(new PhaseDefinition("Phase 1", mainTarget)),
							new BuffAddTrigger(mainTarget, SkillIds.Invulnerability,
								new PhaseDefinition("Split 1", split1Guardians)),
							new BuffRemoveTrigger(mainTarget, SkillIds.Invulnerability, new PhaseDefinition("Phase 2", mainTarget)),
							new BuffAddTrigger(mainTarget, SkillIds.Invulnerability,
								new PhaseDefinition("Split 2", split2Guardians)),
							new BuffRemoveTrigger(mainTarget, SkillIds.Invulnerability, new PhaseDefinition("Phase 3", mainTarget))
						)).Build();
				}
				case Encounter.Gorseval:
				{
					var chargedSouls = agents.OfType<NPC>().Where(x => x.SpeciesId == SpeciesIds.ChargedSoul)
						.ToArray();

					var split1Souls = chargedSouls.Length >= 4 ? chargedSouls.Take(4).ToArray() : new Agent[0];
					var split2Souls = chargedSouls.Length >= 8 ? chargedSouls.Skip(4).Take(4).ToArray() : new Agent[0];

					return GetDefaultBuilder(encounter, mainTarget)
						.WithPhases(new PhaseSplitter(
							new StartTrigger(new PhaseDefinition("Phase 1", mainTarget)),
							new BuffAddTrigger(mainTarget, SkillIds.GorsevalInvulnerability,
								new PhaseDefinition("Split 1", split1Souls)),
							new BuffRemoveTrigger(mainTarget, SkillIds.GorsevalInvulnerability,
								new PhaseDefinition("Phase 2", mainTarget)),
							new BuffAddTrigger(mainTarget, SkillIds.GorsevalInvulnerability,
								new PhaseDefinition("Split 2", split2Souls)),
							new BuffRemoveTrigger(mainTarget, SkillIds.GorsevalInvulnerability,
								new PhaseDefinition("Phase 3", mainTarget))
						)).Build();
				}
				case Encounter.Sabetha:
				{
					var kernan = GetTargetBySpeciesId(agents, SpeciesIds.Kernan);
					var knuckles = GetTargetBySpeciesId(agents, SpeciesIds.Knuckles);
					var karde = GetTargetBySpeciesId(agents, SpeciesIds.Karde);

					return GetDefaultBuilder(encounter, mainTarget)
						.WithPhases(new PhaseSplitter(
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
						)).Build();
				}
				// Raids - Wing 2
				case Encounter.Slothasor:
					return GetDefaultBuilder(encounter, mainTarget).Build();
				case Encounter.BanditTrio:
				{
					var berg = GetTargetBySpeciesId(agents, SpeciesIds.Berg);
					var zane = GetTargetBySpeciesId(agents, SpeciesIds.Zane);
					var narella = GetTargetBySpeciesId(agents, SpeciesIds.Narella);
					var prisoner = GetTargetBySpeciesId(agents, SpeciesIds.TrioCagePrisoner);

					var targets = new Agent[] {berg, zane, narella}.Where(x => x != null).ToArray();
					var builder = GetDefaultBuilder(encounter, targets);
					if (berg != null && zane != null && narella != null && prisoner != null)
					{
						builder.WithResult(new AllCombinedResultDeterminer(
							new AgentKilledDeterminer(berg), // Berg has to die
							new AgentKilledDeterminer(zane), // So does Zane
							new AgentAliveDeterminer(prisoner), // The prisoner in the cage must survive
							new AgentKilledDeterminer(narella) // And finally, Narella has to perish as well
						));
					}
					else
					{
						builder.WithResult(new ConstantResultDeterminer(EncounterResult.Unknown));
					}

					return builder.Build();
				}
				case Encounter.Matthias:
					return GetDefaultBuilder(encounter, mainTarget).Build();
				// Raids - Wing 3
				case Encounter.Escort:
					return GetDefaultBuilder(encounter, new Agent[0]).Build();
				case Encounter.KeepConstruct:
					return GetDefaultBuilder(encounter, mainTarget).Build();
				case Encounter.TwistedCastle:
				{
					return GetDefaultBuilder(encounter, new Agent[0])
						.WithResult(new RewardDeterminer(496))
						.Build();
				}
				case Encounter.Xera:
				{
					var secondPhaseXera = GetTargetBySpeciesId(agents, SpeciesIds.XeraSecondPhase);

					var builder = GetDefaultBuilder(encounter, mainTarget);
					if (secondPhaseXera == null)
					{
						builder.WithResult(new ConstantResultDeterminer(EncounterResult.Failure));
					}
					else
					{
						builder.WithResult(new AgentCombatExitDeterminer(secondPhaseXera))
							.WithTargets(new List<Agent>() {mainTarget, secondPhaseXera});
					}

					return builder.Build();
				}
				// Raids - Wing 4
				case Encounter.Cairn:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new SkillPresentModeDeterminer(SkillIds.CairnCountdown))
						.Build();
				}
				case Encounter.MursaatOverseer:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 29_000_000))
						.Build();
				}
				case Encounter.Samarog:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 39_000_000))
						.Build();
				}
				case Encounter.Deimos:
				{
					var builder = GetDefaultBuilder(encounter, mainTarget);
					if (gameBuild != null && gameBuild < GameBuilds.AhdashimRelease)
					{
						// This release reworked rewards, currently a reward event is not present if an encounter was
						// finished a second time within a week. Before that, we can safely just check for the
						// presence of such a reward.

						// We cannot use the targetable detection method before they were introduced, and there was
						// a long period of time when logs did not contain the main gadget so we need to rely on this.
						builder.WithResult(new RewardDeterminer(525));
					}
					else
					{
						Gadget mainGadget = agents.OfType<Gadget>().FirstOrDefault(x => x.VolatileId == GadgetIds.DeimosLastPhase);

						if (mainGadget != null)
						{
							builder.WithResult(new TargetableDeterminer(mainGadget.AttackTargets.Single(), true, false));
							builder.WithTargets(new List<Agent>() {mainTarget, mainGadget});
						}
						else
						{
							builder.WithResult(new ConstantResultDeterminer(EncounterResult.Unknown));
						}
					}

					return builder.WithModes(new AgentHealthModeDeterminer(mainTarget, 42_000_000))
						.Build();
				}
				// Raids - Wing 5
				case Encounter.SoullessHorror:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AgentBuffGainedDeterminer(mainTarget, SkillIds.SoullessHorrorDetermined))
						// Necrosis is applied faster in Challenge Mode. It is first removed and then reapplied
						// so we check the remaining time of the removed buff.
						.WithModes(new RemovedBuffStackRemainingTimeModeDeterminer(SkillIds.Necrosis,
							EncounterMode.Challenge, 23000, EncounterMode.Normal, 18000, EncounterMode.Normal))
						.Build();
				}
				case Encounter.RiverOfSouls:
				{
					long startTime = events.FirstOrDefault()?.Time ?? -1;
					return GetDefaultBuilder(encounter, mainTarget)
						// At the end of the event, 8 of the rifts become untargetable
						.WithResult(new GroupedEventDeterminer<TargetableChangeEvent>(
							e => e.Time - startTime > 3000 && e.IsTargetable == false, 8, 1000))
						.Build();
				}
				case Encounter.Eyes:
				{
					var fate = GetTargetBySpeciesId(agents, SpeciesIds.EyeOfFate);
					var judgment = GetTargetBySpeciesId(agents, SpeciesIds.EyeOfJudgment);

					var builder = GetDefaultBuilder(encounter, new[] {fate, judgment});
					if (fate == null || judgment == null)
					{
						builder.WithResult(new ConstantResultDeterminer(EncounterResult.Unknown));
						builder.WithTargets(new Agent[] {fate, judgment}.Where(x => x != null).ToList());
					}
					else
					{
						builder.WithResult(new AnyCombinedResultDeterminer(
							new AgentDeadDeterminer(judgment),
							new AgentDeadDeterminer(fate)
						));
					}

					return builder.Build();
				}
				case Encounter.Dhuum:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 39_000_000))
						.Build();
				}
				// Raids - Wing 6
				case Encounter.ConjuredAmalgamate:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new NPCSpawnDeterminer(SpeciesIds.RoleplayZommoros))
						.WithModes(new GroupedSpawnModeDeterminer(agent => agent is NPC npc && npc.SpeciesId == SpeciesIds.ConjuredGreatsword, 3, 200))
						.Build();
				}
				case Encounter.TwinLargos:
				{
					var nikare = GetTargetBySpeciesId(agents, SpeciesIds.Nikare);
					var kenut = GetTargetBySpeciesId(agents, SpeciesIds.Kenut);

					var bosses = new List<NPC>();
					if (nikare != null) bosses.Add(nikare);
					if (kenut != null) bosses.Add(kenut);

					var builder = GetDefaultBuilder(encounter, bosses);
					if (nikare == null || kenut == null)
					{
						builder.WithResult(new ConstantResultDeterminer(EncounterResult.Unknown));
						builder.WithTargets(new Agent[] {nikare, kenut}.Where(x => x != null).ToList());
					}
					else
					{
						builder.WithResult(new AnyCombinedResultDeterminer(
							new AgentDeadDeterminer(nikare),
							new AgentDeadDeterminer(kenut)
						));
						builder.WithPhases(new PhaseSplitter(
							new StartTrigger(new PhaseDefinition("Nikare's platform", nikare)),
							new BuffAddTrigger(nikare, SkillIds.Determined,
								new PhaseDefinition("Kenut's platform", kenut)),
							new BuffAddTrigger(kenut, SkillIds.Determined,
								new PhaseDefinition("Split phase", nikare, kenut)))
						);
					}

					if (nikare != null)
					{
						builder.WithModes(new AgentHealthModeDeterminer(nikare, 19_000_000));
					}

					return builder.Build();
				}
				case Encounter.Qadim:
				{
					var hydra = GetTargetBySpeciesId(agents, SpeciesIds.AncientInvokedHydra);
					var destroyer = GetTargetBySpeciesId(agents, SpeciesIds.ApocalypseBringer);
					var matriarch = GetTargetBySpeciesId(agents, SpeciesIds.WyvernMatriarch);
					var patriarch = GetTargetBySpeciesId(agents, SpeciesIds.WyvernPatriarch);

					return GetDefaultBuilder(encounter, mainTarget)
						.WithPhases(new PhaseSplitter(
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
								new PhaseDefinition("Qadim 33-0%", mainTarget))))
						.WithResult(new AgentCombatExitDeterminer(mainTarget))
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 21_000_000))
						.Build();
				}
				// Raids - Wing 7
				case Encounter.Adina:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 24_000_000))
						.Build();
				}
				case Encounter.Sabir:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 32_000_000))
						.Build();
				}
				case Encounter.QadimThePeerless:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 50_000_000))
						.Build();
				}
				// Challenge Mode fractals
				case Encounter.MAMA:
				case Encounter.SiaxTheCorrupted:
				case Encounter.EnsolyssOfTheEndlessTorment:
				case Encounter.Skorvald:
				case Encounter.Artsariiv:
				case Encounter.Arkk:
				{
					// TODO: Artsariiv and Arkk currently rely on the killing blow event, which is error-prone
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new ConstantModeDeterminer(EncounterMode.Challenge))
						.Build();
				}
				// TODO: Check if these are all the possible kitty golems
				case Encounter.StandardKittyGolem:
				case Encounter.MediumKittyGolem:
				case Encounter.LargeKittyGolem:
				case Encounter.MassiveKittyGolem:
				{
					// TODO: Improve the success detection if possible
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new TransformResultDeterminer(
							new AgentKillingBlowDeterminer(mainTarget),
							result => result == EncounterResult.Failure ? EncounterResult.Unknown : result)
						).Build();
				}
				case Encounter.Freezie:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AgentBuffGainedDeterminer(mainTarget, SkillIds.Determined))
						.Build();
				}
				default:
					return GetDefaultBuilder(encounter, mainTarget, mergeMainTarget: false).Build();
			}
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
					case SpeciesIds.MushroomKing:
						return Encounter.Escort;
					case SpeciesIds.KeepConstruct:
						return Encounter.KeepConstruct;
					case SpeciesIds.HauntingStatue:
						return Encounter.TwistedCastle;
					case SpeciesIds.Xera:
					case SpeciesIds.XeraSecondPhase:
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
					case SpeciesIds.BrokenKing:
						return Encounter.BrokenKing;
					case SpeciesIds.EaterOfSouls:
						return Encounter.EaterOfSouls;
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
					case SpeciesIds.WhisperOfJormag:
						return Encounter.WhisperOfJormag;
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

		private static EncounterIdentifierBuilder GetDefaultBuilder(Encounter encounter, Agent mainTarget, bool mergeMainTarget = true)
		{
			var builder = new EncounterIdentifierBuilder(
				encounter,
				new List<Agent> {mainTarget},
				new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", mainTarget))),
				new AgentKilledDeterminer(mainTarget),
				new ConstantModeDeterminer(EncounterMode.Normal)
			);
			if (mergeMainTarget && mainTarget is NPC npc)
			{
				// Gadgets do not have to be merged as they never go out of reporting range.
				builder.AddPostProcessingStep(new MergeSingletonNPC(npc.SpeciesId));
			}

			return builder;
		}

		private static EncounterIdentifierBuilder GetDefaultBuilder(Encounter encounter, IEnumerable<Agent> mainTargets)
		{
			var targets = mainTargets.ToArray();
			IResultDeterminer result;
			if (targets.Length > 0)
			{
				result = new AllCombinedResultDeterminer(targets
					.Select<Agent, IResultDeterminer>(target => new AgentKilledDeterminer(target))
					.ToArray()
				);
			}
			else
			{
				result = new ConstantResultDeterminer(EncounterResult.Unknown);
			}

			return new EncounterIdentifierBuilder(
				encounter,
				targets.ToList(),
				new PhaseSplitter(new StartTrigger(new PhaseDefinition("Default phase", targets))),
				result,
				new ConstantModeDeterminer(EncounterMode.Normal)
			);
		}

		// TODO: Remove
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