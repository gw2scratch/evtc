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
using ScratchEVTCParser.GW2Api.V2;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Parsed;
using ScratchEVTCParser.Statistics;
using ScratchLogHTMLGenerator;

namespace ScratchLogBrowser
{
	public class BrowserForm : Form
	{
		private static readonly Padding MainTabPadding = new Padding(2);

		private readonly OpenFileDialog openFileDialog;
		private readonly GridView<ParsedAgent> agentItemGridView;
		private readonly GridView<ParsedSkill> skillsGridView;
		private readonly GridView<ParsedCombatItem> combatItemsGridView;
		private readonly Label parsedStateLabel;

		// Processed events
		private readonly EventListControl eventListControl;

		private readonly FilterCollection<ParsedCombatItem>
			parsedCombatItems = new FilterCollection<ParsedCombatItem>();

		private readonly FilterCollection<ParsedAgent> parsedAgents = new FilterCollection<ParsedAgent>();
		private readonly FilterCollection<ParsedSkill> parsedSkills = new FilterCollection<ParsedSkill>();

		// Processed event filtering
		private readonly List<Event> eventList = new List<Event>();

		// Processed agents
		private readonly FilterCollection<Agent> agents = new FilterCollection<Agent>();
		private readonly AgentControl agentControl;

		// Statistics
		private readonly JsonSerializationControl statisticsJsonControl;

		// HTML
		private readonly ButtonMenuItem saveHtmlMenuItem;
		private readonly SaveFileDialog saveHtmlFileDialog;
		private readonly WebView webView = new WebView();
		private string LogHtml { get; set; } = "";

		// API data
		private readonly ApiDataSection apiDataSection;
		private GW2ApiData apiData = null;

		private GW2ApiData ApiData
		{
			get => apiData;
			set
			{
				apiData = value;
				apiDataSection.ApiData = apiData;
			}
		}

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

			var loadApiDataButton = new ButtonMenuItem {Text = "&Load Data from GW2 API"};
			loadApiDataButton.Click += LoadApiData;
			loadApiDataButton.Shortcut = Application.Instance.CommonModifier | Keys.L;

			var apiDataMenuItem = new ButtonMenuItem() {Text = "&GW2 API Data"};
			apiDataMenuItem.Items.Add(loadApiDataButton);

			Menu = new MenuBar(fileMenuItem, apiDataMenuItem);

			openFileDialog = new OpenFileDialog();
			openFileDialog.Filters.Add(new FileFilter("EVTC logs", ".evtc", ".evtc.zip"));

			saveHtmlFileDialog = new SaveFileDialog();
			saveHtmlFileDialog.Filters.Add(new FileFilter("HTML files", ".html", ".htm", "*.*"));

			parsedStateLabel = new Label {Text = "No log parsed yet."};

			agentItemGridView = new GridViewGenerator().GetGridView<ParsedAgent>();
			skillsGridView = new GridViewGenerator().GetGridView<ParsedSkill>();
			combatItemsGridView = new GridViewGenerator().GetGridView<ParsedCombatItem>();
			agentItemGridView.DataStore = parsedAgents;
			skillsGridView.DataStore = parsedSkills;
			combatItemsGridView.DataStore = parsedCombatItems;
			new GridViewSorter<ParsedAgent>(agentItemGridView, parsedAgents).EnableSorting();
			new GridViewSorter<ParsedSkill>(skillsGridView, parsedSkills).EnableSorting();
			new GridViewSorter<ParsedCombatItem>(combatItemsGridView, parsedCombatItems).EnableSorting();

			eventListControl = new EventListControl();

			var agentsGridView = new GridViewGenerator().GetGridView<Agent>();
			agentsGridView.DataStore = agents;
			agentsGridView.SelectedItemsChanged += AgentGridViewOnSelectedKeyChanged;
			agentsGridView.Columns.Insert(0, new GridColumn()
			{
				HeaderText = "Type",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<object, string>(x => x.GetType().Name)
				}
			});

			new GridViewSorter<Agent>(agentsGridView, agents).EnableSorting();

			agentControl = new AgentControl();
			var mainTabControl = new TabControl();

