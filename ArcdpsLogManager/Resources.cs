using System;
using System.IO;
using System.Reflection;
using Eto.Drawing;

namespace GW2Scratch.ArcdpsLogManager
{
	public static class Resources
	{
		private const string ImageDirectoryName = "Images";

		private static string GetImagePath(string imageName)
		{
			return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ImageDirectoryName,
				imageName);
		}

		private static Bitmap GetImage(string imageName)
		{
			return new Bitmap(GetImagePath(imageName));
		}

		public static Icon GetProgramIcon()
		{
			return new Icon(1, GetImage("program_icon.png"));
		}

		public static Image GetTinyIconWarrior() => GetImage("Tango/Warrior_tango_icon_20px.png");
		public static Image GetTinyIconGuardian() => GetImage("Tango/Guardian_tango_icon_20px.png");
		public static Image GetTinyIconRevenant() => GetImage("Tango/Revenant_tango_icon_20px.png");
		public static Image GetTinyIconRanger() => GetImage("Tango/Ranger_tango_icon_20px.png");
		public static Image GetTinyIconThief() => GetImage("Tango/Thief_tango_icon_20px.png");
		public static Image GetTinyIconEngineer() => GetImage("Tango/Engineer_tango_icon_20px.png");
		public static Image GetTinyIconNecromancer() => GetImage("Tango/Necromancer_tango_icon_20px.png");
		public static Image GetTinyIconElementalist() => GetImage("Tango/Elementalist_tango_icon_20px.png");
		public static Image GetTinyIconMesmer() => GetImage("Tango/Mesmer_tango_icon_20px.png");
		public static Image GetTinyIconBerserker() => GetImage("Tango/Berserker_tango_icon_20px.png");
		public static Image GetTinyIconSpellbreaker() => GetImage("Tango/Spellbreaker_tango_icon_20px.png");
		public static Image GetTinyIconDragonhunter() => GetImage("Tango/Dragonhunter_tango_icon_20px.png");
		public static Image GetTinyIconFirebrand() => GetImage("Tango/Firebrand_tango_icon_20px.png");
		public static Image GetTinyIconHerald() => GetImage("Tango/Herald_tango_icon_20px.png");
		public static Image GetTinyIconRenegade() => GetImage("Tango/Renegade_tango_icon_20px.png");
		public static Image GetTinyIconDruid() => GetImage("Tango/Druid_tango_icon_20px.png");
		public static Image GetTinyIconSoulbeast() => GetImage("Tango/Soulbeast_tango_icon_20px.png");
		public static Image GetTinyIconDaredevil() => GetImage("Tango/Daredevil_tango_icon_20px.png");
		public static Image GetTinyIconDeadeye() => GetImage("Tango/Deadeye_tango_icon_20px.png");
		public static Image GetTinyIconScrapper() => GetImage("Tango/Scrapper_tango_icon_20px.png");
		public static Image GetTinyIconHolosmith() => GetImage("Tango/Holosmith_tango_icon_20px.png");
		public static Image GetTinyIconReaper() => GetImage("Tango/Reaper_tango_icon_20px.png");
		public static Image GetTinyIconScourge() => GetImage("Tango/Scourge_tango_icon_20px.png");
		public static Image GetTinyIconTempest() => GetImage("Tango/Tempest_tango_icon_20px.png");
		public static Image GetTinyIconWeaver() => GetImage("Tango/Weaver_tango_icon_20px.png");
		public static Image GetTinyIconChronomancer() => GetImage("Tango/Chronomancer_tango_icon_20px.png");
		public static Image GetTinyIconMirage() => GetImage("Tango/Mirage_tango_icon_20px.png");
		public static Image GetTinyIconRaid() => GetImage("ArenaNet/raid_icon_32px.png");
		public static Image GetTinyIconFractals() => GetImage("ArenaNet/fractals_icon_32px.png");
		public static Image GetTinyIconGuildRegistrar() => GetImage("ArenaNet/guild_registrar_icon_26px.png");
		public static Image GetTinyIconCommander() => GetImage("ArenaNet/commander_tag_red_32px.png");
		public static Image GetTinyIconStrike() => GetImage("ArenaNet/strike_icon_32px.png");
		public static Image GetTinyIconTrainingArea() => GetImage("ArenaNet/training_area_32px.png");
		public static Image GetTinyIconWorldVersusWorld() => GetImage("ArenaNet/world_vs_world_32px.png");
		public static Image GetTinyIconUncategorized() => GetImage("ArenaNet/uncategorized_32px.png");
		public static Image GetTinyIconFestival() => GetImage("ArenaNet/festivals_32px.png");

		// Fractal Instabilities
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
		public static Image GetInstabilityImage(string iconName)
		{
			return GetImage($"ArenaNet/fractal_instabilities/{iconName}.png");
		}

