using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace GW2Scratch.ArcdpsLogManager.Avalonia
{
	/// <summary>
	/// Loads embedded image assets as Avalonia <see cref="Bitmap"/>s. This is the Avalonia
	/// counterpart of the Eto <c>GW2Scratch.ArcdpsLogManager.Resources</c> class, which returned
	/// <c>Eto.Drawing.Image</c>. The PNGs are the same files (linked from the Eto project during
	/// the migration, see the csproj), exposed here as <c>avares://</c> assets.
	/// </summary>
	public static class Resources
	{
		private const string BaseUri = "avares://GW2Scratch.ArcdpsLogManager.Avalonia/Assets/Images/";

		private static Bitmap GetImage(string dottedName)
		{
			// The Eto resource names use '.' as the path separator (e.g.
			// "Tango.Warrior_tango_icon_20px.png"). Convert everything except the trailing
			// extension to a real path segment so it resolves as an avares:// asset.
			int lastDot = dottedName.LastIndexOf('.');
			string path = dottedName.Substring(0, lastDot).Replace('.', '/') + dottedName.Substring(lastDot);
			var uri = new Uri(BaseUri + path);
			using var stream = AssetLoader.Open(uri);
			return new Bitmap(stream);
		}

		public static WindowIcon GetProgramIcon()
		{
			var uri = new Uri(BaseUri + "program_icon.ico");
			using var stream = AssetLoader.Open(uri);
			return new WindowIcon(stream);
		}

		public static Bitmap GetTinyIconUnknown() => GetImage("unknown_20px.png");

		// PROFESSIONS
		public static Bitmap GetTinyIconWarrior() => GetImage("Tango.Warrior_tango_icon_20px.png");
		public static Bitmap GetTinyIconGuardian() => GetImage("Tango.Guardian_tango_icon_20px.png");
		public static Bitmap GetTinyIconRevenant() => GetImage("Tango.Revenant_tango_icon_20px.png");
		public static Bitmap GetTinyIconRanger() => GetImage("Tango.Ranger_tango_icon_20px.png");
		public static Bitmap GetTinyIconThief() => GetImage("Tango.Thief_tango_icon_20px.png");
		public static Bitmap GetTinyIconEngineer() => GetImage("Tango.Engineer_tango_icon_20px.png");
		public static Bitmap GetTinyIconNecromancer() => GetImage("Tango.Necromancer_tango_icon_20px.png");
		public static Bitmap GetTinyIconElementalist() => GetImage("Tango.Elementalist_tango_icon_20px.png");
		public static Bitmap GetTinyIconMesmer() => GetImage("Tango.Mesmer_tango_icon_20px.png");

		// SPECIALIZATIONS
		// Warrior
		public static Bitmap GetTinyIconBerserker() => GetImage("Tango.Berserker_tango_icon_20px.png");
		public static Bitmap GetTinyIconSpellbreaker() => GetImage("Tango.Spellbreaker_tango_icon_20px.png");
		public static Bitmap GetTinyIconBladesworn() => GetImage("Tango.Bladesworn_tango_icon_20px.png");
		public static Bitmap GetTinyIconParagon() => GetImage("Tango.Paragon_tango_icon_20px.png");
		// Guardian
		public static Bitmap GetTinyIconDragonhunter() => GetImage("Tango.Dragonhunter_tango_icon_20px.png");
		public static Bitmap GetTinyIconFirebrand() => GetImage("Tango.Firebrand_tango_icon_20px.png");
		public static Bitmap GetTinyIconWillbender() => GetImage("Tango.Willbender_tango_icon_20px.png");
		public static Bitmap GetTinyIconLuminary() => GetImage("Tango.Luminary_tango_icon_20px.png");
		// Revenant
		public static Bitmap GetTinyIconHerald() => GetImage("Tango.Herald_tango_icon_20px.png");
		public static Bitmap GetTinyIconRenegade() => GetImage("Tango.Renegade_tango_icon_20px.png");
		public static Bitmap GetTinyIconVindicator() => GetImage("Tango.Vindicator_tango_icon_20px.png");
		public static Bitmap GetTinyIconConduit() => GetImage("Tango.Conduit_tango_icon_20px.png");
		// Ranger
		public static Bitmap GetTinyIconDruid() => GetImage("Tango.Druid_tango_icon_20px.png");
		public static Bitmap GetTinyIconSoulbeast() => GetImage("Tango.Soulbeast_tango_icon_20px.png");
		public static Bitmap GetTinyIconUntamed() => GetImage("Tango.Untamed_tango_icon_20px.png");
		public static Bitmap GetTinyIconGaleshot() => GetImage("Tango.Galeshot_tango_icon_20px.png");
		// Thief
		public static Bitmap GetTinyIconDaredevil() => GetImage("Tango.Daredevil_tango_icon_20px.png");
		public static Bitmap GetTinyIconDeadeye() => GetImage("Tango.Deadeye_tango_icon_20px.png");
		public static Bitmap GetTinyIconSpecter() => GetImage("Tango.Specter_tango_icon_20px.png");
		public static Bitmap GetTinyIconAntiquary() => GetImage("Tango.Antiquary_tango_icon_20px.png");
		// Engineer
		public static Bitmap GetTinyIconScrapper() => GetImage("Tango.Scrapper_tango_icon_20px.png");
		public static Bitmap GetTinyIconHolosmith() => GetImage("Tango.Holosmith_tango_icon_20px.png");
		public static Bitmap GetTinyIconMechanist() => GetImage("Tango.Mechanist_tango_icon_20px.png");
		public static Bitmap GetTinyIconAmalgam() => GetImage("Tango.Amalgam_tango_icon_20px.png");
		// Necromancer
		public static Bitmap GetTinyIconReaper() => GetImage("Tango.Reaper_tango_icon_20px.png");
		public static Bitmap GetTinyIconScourge() => GetImage("Tango.Scourge_tango_icon_20px.png");
		public static Bitmap GetTinyIconHarbinger() => GetImage("Tango.Harbinger_tango_icon_20px.png");
		public static Bitmap GetTinyIconRitualist() => GetImage("Tango.Ritualist_tango_icon_20px.png");
		// Elementalist
		public static Bitmap GetTinyIconTempest() => GetImage("Tango.Tempest_tango_icon_20px.png");
		public static Bitmap GetTinyIconWeaver() => GetImage("Tango.Weaver_tango_icon_20px.png");
		public static Bitmap GetTinyIconCatalyst() => GetImage("Tango.Catalyst_tango_icon_20px.png");
		public static Bitmap GetTinyIconEvoker() => GetImage("Tango.Evoker_tango_icon_20px.png");
		// Mesmer
		public static Bitmap GetTinyIconChronomancer() => GetImage("Tango.Chronomancer_tango_icon_20px.png");
		public static Bitmap GetTinyIconMirage() => GetImage("Tango.Mirage_tango_icon_20px.png");
		public static Bitmap GetTinyIconVirtuoso() => GetImage("Tango.Virtuoso_tango_icon_20px.png");
		public static Bitmap GetTinyIconTroubadour() => GetImage("Tango.Troubadour_tango_icon_20px.png");

		// CATEGORIES
		public static Bitmap GetTinyIconRaid() => GetImage("ArenaNet.raid_icon_32px.png");
		public static Bitmap GetTinyIconFractals() => GetImage("ArenaNet.fractals_icon_32px.png");
		public static Bitmap GetTinyIconGuildRegistrar() => GetImage("ArenaNet.guild_registrar_icon_26px.png");
		public static Bitmap GetTinyIconCommander() => GetImage("ArenaNet.commander_tag_red_32px.png");
		public static Bitmap GetTinyIconRaidEncounter() => GetImage("ArenaNet.raid_encounter_icon_32px.png");
		public static Bitmap GetTinyIconTrainingArea() => GetImage("ArenaNet.training_area_32px.png");
		public static Bitmap GetTinyIconWorldVersusWorld() => GetImage("ArenaNet.world_vs_world_32px.png");
		public static Bitmap GetTinyIconUncategorized() => GetImage("ArenaNet.uncategorized_32px.png");
		public static Bitmap GetTinyIconFestival() => GetImage("ArenaNet.festivals_32px.png");
		public static Bitmap GetTinyIconIcebroodSaga() => GetImage("ArenaNet.icebrood_saga_32px.png");
		public static Bitmap GetTinyIconEndOfDragons() => GetImage("ArenaNet.end_of_dragons_32px.png");
		public static Bitmap GetTinyIconSecretsOfTheObscure() => GetImage("ArenaNet.secrets_of_the_obscure_32px.png");
		public static Bitmap GetTinyIconVisionsOfEternity() => GetImage("ArenaNet.visions_of_eternity_32px.png");
		public static Bitmap GetTinyIconInstance() => GetImage("ArenaNet.storyline_32px.png");

		// FRACTAL INSTABILITIES
		public static Bitmap GetInstabilityImage(string iconName)
		{
			return GetImage($"ArenaNet.fractal_instabilities.{iconName}.png");
		}

		public static Bitmap GetTinyIconAdrenalineRush() => GetInstabilityImage("adrenaline_rush_32px");
		public static Bitmap GetTinyIconAfflicted() => GetInstabilityImage("afflicted_32px");
		public static Bitmap GetTinyIconBirds() => GetInstabilityImage("birds_32px");
		public static Bitmap GetTinyIconBoonOverload() => GetInstabilityImage("boon_overload_32px");
		public static Bitmap GetTinyIconFluxBomb() => GetInstabilityImage("flux_bomb_32px");
		public static Bitmap GetTinyIconFractalVindicators() => GetInstabilityImage("fractal_vindicators_32px");
		public static Bitmap GetTinyIconFrailty() => GetInstabilityImage("frailty_32px");
		public static Bitmap GetTinyIconHamstrung() => GetInstabilityImage("hamstrung_32px");
		public static Bitmap GetTinyIconLastLaugh() => GetInstabilityImage("last_laugh_32px");
		public static Bitmap GetTinyIconMistsConvergence() => GetInstabilityImage("mists_convergence_32px");
		public static Bitmap GetTinyIconNoPainNoGain() => GetInstabilityImage("no_pain_no_gain_32px");
		public static Bitmap GetTinyIconOutflanked() => GetInstabilityImage("outflanked_32px");
		public static Bitmap GetTinyIconSlipperySlope() => GetInstabilityImage("slippery_slope_32px");
		public static Bitmap GetTinyIconSocialAwkwardness() => GetInstabilityImage("social_awkwardness_32px");
		public static Bitmap GetTinyIconStickTogether() => GetInstabilityImage("stick_together_32px");
		public static Bitmap GetTinyIconSugarRush() => GetInstabilityImage("sugar_rush_32px");
		public static Bitmap GetTinyIconToxicSickness() => GetInstabilityImage("toxic_sickness_32px");
		public static Bitmap GetTinyIconToxicTrail() => GetInstabilityImage("toxic_trail_32px");
		public static Bitmap GetTinyIconVengeance() => GetInstabilityImage("vengeance_32px");
		public static Bitmap GetTinyIconWeBleedFire() => GetInstabilityImage("we_bleed_fire_32px");

		// RAID BOSSES
		public static Bitmap GetGenericRaidWingIcon() => GetImage("ArenaNet.raid_wing_32px.png");

		// WING 1
		public static Bitmap GetValeGuardianIcon() => GetEncounterImage("Mini_Vale_Guardian");
		public static Bitmap GetSpiritRaceIcon() => GetEncounterImage("Spirit_Thread");
		public static Bitmap GetGorsevalIcon() => GetEncounterImage("Mini_Gorseval_the_Multifarious");
		public static Bitmap GetSabethaIcon() => GetEncounterImage("Mini_Sabetha");

		// WING 2
		public static Bitmap GetSlothasorIcon() => GetEncounterImage("Mini_Slothasor");
		public static Bitmap GetBanditTrioIcon() => GetEncounterImage("Mini_Narella");
		public static Bitmap GetMatthiasIcon() => GetEncounterImage("Mini_Matthias_Abomination");

		// WING 3
		public static Bitmap GetEscortIcon() => GetEncounterImage("Mini_McLeod_the_Silent");
		public static Bitmap GetKeepConstructIcon() => GetEncounterImage("Mini_Keep_Construct");
		public static Bitmap GetTwistedCastleIcon() => GetEncounterImage("Twisted_Castle");
		public static Bitmap GetXeraIcon() => GetEncounterImage("Mini_Xera");

		// WING 4
		public static Bitmap GetCairnIcon() => GetEncounterImage("Mini_Cairn_the_Indomitable");
		public static Bitmap GetMursaatOverseerIcon() => GetEncounterImage("Mini_Mursaat_Overseer");
		public static Bitmap GetSamarogIcon() => GetEncounterImage("Mini_Samarog");
		public static Bitmap GetDeimosIcon() => GetEncounterImage("Deimos");

		// WING 5
		public static Bitmap GetDesminaIcon() => GetEncounterImage("Mini_Desmina");
		public static Bitmap GetRiverOfSoulsIcon() => GetEncounterImage("River_Of_Souls");
		public static Bitmap GetBrokenKingIcon() => GetEncounterImage("Mini_Broken_King");
		public static Bitmap GetEaterOfSoulsIcon() => GetEncounterImage("Eater_Of_Souls");
		public static Bitmap GetEyesIcon() => GetEncounterImage("Eyes_Of_Fate_And_Judgment");
		public static Bitmap GetDhuumIcon() => GetEncounterImage("Mini_Dhuum");

		// WING 6
		public static Bitmap GetConjuredAmalgamateIcon() => GetEncounterImage("Conjured_Amalgamate");
		public static Bitmap GetTwinLargosIcon() => GetEncounterImage("Mini_Nikare");
		public static Bitmap GetQadimIcon() => GetEncounterImage("Mini_Qadim");

		// WING 7
		public static Bitmap GetCardinalAdinaIcon() => GetEncounterImage("Mini_Earth_Djinn");
		public static Bitmap GetCardinalSabirIcon() => GetEncounterImage("Mini_Air_Djinn");
		public static Bitmap GetQadimThePeerlessIcon() => GetEncounterImage("Mini_Qadim_the_Peerless");

		// WING 8
		public static Bitmap GetGreerIcon() => GetEncounterImage("Greer");
		public static Bitmap GetDecimaIcon() => GetEncounterImage("Decima");
		public static Bitmap GetUraIcon() => GetEncounterImage("Ura");

		// RAID ENCOUNTERS - ICEBROOD SAGA
		public static Bitmap GetShiverpeaksPassIcon() => GetEncounterImage("Mini_Icebrood_Construct");
		public static Bitmap GetVoiceAndClawOfTheFallenIcon() => GetEncounterImage("Mini_Cloudseeker");
		public static Bitmap GetFraenirOfJormagIcon() => GetEncounterImage("Mini_Icebrood_Construct");
		public static Bitmap GetBoneskinnerIcon() => GetEncounterImage("Mini_Boneskinner");
		public static Bitmap GetWhisperOfJormagIcon() => GetEncounterImage("Mini_Whisper_of_Jormag");
		public static Bitmap GetVariniaStormsounderIcon() => GetEncounterImage("Mini_Varinia_Stormsounder");

		// RAID ENCOUNTERS - END OF DRAGONS
		public static Bitmap GetAetherbladeHideoutIcon() => GetEncounterImage("Aetherblade_Hideout");
		public static Bitmap GetXunlaiJadeJunkyardIcon() => GetEncounterImage("Xunlai_Jade_Junkyard");
		public static Bitmap GetKainengOverlookIcon() => GetEncounterImage("Kaineng_Overlook");
		public static Bitmap GetHarvestTempleIcon() => GetEncounterImage("Harvest_Temple");
		public static Bitmap GetOldLionsCourtIcon() => GetEncounterImage("Mini_Vermillion_Assault_Knight");

		// RAID ENCOUNTERS - SECRETS OF THE OBSCURE
		public static Bitmap GetCosmicObservatoryIcon() => GetEncounterImage("Mini_Dagda");
		public static Bitmap GetTempleOfFebeIcon() => GetEncounterImage("Mini_Cerus");

		// RAID ENCOUNTERS - VISIONS OF ETERNITY
		public static Bitmap GetGuardiansGladeIcon() => GetEncounterImage("Kela"); // TODO Update image

		// FRACTALS
		public static Bitmap GetGenericFractalMapIcon() => GetImage("ArenaNet.fractal_map_32px.png");
		public static Bitmap GetMAMAIcon() => GetEncounterImage("Mini_MAMA");
		public static Bitmap GetSiaxTheCorruptedIcon() => GetEncounterImage("Mini_Toxic_Warlock");
		public static Bitmap GetEnsolyssOfTheEndlessTormentIcon() => GetEncounterImage("Mini_Ensolyss");
		public static Bitmap GetSkorvaldIcon() => GetEncounterImage("Skorvald");
		public static Bitmap GetArtsariivIcon() => GetEncounterImage("Artsariiv");
		public static Bitmap GetArkkIcon() => GetEncounterImage("Arkk");
		public static Bitmap GetElementalAiKeeperOfThePeakIcon() => GetEncounterImage("Elemental_Ai");
		public static Bitmap GetDarkAiKeeperOfThePeakIcon() => GetEncounterImage("Dark_Ai");
		public static Bitmap GetBothPhasesAiKeeperOfThePeakIcon() => GetEncounterImage("Both_Phases_Ai");
		public static Bitmap GetKanaxaiIcon() => GetEncounterImage("Mini_Kanaxai");
		public static Bitmap GetEparchIcon() => GetEncounterImage("Mini_Eparch");
		public static Bitmap GetWhisperingShadowIcon() => GetEncounterImage("Whispering_Shadow");

		// RAID ENCOUNTERS - FESTIVALS
		public static Bitmap GetFreezieIcon() => GetEncounterImage("Mini_Freezie");

		// TRAINING AREA
		public static Bitmap GetStandardKittyGolemIcon() => GetEncounterImage("Mini_Professor_Mew");
		public static Bitmap GetMediumKittyGolemIcon() => GetEncounterImage("Mini_Snuggles");
		public static Bitmap GetLargeKittyGolemIcon() => GetEncounterImage("Mini_Baron_von_Scrufflebutt");
		public static Bitmap GetMassiveKittyGolemIcon() => GetEncounterImage("Mini_Mister_Mittens");

		// WORLD VS WORLD
		public static Bitmap GetEternalBattlegroundsIcon() => GetEncounterImage("Commander_tag_yellow");
		public static Bitmap GetRedBorderlandsIcon() => GetEncounterImage("Commander_tag_red");
		public static Bitmap GetBlueBorderlandsIcon() => GetEncounterImage("Commander_tag_blue");
		public static Bitmap GetGreenBorderlandsIcon() => GetEncounterImage("Commander_tag_green");
		public static Bitmap GetObsidianSanctumIcon() => GetEncounterImage("Commander_tag_purple");
		public static Bitmap GetEdgeOfTheMistsIcon() => GetEncounterImage("Commander_tag_white");
		public static Bitmap GetArmisticeBastionIcon() => GetEncounterImage("Armistice_Bastion_Pass");

		private static Bitmap GetEncounterImage(string iconName)
		{
			return GetImage($"ArenaNet.Bosses.{iconName}.png");
		}

		// MISC
		public static Bitmap GetCopyButtonEnabledIcon() => GetImage($"Misc.Copy_Black_36x36.png");
		public static Bitmap GetCopyButtonDisabledIcon() => GetImage($"Misc.Copy_Grey_36x36.png");

		// WEEKLY CLEARS
		public static Bitmap GetGreenCheckIcon() => GetImage($"Misc.green_check.png");
		public static Bitmap GetRedCrossIcon() => GetImage($"Misc.red_cross.png");
		public static Bitmap GetGrayQuestionMarkIcon() => GetImage($"Misc.gray_question_mark.png");
		public static Bitmap GetNotYetAvailableIcon() => GetImage($"Misc.not_yet_available.png");
		public static Bitmap GetWideIcebroodSagaIcon() => GetImage("ArenaNet.icebrood_saga_96px_64px.png");
		public static Bitmap GetWideEndOfDragonsIcon() => GetImage("ArenaNet.end_of_dragons_96px_64px.png");
		public static Bitmap GetWideSecretsOfTheObscureIcon() => GetImage("ArenaNet.secrets_of_the_obscure_96px_64px.png");
		public static Bitmap GetWideVisionsOfEternityIcon() => GetImage("ArenaNet.visions_of_eternity_96px_64px.png");
		public static Bitmap GetWideRaidWing1Icon() => GetImage("ArenaNet.raid_wing1_96px_64px.png");
		public static Bitmap GetWideRaidWing2Icon() => GetImage("ArenaNet.raid_wing2_96px_64px.png");
		public static Bitmap GetWideRaidWing3Icon() => GetImage("ArenaNet.raid_wing3_96px_64px.png");
		public static Bitmap GetWideRaidWing4Icon() => GetImage("ArenaNet.raid_wing4_96px_64px.png");
		public static Bitmap GetWideRaidWing5Icon() => GetImage("ArenaNet.raid_wing5_96px_64px.png");
		public static Bitmap GetWideRaidWing6Icon() => GetImage("ArenaNet.raid_wing6_96px_64px.png");
		public static Bitmap GetWideRaidWing7Icon() => GetImage("ArenaNet.raid_wing7_96px_64px.png");
		public static Bitmap GetWideRaidWing8Icon() => GetImage("ArenaNet.raid_wing8_96px_64px.png");
	}
}
