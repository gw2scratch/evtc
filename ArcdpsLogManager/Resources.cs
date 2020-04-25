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

		public static Image GetTinyIconWarrior()
		{
			return GetImage("Warrior_tango_icon_20px.png");
		}

		public static Image GetTinyIconGuardian()
		{
			return GetImage("Guardian_tango_icon_20px.png");
		}

		public static Image GetTinyIconRevenant()
		{
			return GetImage("Revenant_tango_icon_20px.png");
		}

		public static Image GetTinyIconRanger()
		{
			return GetImage("Ranger_tango_icon_20px.png");
		}

		public static Image GetTinyIconThief()
		{
			return GetImage("Thief_tango_icon_20px.png");
		}

		public static Image GetTinyIconEngineer()
		{
			return GetImage("Engineer_tango_icon_20px.png");
		}

		public static Image GetTinyIconNecromancer()
		{
			return GetImage("Necromancer_tango_icon_20px.png");
		}

		public static Image GetTinyIconElementalist()
		{
			return GetImage("Elementalist_tango_icon_20px.png");
		}

		public static Image GetTinyIconMesmer()
		{
			return GetImage("Mesmer_tango_icon_20px.png");
		}

		public static Image GetTinyIconBerserker()
		{
			return GetImage("Berserker_tango_icon_20px.png");
		}

		public static Image GetTinyIconSpellbreaker()
		{
			return GetImage("Spellbreaker_tango_icon_20px.png");
		}

		public static Image GetTinyIconDragonhunter()
		{
			return GetImage("Dragonhunter_tango_icon_20px.png");
		}

		public static Image GetTinyIconFirebrand()
		{
			return GetImage("Firebrand_tango_icon_20px.png");
		}

		public static Image GetTinyIconHerald()
		{
			return GetImage("Herald_tango_icon_20px.png");
		}

		public static Image GetTinyIconRenegade()
		{
			return GetImage("Renegade_tango_icon_20px.png");
		}

		public static Image GetTinyIconDruid()
		{
			return GetImage("Druid_tango_icon_20px.png");
		}

		public static Image GetTinyIconSoulbeast()
		{
			return GetImage("Soulbeast_tango_icon_20px.png");
		}

		public static Image GetTinyIconDaredevil()
		{
			return GetImage("Daredevil_tango_icon_20px.png");
		}

		public static Image GetTinyIconDeadeye()
		{
			return GetImage("Deadeye_tango_icon_20px.png");
		}

		public static Image GetTinyIconScrapper()
		{
			return GetImage("Scrapper_tango_icon_20px.png");
		}

		public static Image GetTinyIconHolosmith()
		{
			return GetImage("Holosmith_tango_icon_20px.png");
		}

		public static Image GetTinyIconReaper()
		{
			return GetImage("Reaper_tango_icon_20px.png");
		}

		public static Image GetTinyIconScourge()
		{
			return GetImage("Scourge_tango_icon_20px.png");
		}

		public static Image GetTinyIconTempest()
		{
			return GetImage("Tempest_tango_icon_20px.png");
		}

		public static Image GetTinyIconWeaver()
		{
			return GetImage("Weaver_tango_icon_20px.png");
		}

		public static Image GetTinyIconChronomancer()
		{
			return GetImage("Chronomancer_tango_icon_20px.png");
		}

		public static Image GetTinyIconMirage()
		{
			return GetImage("Mirage_tango_icon_20px.png");
		}

		public static Image GetTinyIconRaid()
		{
			return GetImage("raid_icon_32px.png");
		}

		public static Image GetTinyIconFractals()
		{
			return GetImage("fractals_icon_32px.png");
		}

		public static Image GetTinyIconGuildRegistrar()
		{
			return GetImage("guild_registrar_icon_26px.png");
		}
	}
}