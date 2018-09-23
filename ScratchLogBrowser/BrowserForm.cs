using System;
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
		private readonly GridView<Event> eventsGridView;
		private readonly JsonSerializationControl eventJsonControl;

		// Processed event filtering
		private readonly TextBox eventAgentNameTextBox;
		private readonly DropDown eventNameDropDown;
		private readonly FilterCollection<Event> eventCollection = new FilterCollection<Event>();

		// Processed agents
		private readonly GridView<Agent> agentsGridView;
		private readonly AgentControl agentControl;

		// Statistics
		private readonly JsonSerializationControl statisticsJsonControl;

		// HTML
		private readonly Button saveHtmlButton;
		private readonly SaveFileDialog saveHtmlFileDialog;
		private readonly WebView webView = new WebView();
		private string LogHtml { get; set; } = "";


		public BrowserForm()
		{
			ApplyEventFilters();

			Title = "Scratch EVTC Browser";
			ClientSize = new Size(800, 600);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			openFileDialog = new OpenFileDialog();
			openFileDialog.Filters.Add(new FileFilter("EVTC logs", ".evtc", ".evtc.zip"));

			saveHtmlFileDialog = new SaveFileDialog();
			saveHtmlFileDialog.Filters.Add(new FileFilter(".html file", ".html", ".htm", ".*"));

			parsedStateLabel = new Label {Text = "No log parsed yet."};

			agentItemGridView = new GridViewGenerator().GetGridView<ParsedAgent>();
			skillsGridView = new GridViewGenerator().GetGridView<ParsedSkill>();
			combatItemsGridView = new GridViewGenerator().GetGridView<ParsedCombatItem>();

			eventsGridView = new GridView<Event>();
			eventsGridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Time",
				DataCell = new TextBoxCell("Time")
			});
			eventsGridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Event Type",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<object, string>(x => x.GetType().Name)
				}
			});
			eventsGridView.SelectedItemsChanged += EventsGridViewOnSelectedKeyChanged;
			eventsGridView.DataStore = eventCollection;

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

			eventJsonControl = new JsonSerializationControl();
			agentControl = new AgentControl();
			var mainTabControl = new TabControl();

			var parsedTabControl = new TabControl();
			parsedTabControl.Pages.Add(new TabPage(agentItemGridView) {Text = "Agents"});
			parsedTabControl.Pages.Add(new TabPage(skillsGridView) {Text = "Skills"});
			parsedTabControl.Pages.Add(new TabPage(combatItemsGridView) {Text = "Combat Items"});

			var eventsDetailLayout = new DynamicLayout();
			eventsDetailLayout.BeginVertical(new Padding(10));
			eventsDetailLayout.BeginGroup("Filters", new Padding(10));
			eventAgentNameTextBox = new TextBox();
			eventAgentNameTextBox.TextChanged += (s, e) => eventCollection.Refresh();
			eventNameDropDown = new DropDown {DataStore = new[] {""}};
			eventNameDropDown.SelectedValueChanged += (s, e) => eventCollection.Refresh();
			eventsDetailLayout.AddRow(new Label {Text = "Event type"}, eventNameDropDown, null);
			eventsDetailLayout.AddRow(new Label {Text = "Agent name"}, eventAgentNameTextBox, null);
			eventsDetailLayout.EndGroup();
			eventsDetailLayout.Add(eventJsonControl.Control);
			eventsDetailLayout.EndVertical();

			var eventsSplitter = new Splitter {Panel1 = eventsGridView, Panel2 = eventsDetailLayout, Position = 300};

			var agentsDetailLayout = new DynamicLayout();
			agentsDetailLayout.BeginVertical(new Padding(10));
			agentsDetailLayout.Add(agentControl.Control);
			agentsDetailLayout.EndVertical();

			var agentSplitter = new Splitter {Panel1 = agentsGridView, Panel2 = agentsDetailLayout, Position = 300};

			var processedTabControl = new TabControl();
			processedTabControl.Pages.Add(new TabPage(eventsSplitter) {Text = "Events"});
			processedTabControl.Pages.Add(new TabPage(agentSplitter) {Text = "Agents"});

			var statisticsTabControl = new TabControl();
			statisticsJsonControl = new JsonSerializationControl();
			statisticsTabControl.Pages.Add(new TabPage(statisticsJsonControl.Control) {Text = "General"});

			saveHtmlButton = new Button {Text = "Save .html file", Enabled = false};
			saveHtmlButton.Click += SaveHtmlButtonOnClick;
			var htmlLayout = new DynamicLayout();
			htmlLayout.AddRow(saveHtmlButton);
			htmlLayout.AddRow(webView);

			mainTabControl.Pages.Add(new TabPage(parsedTabControl) {Text = "Parsed data"});
			mainTabControl.Pages.Add(new TabPage(processedTabControl) {Text = "Processed data"});
			mainTabControl.Pages.Add(new TabPage(statisticsTabControl) {Text = "Statistics"});
			mainTabControl.Pages.Add(new TabPage(htmlLayout) {Text = "HTML"});

			var openFileButton = new Button {Text = "Open log"};
			openFileButton.Click += OpenFileButtonOnClick;

			formLayout.BeginVertical();
			formLayout.AddRow(openFileButton);
			formLayout.AddRow(parsedStateLabel);
			formLayout.AddRow();
			formLayout.AddRow(mainTabControl);
			formLayout.EndVertical();
		}

		private void SaveHtmlButtonOnClick(object sender, EventArgs e)
		{
			var result = saveHtmlFileDialog.ShowDialog((Control) sender);
			if (result == DialogResult.Ok)
			{
				File.WriteAllText(saveHtmlFileDialog.FileName, LogHtml);
			}
		}

		private void ApplyEventFilters()
		{
			eventCollection.Filter = ev =>
				EventFilters.IsAgentInEvent(ev, eventAgentNameTextBox.Text) &&
				EventFilters.IsTypeName(ev, (string) eventNameDropDown.SelectedValue);
		}

		private void AgentGridViewOnSelectedKeyChanged(object sender, EventArgs e)
		{
			var gridView = (GridView<Agent>) sender;
			var agent = gridView.SelectedItem;
			agentControl.Agent = agent;
		}

		private void EventsGridViewOnSelectedKeyChanged(object sender, EventArgs e)
		{
			var gridView = (GridView<Event>) sender;
			var ev = gridView.SelectedItem;
			eventJsonControl.Object = ev;
		}

		private void OpenFileButtonOnClick(object s, EventArgs e)
		{
			var result = openFileDialog.ShowDialog((Control) s);
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

					eventCollection.Clear();
					eventCollection.AddRange(processedLog.Events);
					eventNameDropDown.DataStore =
						new[] {""}.Concat(
							eventCollection.Select(x => x.GetType().Name).Distinct().OrderBy(x => x)).ToArray();
					eventNameDropDown.SelectedIndex = 0;
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
					saveHtmlButton.Enabled = true;
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