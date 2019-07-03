using System;
using System.IO;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Statistics.Buffs;

namespace ScratchLogHTMLGenerator.Sections.ScratchData
{
	public class BuffDataPage : Page
	{
		private readonly BuffData buffData;

		public BuffDataPage(BuffData buffData, ITheme theme) : base("Buff data", true, theme)
		{
			this.buffData = buffData;
		}

		public override void WriteHtml(TextWriter writer)
		{
			int graphId = 0;
			foreach (var agentBuffData in buffData.AgentBuffData.Values)
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
	}
}