using System;
using ScratchEVTCParser.Model.Agents;

namespace ScratchLogHTMLGenerator
{
	public class DefaultTheme : ITheme
	{
		public string GetProfessionColorMedium(Player player)
		{
			switch (player.Profession)
			{
				case Profession.Warrior:
					return "#FFD166";
				case Profession.Guardian:
					return "#72C1D9";
				case Profession.Revenant:
					return "#D16E5A";
				case Profession.Engineer:
					return "#D09C59";
				case Profession.Ranger:
					return "#8CDC82";
				case Profession.Thief:
					return "#C08F95";
				case Profession.Elementalist:
					return "#F68A87";
				case Profession.Necromancer:
					return "#52A76F";
				case Profession.Mesmer:
					return "#B679D5";
				default:
					throw new ArgumentOutOfRangeException(nameof(player.Profession));
			}
		}

		public string GetProfessionColorLight(Player player)
		{
			switch (player.Profession)
			{
				case Profession.Warrior:
					return "#FFF2A4";
				case Profession.Guardian:
					return "#BCE8FD";
				case Profession.Revenant:
					return "#E4AEA3";
				case Profession.Ranger:
					return "#D2F6BC";
				case Profession.Thief:
					return "#DEC6C9";
				case Profession.Engineer:
					return "#E8BC84";
				case Profession.Necromancer:
					return "#BFE6D0";
				case Profession.Elementalist:
					return "#F6BEBC";
				case Profession.Mesmer:
					return "#D09EEA";
				default:
					throw new ArgumentOutOfRangeException(nameof(player.Profession));
			}
		}

		public string GetProfessionColorLightTransparent(Player player)
		{
			switch (player.Profession)
			{
				case Profession.Warrior:
					return "rgba(255,242,164,0.4)";
				case Profession.Guardian:
					return "rgba(188,232,253,0.4)";
				case Profession.Revenant:
					return "rgba(228,174,163,0.4)";
				case Profession.Ranger:
					return "rgba(210,246,188,0.4)";
				case Profession.Thief:
					return "rgba(222,198,201,0.4)";
				case Profession.Engineer:
					return "rgba(232,188,132,0.4)";
				case Profession.Necromancer:
					return "rgba(191,230,208,0.4)";
				case Profession.Elementalist:
					return "rgba(246,190,188,0.4)";
				case Profession.Mesmer:
					return "rgba(208,158,234,0.4)";
				default:
					throw new ArgumentOutOfRangeException(nameof(player.Profession));
			}
		}

