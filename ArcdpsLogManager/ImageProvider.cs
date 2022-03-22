using System;
using Eto.Drawing;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager
{
	public class ImageProvider
	{
		// PROFESSIONS
		private Lazy<Image> TinyIconWarrior { get; } = new Lazy<Image>(Resources.GetTinyIconWarrior);
		private Lazy<Image> TinyIconGuardian { get; } = new Lazy<Image>(Resources.GetTinyIconGuardian);
		private Lazy<Image> TinyIconRevenant { get; } = new Lazy<Image>(Resources.GetTinyIconRevenant);
		private Lazy<Image> TinyIconRanger { get; } = new Lazy<Image>(Resources.GetTinyIconRanger);
		private Lazy<Image> TinyIconThief { get; } = new Lazy<Image>(Resources.GetTinyIconThief);
		private Lazy<Image> TinyIconEngineer { get; } = new Lazy<Image>(Resources.GetTinyIconEngineer);
		private Lazy<Image> TinyIconNecromancer { get; } = new Lazy<Image>(Resources.GetTinyIconNecromancer);
		private Lazy<Image> TinyIconElementalist { get; } = new Lazy<Image>(Resources.GetTinyIconElementalist);
		private Lazy<Image> TinyIconMesmer { get; } = new Lazy<Image>(Resources.GetTinyIconMesmer);
		private Lazy<Image> TinyIconUnknownProfession { get; } = new Lazy<Image>(Resources.GetTinyIconUnknown);

		// SPECIALIZATIONS
		private Lazy<Image> TinyIconBerserker { get; } = new Lazy<Image>(Resources.GetTinyIconBerserker);
		private Lazy<Image> TinyIconSpellbreaker { get; } = new Lazy<Image>(Resources.GetTinyIconSpellbreaker);
		private Lazy<Image> TinyIconBladesworn { get; } = new Lazy<Image>(Resources.GetTinyIconBladesworn);
		private Lazy<Image> TinyIconDragonhunter { get; } = new Lazy<Image>(Resources.GetTinyIconDragonhunter);
		private Lazy<Image> TinyIconWillbender { get; } = new Lazy<Image>(Resources.GetTinyIconWillbender);
		private Lazy<Image> TinyIconFirebrand { get; } = new Lazy<Image>(Resources.GetTinyIconFirebrand);
		private Lazy<Image> TinyIconHerald { get; } = new Lazy<Image>(Resources.GetTinyIconHerald);
		private Lazy<Image> TinyIconRenegade { get; } = new Lazy<Image>(Resources.GetTinyIconRenegade);
		private Lazy<Image> TinyIconVindicator { get; } = new Lazy<Image>(Resources.GetTinyIconVindicator);
		private Lazy<Image> TinyIconDruid { get; } = new Lazy<Image>(Resources.GetTinyIconDruid);
		private Lazy<Image> TinyIconSoulbeast { get; } = new Lazy<Image>(Resources.GetTinyIconSoulbeast);
		private Lazy<Image> TinyIconUntamed { get; } = new Lazy<Image>(Resources.GetTinyIconUntamed);
		private Lazy<Image> TinyIconDaredevil { get; } = new Lazy<Image>(Resources.GetTinyIconDaredevil);
		private Lazy<Image> TinyIconDeadeye { get; } = new Lazy<Image>(Resources.GetTinyIconDeadeye);
		private Lazy<Image> TinyIconSpecter { get; } = new Lazy<Image>(Resources.GetTinyIconSpecter);
		private Lazy<Image> TinyIconScrapper { get; } = new Lazy<Image>(Resources.GetTinyIconScrapper);
		private Lazy<Image> TinyIconHolosmith { get; } = new Lazy<Image>(Resources.GetTinyIconHolosmith);
		private Lazy<Image> TinyIconMechanist { get; } = new Lazy<Image>(Resources.GetTinyIconMechanist);
		private Lazy<Image> TinyIconReaper { get; } = new Lazy<Image>(Resources.GetTinyIconReaper);
		private Lazy<Image> TinyIconScourge { get; } = new Lazy<Image>(Resources.GetTinyIconScourge);
		private Lazy<Image> TinyIconHarbinger { get; } = new Lazy<Image>(Resources.GetTinyIconHarbinger);
		private Lazy<Image> TinyIconTempest { get; } = new Lazy<Image>(Resources.GetTinyIconTempest);
		private Lazy<Image> TinyIconWeaver { get; } = new Lazy<Image>(Resources.GetTinyIconWeaver);
		private Lazy<Image> TinyIconCatalyst { get; } = new Lazy<Image>(Resources.GetTinyIconCatalyst);
		private Lazy<Image> TinyIconChronomancer { get; } = new Lazy<Image>(Resources.GetTinyIconChronomancer);
		private Lazy<Image> TinyIconMirage { get; } = new Lazy<Image>(Resources.GetTinyIconMirage);
		private Lazy<Image> TinyIconVirtuoso { get; } = new Lazy<Image>(Resources.GetTinyIconVirtuoso);

		// CATEGORIES
		private Lazy<Image> TinyIconRaid { get; } = new Lazy<Image>(Resources.GetTinyIconRaid);
		private Lazy<Image> TinyIconFractals { get; } = new Lazy<Image>(Resources.GetTinyIconFractals);
		private Lazy<Image> TinyIconLog { get; } = new Lazy<Image>(Resources.GetTinyIconGuildRegistrar);
		private Lazy<Image> TinyIconCommander { get; } = new Lazy<Image>(Resources.GetTinyIconCommander);
		private Lazy<Image> TinyIconStrike { get; } = new Lazy<Image>(Resources.GetTinyIconStrike);
		private Lazy<Image> TinyIconTrainingArea { get; } = new Lazy<Image>(Resources.GetTinyIconTrainingArea);
		private Lazy<Image> TinyIconWorldVersusWorld { get; } = new Lazy<Image>(Resources.GetTinyIconWorldVersusWorld);
		private Lazy<Image> TinyIconUncategorized { get; } = new Lazy<Image>(Resources.GetTinyIconUncategorized);
		private Lazy<Image> TinyIconFestival { get; } = new Lazy<Image>(Resources.GetTinyIconFestival);
		private Lazy<Image> TinyIconIcebroodSaga { get; } = new Lazy<Image>(Resources.GetTinyIconIcebroodSaga);
		private Lazy<Image> TinyIconEndOfDragons { get; } = new Lazy<Image>(Resources.GetTinyIconEndOfDragons);

		// RAIDS
		private Lazy<Image> GenericRaidWing { get; } = new Lazy<Image>(Resources.GetGenericRaidWingIcon);

		// RAID BOSSES
		// WING 1
		private Lazy<Image> ValeGuardianIcon { get; } = new Lazy<Image>(Resources.GetValeGuardianIcon);
		private Lazy<Image> GorsevalIcon { get; } = new Lazy<Image>(Resources.GetGorsevalIcon);
		private Lazy<Image> SabethaIcon { get; } = new Lazy<Image>(Resources.GetSabethaIcon);

		// WING 2
		private Lazy<Image> SlothasorIcon { get; } = new Lazy<Image>(Resources.GetSlothasorIcon);
		private Lazy<Image> BanditTrioIcon { get; } = new Lazy<Image>(Resources.GetBanditTrioIcon);
		private Lazy<Image> MatthiasIcon { get; } = new Lazy<Image>(Resources.GetMatthiasIcon);

		// WING 3
		private Lazy<Image> EscortIcon { get; } = new Lazy<Image>(Resources.GetEscortIcon);
		private Lazy<Image> KeepConstructIcon { get; } = new Lazy<Image>(Resources.GetKeepConstructIcon);
		private Lazy<Image> TwistedCastleIcon { get; } = new Lazy<Image>(Resources.GetTwistedCastleIcon);
		private Lazy<Image> XeraIcon { get; } = new Lazy<Image>(Resources.GetXeraIcon);

		// WING 4
		private Lazy<Image> CairnIcon { get; } = new Lazy<Image>(Resources.GetCairnIcon);
		private Lazy<Image> MursaatOverseerIcon { get; } = new Lazy<Image>(Resources.GetMursaatOverseerIcon);
		private Lazy<Image> SamarogIcon { get; } = new Lazy<Image>(Resources.GetSamarogIcon);
		private Lazy<Image> DeimosIcon { get; } = new Lazy<Image>(Resources.GetDeimosIcon);

		// WING 5
		private Lazy<Image> SoullessHorrorIcon { get; } = new Lazy<Image>(Resources.GetDesminaIcon);
		private Lazy<Image> RiverOfSoulsIcon { get; } = new Lazy<Image>(Resources.GetRiverOfSoulsIcon);
		private Lazy<Image> BrokenKingIcon { get; } = new Lazy<Image>(Resources.GetBrokenKingIcon);
		private Lazy<Image> EaterOfSoulsIcon { get; } = new Lazy<Image>(Resources.GetEaterOfSoulsIcon);
		private Lazy<Image> EyesIcon { get; } = new Lazy<Image>(Resources.GetEyesIcon);
		private Lazy<Image> DhuumIcon { get; } = new Lazy<Image>(Resources.GetDhuumIcon);

		// WING 6
		private Lazy<Image> ConjuredAmalgamatedIcon { get; } = new Lazy<Image>(Resources.GetConjuredAmalgamatedIcon);
		private Lazy<Image> TwinLargosIcon { get; } = new Lazy<Image>(Resources.GetTwinLargosIcon);
		private Lazy<Image> QadimIcon { get; } = new Lazy<Image>(Resources.GetQadimIcon);

		// WING 7
		private Lazy<Image> CardinalAdinaIcon { get; } = new Lazy<Image>(Resources.GetCardinalAdinaIcon);
		private Lazy<Image> CardinalSabirIcon { get; } = new Lazy<Image>(Resources.GetCardinalSabirIcon);
		private Lazy<Image> QadimThePeerlessIcon { get; } = new Lazy<Image>(Resources.GetQadimThePeerlessIcon);

		// STRIKES - ICEBROOD SAGA
		private Lazy<Image> ShiverpeaksPassIcon { get; } = new Lazy<Image>(Resources.GetShiverpeaksPassIcon);
		private Lazy<Image> VoiceAndClawOfTheFallenIcon { get; } = new Lazy<Image>(Resources.GetVoiceAndClawOfTheFallenIcon);
		private Lazy<Image> FraenirOfJormagIcon { get; } = new Lazy<Image>(Resources.GetFraenirOfJormagIcon);
		private Lazy<Image> BoneskinnerIcon { get; } = new Lazy<Image>(Resources.GetBoneskinnerIcon);
		private Lazy<Image> WhisperOfJormagIcon { get; } = new Lazy<Image>(Resources.GetWhisperOfJormagIcon);
		private Lazy<Image> VariniaStormsounderIcon { get; } = new Lazy<Image>(Resources.GetVariniaStormsounderIcon);
		
		// STRIKES - END OF DRAGONS
		private Lazy<Image> AetherbladeHideoutIcon { get; } = new Lazy<Image>(Resources.GetAetherbladeHideoutIcon);
		private Lazy<Image> XunlaiJadeJunkyardIcon { get; } = new Lazy<Image>(Resources.GetXunlaiJadeJunkyardIcon);
		private Lazy<Image> KainengOverlookIcon { get; } = new Lazy<Image>(Resources.GetKainengOverlookIcon);
		private Lazy<Image> HarvestTempleIcon { get; } = new Lazy<Image>(Resources.GetHarvestTempleIcon);

		// FRACTALS
		private Lazy<Image> MAMAIcon { get; } = new Lazy<Image>(Resources.GetMAMAIcon);
		private Lazy<Image> SiaxTheCorruptedIcon { get; } = new Lazy<Image>(Resources.GetSiaxTheCorruptedIcon);
		private Lazy<Image> EnsolyssOfTheEndlessTormentIcon { get; } = new Lazy<Image>(Resources.GetEnsolyssOfTheEndlessTormentIcon);
		private Lazy<Image> SkorvaldIcon { get; } = new Lazy<Image>(Resources.GetSkorvaldIcon);
		private Lazy<Image> ArtsariivIcon { get; } = new Lazy<Image>(Resources.GetArtsariivIcon);
		private Lazy<Image> ArkkIcon { get; } = new Lazy<Image>(Resources.GetArkkIcon);
		private Lazy<Image> AiKeeperOfThePeakIcon { get; } = new Lazy<Image>(Resources.GetBothPhasesAiKeeperOfThePeakIcon);
		private Lazy<Image> ElementalAiKeeperOfThePeakIcon { get; } = new Lazy<Image>(Resources.GetElementalAiKeeperOfThePeakIcon);
		private Lazy<Image> DarkAiKeeperOfThePeakIcon { get; } = new Lazy<Image>(Resources.GetDarkAiKeeperOfThePeakIcon);
		private Lazy<Image> BothPhasesAiKeeperOfThePeakIcon { get; } = new Lazy<Image>(Resources.GetBothPhasesAiKeeperOfThePeakIcon);

		// FESTIVALS
		private Lazy<Image> FreezieIcon { get; } = new Lazy<Image>(Resources.GetFreezieIcon);

		// TRAINING AREA
		private Lazy<Image> StandardKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetStandardKittyGolemIcon);
		private Lazy<Image> MediumKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetMediumKittyGolemIcon);
		private Lazy<Image> LargeKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetLargeKittyGolemIcon);
		private Lazy<Image> MassiveKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetMassiveKittyGolemIcon);

		// WORLD VS WORLD
		private Lazy<Image> EternalBattlegroundsIcon { get; } = new Lazy<Image>(Resources.GetEternalBattlegroundsIcon);
		private Lazy<Image> RedBorderlandsIcon { get; } = new Lazy<Image>(Resources.GetRedBorderlandsIcon);
		private Lazy<Image> BlueBorderlandsIcon { get; } = new Lazy<Image>(Resources.GetBlueBorderlandsIcon);
		private Lazy<Image> GreenBorderlandsIcon { get; } = new Lazy<Image>(Resources.GetGreenBorderlandsIcon);
		private Lazy<Image> ObsidianSanctumIcon { get; } = new Lazy<Image>(Resources.GetObsidianSanctumIcon);
		private Lazy<Image> EdgeOfTheMistsIcon { get; } = new Lazy<Image>(Resources.GetEdgeOfTheMistsIcon);
		private Lazy<Image> ArmisticeBastionIcon { get; } = new Lazy<Image>(Resources.GetArmisticeBastionIcon);
		
		// INSTABILITIES
		private Lazy<Image> AdrenalineRushIcon { get; } = new Lazy<Image>(Resources.GetTinyIconAdrenalineRush);
		private Lazy<Image> AfflictedIcon { get; } = new Lazy<Image>(Resources.GetTinyIconAfflicted);
		private Lazy<Image> BirdsIcon { get; } = new Lazy<Image>(Resources.GetTinyIconBirds);
		private Lazy<Image> BoonOverloadIcon { get; } = new Lazy<Image>(Resources.GetTinyIconBoonOverload);
		private Lazy<Image> FluxBombIcon { get; } = new Lazy<Image>(Resources.GetTinyIconFluxBomb);
		private Lazy<Image> FractalVindicatorsIcon { get; } = new Lazy<Image>(Resources.GetTinyIconFractalVindicators);
		private Lazy<Image> FrailtyIcon { get; } = new Lazy<Image>(Resources.GetTinyIconFrailty);
		private Lazy<Image> HamstrungIcon { get; } = new Lazy<Image>(Resources.GetTinyIconHamstrung);
		private Lazy<Image> LastLaughIcon { get; } = new Lazy<Image>(Resources.GetTinyIconLastLaugh);
		private Lazy<Image> MistsConvergenceIcon { get; } = new Lazy<Image>(Resources.GetTinyIconMistsConvergence);
		private Lazy<Image> NoPainNoGainIcon { get; } = new Lazy<Image>(Resources.GetTinyIconNoPainNoGain);
		private Lazy<Image> OutflankedIcon { get; } = new Lazy<Image>(Resources.GetTinyIconOutflanked);
		private Lazy<Image> SlipperySlopeIcon { get; } = new Lazy<Image>(Resources.GetTinyIconSlipperySlope);
		private Lazy<Image> SocialAwkwardnessIcon { get; } = new Lazy<Image>(Resources.GetTinyIconSocialAwkwardness);
		private Lazy<Image> StickTogetherIcon { get; } = new Lazy<Image>(Resources.GetTinyIconStickTogether);
		private Lazy<Image> SugarRushIcon { get; } = new Lazy<Image>(Resources.GetTinyIconSugarRush);
		private Lazy<Image> ToxicSicknessIcon { get; } = new Lazy<Image>(Resources.GetTinyIconToxicSickness);
		private Lazy<Image> ToxicTrailIcon { get; } = new Lazy<Image>(Resources.GetTinyIconToxicTrail);
		private Lazy<Image> VengeanceIcon { get; } = new Lazy<Image>(Resources.GetTinyIconVengeance);
		private Lazy<Image> WeBleedFireIcon { get; } = new Lazy<Image>(Resources.GetTinyIconWeBleedFire);


		public Image GetTinyLogIcon() => TinyIconLog.Value;
		public Image GetTinyFractalsIcon() => TinyIconFractals.Value;
		public Image GetTinyRaidIcon() => TinyIconRaid.Value;
		public Image GetTinyCommanderIcon() => TinyIconCommander.Value;
		public Image GetTinyStrikeIcon() => TinyIconStrike.Value;
		public Image GetTinyTrainingAreaIcon() => TinyIconTrainingArea.Value;
		public Image GetTinyWorldVersusWorldIcon() => TinyIconWorldVersusWorld.Value;
		public Image GetTinyUncategorizedIcon() => TinyIconUncategorized.Value;
		public Image GetTinyFestivalIcon() => TinyIconFestival.Value;
		public Image GetTinyIcebroodSagaIcon() => TinyIconIcebroodSaga.Value;
		public Image GetTinyEndOfDragonsIcon() => TinyIconEndOfDragons.Value;

		public Image GetTinyProfessionIcon(Profession profession)
		{
			return profession switch
			{
				Profession.Warrior => TinyIconWarrior.Value,
				Profession.Guardian => TinyIconGuardian.Value,
				Profession.Revenant => TinyIconRevenant.Value,
				Profession.Ranger => TinyIconRanger.Value,
				Profession.Thief => TinyIconThief.Value,
				Profession.Engineer => TinyIconEngineer.Value,
				Profession.Necromancer => TinyIconNecromancer.Value,
				Profession.Elementalist => TinyIconElementalist.Value,
				Profession.Mesmer => TinyIconMesmer.Value,
				Profession.None => TinyIconUnknownProfession.Value,
				_ => throw new ArgumentOutOfRangeException(nameof(profession)),
			};
		}

		public Image GetTinyProfessionIcon(EliteSpecialization specialization)
		{
			return specialization switch
			{
				EliteSpecialization.Berserker => TinyIconBerserker.Value,
				EliteSpecialization.Spellbreaker => TinyIconSpellbreaker.Value,
				EliteSpecialization.Bladesworn => TinyIconBladesworn.Value,
				EliteSpecialization.Dragonhunter => TinyIconDragonhunter.Value,
				EliteSpecialization.Firebrand => TinyIconFirebrand.Value,
				EliteSpecialization.Willbender => TinyIconWillbender.Value,
				EliteSpecialization.Herald => TinyIconHerald.Value,
				EliteSpecialization.Renegade => TinyIconRenegade.Value,
				EliteSpecialization.Vindicator => TinyIconVindicator.Value,
				EliteSpecialization.Druid => TinyIconDruid.Value,
				EliteSpecialization.Soulbeast => TinyIconSoulbeast.Value,
				EliteSpecialization.Untamed => TinyIconUntamed.Value,
				EliteSpecialization.Daredevil => TinyIconDaredevil.Value,
				EliteSpecialization.Deadeye => TinyIconDeadeye.Value,
				EliteSpecialization.Scrapper => TinyIconScrapper.Value,
				EliteSpecialization.Holosmith => TinyIconHolosmith.Value,
				EliteSpecialization.Mechanist => TinyIconMechanist.Value,
				EliteSpecialization.Reaper => TinyIconReaper.Value,
				EliteSpecialization.Scourge => TinyIconScourge.Value,
				EliteSpecialization.Harbinger => TinyIconHarbinger.Value,
				EliteSpecialization.Tempest => TinyIconTempest.Value,
				EliteSpecialization.Weaver => TinyIconWeaver.Value,
				EliteSpecialization.Chronomancer => TinyIconChronomancer.Value,
				EliteSpecialization.Mirage => TinyIconMirage.Value,
				EliteSpecialization.Virtuoso => TinyIconVirtuoso.Value,
				EliteSpecialization.Specter => TinyIconSpecter.Value,
				EliteSpecialization.Catalyst => TinyIconCatalyst.Value,
				_ => throw new ArgumentOutOfRangeException(nameof(specialization)),
			};
		}

		public Image GetTinyProfessionIcon(LogPlayer player)
		{
			if (player.EliteSpecialization == EliteSpecialization.None)
			{
				return GetTinyProfessionIcon(player.Profession);
			}

			return GetTinyProfessionIcon(player.EliteSpecialization);
		}

		public Image GetTinyEncounterIcon(Encounter encounter)
		{
			return encounter switch
			{
				// RAIDS
				// W1
				Encounter.ValeGuardian => ValeGuardianIcon.Value,
				Encounter.Gorseval => GorsevalIcon.Value,
				Encounter.Sabetha => SabethaIcon.Value,
				// W2
				Encounter.Slothasor => SlothasorIcon.Value,
				Encounter.BanditTrio => BanditTrioIcon.Value,
				Encounter.Matthias => MatthiasIcon.Value,
				// W3
				Encounter.Escort => EscortIcon.Value,
				Encounter.KeepConstruct => KeepConstructIcon.Value,
				Encounter.TwistedCastle => TwistedCastleIcon.Value,
				Encounter.Xera => XeraIcon.Value,
				// W4
				Encounter.Cairn => CairnIcon.Value,
				Encounter.MursaatOverseer => MursaatOverseerIcon.Value,
				Encounter.Samarog => SamarogIcon.Value,
				Encounter.Deimos => DeimosIcon.Value,
				// W5
				Encounter.SoullessHorror => SoullessHorrorIcon.Value,
				Encounter.RiverOfSouls => RiverOfSoulsIcon.Value,
				Encounter.BrokenKing => BrokenKingIcon.Value,
				Encounter.EaterOfSouls => EaterOfSoulsIcon.Value,
				Encounter.Eyes => EyesIcon.Value,
				Encounter.Dhuum => DhuumIcon.Value,
				// W6
				Encounter.ConjuredAmalgamate => ConjuredAmalgamatedIcon.Value,
				Encounter.TwinLargos => TwinLargosIcon.Value,
				Encounter.Qadim => QadimIcon.Value,
				// W7
				Encounter.Adina => CardinalAdinaIcon.Value,
				Encounter.Sabir => CardinalSabirIcon.Value,
				Encounter.QadimThePeerless => QadimThePeerlessIcon.Value,
				// STRIKES - ICEBROOD SAGA
				Encounter.ShiverpeaksPass => ShiverpeaksPassIcon.Value,
				Encounter.VoiceAndClawOfTheFallen => VoiceAndClawOfTheFallenIcon.Value,
				Encounter.FraenirOfJormag => FraenirOfJormagIcon.Value,
				Encounter.Boneskinner => BoneskinnerIcon.Value,
				Encounter.WhisperOfJormag => WhisperOfJormagIcon.Value,
				Encounter.VariniaStormsounder => VariniaStormsounderIcon.Value,
				// STRIKES - END OF DRAGONSa
				Encounter.AetherbladeHideout => AetherbladeHideoutIcon.Value,
				Encounter.XunlaiJadeJunkyard => XunlaiJadeJunkyardIcon.Value,
				Encounter.KainengOverlook => KainengOverlookIcon.Value,
				Encounter.HarvestTemple => HarvestTempleIcon.Value,
				// FRACTALS
				Encounter.MAMA => MAMAIcon.Value,
				Encounter.SiaxTheCorrupted => SiaxTheCorruptedIcon.Value,
				Encounter.EnsolyssOfTheEndlessTorment => EnsolyssOfTheEndlessTormentIcon.Value,
				Encounter.Skorvald => SkorvaldIcon.Value,
				Encounter.Artsariiv => ArtsariivIcon.Value,
				Encounter.Arkk => ArkkIcon.Value,
#pragma warning disable 618
				Encounter.AiKeeperOfThePeak => AiKeeperOfThePeakIcon.Value, // unused?
#pragma warning restore 618
				Encounter.AiKeeperOfThePeakDayOnly => ElementalAiKeeperOfThePeakIcon.Value,
				Encounter.AiKeeperOfThePeakNightOnly => DarkAiKeeperOfThePeakIcon.Value,
				Encounter.AiKeeperOfThePeakDayAndNight => BothPhasesAiKeeperOfThePeakIcon.Value,
				// FESTIVALS
				Encounter.Freezie => FreezieIcon.Value,
				// TRAINING AREA
				Encounter.StandardKittyGolem => StandardKittyGolemIcon.Value,
				Encounter.MediumKittyGolem => MediumKittyGolemIcon.Value,
				Encounter.LargeKittyGolem => LargeKittyGolemIcon.Value,
				Encounter.MassiveKittyGolem => MassiveKittyGolemIcon.Value,
				_ => null
			};
		}

		public Image GetRaidWingIcon()
		{
			return GenericRaidWing.Value;
		}

		public Image GetWvWMapIcon(GameMap map)
		{
			return map switch
			{
				GameMap.EternalBattlegrounds => EternalBattlegroundsIcon.Value,
				GameMap.RedDesertBorderlands => RedBorderlandsIcon.Value,
				GameMap.BlueAlpineBorderlands => BlueBorderlandsIcon.Value,
				GameMap.GreenAlpineBorderlands => GreenBorderlandsIcon.Value,
				GameMap.ObsidianSanctum => ObsidianSanctumIcon.Value,
				GameMap.EdgeOfTheMists => EdgeOfTheMistsIcon.Value,
				GameMap.ArmisticeBastion => ArmisticeBastionIcon.Value,
				_ => null
			};
		}

		public Image GetMistlockInstabilityIcon(MistlockInstability instability)
		{
			return instability switch
			{
				MistlockInstability.AdrenalineRush => AdrenalineRushIcon.Value,
				MistlockInstability.Afflicted => AfflictedIcon.Value,
				MistlockInstability.BoonOverload => BoonOverloadIcon.Value,
				MistlockInstability.FluxBomb => FluxBombIcon.Value,
				MistlockInstability.FractalVindicators => FractalVindicatorsIcon.Value,
				MistlockInstability.Frailty => FrailtyIcon.Value,
				MistlockInstability.Hamstrung => HamstrungIcon.Value,
				MistlockInstability.LastLaugh => LastLaughIcon.Value,
				MistlockInstability.MistsConvergence => MistsConvergenceIcon.Value,
				MistlockInstability.NoPainNoGain => NoPainNoGainIcon.Value,
				MistlockInstability.Outflanked => OutflankedIcon.Value,
				MistlockInstability.SocialAwkwardness => SocialAwkwardnessIcon.Value,
				MistlockInstability.StickTogether => StickTogetherIcon.Value,
				MistlockInstability.SugarRush => SugarRushIcon.Value,
				MistlockInstability.ToxicSickness => ToxicSicknessIcon.Value,
				MistlockInstability.ToxicTrail => ToxicTrailIcon.Value,
				MistlockInstability.Vengeance => VengeanceIcon.Value,
				MistlockInstability.WeBleedFire => WeBleedFireIcon.Value,
				MistlockInstability.Birds => BirdsIcon.Value,
				MistlockInstability.SlipperySlope => SlipperySlopeIcon.Value,
				_ => throw new ArgumentOutOfRangeException(nameof(instability), instability, null)
			};
		}
	}
}