using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Parsed;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Processing;

public class EncounterIdentifier : IEncounterIdentifier
{
	public Encounter IdentifyEncounter(ushort triggerId, LogProcessorState state)
	{
		// Important: when adding a new encounter, make sure you also add it to the IdentifyPotentialEncounters method.

		IEnumerable<Agent> mainTargets = state.Agents.Where(x =>
			x is NPC npc && npc.SpeciesId == triggerId ||
			x is Gadget gadget && gadget.VolatileId == triggerId);

		Encounter encounter = Encounter.Other;

		foreach (var mainTarget in mainTargets)
		{
			if (mainTarget is NPC boss)
			{
				switch (boss.SpeciesId)
				{
					case SpeciesIds.ValeGuardian:
						encounter = Encounter.ValeGuardian;
						break;
					case SpeciesIds.Gorseval:
						encounter = Encounter.Gorseval;
						break;
					case SpeciesIds.Sabetha:
						encounter = Encounter.Sabetha;
						break;
					case SpeciesIds.Slothasor:
						encounter = Encounter.Slothasor;
						break;
					case SpeciesIds.Berg:
					case SpeciesIds.Zane:
					case SpeciesIds.Narella:
						encounter = Encounter.BanditTrio;
						break;
					case SpeciesIds.MatthiasGabrel:
						encounter = Encounter.Matthias;
						break;
					case SpeciesIds.MushroomKing:
					case SpeciesIds.McLeod:
						encounter = Encounter.Escort;
						break;
					case SpeciesIds.KeepConstruct:
						encounter = Encounter.KeepConstruct;
						break;
					case SpeciesIds.HauntingStatue:
						encounter = Encounter.TwistedCastle;
						break;
					case SpeciesIds.Xera:
						// Twisted Castle logs sometimes get Xera as the main target when the player is too close to her
						if (state.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.HauntingStatue))
						{
							return Encounter.TwistedCastle;
						}
						encounter = Encounter.Xera;
						break;
					case SpeciesIds.XeraSecondPhase:
						encounter = Encounter.Xera;
						break;
					case SpeciesIds.CairnTheIndomitable:
						encounter = Encounter.Cairn;
						break;
					case SpeciesIds.MursaatOverseer:
						encounter = Encounter.MursaatOverseer;
						break;
					case SpeciesIds.Samarog:
						encounter = Encounter.Samarog;
						break;
					case SpeciesIds.Deimos:
						encounter = Encounter.Deimos;
						break;
					case SpeciesIds.SoullessHorror:
						encounter = Encounter.SoullessHorror;
						break;
					case SpeciesIds.Desmina:
						encounter = Encounter.RiverOfSouls;
						break;
					case SpeciesIds.BrokenKing:
						encounter = Encounter.BrokenKing;
						break;
					case SpeciesIds.EaterOfSouls:
						encounter = Encounter.EaterOfSouls;
						break;
					case SpeciesIds.EyeOfJudgment:
					case SpeciesIds.EyeOfFate:
						encounter = Encounter.Eyes;
						break;
					case SpeciesIds.Dhuum:
						// Eyes logs sometimes get Dhuum as the main target when the player is too close to him
						if (state.Agents.OfType<NPC>().Any(x => x.SpeciesId == SpeciesIds.EyeOfFate))
						{
							return Encounter.Eyes;
						}

						encounter = Encounter.Dhuum;
						break;
					case SpeciesIds.Nikare:
					case SpeciesIds.Kenut:
						encounter = Encounter.TwinLargos;
						break;
					case SpeciesIds.Qadim:
						encounter = Encounter.Qadim;
						break;
					case SpeciesIds.CardinalAdina:
						// During initial instance clearing, it is possible for the squad to start combat (clearing) near one boss
						// and then move to the other and start without leaving combat.
						// We check for the first cast skill of Sabir, that should happen reasonably early.
						// If both bosses were attacked at the same time, then choosing one at random is acceptable as well.
						encounter = state.Skills.Any(x => x.Id == SkillIds.SabirFirstAutoattack) ? Encounter.Sabir : Encounter.Adina;
						break;
					case SpeciesIds.CardinalSabir:
						// During initial instance clearing, it is possible for the squad to start combat (clearing) near one boss
						// and then move to the other and start without leaving combat.
						// We check for the first cast skill of Adina (often within a few milliseconds).
						// If both bosses were attacked at the same time, then choosing one at random is acceptable as well.
						encounter = state.Skills.Any(x => x.Id == SkillIds.AdinaChargeUp) ? Encounter.Adina : Encounter.Sabir;
						break;
					case SpeciesIds.QadimThePeerless:
						encounter = Encounter.QadimThePeerless;
						break;
					case SpeciesIds.Greer:
						encounter = Encounter.Greer;
						break;
					case SpeciesIds.Decima:
					case SpeciesIds.DecimaChallengeMode:
						encounter = Encounter.Decima;
						break;
					case SpeciesIds.Ura:
						encounter = Encounter.Ura;
						break;
					case SpeciesIds.StandardKittyGolem:
						encounter = Encounter.StandardKittyGolem;
						break;
					case SpeciesIds.MediumKittyGolem:
						encounter = Encounter.MediumKittyGolem;
						break;
					case SpeciesIds.LargeKittyGolem:
						encounter = Encounter.LargeKittyGolem;
						break;
					case SpeciesIds.MassiveKittyGolem:
						encounter = Encounter.MassiveKittyGolem;
						break;
					case SpeciesIds.MAMA:
						encounter = Encounter.MAMA;
						break;
					case SpeciesIds.SiaxTheCorrupted:
						encounter = Encounter.SiaxTheCorrupted;
						break;
					case SpeciesIds.EnsolyssOfTheEndlessTorment:
						encounter = Encounter.EnsolyssOfTheEndlessTorment;
						break;
					case SpeciesIds.Skorvald:
						encounter = Encounter.Skorvald;
						break;
					case SpeciesIds.Artsariiv:
						encounter = Encounter.Artsariiv;
						break;
					case SpeciesIds.Arkk:
						encounter = Encounter.Arkk;
						break;
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

						if (state.Skills.All(x => x.Id != SkillIds.Determined895))
						{
							// This is a quick path that doesn't require enumerating through events. 

							// As there is no Determined, the log is a failure, and this cannot occur in a log
							// that has both phases (as the Determined buff is applied in between).

							bool hasDarkPhase = state.Skills.Any(x => x.Id == SkillIds.AiDarkEarlySkill);
							encounter = hasDarkPhase
								? Encounter.AiKeeperOfThePeakNightOnly
								: Encounter.AiKeeperOfThePeakDayOnly;
						}
						else
						{
							bool inDark = false;
							bool determinedPreDark = false;
							foreach (var ev in state.Events)
							{
								if (ev is BuffApplyEvent { Buff.Id: SkillIds.Determined895 } and not InitialBuffEvent)
								{
									// This buff application is the transition between the two phases.
									// This works because we stop enumerating events once we reach the dark phase.
									determinedPreDark = true;
								}

								if (ev is SkillCastEvent { Skill.Id: SkillIds.AiDarkEarlySkill })
								{
									inDark = true;
									break;
								}
							}

							if (inDark)
							{
								encounter = determinedPreDark
									? Encounter.AiKeeperOfThePeakDayAndNight
									: Encounter.AiKeeperOfThePeakNightOnly;
							}
							else
							{
								encounter = Encounter.AiKeeperOfThePeakDayOnly;
							}
						}
						break;
					}
					case SpeciesIds.KanaxaiNM:
					case SpeciesIds.KanaxaiCM:
						encounter = Encounter.Kanaxai;
						break;
					case SpeciesIds.Eparch:
						encounter = Encounter.Eparch;
						break;
					case SpeciesIds.WhisperingShadow:
						encounter = Encounter.WhisperingShadow;
						break;
					case SpeciesIds.Freezie:
						encounter = Encounter.Freezie;
						break;
					case SpeciesIds.IcebroodConstruct:
						encounter = Encounter.ShiverpeaksPass;
						break;
					case SpeciesIds.VoiceOfTheFallen:
					case SpeciesIds.ClawOfTheFallen:
						encounter = Encounter.VoiceAndClawOfTheFallen;
						break;
					case SpeciesIds.FraenirOfJormag:
						encounter = Encounter.FraenirOfJormag;
						break;
					case SpeciesIds.Boneskinner:
						encounter = Encounter.Boneskinner;
						break;
					case SpeciesIds.WhisperOfJormag:
						encounter = Encounter.WhisperOfJormag;
						break;
					case SpeciesIds.VariniaStormsounder:
						encounter = Encounter.VariniaStormsounder;
						break;
					case SpeciesIds.HeartsAndMindsMordremoth:
						encounter = Encounter.Mordremoth;
						break;
					case SpeciesIds.MaiTrin:
						encounter = Encounter.AetherbladeHideout;
						break;
					case SpeciesIds.Ankka:
						encounter = Encounter.XunlaiJadeJunkyard;
						break;
					case SpeciesIds.MinisterLi:
					case SpeciesIds.MinisterLiChallengeMode:
						encounter = Encounter.KainengOverlook;
						break;
					case SpeciesIds.VoidAmalgamate:
					case SpeciesIds.VoidMelter:
						encounter = Encounter.HarvestTemple;
						break;
					case SpeciesIds.PrototypeVermillion:
					case SpeciesIds.PrototypeArsenite:
					case SpeciesIds.PrototypeIndigo:
					case SpeciesIds.PrototypeVermillionChallengeMode:
					case SpeciesIds.PrototypeArseniteChallengeMode:
					case SpeciesIds.PrototypeIndigoChallengeMode:
						encounter = Encounter.OldLionsCourt;
						break;
					case SpeciesIds.Dagda:
						encounter = Encounter.CosmicObservatory;
						break;
					case SpeciesIds.Cerus:
						encounter = Encounter.TempleOfFebe;
						break;
					case SpeciesIds.Kela:
						encounter = Encounter.GuardiansGlade;
						break;
						// Important: when adding a new encounter, make sure you also add it to the IdentifyPotentialEncounters method.
				}
				state.MainTarget = boss;
				return encounter;
			}
			else if (mainTarget is Gadget gadgetBoss)
			{
				switch (gadgetBoss.VolatileId)
				{
					case GadgetIds.EtherealBarrier:
					case GadgetIds.EtherealBarrierChina:
						encounter = Encounter.SpiritRace;
						break;
					case GadgetIds.ConjuredAmalgamate:
						encounter = Encounter.ConjuredAmalgamate;
						break;
					case GadgetIds.TheDragonvoidFinal:
						encounter = Encounter.HarvestTemple;
						break;
					case GadgetIds.TheDragonvoid:
						encounter = Encounter.HarvestTemple;
						break;
						// Important: when adding a new encounter, make sure you also add it to the IdentifyPotentialEncounters method.
				}
				state.MainTarget = gadgetBoss;
				return encounter;
			}
		}

		return encounter;
	}

	public IEnumerable<Encounter> IdentifyPotentialEncounters(ParsedBossData bossData)
	{
		switch (bossData.ID)
		{
			case ArcdpsBossIds.WorldVersusWorld:
				return new[] { Encounter.WorldVersusWorld };
			case ArcdpsBossIds.Map:
				return new[] { Encounter.Map };
			case SpeciesIds.ValeGuardian:
				return new[] { Encounter.ValeGuardian };
			case SpeciesIds.Gorseval:
				return new[] { Encounter.Gorseval };
			case SpeciesIds.Sabetha:
				return new[] { Encounter.Sabetha };
			case SpeciesIds.Slothasor:
				return new[] { Encounter.Slothasor };
			case SpeciesIds.Berg:
			case SpeciesIds.Zane:
			case SpeciesIds.Narella:
				return new[] { Encounter.BanditTrio };
			case SpeciesIds.MatthiasGabrel:
				return new[] { Encounter.Matthias };
			case SpeciesIds.MushroomKing:
			case SpeciesIds.McLeod:
				return new[] { Encounter.Escort };
			case SpeciesIds.KeepConstruct:
				return new[] { Encounter.KeepConstruct };
			case SpeciesIds.HauntingStatue:
				return new[] { Encounter.TwistedCastle };
			case SpeciesIds.Xera:
				// Twisted Castle logs sometimes get Xera as the main target when the player is too close to her
				return new[] { Encounter.TwistedCastle, Encounter.Xera };
			case SpeciesIds.XeraSecondPhase:
				return new[] { Encounter.Xera };
			case SpeciesIds.CairnTheIndomitable:
				return new[] { Encounter.Cairn };
			case SpeciesIds.MursaatOverseer:
				return new[] { Encounter.MursaatOverseer };
			case SpeciesIds.Samarog:
				return new[] { Encounter.Samarog };
			case SpeciesIds.Deimos:
				return new[] { Encounter.Deimos };
			case SpeciesIds.SoullessHorror:
				return new[] { Encounter.SoullessHorror };
			case SpeciesIds.Desmina:
				return new[] { Encounter.RiverOfSouls };
			case SpeciesIds.BrokenKing:
				return new[] { Encounter.BrokenKing };
			case SpeciesIds.EaterOfSouls:
				return new[] { Encounter.EaterOfSouls };
			case SpeciesIds.EyeOfJudgment:
			case SpeciesIds.EyeOfFate:
				return new[] { Encounter.Eyes };
			case SpeciesIds.Dhuum:
				// Eyes logs sometimes get Dhuum as the main target when the player is too close to him
				return new[] { Encounter.Eyes, Encounter.Dhuum };
			case SpeciesIds.Nikare:
			case SpeciesIds.Kenut:
				return new[] { Encounter.TwinLargos };
			case SpeciesIds.Qadim:
				return new[] { Encounter.Qadim };
			case SpeciesIds.CardinalAdina:
				return new[] { Encounter.Adina, Encounter.Sabir };
			case SpeciesIds.CardinalSabir:
				return new[] { Encounter.Sabir, Encounter.Adina };
			case SpeciesIds.QadimThePeerless:
				return new[] { Encounter.QadimThePeerless };
			case SpeciesIds.Greer:
				return new[] { Encounter.Greer };
			case SpeciesIds.Decima:
			case SpeciesIds.DecimaChallengeMode:
				return new[] { Encounter.Decima };
			case SpeciesIds.Ura:
				return new[] { Encounter.Ura };
			case SpeciesIds.StandardKittyGolem:
				return new[] { Encounter.StandardKittyGolem };
			case SpeciesIds.MediumKittyGolem:
				return new[] { Encounter.MediumKittyGolem };
			case SpeciesIds.LargeKittyGolem:
				return new[] { Encounter.LargeKittyGolem };
			case SpeciesIds.MassiveKittyGolem:
				return new[] { Encounter.MassiveKittyGolem };
			case SpeciesIds.MAMA:
				return new[] { Encounter.MAMA };
			case SpeciesIds.SiaxTheCorrupted:
				return new[] { Encounter.SiaxTheCorrupted };
			case SpeciesIds.EnsolyssOfTheEndlessTorment:
				return new[] { Encounter.EnsolyssOfTheEndlessTorment };
			case SpeciesIds.Skorvald:
				return new[] { Encounter.Skorvald };
			case SpeciesIds.Artsariiv:
				return new[] { Encounter.Artsariiv };
			case SpeciesIds.Arkk:
				return new[] { Encounter.Arkk };
			case SpeciesIds.AiKeeperOfThePeak:
				return new[] { Encounter.AiKeeperOfThePeakDayAndNight, Encounter.AiKeeperOfThePeakNightOnly, Encounter.AiKeeperOfThePeakDayOnly };
			case SpeciesIds.KanaxaiNM:
				return new[] { Encounter.Kanaxai };
			case SpeciesIds.KanaxaiCM:
				return new[] { Encounter.Kanaxai };
			case SpeciesIds.Eparch:
				return new[] { Encounter.Eparch };
			case SpeciesIds.WhisperingShadow:
				return new[] { Encounter.WhisperingShadow };
			case SpeciesIds.Freezie:
				return new[] { Encounter.Freezie };
			case SpeciesIds.IcebroodConstruct:
				return new[] { Encounter.ShiverpeaksPass };
			case SpeciesIds.VoiceOfTheFallen:
			case SpeciesIds.ClawOfTheFallen:
				return new[] { Encounter.VoiceAndClawOfTheFallen };
			case SpeciesIds.FraenirOfJormag:
				return new[] { Encounter.FraenirOfJormag };
			case SpeciesIds.Boneskinner:
				return new[] { Encounter.Boneskinner };
			case SpeciesIds.WhisperOfJormag:
				return new[] { Encounter.WhisperOfJormag };
			case SpeciesIds.VariniaStormsounder:
				return new[] { Encounter.VariniaStormsounder };
			case SpeciesIds.HeartsAndMindsMordremoth:
				return new[] { Encounter.Mordremoth };
			case SpeciesIds.MaiTrin:
				return new[] { Encounter.AetherbladeHideout };
			case SpeciesIds.Ankka:
				return new[] { Encounter.XunlaiJadeJunkyard };
			case SpeciesIds.MinisterLi:
			case SpeciesIds.MinisterLiChallengeMode:
				return new[] { Encounter.KainengOverlook };
			case SpeciesIds.VoidAmalgamate:
			case SpeciesIds.VoidMelter:
				return new[] { Encounter.HarvestTemple };
			case SpeciesIds.PrototypeVermillion:
			case SpeciesIds.PrototypeArsenite:
			case SpeciesIds.PrototypeIndigo:
			case SpeciesIds.PrototypeVermillionChallengeMode:
			case SpeciesIds.PrototypeArseniteChallengeMode:
			case SpeciesIds.PrototypeIndigoChallengeMode:
				return new[] { Encounter.OldLionsCourt };
			case SpeciesIds.Dagda:
				return new[] { Encounter.CosmicObservatory };
			case SpeciesIds.Cerus:
				return new[] { Encounter.TempleOfFebe };
			case SpeciesIds.Kela:
				return new[] { Encounter.GuardiansGlade };
			case GadgetIds.EtherealBarrier:
			case GadgetIds.EtherealBarrierChina:
				return new[] { Encounter.SpiritRace };
			case GadgetIds.ConjuredAmalgamate:
				return new[] { Encounter.ConjuredAmalgamate };
			case GadgetIds.TheDragonvoidFinal:
				return new[] { Encounter.HarvestTemple };
			case GadgetIds.TheDragonvoid:
				return new[] { Encounter.HarvestTemple };
			default:
				return new[] { Encounter.Other };
		}
	}
}