		public string GetBigProfessionIconUrl(Player player)
		{
			if (player.EliteSpecialization == EliteSpecialization.None)
			{
				switch (player.Profession)
				{
					case Profession.Warrior:
						return "https://wiki.guildwars2.com/images/d/db/Warrior_tango_icon_200px.png";
					case Profession.Guardian:
						return "https://wiki.guildwars2.com/images/6/6c/Guardian_tango_icon_200px.png";
					case Profession.Revenant:
						return "https://wiki.guildwars2.com/images/a/a8/Revenant_tango_icon_200px.png";
					case Profession.Ranger:
						return "https://wiki.guildwars2.com/images/5/51/Ranger_tango_icon_200px.png";
					case Profession.Thief:
						return "https://wiki.guildwars2.com/images/1/19/Thief_tango_icon_200px.png";
					case Profession.Engineer:
						return "https://wiki.guildwars2.com/images/2/2f/Engineer_tango_icon_200px.png";
					case Profession.Necromancer:
						return "https://wiki.guildwars2.com/images/c/cd/Necromancer_tango_icon_200px.png";
					case Profession.Elementalist:
						return "https://wiki.guildwars2.com/images/a/a0/Elementalist_tango_icon_200px.png";
					case Profession.Mesmer:
						return "https://wiki.guildwars2.com/images/7/73/Mesmer_tango_icon_200px.png";
					default:
						throw new ArgumentOutOfRangeException(nameof(player.Profession));
				}
			}

			switch (player.EliteSpecialization)
			{
				case EliteSpecialization.Berserker:
					return "https://wiki.guildwars2.com/images/8/80/Berserker_tango_icon_200px.png";
				case EliteSpecialization.Spellbreaker:
					return "https://wiki.guildwars2.com/images/7/78/Spellbreaker_tango_icon_200px.png";
				case EliteSpecialization.Dragonhunter:
					return "https://wiki.guildwars2.com/images/1/1f/Dragonhunter_tango_icon_200px.png";
				case EliteSpecialization.Firebrand:
					return "https://wiki.guildwars2.com/images/7/73/Firebrand_tango_icon_200px.png";
				case EliteSpecialization.Herald:
					return "https://wiki.guildwars2.com/images/c/c7/Herald_tango_icon_200px.png";
				case EliteSpecialization.Renegade:
					return "https://wiki.guildwars2.com/images/b/bc/Renegade_tango_icon_200px.png";
				case EliteSpecialization.Druid:
					return "https://wiki.guildwars2.com/images/6/6d/Druid_tango_icon_200px.png";
				case EliteSpecialization.Soulbeast:
					return "https://wiki.guildwars2.com/images/f/f6/Soulbeast_tango_icon_200px.png";
				case EliteSpecialization.Daredevil:
					return "https://wiki.guildwars2.com/images/c/ca/Daredevil_tango_icon_200px.png";
				case EliteSpecialization.Deadeye:
					return "https://wiki.guildwars2.com/images/b/b0/Deadeye_tango_icon_200px.png";
				case EliteSpecialization.Scrapper:
					return "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png";
				case EliteSpecialization.Holosmith:
					return "https://wiki.guildwars2.com/images/a/ae/Holosmith_tango_icon_200px.png";
				case EliteSpecialization.Reaper:
					return "https://wiki.guildwars2.com/images/9/95/Reaper_tango_icon_200px.png";
				case EliteSpecialization.Scourge:
					return "https://wiki.guildwars2.com/images/8/8a/Scourge_tango_icon_200px.png";
				case EliteSpecialization.Tempest:
					return "https://wiki.guildwars2.com/images/9/90/Tempest_tango_icon_200px.png";
				case EliteSpecialization.Weaver:
					return "https://wiki.guildwars2.com/images/3/31/Weaver_tango_icon_200px.png";
				case EliteSpecialization.Chronomancer:
					return "https://wiki.guildwars2.com/images/8/8b/Chronomancer_tango_icon_200px.png";
				case EliteSpecialization.Mirage:
					return "https://wiki.guildwars2.com/images/a/a9/Mirage_tango_icon_200px.png";
				default:
					throw new ArgumentOutOfRangeException(nameof(player.EliteSpecialization));
			}
		}

		public string GetTinyProfessionIconUrl(Player player)
		{
		    if (player.EliteSpecialization == EliteSpecialization.None)
		    {
			    switch (player.Profession)
			    {
				    case Profession.Warrior:
					    return "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png";
				    case Profession.Guardian:
					    return "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png";
				    case Profession.Revenant:
					    return "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png";
				    case Profession.Ranger:
					    return "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png";
				    case Profession.Thief:
					    return "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png";
				    case Profession.Engineer:
					    return "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png";
				    case Profession.Necromancer:
					    return "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png";
				    case Profession.Elementalist:
					    return "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png";
				    case Profession.Mesmer:
					    return "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png";
				    default:
					    throw new ArgumentOutOfRangeException(nameof(player.Profession));
			    }
		    }

			switch (player.EliteSpecialization)
			{
				case EliteSpecialization.Berserker:
					return "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png";
				case EliteSpecialization.Spellbreaker:
					return "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png";
				case EliteSpecialization.Dragonhunter:
					return "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
				case EliteSpecialization.Firebrand:
					return "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png";
				case EliteSpecialization.Herald:
					return "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png";
				case EliteSpecialization.Renegade:
					return "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png";
				case EliteSpecialization.Druid:
					return "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png";
				case EliteSpecialization.Soulbeast:
					return "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png";
				case EliteSpecialization.Daredevil:
					return "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png";
				case EliteSpecialization.Deadeye:
					return "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png";
				case EliteSpecialization.Scrapper:
					return "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png";
				case EliteSpecialization.Holosmith:
					return "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png";
				case EliteSpecialization.Reaper:
					return "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png";
				case EliteSpecialization.Scourge:
					return "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png";
				case EliteSpecialization.Tempest:
					return "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png";
				case EliteSpecialization.Weaver:
					return "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png";
				case EliteSpecialization.Chronomancer:
					return "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png";
				case EliteSpecialization.Mirage:
					return "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png";
				default:
					throw new ArgumentOutOfRangeException(nameof(player.EliteSpecialization));
			}
		}
	}
}