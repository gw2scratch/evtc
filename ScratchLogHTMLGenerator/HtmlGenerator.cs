using System.IO;
using System.Linq;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;
using ScratchEVTCParser.Statistics;

namespace ScratchLogHTMLGenerator
{
	public class HtmlGenerator
	{
		public void WriteHtml(TextWriter writer, LogStatistics statistics)
		{
			string result = (statistics.EncounterResult == EncounterResult.Success ? "Success"
				: statistics.EncounterResult == EncounterResult.Failure ? "Failure" : "Result unknown");

			writer.Write($@"<!DOCTYPE html>
<html>
<head>
    <title>{statistics.EncounterName}</title>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.1/css/bulma.min.css'>
    <script defer src='https://use.fontawesome.com/releases/v5.1.0/js/all.js'></script>
</head>
<body>
<section class='section'>
<div class='container'>
    <h1 class='title'>{statistics.EncounterName}</h1>
    <div class='subtitle'>{result} in {statistics.FightTimeMs / 1000 / 60}m {statistics.FightTimeMs / 1000 % 60}s {statistics.FightTimeMs % 1000}ms</div>
<div class='content'>
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th>Name</th>
			<th>Boss DPS</th>
			<th>Boss Power DPS</th>
			<th>Boss Condition DPS</th>
        </tr>
        </thead>
        <tbody>");
			foreach (var pair in statistics.BossDamageByAgent.Where(x => x.Key is Player).OrderByDescending(x => x.Value.TotalDps))
			{
				var player = (Player) pair.Key;
				var damage = pair.Value;
				string specialization = player.EliteSpecialization == EliteSpecialization.None
					? player.Profession.ToString()
					: player.EliteSpecialization.ToString();
				writer.Write($@"
		<tr>
            <td>{player.Name} ({specialization})</td>
            <td>{damage.TotalDps:0}</td>
            <td>{damage.PhysicalDps:0}</td>
            <td>{damage.ConditionDps:0}</td>
		</tr>");
			}
			writer.Write($@"
		<tr>
            <td>Total</td>
            <td>{statistics.BossDps:0}</td>
            <td>{statistics.BossPhysicalDps:0}</td>
            <td>{statistics.BossConditionDps:0}</td>
		</tr>
        </tbody>
    </table>
</div>
</div>
</section>
</body>
</html>");
		}
	}
}