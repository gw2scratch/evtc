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
	public class EncounterDataProvider : IEncounterDataProvider
	{
		public IEncounterData GetEncounterData(Encounter encounter, Agent mainTarget, IReadOnlyList<Agent> agents, int? gameBuild, LogType logType)
		{
			switch (logType)
			{
				case LogType.PvE:
					return GetPvEEncounterData(encounter, mainTarget, agents, gameBuild);
				case LogType.WorldVersusWorld:
					return GetWvWEncounterData(agents);
				case LogType.Map:
					return GetMapEncounterData();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		private IEncounterData GetMapEncounterData()
		{
			var builder = new EncounterDataBuilder(
				Encounter.Map,
				new List<Agent>(),
				new ConstantResultDeterminer(EncounterResult.Unknown),
				new ConstantModeDeterminer(EncounterMode.Unknown),
				new ConstantHealthDeterminer(null)
			);

			return builder.Build();
		}

		private IEncounterData GetWvWEncounterData(IReadOnlyList<Agent> agents)
		{
			return new WorldVersusWorldEncounterData(
				agents.OfType<Player>().Where(x => x.Subgroup == -1)
			);
		}

		private IEncounterData GetPvEEncounterData(Encounter encounter, Agent mainTarget, IReadOnlyList<Agent> agents, int? gameBuild)
		{
			switch (encounter)
			{
				// Raids - Wing 1
				case Encounter.ValeGuardian:
					return GetDefaultBuilder(encounter, mainTarget).Build();
				case Encounter.SpiritRace:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new RewardDeterminer(404))
						.Build();
				}
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

					var targets = new Agent[] { berg, zane, narella }.Where(x => x != null).ToArray();

					return GetDefaultBuilder(encounter, targets)
						.WithResult(new ConditionalResultDeterminer(
							(berg != null && zane != null && narella != null && prisoner != null, new AllCombinedResultDeterminer(
								new AgentKilledDeterminer(berg), // Berg has to die
								new AgentKilledDeterminer(zane), // So does Zane
								new AgentAliveDeterminer(prisoner), // The prisoner in the cage must survive
								new AgentKilledDeterminer(narella) // And finally, Narella has to perish as well
							)),
							(true, new ConstantResultDeterminer(EncounterResult.Unknown))
						)).Build();
				}
				case Encounter.Matthias:
					return GetDefaultBuilder(encounter, mainTarget).Build();
				// Raids - Wing 3
				case Encounter.Escort:
				{
					var mcleod = GetTargetBySpeciesId(agents, SpeciesIds.McLeod);
					return GetDefaultBuilder(encounter, mcleod).Build();
				}
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
					var targets = new List<Agent> { mainTarget, secondPhaseXera }.Where(x => x != null).ToList();

					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new ConditionalResultDeterminer(
							(secondPhaseXera == null, new ConstantResultDeterminer(EncounterResult.Failure)),
							// Second phase Xera may infrequently appear drop out of combat for a moment at the start of the phase
							// before entering combat again. By enforcing a minimum time since her spawn, we can fairly safely
							// ensure that this will be ignored. It is also very unlikely the boss would be defeated in such a short time,
							// barring extreme exploits of broken game skills.
							// Even such exploits from the past would have trouble meeting this time requirement (Shadow Flare, Renegade Invoke Torment).
							(true, new AgentCombatExitDeterminer(secondPhaseXera) { MinTimeSinceSpawn = 10000 })
						))
						.WithHealth(new ConditionalHealthDeterminer(
							(secondPhaseXera == null, new MaxMinHealthDeterminer()),
							(true, new SequentialHealthDeterminer(mainTarget, secondPhaseXera))
						))
						.WithTargets(targets)
						.Build();
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
					// This release reworked rewards, currently a reward event is not present if an encounter was
					// finished a second time within a week. Before that, we can safely just check for the
					// presence of such a reward.

					// We cannot use the targetable detection method before they were introduced, and there was
					// a long period of time when logs did not contain the main gadget so we need to rely on this.
					bool canUseReward = gameBuild != null && gameBuild < GameBuilds.AhdashimRelease;


					// Deimos, the NPC, is replaced with a gadget for the last 10% of the fight.
					// There may sometimes be other gadgets with the same id. They do not, however,
					// have an attack target. They also have lower maximum health values.
					Gadget mainGadget = agents.OfType<Gadget>()
						.FirstOrDefault(x => x.VolatileId == GadgetIds.DeimosLastPhase && x.AttackTargets.Count == 1);
					Gadget prisoner = agents.OfType<Gadget>()
						.FirstOrDefault(x => x.VolatileId == GadgetIds.ShackledPrisoner);

					AttackTarget attackTarget = mainGadget?.AttackTargets?.SingleOrDefault();
					bool canUseTargets = mainGadget != null && attackTarget != null && prisoner != null;

					var targets = new List<Agent> { mainTarget, mainGadget }.Where(x => x != null).ToList();

					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 42_000_000))
						.WithTargets(targets)
						.WithResult(new ConditionalResultDeterminer(
							(canUseReward, new RewardDeterminer(525)),
							(canUseTargets, new AllCombinedResultDeterminer(
								new TargetableDeterminer(attackTarget, true, false),
								// If the log continues recording for longer than usual, it will record the attack target going untargetable.
								// However, at the same time it will also record the Shackled Prisoner having their health reset.
								// This health reset cannot happen in a successful log as there is no Shackled Prisoner at that point.
								new TransformResultDeterminer(
									new AgentHealthResetDeterminer(prisoner),
									result => result == EncounterResult.Success ? EncounterResult.Failure : EncounterResult.Success
								)
							)),
							(true, new ConstantResultDeterminer(EncounterResult.Unknown))
						))
						.WithHealth(new ConditionalHealthDeterminer(
							// The health of the Deimos gadget on the upper platform is set to 10% only after
							// the NPC used in the first part of the fight reaches 10% of its health.

							// If there has already been an attempt in this instance before, the gadget
							// retains its health from the previous attempt until the last phase is reached again.
							(canUseTargets, new SequentialHealthDeterminer(mainTarget, mainGadget)),
							(true, new MaxMinHealthDeterminer())
						)).Build();
				}
				// Raids - Wing 5
				case Encounter.SoullessHorror:
				{
					// It is fairly common for a failure to record long enough to include the respawned boss, especially
					// if the fight resets with some players still alive (a common bug after someone leaves an instance mid-fight).
					return GetDefaultBuilder(encounter, mainTarget, mergeMainTarget: false)
						.WithResult(new AgentBuffGainedDeterminer(mainTarget, SkillIds.SoullessHorrorDetermined))
						// Necrosis is applied faster in Challenge Mode. It is first removed and then reapplied
						// so we check the remaining time of the removed buff.
						.WithModes(new RemovedBuffStackRemainingTimeModeDeterminer(SkillIds.Necrosis,
							EncounterMode.Challenge, 23000, EncounterMode.Normal, 18000))
						.Build();
				}
				case Encounter.RiverOfSouls:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						// At the end of the event, 8 of the rifts become untargetable
						.WithResult(new GroupedEventDeterminer<TargetableChangeEvent>(
							e => e.IsTargetable == false, 8, 1000, null, null, 3000))
						.Build();
				}
				case Encounter.Eyes:
				{
					var fate = GetTargetBySpeciesId(agents, SpeciesIds.EyeOfFate);
					var judgment = GetTargetBySpeciesId(agents, SpeciesIds.EyeOfJudgment);

					var targets = new Agent[] { fate, judgment }.Where(x => x != null).ToList();

					return GetDefaultBuilder(encounter, new[] { fate, judgment })
						.WithResult(new ConditionalResultDeterminer(
							(fate == null || judgment == null, new ConstantResultDeterminer(EncounterResult.Unknown)),
							(true, new AnyCombinedResultDeterminer(
								new AgentDeadDeterminer(judgment),
								new AgentDeadDeterminer(fate)
							))
						))
						.WithTargets(targets)
						.Build();
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

					var targets = new Agent[] { nikare, kenut }.Where(x => x != null).ToList();

					return GetDefaultBuilder(encounter, targets)
						.WithResult(new ConditionalResultDeterminer(
							// If the fight does not progress far enough, Kenut might not be present in the log.
							(kenut == null, new ConstantResultDeterminer(EncounterResult.Failure)),
							(nikare == null, new ConstantResultDeterminer(EncounterResult.Unknown)),
							(nikare != null && kenut != null, new AllCombinedResultDeterminer(
								new AgentDeadDeterminer(nikare),
								new AgentDeadDeterminer(kenut)
							))
						))
						.WithModes(new ConditionalModeDeterminer(
							(nikare != null, new AgentHealthModeDeterminer(nikare, 19_000_000))
						))
						.WithTargets(targets)
						.Build();
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
					// We need to explicitly find Adina as this may be a Sabir main target log.
					var adina = GetTargetBySpeciesId(agents, SpeciesIds.CardinalAdina);
					return GetDefaultBuilder(encounter, adina ?? mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 24_000_000))
						.Build();
				}
				case Encounter.Sabir:
				{
					// We need to explicitly find Sabir this may be an Adina main target log.
					var sabir = GetTargetBySpeciesId(agents, SpeciesIds.CardinalSabir);
					return GetDefaultBuilder(encounter, sabir ?? mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 32_000_000))
						.Build();
				}
				case Encounter.QadimThePeerless:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new AgentHealthModeDeterminer(mainTarget, 50_000_000))
						.Build();
				}
				// Raids - Wing 8
				case Encounter.Greer:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new NPCPresentModeDeterminer(SpeciesIds.Ereg))
						.Build();
				}
				case Encounter.Decima:
				{
					// Decima when it first released had 83,288,232 HP and got nerfed without patch to 70,795,000.
					// Some logs with the original HP exist.
					
					var mode = mainTarget switch
					{
						NPC { SpeciesId: SpeciesIds.Decima } => EncounterMode.Normal,
						NPC { SpeciesId: SpeciesIds.DecimaChallengeMode } => EncounterMode.Challenge,
						_ => EncounterMode.Unknown,
					};
					
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new ConstantModeDeterminer(mode))
						.Build();
				}
				case Encounter.Ura:
				{
					// Normal mode 61,345,440
					// Challenge mode 79,749,072
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new FallbackModeDeterminer(
							// A preliminary guess
							new AgentHealthModeDeterminer(mainTarget, 80_000_000, EncounterMode.LegendaryChallenge),
							new AgentHealthModeDeterminer(mainTarget, 70_000_000, EncounterMode.Challenge),
							finalFallbackMode: null
						))
						.Build();
				}
				// Challenge Mode fractals
				case Encounter.Skorvald:
				{
					// Skorvald the Shattered is the same species in Challenge and Normal mode,
					// unlike most other fractal CM encounters
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AnyCombinedResultDeterminer(
								new AgentKillingBlowDeterminer(mainTarget),
								new AgentBuffGainedDeterminer(mainTarget, SkillIds.Determined895)))
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
					
					// As of build 151966, there is a 2 second Determined applied at the start of each attempt.
					// We just ignore it based on health.

					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new BuffAppliedBelowHealthThresholdDeterminer(mainTarget, 0.9f, SkillIds.Determined895))
						.WithModes(new ConstantModeDeterminer(EncounterMode.Challenge))
						.Build();
				}
				case Encounter.AiKeeperOfThePeakDayAndNight:
				{
					// See the Encounter.AiKeeperOfThePeak case above for a more detailed description
					// of the encounter flow.

					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new BuffAppliedAfterSkillCastDeterminer(mainTarget, SkillIds.AiDarkEarlySkill, SkillIds.Determined895))
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

					var targets = new Agent[] { voice, claw }.Where(x => x != null).ToList();

					return GetDefaultBuilder(encounter, targets)
						.WithResult(new ConditionalResultDeterminer(
							(voice != null && claw != null, new AllCombinedResultDeterminer(
								new AgentDeadDeterminer(voice),
								new AgentDeadDeterminer(claw)
							)),
							(true, new ConstantResultDeterminer(EncounterResult.Unknown))
						))
						.WithTargets(targets)
						.Build();
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
					// This is the gadget that represents the first 5 dragons.
					Gadget firstGadget = agents.OfType<Gadget>().FirstOrDefault(x =>
						x.VolatileId == GadgetIds.TheDragonvoid && x.AttackTargets.Count == 3);
					
					// This is the gadget that represents Soo-Won. The previous phases share the same
					// gadget (GadgetIds.TheDragonvoid), but this one has a unique one with different max health.
					Gadget finalGadget = agents.OfType<Gadget>().FirstOrDefault(x =>
						x.VolatileId == GadgetIds.TheDragonvoidFinal && x.AttackTargets.Count == 3);

					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new ConditionalModeDeterminer(
							(firstGadget != null, new GroupedSpawnModeDeterminer(agent => agent is NPC { SpeciesId: SpeciesIds.VoidMelter }, 6, 200))
						))
						.WithResult(new ConditionalResultDeterminer(
							// We use a health threshold instead of checking for the gadget going
							// through enable->disable->enable->disable to make it possible to correctly detect success
							// if the PoV player joins the instance late. This should work unless they join after
							// the last health update.
							
							// Note that the gadget keeps its initial health from the previous attempt if the fight
							// is reset within the same instance.
							
							(finalGadget != null, new AllCombinedResultDeterminer(
								timeSelection: AllCombinedResultDeterminer.TimeSelection.Min,
								new AnyCombinedResultDeterminer(
									// The final gadget is likely to be null in case parsing and processing is merged and encounter data is evaluated
									// for the first time as combat items were not parsed yet to find attack target assignments.
									// For this reason, we need to create a fake targetable determiner that will never be satisfied when final gadget is null.
									// We need to provide the same type of determiner as if final gadget was present for event pruning to work correctly.
									finalGadget?.AttackTargets.Select(x => (IResultDeterminer) new TargetableDeterminer(x, true, false, true, false)).ToArray()
										?? new IResultDeterminer[] { new TargetableDeterminer(null, true, false, true, false) }
								),
								new SkillCastAroundBuffApplyDeterminer(x => x is Player, SkillIds.HarvestTempleLiftOff, SkillIds.Determined895, 1000)
							))
						))
						.WithHealth(new AgentHealthDeterminer(null).RequiredEventTypes, new List<uint>(), new List<PhysicalDamageEvent.Result>(),
							log =>
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
							})
						.Build();
				}
				case Encounter.OldLionsCourt:
				{
					var targetOptions = new (int Normal, int Challenge)[]
					{
						(SpeciesIds.PrototypeArsenite, SpeciesIds.PrototypeArseniteChallengeMode),
						(SpeciesIds.PrototypeVermillion, SpeciesIds.PrototypeVermillionChallengeMode),
						(SpeciesIds.PrototypeIndigo, SpeciesIds.PrototypeIndigoChallengeMode)
					};

					if (mainTarget is not NPC npc)
					{
						throw new InvalidOperationException("Old Lion's Court should have NPC as the target.");
					}

					bool isChallenge = (targetOptions.Select(target => target.Challenge).Contains(npc.SpeciesId));
					bool isNormal = (targetOptions.Select(target => target.Normal).Contains(npc.SpeciesId));
					EncounterMode mode = (isNormal, isChallenge) switch
					{
						(false, false) => EncounterMode.Unknown,
						(false, true) => EncounterMode.Challenge,
						(true, false) => EncounterMode.Normal,
						(true, true) => EncounterMode.Unknown,
					};

					Agent[] targets;
					if (mode == EncounterMode.Normal)
					{
						var vermillion = GetTargetBySpeciesId(agents, SpeciesIds.PrototypeVermillion);
						var arsenite = GetTargetBySpeciesId(agents, SpeciesIds.PrototypeArsenite);
						var indigo = GetTargetBySpeciesId(agents, SpeciesIds.PrototypeIndigo);
						targets = new Agent[] { vermillion, arsenite, indigo }.Where(x => x != null).ToArray();
					}
					else if (mode == EncounterMode.Challenge)
					{
						var vermillion = GetTargetBySpeciesId(agents, SpeciesIds.PrototypeVermillionChallengeMode);
						var arsenite = GetTargetBySpeciesId(agents, SpeciesIds.PrototypeArseniteChallengeMode);
						var indigo = GetTargetBySpeciesId(agents, SpeciesIds.PrototypeIndigoChallengeMode);
						targets = new Agent[] { vermillion, arsenite, indigo }.Where(x => x != null).ToArray();
					}
					else
					{
						targets = Array.Empty<Agent>();
					}

					return GetDefaultBuilder(encounter, targets)
						.WithModes(new ConstantModeDeterminer(mode))
						.WithResult(new ConditionalResultDeterminer(
							(targets.Length == 3 && mode != EncounterMode.Unknown, new AllCombinedResultDeterminer(
								targets.Select(x => new AgentKilledDeterminer(x)).ToArray<IResultDeterminer>()
							))
						))
						.Build();
				}
				case Encounter.Kanaxai:
				{
					var mode = mainTarget switch
					{
						NPC { SpeciesId: SpeciesIds.KanaxaiNM } => EncounterMode.Normal,
						NPC { SpeciesId: SpeciesIds.KanaxaiCM } => EncounterMode.Challenge,
						_ => EncounterMode.Unknown,
					};
					
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AgentBuffGainedDeterminer(mainTarget, SkillIds.Determined))
						.WithModes(new ConstantModeDeterminer(mode))
						.Build();
				}
				case Encounter.CosmicObservatory:
				{
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(
							new AnyCombinedResultDeterminer(
								new AgentKilledDeterminer(mainTarget),
								// The boss can stay alive for a long time, but it will be at zero health.
								// This makes this method reliable enough:
								new AgentBelowHealthThresholdDeterminer(mainTarget, 1e-6f)
							)
						).WithModes(new ConditionalModeDeterminer(
							(gameBuild != null && gameBuild < GameBuilds.CosmicObservatoryCMRelease, new ConstantModeDeterminer(EncounterMode.Normal)),
							(true, new AgentHealthModeDeterminer(mainTarget, 56_600_000))))
                        .Build();
				}
				case Encounter.TempleOfFebe:
				{
					// NM: before CM release 47,188,800, after CM release: 53,087,400
					// CM: on release 106,174,800 first phase, second phase 159M
					// CM: after first fix 130,064,136 in all phases
					// CM: made easier at 106,174,800 health (2024-03-19)
					// Legendary Challenge: keeps 130,064,136 (introduced in 2024-03-19)
					
					// Achievements were given retroactively, so we count everything above 106,174,800
					// as legendary challenge even before its introduction
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new ConditionalModeDeterminer(
							// The first version of CM with varying health is also recognized as legendary challenge.
							// There were no publicly known kills of this fight, but it makes sense to recognize
							// the attempts as the legendary version.
							(gameBuild != null && gameBuild < GameBuilds.TempleOfFebeHealthFix,
								new AgentHealthModeDeterminer(mainTarget, 60_000_000, EncounterMode.LegendaryChallenge)),
							(true, new FallbackModeDeterminer(
								new AgentHealthModeDeterminer(mainTarget, 107_000_000, EncounterMode.LegendaryChallenge),
								new AgentHealthModeDeterminer(mainTarget, 60_000_000, EncounterMode.Challenge),
								finalFallbackMode: null))))
						.Build();
				}
				case Encounter.Eparch:
				{
					// Normal Mode Release: 31.771.528
					// Challenge Mode Release: 32.618.906
					// Challenge Mode Release (Normal Mode T4): 19.857.206
					// HP Nerfs Patch (Challenge Mode): 22.833.236 
					// HP Nerfs Patch (Normal Mode T4): 13.900.044
					return GetDefaultBuilder(encounter, mainTarget)
						.WithResult(new AnyCombinedResultDeterminer(
								new AgentKillingBlowDeterminer(mainTarget),
								new BuffAppliedBelowHealthThresholdDeterminer(mainTarget, 0.2f, SkillIds.Determined)))
						.WithModes(new ConditionalModeDeterminer(
							(gameBuild != null && gameBuild < GameBuilds.LonelyTowerCMRelease,
								new ConstantModeDeterminer(EncounterMode.Normal)),
							(gameBuild != null && gameBuild >= GameBuilds.LonelyTowerCMRelease && gameBuild < GameBuilds.LonelyTowerHPNerf2,
								new AgentHealthModeDeterminer(mainTarget, 31_000_000)),
							(gameBuild != null && gameBuild >= GameBuilds.LonelyTowerHPNerf2,
								new AgentHealthModeDeterminer(mainTarget, 21_000_000))
							))
						.Build();
				}
				case Encounter.WhisperingShadow:
				{
					// Tier 1 12.404.224 HP
					// Tier 2 14.202.094 HP
					// Tier 3 16.941.704 HP
					// Tier 4 & CM 19.082.024 HP
					return GetDefaultBuilder(encounter, mainTarget)
						.WithModes(new ConditionalModeDeterminer(
							(gameBuild != null && gameBuild >= GameBuilds.KinfallCMRelease, new SkillPresentModeDeterminer(SkillIds.LifeFireCircleCM, EncounterMode.Challenge))
							))
						.Build();
				}
				default:
					return GetDefaultBuilder(encounter, mainTarget, mergeMainTarget: false).Build();
			}
		}


		private static EncounterDataBuilder GetDefaultBuilder(Encounter encounter, Agent mainTarget, bool mergeMainTarget = true)
		{
			var builder = new EncounterDataBuilder(
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

		private static EncounterDataBuilder GetDefaultBuilder(Encounter encounter, IEnumerable<Agent> mainTargets, bool mergeMainTargets = true)
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
			var builder = new EncounterDataBuilder(
				encounter,
				targets.ToList(),
				result,
				new EmboldenedDetectingModeDeterminer(),
				new MaxMinHealthDeterminer()
			);

			if (mergeMainTargets)
			{
				foreach (var mainTarget in targets)
				{
					if (mainTarget is NPC npc)
					{
						// Gadgets do not have to be merged as they never go out of reporting range.
						builder.AddPostProcessingStep(new MergeSingletonNPC(npc.SpeciesId));
					}
				}
			}

			return builder;
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