		// RAID BOSSES
		public static Image GetGenericRaidWingIcon() => GetImage("ArenaNet/raid_wing_32px.png");
		// WING 1
		public static Image GetValeGuardianIcon() => GetBossImage("Mini_Vale_Guardian");
		public static Image GetGorsevalIcon() => GetBossImage("Mini_Gorseval_the_Multifarious");
		public static Image GetSabethaIcon() => GetBossImage("Mini_Sabetha");
		// WING 2
		public static Image GetSlothasorIcon() => GetBossImage("Mini_Slothasor");
		public static Image GetBanditTrioIcon() => GetBossImage("Mini_Narella");
		public static Image GetMatthiasIcon() => GetBossImage("Mini_Matthias_Abomination");
		// WING 3
		public static Image GetEscortIcon() => GetBossImage("Mini_McLeod_the_Silent");
		public static Image GetKeepConstructIcon() => GetBossImage("Mini_Keep_Construct");
		public static Image GetTwistedCastleIcon() => GetBossImage("Legendary_Insight");
		public static Image GetXeraIcon() => GetBossImage("Mini_Xera");
		// WING 4
		public static Image GetCairnIcon() => GetBossImage("Mini_Cairn_the_Indomitable");
		public static Image GetMursaatOverseerIcon() => GetBossImage("Mini_Mursaat_Overseer");
		public static Image GetSamarogIcon() => GetBossImage("Mini_Samarog");
		public static Image GetDeimosIcon() => GetBossImage("Mini_Saul");
		// WING 5
		public static Image GetDesminaIcon() => GetBossImage("Mini_Desmina");
		public static Image GetRiverOfSoulsIcon() => GetBossImage("Legendary_Divination");
		public static Image GetBrokenKingIcon() => GetBossImage("Mini_Broken_King");
		public static Image GetEaterOfSoulsIcon() => GetBossImage("Legendary_Divination");
		public static Image GetEyesIcon() => GetBossImage("Legendary_Divination");
		public static Image GetDhuumIcon() => GetBossImage("Mini_Dhuum");
		// WING 6
		public static Image GetConjuredAmalgamatedIcon() => GetBossImage("Legendary_Divination");
		public static Image GetTwinLargosIcon() => GetBossImage("Mini_Nikare");
		public static Image GetQadimIcon() => GetBossImage("Mini_Qadim");
		// WING 7
		public static Image GetCardinalAdinaIcon() => GetBossImage("Mini_Earth_Djinn");
		public static Image GetCardinalSabirIcon() => GetBossImage("Mini_Air_Djinn");
		public static Image GetQadimThePeerlessIcon() => GetBossImage("Mini_Qadim_the_Peerless");
		// STRIKES
		public static Image GetShiverpeaksPassIcon() => GetBossImage("Mini_Icebrood_Construct");
		public static Image GetVoiceAndClawOfTheFallenIcon() => GetBossImage("Mini_Cloudseeker");
		public static Image GetFraenirOfJormagIcon() => GetBossImage("Mini_Icebrood_Construct");
		public static Image GetBoneskinnerIcon() => GetBossImage("Mini_Boneskinner");
		public static Image GetWhisperOfJormagIcon() => GetBossImage("Mini_Whisper_of_Jormag");
		public static Image GetVariniaStormsounderIcon() => GetBossImage("Mini_Varinia_Stormsounder");
		// FRACTALS
		public static Image GetMAMAIcon() => GetBossImage("Mini_MAMA");
		public static Image GetSiaxTheCorruptedIcon() => GetBossImage("Mini_Toxic_Warlock");
		public static Image GetEnsolyssOfTheEndlessTormentIcon() => GetBossImage("Mini_Ensolyss");
		public static Image GetSkorvaldIcon() => GetBossImage("Endless_Fury_Combat_Tonic");
		public static Image GetArtsariivIcon() => GetBossImage("Unstable_Fractal_Essence");
		public static Image GetArkkIcon() => GetBossImage("Endless_Chaos_Combat_Tonic");
		public static Image GetAiKeeperOfThePeakIcon() => GetBossImage("Endless_Inner_Demon_Combat_Tonic");
		// FESTIVALS
		public static Image GetFreezieIcon() => GetBossImage("Mini_Freezie");
		// TRAINING AREA
		public static Image GetStandardKittyGolemIcon() => GetBossImage("Mini_Professor_Mew");
		public static Image GetMediumKittyGolemIcon() => GetBossImage("Mini_Snuggles");
		public static Image GetLargeKittyGolemIcon() => GetBossImage("Mini_Baron_von_Scrufflebutt");
		public static Image GetMassiveKittyGolemIcon() => GetBossImage("Mini_Mister_Mittens");
		// WORLD VS WORLD
		public static Image GetWorldVersusWorldIcon() => GetBossImage("Commander_tag_(blue)");
		private static Image GetBossImage(string iconName)
		{
			return GetImage($"ArenaNet/Bosses/{iconName}.png");
		}
	}
}