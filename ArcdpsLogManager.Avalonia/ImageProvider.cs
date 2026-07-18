using System;
using Avalonia.Media.Imaging;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager.Avalonia
{
	/// <summary>
	/// The Avalonia counterpart of the Eto <c>GW2Scratch.ArcdpsLogManager.ImageProvider</c>.
	/// Maps game entities to cached <see cref="Bitmap"/>s (the Eto version used
	/// <c>Eto.Drawing.Image</c>). The <see cref="Lazy{T}"/> caching pattern is preserved so each
	/// icon bitmap is decoded once and reused.
	/// </summary>
	public class ImageProvider
	{
		// PROFESSIONS
		private Lazy<Bitmap> TinyIconWarrior { get; } = new Lazy<Bitmap>(Resources.GetTinyIconWarrior);
		private Lazy<Bitmap> TinyIconGuardian { get; } = new Lazy<Bitmap>(Resources.GetTinyIconGuardian);
		private Lazy<Bitmap> TinyIconRevenant { get; } = new Lazy<Bitmap>(Resources.GetTinyIconRevenant);
		private Lazy<Bitmap> TinyIconRanger { get; } = new Lazy<Bitmap>(Resources.GetTinyIconRanger);
		private Lazy<Bitmap> TinyIconThief { get; } = new Lazy<Bitmap>(Resources.GetTinyIconThief);
		private Lazy<Bitmap> TinyIconEngineer { get; } = new Lazy<Bitmap>(Resources.GetTinyIconEngineer);
		private Lazy<Bitmap> TinyIconNecromancer { get; } = new Lazy<Bitmap>(Resources.GetTinyIconNecromancer);
		private Lazy<Bitmap> TinyIconElementalist { get; } = new Lazy<Bitmap>(Resources.GetTinyIconElementalist);
		private Lazy<Bitmap> TinyIconMesmer { get; } = new Lazy<Bitmap>(Resources.GetTinyIconMesmer);
		private Lazy<Bitmap> TinyIconUnknownProfession { get; } = new Lazy<Bitmap>(Resources.GetTinyIconUnknown);

		// SPECIALIZATIONS
		// Warrior
		private Lazy<Bitmap> TinyIconBerserker { get; } = new Lazy<Bitmap>(Resources.GetTinyIconBerserker);
		private Lazy<Bitmap> TinyIconSpellbreaker { get; } = new Lazy<Bitmap>(Resources.GetTinyIconSpellbreaker);
		private Lazy<Bitmap> TinyIconBladesworn { get; } = new Lazy<Bitmap>(Resources.GetTinyIconBladesworn);
		private Lazy<Bitmap> TinyIconParagon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconParagon);
		// Guardian
		private Lazy<Bitmap> TinyIconDragonhunter { get; } = new Lazy<Bitmap>(Resources.GetTinyIconDragonhunter);
		private Lazy<Bitmap> TinyIconWillbender { get; } = new Lazy<Bitmap>(Resources.GetTinyIconWillbender);
		private Lazy<Bitmap> TinyIconFirebrand { get; } = new Lazy<Bitmap>(Resources.GetTinyIconFirebrand);
		private Lazy<Bitmap> TinyIconLuminary { get; } = new Lazy<Bitmap>(Resources.GetTinyIconLuminary);
		// Revenant
		private Lazy<Bitmap> TinyIconHerald { get; } = new Lazy<Bitmap>(Resources.GetTinyIconHerald);
		private Lazy<Bitmap> TinyIconRenegade { get; } = new Lazy<Bitmap>(Resources.GetTinyIconRenegade);
		private Lazy<Bitmap> TinyIconVindicator { get; } = new Lazy<Bitmap>(Resources.GetTinyIconVindicator);
		private Lazy<Bitmap> TinyIconConduit { get; } = new Lazy<Bitmap>(Resources.GetTinyIconConduit);
		// Ranger
		private Lazy<Bitmap> TinyIconDruid { get; } = new Lazy<Bitmap>(Resources.GetTinyIconDruid);
		private Lazy<Bitmap> TinyIconSoulbeast { get; } = new Lazy<Bitmap>(Resources.GetTinyIconSoulbeast);
		private Lazy<Bitmap> TinyIconUntamed { get; } = new Lazy<Bitmap>(Resources.GetTinyIconUntamed);
		private Lazy<Bitmap> TinyIconGaleshot { get; } = new Lazy<Bitmap>(Resources.GetTinyIconGaleshot);
		// Thief
		private Lazy<Bitmap> TinyIconDaredevil { get; } = new Lazy<Bitmap>(Resources.GetTinyIconDaredevil);
		private Lazy<Bitmap> TinyIconDeadeye { get; } = new Lazy<Bitmap>(Resources.GetTinyIconDeadeye);
		private Lazy<Bitmap> TinyIconSpecter { get; } = new Lazy<Bitmap>(Resources.GetTinyIconSpecter);
		private Lazy<Bitmap> TinyIconAntiquary { get; } = new Lazy<Bitmap>(Resources.GetTinyIconAntiquary);
		// Engineer
		private Lazy<Bitmap> TinyIconScrapper { get; } = new Lazy<Bitmap>(Resources.GetTinyIconScrapper);
		private Lazy<Bitmap> TinyIconHolosmith { get; } = new Lazy<Bitmap>(Resources.GetTinyIconHolosmith);
		private Lazy<Bitmap> TinyIconMechanist { get; } = new Lazy<Bitmap>(Resources.GetTinyIconMechanist);
		private Lazy<Bitmap> TinyIconAmalgam { get; } = new Lazy<Bitmap>(Resources.GetTinyIconAmalgam);
		// Necromancer
		private Lazy<Bitmap> TinyIconReaper { get; } = new Lazy<Bitmap>(Resources.GetTinyIconReaper);
		private Lazy<Bitmap> TinyIconScourge { get; } = new Lazy<Bitmap>(Resources.GetTinyIconScourge);
		private Lazy<Bitmap> TinyIconHarbinger { get; } = new Lazy<Bitmap>(Resources.GetTinyIconHarbinger);
		private Lazy<Bitmap> TinyIconRitualist { get; } = new Lazy<Bitmap>(Resources.GetTinyIconRitualist);
		// Elementalist
		private Lazy<Bitmap> TinyIconTempest { get; } = new Lazy<Bitmap>(Resources.GetTinyIconTempest);
		private Lazy<Bitmap> TinyIconWeaver { get; } = new Lazy<Bitmap>(Resources.GetTinyIconWeaver);
		private Lazy<Bitmap> TinyIconCatalyst { get; } = new Lazy<Bitmap>(Resources.GetTinyIconCatalyst);
		private Lazy<Bitmap> TinyIconEvoker { get; } = new Lazy<Bitmap>(Resources.GetTinyIconEvoker);
		// Mesmer
		private Lazy<Bitmap> TinyIconChronomancer { get; } = new Lazy<Bitmap>(Resources.GetTinyIconChronomancer);
		private Lazy<Bitmap> TinyIconMirage { get; } = new Lazy<Bitmap>(Resources.GetTinyIconMirage);
		private Lazy<Bitmap> TinyIconVirtuoso { get; } = new Lazy<Bitmap>(Resources.GetTinyIconVirtuoso);
		private Lazy<Bitmap> TinyIconTroubadour { get; } = new Lazy<Bitmap>(Resources.GetTinyIconTroubadour);

		// CATEGORIES
		private Lazy<Bitmap> TinyIconRaid { get; } = new Lazy<Bitmap>(Resources.GetTinyIconRaid);
		private Lazy<Bitmap> TinyIconFractals { get; } = new Lazy<Bitmap>(Resources.GetTinyIconFractals);
		private Lazy<Bitmap> TinyIconLog { get; } = new Lazy<Bitmap>(Resources.GetTinyIconGuildRegistrar);
		private Lazy<Bitmap> TinyIconCommander { get; } = new Lazy<Bitmap>(Resources.GetTinyIconCommander);
		private Lazy<Bitmap> TinyIconRaidEncounter { get; } = new Lazy<Bitmap>(Resources.GetTinyIconRaidEncounter);
		private Lazy<Bitmap> TinyIconTrainingArea { get; } = new Lazy<Bitmap>(Resources.GetTinyIconTrainingArea);
		private Lazy<Bitmap> TinyIconWorldVersusWorld { get; } = new Lazy<Bitmap>(Resources.GetTinyIconWorldVersusWorld);
		private Lazy<Bitmap> TinyIconUncategorized { get; } = new Lazy<Bitmap>(Resources.GetTinyIconUncategorized);
		private Lazy<Bitmap> TinyIconFestival { get; } = new Lazy<Bitmap>(Resources.GetTinyIconFestival);
		private Lazy<Bitmap> TinyIconIcebroodSaga { get; } = new Lazy<Bitmap>(Resources.GetTinyIconIcebroodSaga);
		private Lazy<Bitmap> TinyIconEndOfDragons { get; } = new Lazy<Bitmap>(Resources.GetTinyIconEndOfDragons);
		private Lazy<Bitmap> TinyIconSecretsOfTheObscure { get; } = new Lazy<Bitmap>(Resources.GetTinyIconSecretsOfTheObscure);
		private Lazy<Bitmap> TinyVisionsOfEternity { get; } = new Lazy<Bitmap>(Resources.GetTinyIconVisionsOfEternity);
		private Lazy<Bitmap> TinyIconInstance { get; } = new Lazy<Bitmap>(Resources.GetTinyIconInstance);

		// RAIDS
		private Lazy<Bitmap> GenericRaidWing { get; } = new Lazy<Bitmap>(Resources.GetGenericRaidWingIcon);

		// FRACTALS
		private Lazy<Bitmap> GenericFractalMap { get; } = new Lazy<Bitmap>(Resources.GetGenericFractalMapIcon);

		// RAID BOSSES
		// WING 1
		private Lazy<Bitmap> ValeGuardianIcon { get; } = new Lazy<Bitmap>(Resources.GetValeGuardianIcon);
		private Lazy<Bitmap> SpiritRaceIcon { get; } = new Lazy<Bitmap>(Resources.GetSpiritRaceIcon);
		private Lazy<Bitmap> GorsevalIcon { get; } = new Lazy<Bitmap>(Resources.GetGorsevalIcon);
		private Lazy<Bitmap> SabethaIcon { get; } = new Lazy<Bitmap>(Resources.GetSabethaIcon);

		// WING 2
		private Lazy<Bitmap> SlothasorIcon { get; } = new Lazy<Bitmap>(Resources.GetSlothasorIcon);
		private Lazy<Bitmap> BanditTrioIcon { get; } = new Lazy<Bitmap>(Resources.GetBanditTrioIcon);
		private Lazy<Bitmap> MatthiasIcon { get; } = new Lazy<Bitmap>(Resources.GetMatthiasIcon);

		// WING 3
		private Lazy<Bitmap> EscortIcon { get; } = new Lazy<Bitmap>(Resources.GetEscortIcon);
		private Lazy<Bitmap> KeepConstructIcon { get; } = new Lazy<Bitmap>(Resources.GetKeepConstructIcon);
		private Lazy<Bitmap> TwistedCastleIcon { get; } = new Lazy<Bitmap>(Resources.GetTwistedCastleIcon);
		private Lazy<Bitmap> XeraIcon { get; } = new Lazy<Bitmap>(Resources.GetXeraIcon);

		// WING 4
		private Lazy<Bitmap> CairnIcon { get; } = new Lazy<Bitmap>(Resources.GetCairnIcon);
		private Lazy<Bitmap> MursaatOverseerIcon { get; } = new Lazy<Bitmap>(Resources.GetMursaatOverseerIcon);
		private Lazy<Bitmap> SamarogIcon { get; } = new Lazy<Bitmap>(Resources.GetSamarogIcon);
		private Lazy<Bitmap> DeimosIcon { get; } = new Lazy<Bitmap>(Resources.GetDeimosIcon);

		// WING 5
		private Lazy<Bitmap> SoullessHorrorIcon { get; } = new Lazy<Bitmap>(Resources.GetDesminaIcon);
		private Lazy<Bitmap> RiverOfSoulsIcon { get; } = new Lazy<Bitmap>(Resources.GetRiverOfSoulsIcon);
		private Lazy<Bitmap> BrokenKingIcon { get; } = new Lazy<Bitmap>(Resources.GetBrokenKingIcon);
		private Lazy<Bitmap> EaterOfSoulsIcon { get; } = new Lazy<Bitmap>(Resources.GetEaterOfSoulsIcon);
		private Lazy<Bitmap> EyesIcon { get; } = new Lazy<Bitmap>(Resources.GetEyesIcon);
		private Lazy<Bitmap> DhuumIcon { get; } = new Lazy<Bitmap>(Resources.GetDhuumIcon);

		// WING 6
		private Lazy<Bitmap> ConjuredAmalgamateIcon { get; } = new Lazy<Bitmap>(Resources.GetConjuredAmalgamateIcon);
		private Lazy<Bitmap> TwinLargosIcon { get; } = new Lazy<Bitmap>(Resources.GetTwinLargosIcon);
		private Lazy<Bitmap> QadimIcon { get; } = new Lazy<Bitmap>(Resources.GetQadimIcon);

		// WING 7
		private Lazy<Bitmap> CardinalAdinaIcon { get; } = new Lazy<Bitmap>(Resources.GetCardinalAdinaIcon);
		private Lazy<Bitmap> CardinalSabirIcon { get; } = new Lazy<Bitmap>(Resources.GetCardinalSabirIcon);
		private Lazy<Bitmap> QadimThePeerlessIcon { get; } = new Lazy<Bitmap>(Resources.GetQadimThePeerlessIcon);

		// WING 8
		private Lazy<Bitmap> GreerIcon { get; } = new Lazy<Bitmap>(Resources.GetGreerIcon);
		private Lazy<Bitmap> DecimaIcon { get; } = new Lazy<Bitmap>(Resources.GetDecimaIcon);
		private Lazy<Bitmap> UraIcon { get; } = new Lazy<Bitmap>(Resources.GetUraIcon);

		// RAID ENCOUNTERS - ICEBROOD SAGA
		private Lazy<Bitmap> ShiverpeaksPassIcon { get; } = new Lazy<Bitmap>(Resources.GetShiverpeaksPassIcon);
		private Lazy<Bitmap> VoiceAndClawOfTheFallenIcon { get; } = new Lazy<Bitmap>(Resources.GetVoiceAndClawOfTheFallenIcon);
		private Lazy<Bitmap> FraenirOfJormagIcon { get; } = new Lazy<Bitmap>(Resources.GetFraenirOfJormagIcon);
		private Lazy<Bitmap> BoneskinnerIcon { get; } = new Lazy<Bitmap>(Resources.GetBoneskinnerIcon);
		private Lazy<Bitmap> WhisperOfJormagIcon { get; } = new Lazy<Bitmap>(Resources.GetWhisperOfJormagIcon);
		private Lazy<Bitmap> VariniaStormsounderIcon { get; } = new Lazy<Bitmap>(Resources.GetVariniaStormsounderIcon);

		// RAID ENCOUNTERS - END OF DRAGONS
		private Lazy<Bitmap> AetherbladeHideoutIcon { get; } = new Lazy<Bitmap>(Resources.GetAetherbladeHideoutIcon);
		private Lazy<Bitmap> XunlaiJadeJunkyardIcon { get; } = new Lazy<Bitmap>(Resources.GetXunlaiJadeJunkyardIcon);
		private Lazy<Bitmap> KainengOverlookIcon { get; } = new Lazy<Bitmap>(Resources.GetKainengOverlookIcon);
		private Lazy<Bitmap> HarvestTempleIcon { get; } = new Lazy<Bitmap>(Resources.GetHarvestTempleIcon);
		private Lazy<Bitmap> OldLionsCourtIcon { get; } = new Lazy<Bitmap>(Resources.GetOldLionsCourtIcon);

		// RAID ENCOUNTERS - SECRETS OF THE OBSCURE
		private Lazy<Bitmap> CosmicObservatoryIcon { get; } = new Lazy<Bitmap>(Resources.GetCosmicObservatoryIcon);
		private Lazy<Bitmap> TempleOfFebeIcon { get; } = new Lazy<Bitmap>(Resources.GetTempleOfFebeIcon);

		// RAID ENCOUNTERS - VISIONS OF ETERNITY
		private Lazy<Bitmap> GuardiansGladeIcon { get; } = new Lazy<Bitmap>(Resources.GetGuardiansGladeIcon);

		// FRACTALS
		private Lazy<Bitmap> MAMAIcon { get; } = new Lazy<Bitmap>(Resources.GetMAMAIcon);
		private Lazy<Bitmap> SiaxTheCorruptedIcon { get; } = new Lazy<Bitmap>(Resources.GetSiaxTheCorruptedIcon);
		private Lazy<Bitmap> EnsolyssOfTheEndlessTormentIcon { get; } = new Lazy<Bitmap>(Resources.GetEnsolyssOfTheEndlessTormentIcon);
		private Lazy<Bitmap> SkorvaldIcon { get; } = new Lazy<Bitmap>(Resources.GetSkorvaldIcon);
		private Lazy<Bitmap> ArtsariivIcon { get; } = new Lazy<Bitmap>(Resources.GetArtsariivIcon);
		private Lazy<Bitmap> ArkkIcon { get; } = new Lazy<Bitmap>(Resources.GetArkkIcon);
		private Lazy<Bitmap> AiKeeperOfThePeakIcon { get; } = new Lazy<Bitmap>(Resources.GetBothPhasesAiKeeperOfThePeakIcon);
		private Lazy<Bitmap> ElementalAiKeeperOfThePeakIcon { get; } = new Lazy<Bitmap>(Resources.GetElementalAiKeeperOfThePeakIcon);
		private Lazy<Bitmap> DarkAiKeeperOfThePeakIcon { get; } = new Lazy<Bitmap>(Resources.GetDarkAiKeeperOfThePeakIcon);
		private Lazy<Bitmap> BothPhasesAiKeeperOfThePeakIcon { get; } = new Lazy<Bitmap>(Resources.GetBothPhasesAiKeeperOfThePeakIcon);
		private Lazy<Bitmap> KanaxaiIcon { get; } = new Lazy<Bitmap>(Resources.GetKanaxaiIcon);
		private Lazy<Bitmap> EparchIcon { get; } = new Lazy<Bitmap>(Resources.GetEparchIcon);
		private Lazy<Bitmap> WhisperingShadowIcon { get; } = new Lazy<Bitmap>(Resources.GetWhisperingShadowIcon);

		// RAID ENCOUNTERS - FESTIVALS
		private Lazy<Bitmap> FreezieIcon { get; } = new Lazy<Bitmap>(Resources.GetFreezieIcon);

		// TRAINING AREA
		private Lazy<Bitmap> StandardKittyGolemIcon { get; } = new Lazy<Bitmap>(Resources.GetStandardKittyGolemIcon);
		private Lazy<Bitmap> MediumKittyGolemIcon { get; } = new Lazy<Bitmap>(Resources.GetMediumKittyGolemIcon);
		private Lazy<Bitmap> LargeKittyGolemIcon { get; } = new Lazy<Bitmap>(Resources.GetLargeKittyGolemIcon);
		private Lazy<Bitmap> MassiveKittyGolemIcon { get; } = new Lazy<Bitmap>(Resources.GetMassiveKittyGolemIcon);

		// WORLD VS WORLD
		private Lazy<Bitmap> EternalBattlegroundsIcon { get; } = new Lazy<Bitmap>(Resources.GetEternalBattlegroundsIcon);
		private Lazy<Bitmap> RedBorderlandsIcon { get; } = new Lazy<Bitmap>(Resources.GetRedBorderlandsIcon);
		private Lazy<Bitmap> BlueBorderlandsIcon { get; } = new Lazy<Bitmap>(Resources.GetBlueBorderlandsIcon);
		private Lazy<Bitmap> GreenBorderlandsIcon { get; } = new Lazy<Bitmap>(Resources.GetGreenBorderlandsIcon);
		private Lazy<Bitmap> ObsidianSanctumIcon { get; } = new Lazy<Bitmap>(Resources.GetObsidianSanctumIcon);
		private Lazy<Bitmap> EdgeOfTheMistsIcon { get; } = new Lazy<Bitmap>(Resources.GetEdgeOfTheMistsIcon);
		private Lazy<Bitmap> ArmisticeBastionIcon { get; } = new Lazy<Bitmap>(Resources.GetArmisticeBastionIcon);

		// INSTABILITIES
		private Lazy<Bitmap> AdrenalineRushIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconAdrenalineRush);
		private Lazy<Bitmap> AfflictedIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconAfflicted);
		private Lazy<Bitmap> BirdsIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconBirds);
		private Lazy<Bitmap> BoonOverloadIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconBoonOverload);
		private Lazy<Bitmap> FluxBombIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconFluxBomb);
		private Lazy<Bitmap> FractalVindicatorsIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconFractalVindicators);
		private Lazy<Bitmap> FrailtyIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconFrailty);
		private Lazy<Bitmap> HamstrungIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconHamstrung);
		private Lazy<Bitmap> LastLaughIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconLastLaugh);
		private Lazy<Bitmap> MistsConvergenceIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconMistsConvergence);
		private Lazy<Bitmap> NoPainNoGainIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconNoPainNoGain);
		private Lazy<Bitmap> OutflankedIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconOutflanked);
		private Lazy<Bitmap> SlipperySlopeIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconSlipperySlope);
		private Lazy<Bitmap> SocialAwkwardnessIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconSocialAwkwardness);
		private Lazy<Bitmap> StickTogetherIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconStickTogether);
		private Lazy<Bitmap> SugarRushIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconSugarRush);
		private Lazy<Bitmap> ToxicSicknessIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconToxicSickness);
		private Lazy<Bitmap> ToxicTrailIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconToxicTrail);
		private Lazy<Bitmap> VengeanceIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconVengeance);
		private Lazy<Bitmap> WeBleedFireIcon { get; } = new Lazy<Bitmap>(Resources.GetTinyIconWeBleedFire);

		// MISC
		private Lazy<Bitmap> CopyButtonEnabledIcon { get; } = new Lazy<Bitmap>(Resources.GetCopyButtonEnabledIcon);
		private Lazy<Bitmap> CopyButtonDisabledIcon { get; } = new Lazy<Bitmap>(Resources.GetCopyButtonDisabledIcon);

		// WEEKLY CLEARS
		private Lazy<Bitmap> GreenCheckIcon { get; } = new Lazy<Bitmap>(Resources.GetGreenCheckIcon);
		private Lazy<Bitmap> RedCrossIcon { get; } = new Lazy<Bitmap>(Resources.GetRedCrossIcon);
		private Lazy<Bitmap> GrayQuestionMarkIcon { get; } = new Lazy<Bitmap>(Resources.GetGrayQuestionMarkIcon);
		private Lazy<Bitmap> NotYetAvailableIcon { get; } = new Lazy<Bitmap>(Resources.GetNotYetAvailableIcon);
		private Lazy<Bitmap> WideIcebroodSagaIcon { get; } = new Lazy<Bitmap>(Resources.GetWideIcebroodSagaIcon);
		private Lazy<Bitmap> WideEndOfDragonsIcon { get; } = new Lazy<Bitmap>(Resources.GetWideEndOfDragonsIcon);
		private Lazy<Bitmap> WideSecretsOfTheObscureIcon { get; } = new Lazy<Bitmap>(Resources.GetWideSecretsOfTheObscureIcon);
		private Lazy<Bitmap> WideVisionsOfEternityIcon { get; } = new Lazy<Bitmap>(Resources.GetWideVisionsOfEternityIcon);
		private Lazy<Bitmap> WideRaidWing1Icon { get; } = new Lazy<Bitmap>(Resources.GetWideRaidWing1Icon);
		private Lazy<Bitmap> WideRaidWing2Icon { get; } = new Lazy<Bitmap>(Resources.GetWideRaidWing2Icon);
		private Lazy<Bitmap> WideRaidWing3Icon { get; } = new Lazy<Bitmap>(Resources.GetWideRaidWing3Icon);
		private Lazy<Bitmap> WideRaidWing4Icon { get; } = new Lazy<Bitmap>(Resources.GetWideRaidWing4Icon);
		private Lazy<Bitmap> WideRaidWing5Icon { get; } = new Lazy<Bitmap>(Resources.GetWideRaidWing5Icon);
		private Lazy<Bitmap> WideRaidWing6Icon { get; } = new Lazy<Bitmap>(Resources.GetWideRaidWing6Icon);
		private Lazy<Bitmap> WideRaidWing7Icon { get; } = new Lazy<Bitmap>(Resources.GetWideRaidWing7Icon);
		private Lazy<Bitmap> WideRaidWing8Icon { get; } = new Lazy<Bitmap>(Resources.GetWideRaidWing8Icon);

		public Bitmap GetTinyLogIcon() => TinyIconLog.Value;
		public Bitmap GetTinyFractalsIcon() => TinyIconFractals.Value;
		public Bitmap GetTinyRaidIcon() => TinyIconRaid.Value;
		public Bitmap GetTinyCommanderIcon() => TinyIconCommander.Value;
		public Bitmap GetTinyRaidEncounterIcon() => TinyIconRaidEncounter.Value;
		public Bitmap GetTinyTrainingAreaIcon() => TinyIconTrainingArea.Value;
		public Bitmap GetTinyWorldVersusWorldIcon() => TinyIconWorldVersusWorld.Value;
		public Bitmap GetTinyUncategorizedIcon() => TinyIconUncategorized.Value;
		public Bitmap GetTinyFestivalIcon() => TinyIconFestival.Value;
		public Bitmap GetTinyIcebroodSagaIcon() => TinyIconIcebroodSaga.Value;
		public Bitmap GetTinyEndOfDragonsIcon() => TinyIconEndOfDragons.Value;
		public Bitmap GetTinySecretsOfTheObscureIcon() => TinyIconSecretsOfTheObscure.Value;
		public Bitmap GetTinyVisionsOfEternityIcon() => TinyVisionsOfEternity.Value;
		public Bitmap GetTinyInstanceIcon() => TinyIconInstance.Value;
		public Bitmap GetCopyButtonEnabledImage() => CopyButtonEnabledIcon.Value;
		public Bitmap GetCopyButtonDisabledImage() => CopyButtonDisabledIcon.Value;
		public Bitmap GetGreenCheckIcon() => GreenCheckIcon.Value;
		public Bitmap GetRedCrossIcon() => RedCrossIcon.Value;
		public Bitmap GetGrayQuestionMarkIcon() => GrayQuestionMarkIcon.Value;
		public Bitmap GetNotYetAvailableIcon() => NotYetAvailableIcon.Value;
		public Bitmap GetWideIcebroodSagaIcon() => WideIcebroodSagaIcon.Value;
		public Bitmap GetWideEndOfDragonsIcon() => WideEndOfDragonsIcon.Value;
		public Bitmap GetWideSecretsOfTheObscureIcon() => WideSecretsOfTheObscureIcon.Value;
		public Bitmap GetWideVisionsOfEternityIcon() => WideVisionsOfEternityIcon.Value;
		public Bitmap GetWideRaidWing1Icon() => WideRaidWing1Icon.Value;
		public Bitmap GetWideRaidWing2Icon() => WideRaidWing2Icon.Value;
		public Bitmap GetWideRaidWing3Icon() => WideRaidWing3Icon.Value;
		public Bitmap GetWideRaidWing4Icon() => WideRaidWing4Icon.Value;
		public Bitmap GetWideRaidWing5Icon() => WideRaidWing5Icon.Value;
		public Bitmap GetWideRaidWing6Icon() => WideRaidWing6Icon.Value;
		public Bitmap GetWideRaidWing7Icon() => WideRaidWing7Icon.Value;
		public Bitmap GetWideRaidWing8Icon() => WideRaidWing8Icon.Value;

		public Bitmap GetTinyProfessionIcon(Profession profession)
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

		public Bitmap GetTinyProfessionIcon(EliteSpecialization specialization)
		{
			return specialization switch
			{
				// Warrior
				EliteSpecialization.Berserker => TinyIconBerserker.Value,
				EliteSpecialization.Spellbreaker => TinyIconSpellbreaker.Value,
				EliteSpecialization.Bladesworn => TinyIconBladesworn.Value,
				EliteSpecialization.Paragon => TinyIconParagon.Value,
				// Guardian
				EliteSpecialization.Dragonhunter => TinyIconDragonhunter.Value,
				EliteSpecialization.Firebrand => TinyIconFirebrand.Value,
				EliteSpecialization.Willbender => TinyIconWillbender.Value,
				EliteSpecialization.Luminary => TinyIconLuminary.Value,
				// Revenant
				EliteSpecialization.Herald => TinyIconHerald.Value,
				EliteSpecialization.Renegade => TinyIconRenegade.Value,
				EliteSpecialization.Vindicator => TinyIconVindicator.Value,
				EliteSpecialization.Conduit => TinyIconConduit.Value,
				// Ranger
				EliteSpecialization.Druid => TinyIconDruid.Value,
				EliteSpecialization.Soulbeast => TinyIconSoulbeast.Value,
				EliteSpecialization.Untamed => TinyIconUntamed.Value,
				EliteSpecialization.Galeshot => TinyIconGaleshot.Value,
				// Thief
				EliteSpecialization.Daredevil => TinyIconDaredevil.Value,
				EliteSpecialization.Deadeye => TinyIconDeadeye.Value,
				EliteSpecialization.Specter => TinyIconSpecter.Value,
				EliteSpecialization.Antiquary => TinyIconAntiquary.Value,
				// Engineer
				EliteSpecialization.Scrapper => TinyIconScrapper.Value,
				EliteSpecialization.Holosmith => TinyIconHolosmith.Value,
				EliteSpecialization.Mechanist => TinyIconMechanist.Value,
				EliteSpecialization.Amalgam => TinyIconAmalgam.Value,
				// Necromancer
				EliteSpecialization.Reaper => TinyIconReaper.Value,
				EliteSpecialization.Scourge => TinyIconScourge.Value,
				EliteSpecialization.Harbinger => TinyIconHarbinger.Value,
				EliteSpecialization.Ritualist => TinyIconRitualist.Value,
				// Elementalist
				EliteSpecialization.Tempest => TinyIconTempest.Value,
				EliteSpecialization.Weaver => TinyIconWeaver.Value,
				EliteSpecialization.Catalyst => TinyIconCatalyst.Value,
				EliteSpecialization.Evoker => TinyIconEvoker.Value,
				// Mesmer
				EliteSpecialization.Chronomancer => TinyIconChronomancer.Value,
				EliteSpecialization.Mirage => TinyIconMirage.Value,
				EliteSpecialization.Virtuoso => TinyIconVirtuoso.Value,
				EliteSpecialization.Troubadour => TinyIconTroubadour.Value,
				_ => throw new ArgumentOutOfRangeException(nameof(specialization)),
			};
		}

		public Bitmap GetTinyProfessionIcon(LogPlayer player)
		{
			if (player.EliteSpecialization == EliteSpecialization.None)
			{
				return GetTinyProfessionIcon(player.Profession);
			}

			return GetTinyProfessionIcon(player.EliteSpecialization);
		}

		public Bitmap? GetTinyEncounterIcon(Encounter encounter)
		{
			return encounter switch
			{
				// RAIDS
				// W1
				Encounter.ValeGuardian => ValeGuardianIcon.Value,
				Encounter.SpiritRace => SpiritRaceIcon.Value,
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
				Encounter.ConjuredAmalgamate => ConjuredAmalgamateIcon.Value,
				Encounter.TwinLargos => TwinLargosIcon.Value,
				Encounter.Qadim => QadimIcon.Value,
				// W7
				Encounter.Adina => CardinalAdinaIcon.Value,
				Encounter.Sabir => CardinalSabirIcon.Value,
				Encounter.QadimThePeerless => QadimThePeerlessIcon.Value,
				// W8
				Encounter.Greer => GreerIcon.Value,
				Encounter.Decima => DecimaIcon.Value,
				Encounter.Ura => UraIcon.Value,
				// RAID ENCOUNTERS - ICEBROOD SAGA
				Encounter.ShiverpeaksPass => ShiverpeaksPassIcon.Value,
				Encounter.VoiceAndClawOfTheFallen => VoiceAndClawOfTheFallenIcon.Value,
				Encounter.FraenirOfJormag => FraenirOfJormagIcon.Value,
				Encounter.Boneskinner => BoneskinnerIcon.Value,
				Encounter.WhisperOfJormag => WhisperOfJormagIcon.Value,
				Encounter.VariniaStormsounder => VariniaStormsounderIcon.Value,
				// RAID ENCOUNTERS - END OF DRAGONS
				Encounter.AetherbladeHideout => AetherbladeHideoutIcon.Value,
				Encounter.XunlaiJadeJunkyard => XunlaiJadeJunkyardIcon.Value,
				Encounter.KainengOverlook => KainengOverlookIcon.Value,
				Encounter.HarvestTemple => HarvestTempleIcon.Value,
				Encounter.OldLionsCourt => OldLionsCourtIcon.Value,
				// RAID ENCOUNTERS - SECRETS OF THE OBSCURE
				Encounter.CosmicObservatory => CosmicObservatoryIcon.Value,
				Encounter.TempleOfFebe => TempleOfFebeIcon.Value,
				// RAID ENCOUNTERS - VISIONS OF ETERNITY
				Encounter.GuardiansGlade => GuardiansGladeIcon.Value,
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
				Encounter.Kanaxai => KanaxaiIcon.Value,
				Encounter.Eparch => EparchIcon.Value,
				Encounter.WhisperingShadow => WhisperingShadowIcon.Value,
				// RAID ENCOUNTERS - FESTIVALS
				Encounter.Freezie => FreezieIcon.Value,
				// TRAINING AREA
				Encounter.StandardKittyGolem => StandardKittyGolemIcon.Value,
				Encounter.MediumKittyGolem => MediumKittyGolemIcon.Value,
				Encounter.LargeKittyGolem => LargeKittyGolemIcon.Value,
				Encounter.MassiveKittyGolem => MassiveKittyGolemIcon.Value,
				_ => null
			};
		}

		public Bitmap GetRaidWingIcon()
		{
			return GenericRaidWing.Value;
		}

		public Bitmap GetFractalMapIcon()
		{
			return GenericFractalMap.Value;
		}

		public Bitmap? GetWvWMapIcon(int? mapId)
		{
			return mapId switch
			{
				MapIds.EternalBattlegrounds => EternalBattlegroundsIcon.Value,
				MapIds.RedDesertBorderlands => RedBorderlandsIcon.Value,
				MapIds.BlueAlpineBorderlands => BlueBorderlandsIcon.Value,
				MapIds.GreenAlpineBorderlands => GreenBorderlandsIcon.Value,
				MapIds.ObsidianSanctum => ObsidianSanctumIcon.Value,
				MapIds.EdgeOfTheMists => EdgeOfTheMistsIcon.Value,
				MapIds.ArmisticeBastion => ArmisticeBastionIcon.Value,
				_ => null
			};
		}

		public Bitmap GetMistlockInstabilityIcon(MistlockInstability instability)
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
