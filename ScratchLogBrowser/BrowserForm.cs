using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.Generator;
using ScratchEVTCParser;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Parsed;
using ScratchEVTCParser.Statistics;
using ScratchLogHTMLGenerator;

namespace ScratchLogBrowser
{
	public class BrowserForm : Form
	{
		private readonly OpenFileDialog openFileDialog;
		private readonly GridView<ParsedAgent> agentItemGridView;
		private readonly GridView<ParsedSkill> skillsGridView;
		private readonly GridView<ParsedCombatItem> combatItemsGridView;
		private readonly Label parsedStateLabel;

		// Processed events
		private readonly EventListControl eventListControl;

		// Processed event filtering
		private readonly List<Event> eventList = new List<Event>();

		// Processed agents
		private readonly GridView<Agent> agentsGridView;
		private readonly AgentControl agentControl;

		// Statistics
		private readonly JsonSerializationControl statisticsJsonControl;

		// HTML
        private readonly ButtonMenuItem saveHtmlMenuItem;
		private readonly SaveFileDialog saveHtmlFileDialog;
		private readonly WebView webView = new WebView();
		private string LogHtml { get; set; } = "";

		public BrowserForm()
		{
			Title = "Scratch EVTC Browser";
			ClientSize = new Size(800, 600);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			var openFileMenuItem = new ButtonMenuItem {Text = "&Open EVTC log"};
			openFileMenuItem.Click += OpenFileButtonOnClick;
			openFileMenuItem.Shortcut = Application.Instance.CommonModifier | Keys.O;
			saveHtmlMenuItem = new ButtonMenuItem {Text = "&Save HTML output", Enabled = false};
			saveHtmlMenuItem.Click += SaveHtmlButtonOnClick;
			saveHtmlMenuItem.Shortcut = Application.Instance.CommonModifier | Keys.S;

			var fileMenuItem = new ButtonMenuItem {Text = "&File"};
			fileMenuItem.Items.Add(openFileMenuItem);
			fileMenuItem.Items.Add(saveHtmlMenuItem);

			Menu = new MenuBar(fileMenuItem);

			openFileDialog = new OpenFileDialog();
			openFileDialog.Filters.Add(new FileFilter("EVTC logs", ".evtc", ".evtc.zip"));

			saveHtmlFileDialog = new SaveFileDialog();
			saveHtmlFileDialog.Filters.Add(new FileFilter("HTML files", ".html file", ".html", ".htm", "*.*"));

			parsedStateLabel = new Label {Text = "No log parsed yet."};

			agentItemGridView = new GridViewGenerator().GetGridView<ParsedAgent>();
			skillsGridView = new GridViewGenerator().GetGridView<ParsedSkill>();
			combatItemsGridView = new GridViewGenerator().GetGridView<ParsedCombatItem>();

			eventListControl = new EventListControl();

			agentsGridView = new GridViewGenerator().GetGridView<Agent>();
			agentsGridView.SelectedItemsChanged += AgentGridViewOnSelectedKeyChanged;
			agentsGridView.Columns.Insert(0, new GridColumn()
			{
				HeaderText = "Type",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<object, string>(x => x.GetType().Name)
				}
			});

			agentControl = new AgentControl();
			var mainTabControl = new TabControl();

			var parsedTabControl = new TabControl();
			parsedTabControl.Pages.Add(new TabPage(agentItemGridView) {Text = "Agents"});
			parsedTabControl.Pages.Add(new TabPage(skillsGridView) {Text = "Skills"});
			parsedTabControl.Pages.Add(new TabPage(combatItemsGridView) {Text = "Combat Items"});

			var eventsDetailLayout = new DynamicLayout();
			eventsDetailLayout.BeginVertical(new Padding(10));
			eventsDetailLayout.Add(eventListControl);
			eventsDetailLayout.EndVertical();

			var agentsDetailLayout = new DynamicLayout();
			agentsDetailLayout.BeginVertical(new Padding(10));
			agentsDetailLayout.Add(agentControl);
			agentsDetailLayout.EndVertical();

			var agentSplitter = new Splitter {Panel1 = agentsGridView, Panel2 = agentsDetailLayout, Position = 300};

			var processedTabControl = new TabControl();
			processedTabControl.Pages.Add(new TabPage(eventsDetailLayout) {Text = "Events"});
			processedTabControl.Pages.Add(new TabPage(agentSplitter) {Text = "Agents"});

			var statisticsTabControl = new TabControl();
			statisticsJsonControl = new JsonSerializationControl();
			statisticsTabControl.Pages.Add(new TabPage(statisticsJsonControl) {Text = "General"});

			var htmlLayout = new DynamicLayout();
			htmlLayout.AddRow(webView);

