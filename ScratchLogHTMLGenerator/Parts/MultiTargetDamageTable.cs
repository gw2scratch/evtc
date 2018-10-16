using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics;

namespace ScratchLogHTMLGenerator.Parts
{
	public class MultiTargetDamageTable
	{
		// TODO: Rewrite so this doesn't duplicate code from DamageTable

		private readonly IReadOnlyCollection<TargetSquadDamageData> squadDamageData;

		public MultiTargetDamageTable(IEnumerable<TargetSquadDamageData> squadDamageData)
		{
			this.squadDamageData = squadDamageData.ToArray();
		}

		public void WriteHtml(TextWriter writer)
		{
			var playerData = squadDamageData
				.SelectMany(x => x.DamageData.Select(y => y.Attacker))
				.Where(x => x is Player)
				.Distinct()
				.ToDictionary(x => x, x => new List<DamageData>());

			var targets = new List<Agent>();

			foreach (var targetDamage in squadDamageData)
			{
				targets.Add(targetDamage.Target);
				foreach (var damageData in targetDamage.DamageData.Where(x => x.Attacker is Player))
				{
					playerData[damageData.Attacker].Add(damageData);
				}
			}

			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th><abbr title='Subgroup'>Grp</abbr></th>
            <th></th>
            <th>Name</th>
            <th>Account</th>");

			foreach (var target in targets)
			{
				writer.WriteLine($@"
            <th>{target.Name}</th>");
			}

			writer.WriteLine($@"
			<th><b>Total target <abbr title='Damage per second'>DPS</abbr></b></th>
        </tr>
        </thead>
        <tbody>");

			foreach (var pair in playerData.OrderByDescending(p => p.Value.Sum(d => d.TotalDps)))
			{
				var player = (Player) pair.Key;
				var dps = pair.Value;

				string specialization = player.EliteSpecialization == EliteSpecialization.None
					? player.Profession.ToString()
					: player.EliteSpecialization.ToString();
				writer.WriteLine($@"
		<tr>
            <td>{player.Subgroup}</td>
            <td style='padding: .35em 0 0 0'><img src='{GetProfessionIconUrl(player)}' alt='{specialization}'></td>
            <td>{player.Name}</td>
            <td>{player.AccountName.Substring(1)}</td>");

				foreach (var targetDps in dps)
				{
					writer.WriteLine($@"
			<td>{targetDps.TotalDps:0}</td>");
				}

				writer.WriteLine($@"
			<td><b>{dps.Sum(x => x.TotalDps):0}</b></td>
		</tr>");
			}

			writer.WriteLine($@"
        </tbody>
		<tfoot>
		<tr>
            <td colspan='4'><b>Total</b></td>");

			for (int i = 0; i < targets.Count; i++)
			{
				var targetTotalDps = playerData.Select(x => x.Value[i]).Sum(x => x.TotalDps);
				writer.WriteLine($"<td>{targetTotalDps:0}</td>");
			}

			var totalTargetDps = playerData.Select(x => x.Value.Sum(y => y.TotalDps)).Sum();
            writer.WriteLine($"<td>{totalTargetDps:0}</td>");

			writer.WriteLine($@"
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