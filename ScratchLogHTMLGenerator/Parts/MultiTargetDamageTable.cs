using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics;

namespace ScratchLogHTMLGenerator.Parts
{
	public class MultiTargetDamageTable
	{
		// TODO: Rewrite so this doesn't duplicate code from DamageTable

		private readonly ITheme theme;
		private readonly IReadOnlyCollection<TargetSquadDamageData> squadDamageData;

		public MultiTargetDamageTable(IEnumerable<TargetSquadDamageData> squadDamageData, ITheme theme)
		{
			this.theme = theme;
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
    <table class='table is-narrow is-striped is-hoverable is-fullwidth'>
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
            <td style='padding: .35em 0 0 0'><img src='{theme.GetTinyProfessionIconUrl(player)}' alt='{specialization}'></td>
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
	}
}