using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Transformers;
using GW2Scratch.EVTCAnalytics.Processing.Steps;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	/// <summary>
	/// The encounter identifier used by default. Aims to support encounters logged by default and a few common extras.
	/// </summary>
	public class DefaultEncounterIdentifier : IEncounterIdentifier
	{
		public IEncounterData GetEncounterData(Agent mainTarget, IReadOnlyList<Event> events,
			IReadOnlyList<Agent> agents, IReadOnlyList<Skill> skills, int? gameBuild, LogType logType)
		{
			switch (logType)
			{
				case LogType.PvE:
					return GetPvEEncounterData(mainTarget, events, agents, skills, gameBuild);
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

		private IEncounterData GetPvEEncounterData(Agent mainTarget, IReadOnlyList<Event> events,
			IReadOnlyList<Agent> agents, IReadOnlyList<Skill> skills, int? gameBuild)
		{
			var encounter = IdentifyEncounter(mainTarget, agents, events, skills);

			switch (encounter)
			{
				// Raids - Wing 1
				case Encounter.ValeGuardian:
					return GetDefaultBuilder(encounter, mainTarget).Build();
				case Encounter.Gorseval:
					return GetDefaultBuilder(encounter, mainTarget).Build();
				case Encounter.Sabetha:
					return GetDefaultBuilder(encounter, mainTarget).Build();
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
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new SkillPresentModeDeterminer(34958))
						.Build();
				case Encounter.TwistedCastle:
				{
					return GetDefaultBuilder(encounter, new Agent[0])
						.WithResult(new RewardDeterminer(496))
						.Build();
				}
				case Encounter.Xera:
				{
					// On Xera, there is a gliding phase once you reach 50% of her health. Afterwards, the original Xera NPC
					// gets replaced with a different NPC (with higher maximum health, even) that is set to 50% of its health.
					var secondPhaseXera = GetTargetBySpeciesId(agents, SpeciesIds.XeraSecondPhase);

					var builder = GetDefaultBuilder(encounter, mainTarget);
					if (secondPhaseXera == null)
					{
						builder.WithResult(new ConstantResultDeterminer(EncounterResult.Failure));
					}
					else
					{
						builder.WithHealth(new SequentialHealthDeterminer(mainTarget, secondPhaseXera));
						// Second phase Xera may infrequently appear drop out of combat for a moment at the start of the phase
						// before entering combat again. By enforcing a minimum time since her spawn, we can fairly safely
						// ensure that this will be ignored. It is also very unlikely the boss would be defeated in such a short time,
						// barring extreme exploits of broken game skills.
						// Even such exploits from the past would have trouble meeting this time requirement (Shadow Flare, Renegade Invoke Torment).
						builder.WithResult(new AgentCombatExitDeterminer(secondPhaseXera) {MinTimeSinceSpawn = 10000})
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
						// Deimos, the NPC, is replaced with a gadget for the last 10% of the fight.
						// There may sometimes be other gadgets with the same id. They do not, however,
						// have an attack target. They also have lower maximum health values.
						Gadget mainGadget = agents.OfType<Gadget>()
							.FirstOrDefault(x => x.VolatileId == GadgetIds.DeimosLastPhase && x.AttackTargets.Count == 1);
						Gadget prisoner = agents.OfType<Gadget>()
							.FirstOrDefault(x => x.VolatileId == GadgetIds.ShackledPrisoner);

						if (mainGadget != null)
						{
							var attackTarget = mainGadget.AttackTargets.SingleOrDefault();
							if (attackTarget != null && prisoner != null)
							{
								builder.WithResult(new AllCombinedResultDeterminer(
									new TargetableDeterminer(attackTarget, true, false),
									// If the log continues recording for longer than usual, it will record the attack target going untargetable.
									// However, at the same time it will also record the Shackled Prisoner having their health reset.
									// This health reset cannot happen in a successful log as there is no Shackled Prisoner at that point.
									new TransformResultDeterminer(
										new AgentHealthResetDeterminer(prisoner),
										result => result == EncounterResult.Success ? EncounterResult.Failure : EncounterResult.Success
									)
								));
								// The health of the Deimos gadget on the upper platform is set to 10% only after
								// the NPC used in the first part of the fight reaches 10% of its health.

								// If there has already been an attempt in this instance before, the gadget
								// retains its health from the previous attempt until the last phase is reached again.
								builder.WithHealth(new SequentialHealthDeterminer(mainTarget, mainGadget));
							}
							else
							{
								builder.WithResult(new ConstantResultDeterminer(EncounterResult.Unknown));
							}

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
							EncounterMode.Challenge, 23000, EncounterMode.Normal, 18000))
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
						if (kenut == null)
						{
							// If the fight does not progress far enough, Kenut might not be present in the log.
							builder.WithResult(new ConstantResultDeterminer(EncounterResult.Failure));
						}
						else
						{
							builder.WithResult(new ConstantResultDeterminer(EncounterResult.Unknown));
						}

						builder.WithTargets(new Agent[] {nikare, kenut}.Where(x => x != null).ToList());
					}
					else
					{
						builder.WithResult(new AllCombinedResultDeterminer(
							new AgentDeadDeterminer(nikare),
							new AgentDeadDeterminer(kenut)
						));
					}

					if (nikare != null)
					{
						builder.WithModes(new AgentHealthModeDeterminer(nikare, 19_000_000));
					}

					return builder.Build();
				}
				case Encounter.Qadim:
				{
					return GetDefaultBuilder(encounter, mainTarget)
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
				case Encounter.Skorvald:
				{
					// Skorvald the Shattered is the same species in Challenge and Normal mode,
					// unlike most other fractal CM encounters
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 5_550_000))
						.Build();
				}
				case Encounter.MAMA:
				case Encounter.SiaxTheCorrupted:
				case Encounter.EnsolyssOfTheEndlessTorment:
				case Encounter.Artsariiv:
				case Encounter.Arkk:
				{
					// TODO: Artsariiv and Arkk currently rely on the killing blow event, which is error-prone
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new ConstantModeDeterminer(EncounterMode.Challenge))
						.Build();
				}
#pragma warning disable 618
				case Encounter.AiKeeperOfThePeak:
#pragma warning restore 618
				{
					// This case remains for compatibility reasons, but shouldn't be used after obsoleting
					// the encounter. The versions with phases are the expected code path these days.

					// This encounter has two phases with the same enemy. The enemy gains short invulnerability
					// and regains full health between these two phases.
					// However, if the fight has been progressed into the second (dark) phase and failed, the next attempt
					// starts at the second phase, so the first phase might not be in the log.

					// 895 - Determined, applied at end of first phase along with 762 Determined and a short Daze
					// 53569 - nameless skill used when transitioning between phases, only in the log if both phases are present
					// 61356 - nameless skill cast early in phase 2
					// 895 - Determined, applied at end of second phase along with 762 Determined and a short Daze

					// No 61356 - always a failure, did not reach dark phase, health +100%
					// 61356 && no determined afterwards -> failure in the second (dark) phase
					// 61356 && Determined afterwards -> success
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new BuffAppliedAfterSkillCastDeterminer(mainTarget, SkillIds.AiDarkEarlySkill, SkillIds.Determined895))
						.WithHealth(new ExtraHealthIfSkillPresentHealthDeterminer(SkillIds.AiDarkEarlySkill, 1))
						.WithModes(new ConstantModeDeterminer(EncounterMode.Challenge))
						.Build();
				}
				case Encounter.AiKeeperOfThePeakNightOnly:
				{
					// See the Encounter.AiKeeperOfThePeak case above for a more detailed description
					// of the encounter flow.

					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AgentBuffGainedDeterminer(mainTarget, SkillIds.Determined895))
						.WithModes(new ConstantModeDeterminer(EncounterMode.Challenge))
						.Build();
				}
				case Encounter.AiKeeperOfThePeakDayOnly:
				{
					// See the Encounter.AiKeeperOfThePeak case above for a more detailed description
					// of the encounter flow.

					// Players commonly reset the fight between "day" and "night" phases.
					// As such, reaching the invulnerability is considered a success because the fight progresses
					// and the players only have to do the "night" phase afterwards.

					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AgentBuffGainedDeterminer(mainTarget, SkillIds.Determined895))
						.WithModes(new ConstantModeDeterminer(EncounterMode.Challenge))
						.Build();
				}
				case Encounter.AiKeeperOfThePeakDayAndNight:
				{
					// See the Encounter.AiKeeperOfThePeak case above for a more detailed description
					// of the encounter flow.

					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new BuffAppliedAfterSkillCastDeterminer(mainTarget, SkillIds.AiDarkEarlySkill,
							SkillIds.Determined895))
						.WithModes(new ConstantModeDeterminer(EncounterMode.Challenge))
						.Build();
				}
				// TODO: Check if these are all the possible kitty golems
				case Encounter.StandardKittyGolem:
				case Encounter.MediumKittyGolem:
				case Encounter.LargeKittyGolem:
				case Encounter.MassiveKittyGolem:
				{
					// The kitty golems encounters do not seem to have any way to reliably detect their outcome.
					// They do not die, and if the console is used to remove the golem,
					// the resulting log removes the golem in the same way as if the golem was killed.

					// For this reason we adopt the same error-prone approach as Elite Insights,
					// but we also recognize if a killing blow was dealt against the golem.
					// Killing blows do not appear for some condition-based damage sources,
					// but the 1 million health golem is the only one where 2% damage of its
					// health is likely to be dealt in one hit, and is very likely to result
					// in a killing blow.

					// We do not want to use position data to detect players approaching the console,
					// as they are not sampled often enough to detect teleports, and it is possible
					// that a player may decide to kill the golem from the console at range.

					// A better detection method would be very much appreciated here.
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AnyCombinedResultDeterminer(
								new AgentKillingBlowDeterminer(mainTarget),
								new IgnoreTimeResultDeterminerWrapper(new AgentBelowHealthThresholdDeterminer(mainTarget, 0.02f))
							)
						).Build();
				}
				case Encounter.Freezie:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AgentBuffGainedDeterminer(mainTarget, SkillIds.Determined))
						.Build();
				}
				case Encounter.VoiceAndClawOfTheFallen:
				{
					var voice = GetTargetBySpeciesId(agents, SpeciesIds.VoiceOfTheFallen);
					var claw = GetTargetBySpeciesId(agents, SpeciesIds.ClawOfTheFallen);

					var bosses = new List<NPC>();
					if (voice != null) bosses.Add(voice);
					if (claw != null) bosses.Add(claw);

					var builder = GetDefaultBuilder(encounter, bosses);
					if (voice != null && claw != null)
					{
						builder.WithResult(new AllCombinedResultDeterminer(
							new AgentDeadDeterminer(voice),
							new AgentDeadDeterminer(claw)
						));
					}
					else
					{
						builder.WithResult(new ConstantResultDeterminer(EncounterResult.Unknown));
					}

					return builder.Build();
				}
				case Encounter.Mordremoth:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AgentBuffGainedDeterminer(mainTarget, SkillIds.Determined895))
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 9_569_560))
						.Build();
				}
				case Encounter.AetherbladeHideout:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AgentBuffGainedDeterminer(mainTarget, SkillIds.Determined895))
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 8_800_000))
						.Build();
				}
				case Encounter.XunlaiJadeJunkyard:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						// We cannot check for Determination as Ankka can get Determined any amount of times per phase.
						// We do not really want to track phases as that would likely cause issues if the PoV player
						// joins the instance late.
						// Ankka changes teams twice at the start and then once when she is beaten.
						// We just pick an arbitrary conservative threshold that her health needs to reach first: 50%.
						.WithResult(new TeamChangedBelowHealthThresholdDeterminer(mainTarget, 0.5f))
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 50_000_000))
						.Build();
				}
				case Encounter.KainengOverlook:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						// This has the same rationale behind it as the check for Xunlai Jade Junkyard.
						.WithResult(new TeamChangedBelowHealthThresholdDeterminer(mainTarget, 0.5f))
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 30_000_000))
						.Build();
				}
				case Encounter.HarvestTemple:
				{
					var builder = GetDefaultBuilder(encounter, mainTarget);
						
					// This is the gadget that represents the first 5 dragons.
					Gadget firstGadget = agents.OfType<Gadget>().FirstOrDefault(x =>
						x.VolatileId == GadgetIds.TheDragonvoid && x.AttackTargets.Count == 3);
					if (firstGadget != null)
					{
						builder.WithModes(new GroupedSpawnModeDeterminer(agent => agent is NPC { SpeciesId: SpeciesIds.VoidMelter }, 6, 200));
					}
					
					// This is the gadget that represents Soo-Won. The previous phases share the same
					// gadget (GadgetIds.TheDragonvoid), but this one has a unique one with different max health.
					Gadget finalGadget = agents.OfType<Gadget>().FirstOrDefault(x =>
						x.VolatileId == GadgetIds.TheDragonvoidFinal && x.AttackTargets.Count == 3);
					
					builder.WithHealth(log =>
					{
						const float healthPerPhase = 1.0f / 6.0f;
						
						//bool pastPre = log.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.VoidStormseer);
						bool pastJormag = log.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.VoidWarforged);
						bool pastPrimordus = log.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.VoidBrandbomber);
						bool pastKralkatorikk = log.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.VoidTimeCaster);
						bool pastMordemoth = log.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.VoidGiant);
						bool pastZhaitan = log.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.VoidSaltsprayDragon);
						//bool pastSooWonPhase1 = log.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.VoidObliterator);

						int completedPhases = 0;
						if (pastJormag) completedPhases += 1;
						if (pastPrimordus) completedPhases += 1;
						if (pastKralkatorikk) completedPhases += 1;
						if (pastMordemoth) completedPhases += 1;
						if (pastZhaitan) completedPhases += 1;

						// The first 5 phases use one gadget (firstGadget), the last phase uses a different gadget (finalGadget).
						// These gadgets have different health amounts, but for now we are pretending they are the same,
						// they get the same percentage share as other phases.
						float remainingHealth;
						if (pastZhaitan && finalGadget != null)
						{
							remainingHealth = new AgentHealthDeterminer(finalGadget).GetMainEnemyHealthFraction(log) ?? 1f;
						}
						else
						{
							remainingHealth = new AgentHealthDeterminer(firstGadget).GetMainEnemyHealthFraction(log) ?? 1f;
						}

						// Subtract one extra phase as we are re-adding the health of the current phase.
						float finishedHealth = (completedPhases + 1) * healthPerPhase;
						float remainingInPhase = remainingHealth * healthPerPhase;

						return 1 - finishedHealth + remainingInPhase;
					});
					
					if (finalGadget == null)
					{
						builder.WithResult(new ConstantResultDeterminer(EncounterResult.Unknown));
					}
					else
					{
						// We use a health threshold instead of checking for the gadget going
						// through enable->disable->enable->disable to make it possible to correctly detect success
						// if the PoV player joins the instance late. This should work unless they join after
						// the last health update.
						
						// Note that the gadget keeps its initial health from the previous attempt if the fight
						// is reset within the same instance.
						builder.WithResult(new TargetableChangedBelowHealthThresholdDeterminer(finalGadget, false, 0.3f));
					}

					return builder.Build();
				}
				case Encounter.OldLionsCourt:
				{
					var vermillion = GetTargetBySpeciesId(agents, SpeciesIds.PrototypeVermillion);
					var arsenite = GetTargetBySpeciesId(agents, SpeciesIds.PrototypeArsenite);
					var indigo = GetTargetBySpeciesId(agents, SpeciesIds.PrototypeIndigo);

					var targets = new Agent[] {vermillion, arsenite, indigo}.Where(x => x != null).ToArray();
					var builder = GetDefaultBuilder(encounter, targets);
					if (targets.Length == 3)
					{
						builder.WithResult(new AllCombinedResultDeterminer(
							new AgentKilledDeterminer(vermillion),
							new AgentKilledDeterminer(arsenite),
							new AgentKilledDeterminer(indigo)
						));
					}
					else
					{
						builder.WithResult(new ConstantResultDeterminer(EncounterResult.Unknown));
					}

					if (vermillion != null)
					{
						// We are hoping this works, this is written before CM is released.
						// As of release, health is 14,156,640 in normal mode.
						builder.WithModes(new AgentHealthModeDeterminer(vermillion, 14_200_000));
					}

					return builder.Build();
				}
				default:
					return GetDefaultBuilder(encounter, mainTarget, mergeMainTarget: false).Build();
			}
		}

		public Encounter IdentifyEncounter(Agent mainTarget, IReadOnlyList<Agent> agents, IReadOnlyList<Event> events, IReadOnlyList<Skill> skills)
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
						// Twisted Castle logs sometimes get Xera as the main target when the player is too close to her
						if (agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.HauntingStatue))
						{
							return Encounter.TwistedCastle;
						}

						return Encounter.Xera;
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
					case SpeciesIds.AiKeeperOfThePeak:
					{
						// This encounter has two phases with the same enemy. The enemy gains short invulnerability
						// and regains full health between these two phases.
						// However, if the fight has been progressed into the second (dark) phase and failed, the next attempt
						// starts at the second phase, so the first phase might not be in the log.

						// 895 - Determined, applied at end of first phase along with 762 Determined and a short Daze
						// 53569 - nameless skill used when transitioning between phases, only in the log if both phases are present
						// 61356 - nameless skill cast early in phase 2
						// 895 - Determined, applied at end of second phase along with 762 Determined and a short Daze

						// No 61356 - always a failure, did not reach dark phase, health +100%
						// 61356 && no determined afterwards -> failure in the second (dark) phase
						// 61356 && Determined afterwards -> success

						if (skills.All(x => x.Id != SkillIds.Determined895))
						{
							// This is a quick path that doesn't require enumerating through events. 

							// As there is no Determined, the log is a failure, and this cannot occur in a log
							// that has both phases (as the Determined buff is applied in between).

							bool hasDarkPhase = skills.Any(x => x.Id == SkillIds.AiDarkEarlySkill);
							return hasDarkPhase
								? Encounter.AiKeeperOfThePeakNightOnly
								: Encounter.AiKeeperOfThePeakDayOnly;
						}
						else
						{
							bool inDark = false;
							bool determinedPreDark = false;
							foreach (var ev in events)
							{
								if (ev is BuffApplyEvent {Buff.Id: SkillIds.Determined895 } and not InitialBuffEvent)
								{
									// This buff application is the transition between the two phases.
									// This works because we stop enumerating events once we reach the dark phase.
									determinedPreDark = true;
								}

								if (ev is SkillCastEvent {Skill.Id: SkillIds.AiDarkEarlySkill })
								{
									inDark = true;
									break;
								}
							}

							if (inDark)
							{
								return determinedPreDark
									? Encounter.AiKeeperOfThePeakDayAndNight
									: Encounter.AiKeeperOfThePeakNightOnly;
							}
							else
							{
								return Encounter.AiKeeperOfThePeakDayOnly;
							}
						}
					}
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
					case SpeciesIds.VariniaStormsounder:
						return Encounter.VariniaStormsounder;
					case SpeciesIds.HeartsAndMindsMordremoth:
						return Encounter.Mordremoth;
					case SpeciesIds.MaiTrin:
						return Encounter.AetherbladeHideout;
					case SpeciesIds.Ankka:
						return Encounter.XunlaiJadeJunkyard;
					case SpeciesIds.MinisterLi:
					case SpeciesIds.MinisterLiChallengeMode:
						return Encounter.KainengOverlook;
					case SpeciesIds.VoidAmalgamate:
						return Encounter.HarvestTemple;
					case SpeciesIds.PrototypeVermillion:
					case SpeciesIds.PrototypeArsenite:
					case SpeciesIds.PrototypeIndigo:
						return Encounter.OldLionsCourt;
				}
			}
			else if (mainTarget is Gadget gadgetBoss)
			{
				switch (gadgetBoss.VolatileId)
				{
					case GadgetIds.ConjuredAmalgamate:
						return Encounter.ConjuredAmalgamate;
					case GadgetIds.TheDragonvoidFinal:
						return Encounter.HarvestTemple;
					case GadgetIds.TheDragonvoid:
						return Encounter.HarvestTemple;
				}
			}

			return Encounter.Other;
		}

		private static EncounterIdentifierBuilder GetDefaultBuilder(Encounter encounter, Agent mainTarget, bool mergeMainTarget = true)
		{
			var builder = new EncounterIdentifierBuilder(
				encounter,
				new List<Agent> {mainTarget},
				new AgentKilledDeterminer(mainTarget),
				new EmboldenedDetectingModeDeterminer(),
				new MaxMinHealthDeterminer()
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
				result,
				new EmboldenedDetectingModeDeterminer(),
				new MaxMinHealthDeterminer()
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