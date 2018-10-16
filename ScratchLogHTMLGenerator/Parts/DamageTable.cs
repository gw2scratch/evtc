using System;
using System.IO;
using System.Linq;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics;

namespace ScratchLogHTMLGenerator.Parts
{
	public class DamageTable
	{
		private readonly SquadDamageData squadDamageData;

		public DamageTable(SquadDamageData squadDamageData)
		{
			this.squadDamageData = squadDamageData;
		}

		public void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th><abbr title='Subgroup'>Grp</abbr></th>
            <th></th>
            <th>Name</th>
            <th>Account</th>
			<th><abbr title='Damage per second'>DPS</abbr></th>
			<th>Power DPS</th>
			<th>Condition DPS</th>
        </tr>
        </thead>
        <tbody>");
			foreach (var damageData in squadDamageData.DamageData.Where(x => x.Attacker is Player)
				.OrderByDescending(x => x.TotalDps))
			{
				var player = (Player) damageData.Attacker;
				string specialization = player.EliteSpecialization == EliteSpecialization.None
					? player.Profession.ToString()
					: player.EliteSpecialization.ToString();
				writer.WriteLine($@"
		<tr>
            <td>{player.Subgroup}</td>
            <td style='padding: .35em 0 0 0'><img src='{GetProfessionIconUrl(player)}' alt='{specialization}'></td>
            <td>{player.Name}</td>
            <td>{player.AccountName.Substring(1)}</td>
            <td><b>{damageData.TotalDps:0}</b></td>
            <td>{damageData.PhysicalDps:0}</td>
            <td>{damageData.ConditionDps:0}</td>
		</tr>");
			}

			// TODO: Split total for players and all agents
			writer.WriteLine($@"
        </tbody>
		<tfoot>
		<tr>
            <td colspan='4'><b>Total</b></td>
            <td><b>{squadDamageData.TotalDps:0}</b></td>
            <td>{squadDamageData.TotalPhysicalDps:0}</td>
            <td>{squadDamageData.TotalConditionDps:0}</td>
		</tr>
		</tfoot>
    </table>");
		}

		private string GetProfessionIconUrl(Player player)
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