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
		private readonly ITheme theme;

		public DamageTable(SquadDamageData squadDamageData, ITheme theme)
		{
			this.squadDamageData = squadDamageData;
			this.theme = theme;
		}

		public void WriteHtml(TextWriter writer)
		{
			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable is-fullwidth'>
        <thead>
        <tr>
            <th><abbr title='Subgroup'>Grp</abbr></th>
            <th></th>
            <th>Name</th>
            <th>Account</th>
			<th><abbr title='Damage per second'>DPS</abbr></th>
			<th>Power</th>
			<th>Condition</th>
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
            <td style='padding: .35em 0 0 0'><img src='{theme.GetTinyProfessionIconUrl(player)}' alt='{specialization}'></td>
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
	}
}