			mainTabControl.Pages.Add(new TabPage(parsedTabControl) {Text = "Parsed data"});
			mainTabControl.Pages.Add(new TabPage(processedTabControl) {Text = "Processed data"});
			mainTabControl.Pages.Add(new TabPage(statisticsTabControl) {Text = "Statistics"});
			mainTabControl.Pages.Add(new TabPage(htmlLayout) {Text = "HTML"});
			mainTabControl.Pages.Add(new TabPage(parsedStateLabel) {Text = "Log"});

			formLayout.BeginVertical();
			formLayout.AddRow(mainTabControl);
			formLayout.EndVertical();
		}

		private void SaveHtmlButtonOnClick(object sender, EventArgs e)
		{
			var result = saveHtmlFileDialog.ShowDialog(this);
			if (result == DialogResult.Ok)
			{
				File.WriteAllText(saveHtmlFileDialog.FileName, LogHtml);
			}
		}

		private void AgentGridViewOnSelectedKeyChanged(object sender, EventArgs e)
		{
			var gridView = (GridView<Agent>) sender;
			var agent = gridView.SelectedItem;
			agentControl.Agent = agent;
		}

		private void OpenFileButtonOnClick(object s, EventArgs e)
		{
			var result = openFileDialog.ShowDialog(this);
			if (result == DialogResult.Ok)
			{
				string logFilename = openFileDialog.Filenames.First();
				var statusStringBuilder = new StringBuilder();

                var parser = new EVTCParser();
				var processor = new LogProcessor();
				var statisticsCalculator = new StatisticsCalculator();
				var generator = new HtmlGenerator();

				// Parsing
                var sw = Stopwatch.StartNew();
				ParsedLog parsedLog = null;
				try
				{
					parsedLog = parser.ParseLog(logFilename);
					var parseTime = sw.Elapsed;

					statusStringBuilder.AppendLine($"Parsed in {parseTime}");

					agentItemGridView.DataStore = parsedLog.ParsedAgents.ToArray();
					skillsGridView.DataStore = parsedLog.ParsedSkills.ToArray();
					combatItemsGridView.DataStore = parsedLog.ParsedCombatItems.ToArray();
				}
				catch (Exception ex)
				{
					statusStringBuilder.AppendLine($"Parsing failed: {ex.Message}");
				}

				// Processing
				Log processedLog = null;
				try
				{
					sw.Restart();
					processedLog = processor.GetProcessedLog(parsedLog);
					var processTime = sw.Elapsed;

					statusStringBuilder.AppendLine($"Processed in {processTime}");

					eventList.Clear();
					eventList.AddRange(processedLog.Events);
					eventListControl.Events = eventList;
					agentsGridView.DataStore = processedLog.Agents;
					agentControl.Events = processedLog.Events.ToArray();
				}
				catch (Exception ex)
				{
					statusStringBuilder.AppendLine($"Processing failed: {ex.Message}");
				}

				// Statistics
				LogStatistics stats = null;
				sw.Restart();
				try
				{
					stats = statisticsCalculator.GetStatistics(processedLog);
					var statsTime = sw.Elapsed;

					statusStringBuilder.AppendLine($"Statistics generated in {statsTime}");

					statisticsJsonControl.Object = stats;
				}
				catch (Exception ex)
				{
					statusStringBuilder.AppendLine($"Statistics generation failed: {ex.Message}");
					throw;
				}

				// HTML
				var htmlStringWriter = new StringWriter();
				sw.Restart();
				try
				{
					generator.WriteHtml(htmlStringWriter, stats);
					var htmlTime = sw.Elapsed;

					statusStringBuilder.AppendLine($"HTML generated in {htmlTime}");

					webView.LoadHtml(htmlStringWriter.ToString());
					LogHtml = htmlStringWriter.ToString();
					saveHtmlMenuItem.Enabled = true;
				}
				catch (Exception ex)
				{
					statusStringBuilder.AppendLine($"HTML generation failed: {ex.Message}");
				}

				statusStringBuilder.AppendLine(
					$"Build version: {parsedLog?.LogVersion?.BuildVersion}, revision {parsedLog?.LogVersion?.Revision}");
				statusStringBuilder.AppendLine(
					$"Parsed: {parsedLog?.ParsedAgents?.Length} agents, {parsedLog?.ParsedSkills?.Length} skills, {parsedLog?.ParsedCombatItems?.Length} combat items.");
				statusStringBuilder.AppendLine(
					$"Processed: {processedLog?.Events?.Count()} events, {processedLog?.Agents?.Count()} agents.");
				parsedStateLabel.Text = statusStringBuilder.ToString();
			}
		}
	}
}