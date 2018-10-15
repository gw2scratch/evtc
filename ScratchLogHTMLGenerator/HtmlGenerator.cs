using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;
using ScratchEVTCParser.Statistics;

namespace ScratchLogHTMLGenerator
{
	public class HtmlGenerator
	{
		public void WriteHtml(TextWriter writer, LogStatistics stats)
		{
			string result = (stats.EncounterResult == EncounterResult.Success ? "Success"
				: stats.EncounterResult == EncounterResult.Failure ? "Failure" : "Result unknown");

			writer.WriteLine($@"<!DOCTYPE html>
<html>
<head>
    <title>{stats.EncounterName}</title>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.1/css/bulma.min.css'>
    <script defer src='https://use.fontawesome.com/releases/v5.1.0/js/all.js'></script>
    <script defer src='https://cdn.plot.ly/plotly-latest.min.js'></script>
	<script>
		function openTab(tabLink) {{
			var tabs = document.getElementsByClassName('scratch-tab');
			for (var i = 0; i < tabs.length; i++) {{
				tabs[i].classList.add('is-hidden');
			}}
			var tabLinks = document.getElementsByClassName('scratch-tablink');
			for (var i = 0; i < tabLinks.length; i++) {{
				tabLinks[i].classList.remove('is-active');
			}}
			tabLink.classList.add('is-active');
			var tabName = 'tab-' + tabLink.id.substring('tablink-'.length);
			document.getElementById(tabName).classList.remove('is-hidden');
		}}
	</script>
</head>
<body>
<section class='section'>
<div class='container'>
    <h1 class='title'>{stats.EncounterName}</h1>
    <div class='subtitle'>{result} in {MillisecondsToReadableFormat(stats.FightTimeMs)}</div>
    <div class='columns'>");

			WriteMenu(writer, stats);

			// Summary tab
			writer.WriteLine($@"
        <div id='tab-general' class='content column scratch-tab'>
            <div class='title is-4'>Full Encounter</div>
            <div class='subtitle is-6'>Duration: {MillisecondsToReadableFormat(stats.FullFightSquadDamageData.TimeMs)}</div>

			<div>
                <div class='title is-5'>Total damage in encounter</div>");
			WriteDamageTable(writer, stats.FullFightSquadDamageData);

			writer.WriteLine(@"
			</div>
		</div>");

			// Encounter target tabs
			int enemyTargetIndex = 0;
			foreach (var target in stats.FullFightTargetDamageData)
			{
				writer.WriteLine($@"
        <div id='tab-full-enemy-{enemyTargetIndex++}' class='content column scratch-tab is-hidden'>");
				writer.WriteLine($@"
            <div class='title is-4'>Full Encounter</div>
            <div class='subtitle is-6'>Duration: {MillisecondsToReadableFormat(target.TimeMs)}</div>

			<div>
                <div class='title is-5'>Target damage to {target.Target.Name}</div>");
				WriteDamageTable(writer, target);
				writer.WriteLine($@"
			</div>
        </div>");
			}

			// Scratch data tabs
			writer.WriteLine(@"
        <div id='tab-buffdata' class='content column scratch-tab is-hidden'>");
			WriteBuffData(writer, stats);
			writer.WriteLine(@"
		</div>");

			writer.WriteLine(@"
        <div id='tab-eventcounts' class='content column scratch-tab is-hidden'>");
			WriteEventCounts(writer, stats);
			writer.WriteLine(@"
		</div>");

			writer.WriteLine(@"
        <div id='tab-skills' class='content column scratch-tab is-hidden'>");
			WriteSkillList(writer, stats);
			writer.WriteLine(@"
		</div>");

			writer.WriteLine(@"
        <div id='tab-agents' class='content column scratch-tab is-hidden'>");
			WriteAgentList(writer, stats);
			writer.WriteLine(@"
		</div>");

			// Phase tabs
			int phaseIndex = 0;
			foreach (var phaseStats in stats.PhaseStats)
			{
				writer.WriteLine($@"
        <div id='tab-phase-{phaseIndex}' class='content column scratch-tab is-hidden'>");
				writer.WriteLine($@"
            <div class='title is-4'>Phase: {phaseStats.PhaseName}</div>
            <div class='subtitle is-6'>Duration: {MillisecondsToReadableFormat(phaseStats.PhaseDuration)}</div>

			<div>
                <div class='title is-5'>Total damage</div>");
				WriteDamageTable(writer, phaseStats.TotalDamageData);
				writer.WriteLine($@"
			</div>
        </div>");
				int targetIndex = 0;
				foreach (var target in phaseStats.TargetDamageData)
				{
					writer.WriteLine($@"
        <div id='tab-enemy-{phaseIndex}-{targetIndex++}' class='content column scratch-tab is-hidden'>");
					writer.WriteLine($@"
            <div class='title is-4'>Phase: {phaseStats.PhaseName}</div>
            <div class='subtitle is-6'>Duration: {MillisecondsToReadableFormat(target.TimeMs)}</div>

			<div>
                <div class='title is-5'>Target damage to {target.Target.Name}</div>");
					WriteDamageTable(writer, target);
					writer.WriteLine($@"
			</div>
        </div>");
				}

				phaseIndex++;
			}

			writer.WriteLine($@"
</div>
</div>
</section>
<footer class='footer'>
<div class='container'>
<div class='content has-text-centered'>
<p>
    Generated by the Scratch EVTC Parser.
</p>
<p>
	Time of recording {stats.FightStart.ToUniversalTime():yyyy-MM-dd HH:mm UTC}
    <br>
	Recorded by {stats.LogAuthor.Name}
    <br>
	EVTC version {stats.LogVersion}
</p>
</div>
</div>
</footer>
</body>
</html>");
		}

		private void WriteAgentList(TextWriter writer, LogStatistics stats)
		{
			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th>Agent type</th>
            <th>ID</th>
			<th>Name</th>
			<th>Master</th>
			<th><abbr title='Toughness'>Tgh</abbr></th>
			<th><abbr title='Condition damage'>Cnd</abbr></th>
			<th><abbr title='Healing'>Heal</abbr></th>
			<th><abbr title='Concentration'>Conc</abbr></th>
			<th>Profession</th>
			<th><abbr title='Elite Specialization'>Elite</abbr></th>
			<th>Species ID</th>
			<th>Hitbox Width</th>
			<th>Hitbox Height</th>
        </tr>
        </thead>
        <tbody>");
			foreach (var agent in stats.Agents)
			{
				if (agent is Player player)
				{
					writer.WriteLine($@"
            <tr>
                <td>Player</td>
                <td>{player.Id}</td>
                <td>{player.Name}</td>
                <td>{player.Master?.Name ?? ""}</td>
                <td>{player.Toughness}</td>
                <td>{player.Condition}</td>
                <td>{player.Healing}</td>
                <td>{player.Concentration}</td>
                <td>{player.Profession}</td>
                <td>{player.EliteSpecialization}</td>
                <td></td>
                <td>{player.HitboxWidth}</td>
                <td>{player.HitboxHeight}</td>
            </tr>");
				}
				else if (agent is NPC npc)
				{
					writer.WriteLine($@"
            <tr>
                <td>NPC</td>
                <td>{npc.Id}</td>
                <td>{npc.Name}</td>
                <td>{npc.Master?.Name ?? ""}</td>
                <td>{npc.Toughness}</td>
                <td>{npc.Condition}</td>
                <td>{npc.Healing}</td>
                <td>{npc.Concentration}</td>
                <td></td>
                <td></td>
                <td>{npc.SpeciesId}</td>
                <td>{npc.HitboxWidth}</td>
                <td>{npc.HitboxHeight}</td>
            </tr>");
				}
				else if (agent is Gadget gadget)
				{
					writer.WriteLine($@"
            <tr>
                <td>Gadget</td>
                <td>{gadget.Id}</td>
                <td>{gadget.Name}</td>
                <td>{gadget.Master?.Name ?? ""}</td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td>{gadget.HitboxWidth}</td>
                <td>{gadget.HitboxHeight}</td>
            </tr>");
				}
			}

			writer.WriteLine($@"
        </tbody>
    </table>");
		}

		private void WriteSkillList(TextWriter writer, LogStatistics stats)
		{
			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th>Skill ID</th>
			<th>Skill name</th>
        </tr>
        </thead>
        <tbody>");
			foreach (var skill in stats.Skills)
			{
				writer.WriteLine($@"
            <tr>
                <td>{skill.Id}</td>
                <td>{skill.Name}</td>
            </tr>");
			}

			writer.WriteLine($@"
        </tbody>
		<tfoot>
            <tr>
                <td>Total</td>
                <td>{stats.Skills.Count()} skills</td>
            </tr>
		</tfoot>
    </table>");
		}

		private void WriteBuffData(TextWriter writer, LogStatistics stats)
		{
			int graphId = 0;
			foreach (var agentBuffData in stats.BuffData.AgentBuffData.Values)
			{
				writer.WriteLine($"<div class='title is-4'>Agent: {agentBuffData.Agent.Name}</div>");

				foreach (var collections in agentBuffData.StackCollectionsBySkills.Values)
				{
					var buff = collections.Buff;
					var segments = collections.BuffSegments.ToArray();

					if (segments.Length == 0) continue;

					writer.WriteLine($"<div class='title is-6'>{buff.Name}</div>");

					var graphStartTime = segments.First().TimeStart;

					writer.Write(
						$"<div id=\"buff-graph{graphId}\" style=\"height: 400px;width:800px; display:inline-block \"></div>");
					writer.Write("<script>");
					writer.Write("document.addEventListener(\"DOMContentLoaded\", function() {");
					writer.Write("var data = [");
					writer.Write("{y: [");
					{
						foreach (var segment in segments)
						{
							writer.Write("'" + segment.StackCount + "',");
						}

						writer.Write("'" + segments.Last().StackCount + "'");
					}
					writer.Write("],");
					writer.Write("x: [");
					{
						foreach (var segment in segments)
						{
							double segmentStart = Math.Round(Math.Max(segment.TimeStart - graphStartTime, 0) / 1000.0,
								3);
							writer.Write($"'{segmentStart}',");
						}

						writer.Write("'" + Math.Round((segments.Last().TimeEnd - graphStartTime) / 1000.0, 3) + "'");
					}
					writer.Write("],");
					writer.Write("yaxis: 'y', type: 'scatter',");
					writer.Write(" line: {color:'red', shape: 'hv'},");
					writer.Write($" fill: 'tozeroy', name: \"{buff.Name}\"");
					writer.Write("}");
					writer.Write("];");
					writer.Write("var layout = {");
					writer.Write("barmode:'stack',");
					writer.Write(
						"yaxis: { title: 'Boons', fixedrange: true, dtick: 1.0, tick0: 0, gridcolor: '#909090' }," +
						"legend: { traceorder: 'reversed' }," +
						"hovermode: 'compare'," +
						"font: { color: '#000000' }," +
						"paper_bgcolor: 'rgba(255, 255, 255, 0)'," +
						"plot_bgcolor: 'rgba(255, 255, 255, 0)'");

					writer.Write("};");
					writer.Write($"Plotly.newPlot('buff-graph{graphId++}', data, layout);");
					writer.Write("});");
					writer.Write("</script> ");

					writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th>Stack Count</th>
			<th>Time From</th>
			<th>Time To</th>
        </tr>
        </thead>
        <tbody>");
					foreach (var segment in segments)
					{
						writer.WriteLine($@"
            <tr>
                <td>{segment.StackCount}</td>
                <td>{segment.TimeStart}</td>
                <td>{segment.TimeEnd}</td>
            </tr>");
					}

					writer.WriteLine($@"
        </tbody>
    </table>");
				}
			}
		}

		private void WriteEventCounts(TextWriter writer, LogStatistics stats)
		{
			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th>Event name</th>
			<th>Count</th>
        </tr>
        </thead>
        <tbody>");
			foreach (var eventCount in stats.EventCounts.OrderByDescending(x => x.Value))
			{
				var eventName = eventCount.Key;
				var count = eventCount.Value;
				writer.WriteLine($@"
            <tr>
                <td>{eventName}</td>
                <td>{count}</td>
            </tr>");
			}

			writer.WriteLine($@"
        </tbody>
		<tfoot>
            <tr>
                <td>Total</td>
                <td>{stats.EventCounts.Sum(x => x.Value)}</td>
            </tr>
		</tfoot>
    </table>");
		}

		private void WriteMenu(TextWriter writer, LogStatistics stats)
		{
			writer.WriteLine(@"
	<aside class='menu column is-3'>
		<p class='menu-label'>
			General
		</p>
		<ul class='menu-list'>
		  <li><a id='tablink-general' onclick='openTab(this)' class='scratch-tablink is-active'>Summary</a></li>");

			int enemyTargetIndex = 0;
			foreach (var target in stats.FullFightTargetDamageData)
			{
				writer.WriteLine($@"
          <li><a id='tablink-full-enemy-{enemyTargetIndex++}' onclick='openTab(this)' class='scratch-tablink'>{target.Target.Name}</a></li>");
			}

			writer.WriteLine(@"
		  <li><a>More?</a></li>
		</ul>
		<p class='menu-label'>
			Phases
		</p>
		<ul class='menu-list'>");

			int phaseIndex = 0;
			foreach (var phase in stats.PhaseStats)
			{
				writer.WriteLine($@"
			<li>
                <a id='tablink-phase-{phaseIndex}' onclick='openTab(this)' class='scratch-tablink'>{phase.PhaseName}</a>
                <ul>");

				int targetIndex = 0;
				foreach (var damageData in phase.TargetDamageData)
				{
					writer.WriteLine($@"
                    <li>
                        <a id='tablink-enemy-{phaseIndex}-{targetIndex++}' onclick='openTab(this)' class='scratch-tablink'>
                        {damageData.Target.Name}
                        </a>
                    </li>");
				}

				writer.WriteLine($@"
                </ul>
			</li>");

				phaseIndex++;
			}

			writer.WriteLine(@"
		</ul>
		<p class='menu-label'>
			Scratch Data
		</p>
		<ul class='menu-list'>
		  <li><a>Log version</a></li>
		  <li><a id='tablink-eventcounts' onclick='openTab(this)' class='scratch-tablink'>Event counts</a></li>
		  <li><a id='tablink-agents' onclick='openTab(this)' class='scratch-tablink'>Agent list</a></li>
		  <li><a id='tablink-skills' onclick='openTab(this)' class='scratch-tablink'>Skill list</a></li>
		  <li><a id='tablink-buffdata' onclick='openTab(this)' class='scratch-tablink'>Buff data</a></li>
		  <li><a>More?</a></li>
		</ul>
	</aside>");
		}

		private string MillisecondsToReadableFormat(long milliseconds)
		{
			return $"{milliseconds / 1000 / 60}m {milliseconds / 1000 % 60}s {milliseconds % 1000}ms";
		}

		private void WriteDamageTable(TextWriter writer, SquadDamageData squadDamageData)
		{
			writer.WriteLine($@"
    <table class='table is-narrow is-striped is-hoverable'>
        <thead>
        <tr>
            <th><abbr title='Subgroup'>Grp</abbr></th>
            <th>Class</th>
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
            <td>{specialization}</td>
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