			var parsedTabControl = new TabControl();
			parsedTabControl.Pages.Add(new TabPage(agentItemGridView) {Text = "Agents"});
			parsedTabControl.Pages.Add(new TabPage(skillsGridView) {Text = "Skills"});
			parsedTabControl.Pages.Add(new TabPage(combatItemsGridView) {Text = "Combat Items"});

			var eventsDetailLayout = new DynamicLayout();
			eventsDetailLayout.BeginVertical();
			eventsDetailLayout.Add(eventListControl);
			eventsDetailLayout.EndVertical();

			var agentsDetailLayout = new DynamicLayout();
			agentsDetailLayout.BeginVertical();
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

			apiDataSection = new ApiDataSection();

			mainTabControl.Pages.Add(new TabPage(parsedTabControl) {Text = "Parsed data", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(processedTabControl)
				{Text = "Processed data", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(statisticsTabControl) {Text = "Statistics", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(htmlLayout) {Text = "HTML", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(parsedStateLabel) {Text = "Log", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(apiDataSection) {Text = "Api data", Padding = MainTabPadding});

			formLayout.BeginVertical();
			formLayout.AddRow(mainTabControl);
			formLayout.EndVertical();
		}

		private async void LoadApiData(object sender, EventArgs e)
		{
			var apiData = await GW2ApiData.LoadFromApi(new ApiSkillRepository());
			Application.Instance.Invoke(() => { ApiData = apiData; });
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
				string logFilename = openFileDialog.FileName;
				SelectLog(logFilename);
			}
		}

		public void SelectLog(string logFilename)
		{
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

				Application.Instance.Invoke(() =>
				{
					parsedAgents.Clear();
					parsedAgents.AddRange(parsedLog.ParsedAgents);
					parsedAgents.Refresh();
					parsedSkills.Clear();
					parsedSkills.AddRange(parsedLog.ParsedSkills);
					parsedSkills.Refresh();
					parsedCombatItems.Clear();
					parsedCombatItems.AddRange(parsedLog.ParsedCombatItems);
					parsedCombatItems.Refresh();
				});
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

				Application.Instance.Invoke(() =>
				{
					eventList.Clear();
					eventList.AddRange(processedLog.Events);
					eventListControl.Events = eventList;
					eventListControl.Agents = processedLog.Agents.ToArray();
					agents.Clear();
					agents.AddRange(new FilterCollection<Agent>(processedLog.Agents));
					agents.Refresh();
					agentControl.Events = processedLog.Events.ToArray();
				});
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
				stats = statisticsCalculator.GetStatistics(processedLog, ApiData);
				var statsTime = sw.Elapsed;

				statusStringBuilder.AppendLine($"Statistics generated in {statsTime}");

				// Way too huge by now
				//Application.Instance.Invoke(() => { statisticsJsonControl.Object = stats; });
			}
			catch (Exception ex)
			{
				statusStringBuilder.AppendLine($"Statistics generation failed: {ex.Message}");
			}

			// HTML
			var htmlStringWriter = new StringWriter();
			sw.Restart();
			try
			{
				generator.WriteHtml(htmlStringWriter, stats);
				var htmlTime = sw.Elapsed;

				statusStringBuilder.AppendLine($"HTML generated in {htmlTime}");

				Application.Instance.Invoke(() =>
				{
					webView.LoadHtml(htmlStringWriter.ToString());
					LogHtml = htmlStringWriter.ToString();
					saveHtmlMenuItem.Enabled = true;
				});
			}
			catch (Exception ex)
			{
				statusStringBuilder.AppendLine($"HTML generation failed: {ex.Message}");
			}

			Application.Instance.Invoke(() =>
			{
				statusStringBuilder.AppendLine(
					$"Build version: {parsedLog?.LogVersion?.BuildVersion}, revision {parsedLog?.LogVersion?.Revision}");
				statusStringBuilder.AppendLine(
					$"Parsed: {parsedLog?.ParsedAgents?.Length} agents, {parsedLog?.ParsedSkills?.Length} skills, {parsedLog?.ParsedCombatItems?.Length} combat items.");
				statusStringBuilder.AppendLine(
					$"Processed: {processedLog?.Events?.Count()} events, {processedLog?.Agents?.Count()} agents.");
				parsedStateLabel.Text = statusStringBuilder.ToString();
			});
		}
	}
}