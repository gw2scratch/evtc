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
			//This fixes the program icon not showing up on windows, requires testing on Linux and Mac
			return new Icon(GetImagePath("program_icon.png"));
			//return new Icon(1, GetImage("program_icon.png"));
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

		//raid boss and fractal boss icons

		public static Image GetGenericRaidWingIcon() => GetImage("ArenaNet/raid_wing_32px.png");

	}
}