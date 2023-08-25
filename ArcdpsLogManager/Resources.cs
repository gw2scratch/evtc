using System;
using System.IO;
using System.Reflection;
using Eto.Drawing;

namespace GW2Scratch.ArcdpsLogManager
{
	public static class Resources
	{
		private static string GetResourceName(string imageName)
		{
			return $"GW2Scratch.ArcdpsLogManager.Images.{imageName}";
		}

		private static Bitmap GetImage(string imageName)
		{
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetResourceName(imageName));
			return new Bitmap(stream);
		}

		public static Icon GetProgramIcon()
		{
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetResourceName("program_icon.ico"));
			return new Icon(stream);
		}

		public static Image GetTinyIconUnknown() => GetImage("unknown_20px.png");

		// PROFESSIONS
		public static Image GetTinyIconWarrior() => GetImage("Tango.Warrior_tango_icon_20px.png");
		public static Image GetTinyIconGuardian() => GetImage("Tango.Guardian_tango_icon_20px.png");
		public static Image GetTinyIconRevenant() => GetImage("Tango.Revenant_tango_icon_20px.png");
		public static Image GetTinyIconRanger() => GetImage("Tango.Ranger_tango_icon_20px.png");
		public static Image GetTinyIconThief() => GetImage("Tango.Thief_tango_icon_20px.png");
		public static Image GetTinyIconEngineer() => GetImage("Tango.Engineer_tango_icon_20px.png");
		public static Image GetTinyIconNecromancer() => GetImage("Tango.Necromancer_tango_icon_20px.png");
		public static Image GetTinyIconElementalist() => GetImage("Tango.Elementalist_tango_icon_20px.png");
		public static Image GetTinyIconMesmer() => GetImage("Tango.Mesmer_tango_icon_20px.png");

		// SPECIALIZATIONS
		public static Image GetTinyIconBerserker() => GetImage("Tango.Berserker_tango_icon_20px.png");
		public static Image GetTinyIconSpellbreaker() => GetImage("Tango.Spellbreaker_tango_icon_20px.png");
		public static Image GetTinyIconBladesworn() => GetImage("Tango.Bladesworn_tango_icon_20px.png");
		public static Image GetTinyIconDragonhunter() => GetImage("Tango.Dragonhunter_tango_icon_20px.png");
		public static Image GetTinyIconFirebrand() => GetImage("Tango.Firebrand_tango_icon_20px.png");
		public static Image GetTinyIconWillbender() => GetImage("Tango.Willbender_tango_icon_20px.png");
		public static Image GetTinyIconHerald() => GetImage("Tango.Herald_tango_icon_20px.png");
		public static Image GetTinyIconRenegade() => GetImage("Tango.Renegade_tango_icon_20px.png");
		public static Image GetTinyIconVindicator() => GetImage("Tango.Vindicator_tango_icon_20px.png");
		public static Image GetTinyIconDruid() => GetImage("Tango.Druid_tango_icon_20px.png");
		public static Image GetTinyIconSoulbeast() => GetImage("Tango.Soulbeast_tango_icon_20px.png");
		public static Image GetTinyIconUntamed() => GetImage("Tango.Untamed_tango_icon_20px.png");
		public static Image GetTinyIconDaredevil() => GetImage("Tango.Daredevil_tango_icon_20px.png");
		public static Image GetTinyIconDeadeye() => GetImage("Tango.Deadeye_tango_icon_20px.png");
		public static Image GetTinyIconSpecter() => GetImage("Tango.Specter_tango_icon_20px.png");
		public static Image GetTinyIconScrapper() => GetImage("Tango.Scrapper_tango_icon_20px.png");
		public static Image GetTinyIconHolosmith() => GetImage("Tango.Holosmith_tango_icon_20px.png");
		public static Image GetTinyIconMechanist() => GetImage("Tango.Mechanist_tango_icon_20px.png");
		public static Image GetTinyIconReaper() => GetImage("Tango.Reaper_tango_icon_20px.png");
		public static Image GetTinyIconScourge() => GetImage("Tango.Scourge_tango_icon_20px.png");
		public static Image GetTinyIconHarbinger() => GetImage("Tango.Harbinger_tango_icon_20px.png");
		public static Image GetTinyIconTempest() => GetImage("Tango.Tempest_tango_icon_20px.png");
		public static Image GetTinyIconWeaver() => GetImage("Tango.Weaver_tango_icon_20px.png");
		public static Image GetTinyIconCatalyst() => GetImage("Tango.Catalyst_tango_icon_20px.png");
		public static Image GetTinyIconChronomancer() => GetImage("Tango.Chronomancer_tango_icon_20px.png");
		public static Image GetTinyIconMirage() => GetImage("Tango.Mirage_tango_icon_20px.png");
		public static Image GetTinyIconVirtuoso() => GetImage("Tango.Virtuoso_tango_icon_20px.png");

		// CATEGORIES
		public static Image GetTinyIconRaid() => GetImage("ArenaNet.raid_icon_32px.png");
		public static Image GetTinyIconFractals() => GetImage("ArenaNet.fractals_icon_32px.png");
		public static Image GetTinyIconGuildRegistrar() => GetImage("ArenaNet.guild_registrar_icon_26px.png");
		public static Image GetTinyIconCommander() => GetImage("ArenaNet.commander_tag_red_32px.png");
		public static Image GetTinyIconStrike() => GetImage("ArenaNet.strike_icon_32px.png");
		public static Image GetTinyIconTrainingArea() => GetImage("ArenaNet.training_area_32px.png");
		public static Image GetTinyIconWorldVersusWorld() => GetImage("ArenaNet.world_vs_world_32px.png");
		public static Image GetTinyIconUncategorized() => GetImage("ArenaNet.uncategorized_32px.png");
		public static Image GetTinyIconFestival() => GetImage("ArenaNet.festivals_32px.png");
		public static Image GetTinyIconIcebroodSaga() => GetImage("ArenaNet.icebrood_saga_32px.png");
		public static Image GetTinyIconEndOfDragons() => GetImage("ArenaNet.end_of_dragons_32px.png");
		public static Image GetTinyIconSecretsOfTheObscure() => GetImage("ArenaNet.secrets_of_the_obscure_32px.png");
		public static Image GetTinyIconInstance() => GetImage("ArenaNet.storyline_32px.png");

		// FRACTAL INSTABILITIES
		public static Image GetInstabilityImage(string iconName)
		{
			return GetImage($"ArenaNet.fractal_instabilities.{iconName}.png");
		}

		public static Image GetTinyIconAdrenalineRush() => GetInstabilityImage("adrenaline_rush_32px");
		public static Image GetTinyIconAfflicted() => GetInstabilityImage("afflicted_32px");
		public static Image GetTinyIconBirds() => GetInstabilityImage("birds_32px");
		public static Image GetTinyIconBoonOverload() => GetInstabilityImage("boon_overload_32px");
		public static Image GetTinyIconFluxBomb() => GetInstabilityImage("flux_bomb_32px");
		public static Image GetTinyIconFractalVindicators() => GetInstabilityImage("fractal_vindicators_32px");
		public static Image GetTinyIconFrailty() => GetInstabilityImage("frailty_32px");
		public static Image GetTinyIconHamstrung() => GetInstabilityImage("hamstrung_32px");
		public static Image GetTinyIconLastLaugh() => GetInstabilityImage("last_laugh_32px");
		public static Image GetTinyIconMistsConvergence() => GetInstabilityImage("mists_convergence_32px");
		public static Image GetTinyIconNoPainNoGain() => GetInstabilityImage("no_pain_no_gain_32px");
		public static Image GetTinyIconOutflanked() => GetInstabilityImage("outflanked_32px");
		public static Image GetTinyIconSlipperySlope() => GetInstabilityImage("slippery_slope_32px");
		public static Image GetTinyIconSocialAwkwardness() => GetInstabilityImage("social_awkwardness_32px");
		public static Image GetTinyIconStickTogether() => GetInstabilityImage("stick_together_32px");
		public static Image GetTinyIconSugarRush() => GetInstabilityImage("sugar_rush_32px");
		public static Image GetTinyIconToxicSickness() => GetInstabilityImage("toxic_sickness_32px");
		public static Image GetTinyIconToxicTrail() => GetInstabilityImage("toxic_trail_32px");
		public static Image GetTinyIconVengeance() => GetInstabilityImage("vengeance_32px");
		public static Image GetTinyIconWeBleedFire() => GetInstabilityImage("we_bleed_fire_32px");

		// RAID BOSSES
		public static Image GetGenericRaidWingIcon() => GetImage("ArenaNet.raid_wing_32px.png");

		// WING 1
		public static Image GetValeGuardianIcon() => GetEncounterImage("Mini_Vale_Guardian");
		public static Image GetGorsevalIcon() => GetEncounterImage("Mini_Gorseval_the_Multifarious");
		public static Image GetSabethaIcon() => GetEncounterImage("Mini_Sabetha");

		// WING 2
		public static Image GetSlothasorIcon() => GetEncounterImage("Mini_Slothasor");
		public static Image GetBanditTrioIcon() => GetEncounterImage("Mini_Narella");
		public static Image GetMatthiasIcon() => GetEncounterImage("Mini_Matthias_Abomination");

		// WING 3
		public static Image GetEscortIcon() => GetEncounterImage("Mini_McLeod_the_Silent");
		public static Image GetKeepConstructIcon() => GetEncounterImage("Mini_Keep_Construct");
		public static Image GetTwistedCastleIcon() => GetEncounterImage("Legendary_Insight");
		public static Image GetXeraIcon() => GetEncounterImage("Mini_Xera");

		// WING 4
		public static Image GetCairnIcon() => GetEncounterImage("Mini_Cairn_the_Indomitable");
		public static Image GetMursaatOverseerIcon() => GetEncounterImage("Mini_Mursaat_Overseer");
		public static Image GetSamarogIcon() => GetEncounterImage("Mini_Samarog");
		public static Image GetDeimosIcon() => GetEncounterImage("Mini_Saul");

		// WING 5
		public static Image GetDesminaIcon() => GetEncounterImage("Mini_Desmina");
		public static Image GetRiverOfSoulsIcon() => GetEncounterImage("Legendary_Divination");
		public static Image GetBrokenKingIcon() => GetEncounterImage("Mini_Broken_King");
		public static Image GetEaterOfSoulsIcon() => GetEncounterImage("Legendary_Divination");
		public static Image GetEyesIcon() => GetEncounterImage("Legendary_Divination");
		public static Image GetDhuumIcon() => GetEncounterImage("Mini_Dhuum");

		// WING 6
		public static Image GetConjuredAmalgamatedIcon() => GetEncounterImage("Legendary_Divination");
		public static Image GetTwinLargosIcon() => GetEncounterImage("Mini_Nikare");
		public static Image GetQadimIcon() => GetEncounterImage("Mini_Qadim");

		// WING 7
		public static Image GetCardinalAdinaIcon() => GetEncounterImage("Mini_Earth_Djinn");
		public static Image GetCardinalSabirIcon() => GetEncounterImage("Mini_Air_Djinn");
		public static Image GetQadimThePeerlessIcon() => GetEncounterImage("Mini_Qadim_the_Peerless");

		// STRIKES - ICEBROOD SAGA
		public static Image GetShiverpeaksPassIcon() => GetEncounterImage("Mini_Icebrood_Construct");
		public static Image GetVoiceAndClawOfTheFallenIcon() => GetEncounterImage("Mini_Cloudseeker");
		public static Image GetFraenirOfJormagIcon() => GetEncounterImage("Mini_Icebrood_Construct");
		public static Image GetBoneskinnerIcon() => GetEncounterImage("Mini_Boneskinner");
		public static Image GetWhisperOfJormagIcon() => GetEncounterImage("Mini_Whisper_of_Jormag");
		public static Image GetVariniaStormsounderIcon() => GetEncounterImage("Mini_Varinia_Stormsounder");
		
		// STRIKES - END OF DRAGONS
		public static Image GetAetherbladeHideoutIcon() => GetEncounterImage("Aetherblade_Hideout");
		public static Image GetXunlaiJadeJunkyardIcon() => GetEncounterImage("Xunlai_Jade_Junkyard");
		public static Image GetKainengOverlookIcon() => GetEncounterImage("Kaineng_Overlook");
		public static Image GetHarvestTempleIcon() => GetEncounterImage("Harvest_Temple");
		public static Image GetOldLionsCourtIcon() => GetEncounterImage("Mini_Vermillion_Assault_Knight");

		// FRACTALS
		public static Image GetMAMAIcon() => GetEncounterImage("Mini_MAMA");
		public static Image GetSiaxTheCorruptedIcon() => GetEncounterImage("Mini_Toxic_Warlock");
		public static Image GetEnsolyssOfTheEndlessTormentIcon() => GetEncounterImage("Mini_Ensolyss");
		public static Image GetSkorvaldIcon() => GetEncounterImage("Skorvald");
		public static Image GetArtsariivIcon() => GetEncounterImage("Artsariiv");
		public static Image GetArkkIcon() => GetEncounterImage("Arkk");
		public static Image GetElementalAiKeeperOfThePeakIcon() => GetEncounterImage("Elemental_Ai");
		public static Image GetDarkAiKeeperOfThePeakIcon() => GetEncounterImage("Dark_Ai");
		public static Image GetBothPhasesAiKeeperOfThePeakIcon() => GetEncounterImage("Both_Phases_Ai");
		public static Image GetKanaxaiIcon() => GetEncounterImage("Mini_Kanaxai");

		// FESTIVALS
		public static Image GetFreezieIcon() => GetEncounterImage("Mini_Freezie");

		// TRAINING AREA
		public static Image GetStandardKittyGolemIcon() => GetEncounterImage("Mini_Professor_Mew");
		public static Image GetMediumKittyGolemIcon() => GetEncounterImage("Mini_Snuggles");
		public static Image GetLargeKittyGolemIcon() => GetEncounterImage("Mini_Baron_von_Scrufflebutt");
		public static Image GetMassiveKittyGolemIcon() => GetEncounterImage("Mini_Mister_Mittens");

		// WORLD VS WORLD
		public static Image GetEternalBattlegroundsIcon() => GetEncounterImage("Commander_tag_yellow");
		public static Image GetRedBorderlandsIcon() => GetEncounterImage("Commander_tag_red");
		public static Image GetBlueBorderlandsIcon() => GetEncounterImage("Commander_tag_blue");
		public static Image GetGreenBorderlandsIcon() => GetEncounterImage("Commander_tag_green");
		public static Image GetObsidianSanctumIcon() => GetEncounterImage("Commander_tag_purple");
		public static Image GetEdgeOfTheMistsIcon() => GetEncounterImage("Commander_tag_white");
		public static Image GetArmisticeBastionIcon() => GetEncounterImage("Armistice_Bastion_Pass");

		private static Image GetEncounterImage(string iconName)
		{
			return GetImage($"ArenaNet.Bosses.{iconName}.png");
		}
	}
}