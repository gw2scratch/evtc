using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.Generator;
using ScratchEVTCParser;
using ScratchEVTCParser.Parsed;

namespace ScratchLogBrowser
{
	public class BrowserForm : Form
	{
		public BrowserForm()
		{
			Title = "Scratch EVTC Browser";
			ClientSize = new Size(600, 400);
			var layout = new DynamicLayout();
			Content = layout;

			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filters.Add(new FileFilter("EVTC logs", ".evtc", ".evtc.zip"));

			var parsedStateLabel = new Label {Text = "No log parsed yet."};

			var agentItemGridView = new GridViewGenerator().GetGridView<ParsedAgent>();
			var skillsGridView = new GridViewGenerator().GetGridView<ParsedSkill>();
			var combatItemsGridView = new GridViewGenerator().GetGridView<ParsedCombatItem>();
			var tabControl = new TabControl();
			tabControl.Pages.Add(new TabPage(agentItemGridView) {Text = "Agents"});
			tabControl.Pages.Add(new TabPage(skillsGridView) {Text = "Skills"});
			tabControl.Pages.Add(new TabPage(combatItemsGridView) {Text = "Combat Items"});

			var openFileButton = new Button {Text = "Open log"};
			openFileButton.Click += (s, e) =>
			{
				var result = openFileDialog.ShowDialog((Control) s);
				if (result == DialogResult.Ok)
				{
					string logFilename = openFileDialog.Filenames.First();
					var parser = new EVTCParser();
					var sw = Stopwatch.StartNew();
					var log = parser.ParseLog(logFilename);

					var sb = new StringBuilder();
					sb.AppendLine($"Parsed in {sw.Elapsed}");
					sb.AppendLine($"Build version: {log.LogVersion.BuildVersion}, revision {log.LogVersion.Revision}");
					sb.AppendLine(
						$"Stats: {log.ParsedAgents.Length} agents, {log.ParsedSkills.Length} skills, {log.ParsedCombatItems.Length} combat items.");
					parsedStateLabel.Text = sb.ToString();
					agentItemGridView.DataStore = log.ParsedAgents;
					skillsGridView.DataStore = log.ParsedSkills;
					combatItemsGridView.DataStore = log.ParsedCombatItems;
				}
			};

			layout.BeginVertical();
			layout.AddRow(openFileButton);
			layout.AddRow(parsedStateLabel);
			layout.AddRow();
			layout.AddRow(tabControl);
			layout.EndVertical();
		}
	}
}