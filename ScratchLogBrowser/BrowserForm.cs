using System;
using System.Diagnostics;
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


		public BrowserForm()
		{
			ApplyEventFilters();

			Title = "Scratch EVTC Browser";
			ClientSize = new Size(800, 600);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			openFileDialog = new OpenFileDialog();
			openFileDialog.Filters.Add(new FileFilter("EVTC logs", ".evtc", ".evtc.zip"));

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
			eventsGridView.Width = 400;
			eventsGridView.DataStore = eventCollection;

			agentsGridView = new GridViewGenerator().GetGridView<Agent>();
			agentsGridView.SelectedItemsChanged += AgentGridViewOnSelectedKeyChanged;
			agentsGridView.Width = 400;
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

			var eventsLayout = new DynamicLayout();
			eventsLayout.BeginVertical();
			eventsLayout.BeginHorizontal();
			eventsLayout.Add(eventsGridView);
			eventsLayout.BeginVertical(new Padding(10));
			eventsLayout.BeginGroup("Filters", new Padding(10));
			eventAgentNameTextBox = new TextBox();
			eventAgentNameTextBox.TextChanged += (s, e) => eventCollection.Refresh();

			eventNameDropDown = new DropDown {DataStore = new[] {""}};
			eventNameDropDown.SelectedValueChanged += (s, e) => eventCollection.Refresh();
			eventsLayout.AddRow(new Label {Text = "Event type"}, eventNameDropDown, null);
			eventsLayout.AddRow(new Label {Text = "Agent name"}, eventAgentNameTextBox, null);
			eventsLayout.EndGroup();
			eventsLayout.Add(eventJsonControl.Control);
			eventsLayout.EndVertical();
			eventsLayout.EndHorizontal();
			eventsLayout.EndVertical();

			var agentsLayout = new DynamicLayout();
			agentsLayout.BeginVertical();
			agentsLayout.BeginHorizontal();
			agentsLayout.Add(agentsGridView);
			agentsLayout.BeginVertical();
			agentsLayout.Add(agentControl.Control);
			agentsLayout.EndVertical();
			agentsLayout.EndHorizontal();
			agentsLayout.EndVertical();

			var processedTabControl = new TabControl();
			processedTabControl.Pages.Add(new TabPage(eventsLayout) {Text = "Events"});
			processedTabControl.Pages.Add(new TabPage(agentsLayout) {Text = "Agents"});

			var statisticsTabControl = new TabControl();
			statisticsJsonControl = new JsonSerializationControl();
			statisticsTabControl.Pages.Add(new TabPage(statisticsJsonControl.Control) {Text = "General"});

			mainTabControl.Pages.Add(new TabPage(parsedTabControl) {Text = "Parsed data"});
			mainTabControl.Pages.Add(new TabPage(processedTabControl) {Text = "Processed data"});
			mainTabControl.Pages.Add(new TabPage(statisticsTabControl) {Text = "Statistics"});

			var openFileButton = new Button {Text = "Open log"};
			openFileButton.Click += OnOpenFileButtonOnClick;

			formLayout.BeginVertical();
			formLayout.AddRow(openFileButton);
			formLayout.AddRow(parsedStateLabel);
			formLayout.AddRow();
			formLayout.AddRow(mainTabControl);
			formLayout.EndVertical();
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

		private void OnOpenFileButtonOnClick(object s, EventArgs e)
		{
			var result = openFileDialog.ShowDialog((Control) s);
			if (result == DialogResult.Ok)
			{
				string logFilename = openFileDialog.Filenames.First();
				var parser = new EVTCParser();
				var sw = Stopwatch.StartNew();
				var parsedLog = parser.ParseLog(logFilename);
				var parseTime = sw.Elapsed;

				var processor = new LogProcessor();
				sw.Restart();
				var processedLog = processor.GetProcessedLog(parsedLog);
				var processTime = sw.Elapsed;

				var statisticsCalculator = new StatisticsCalculator();
				sw.Restart();
				var stats = statisticsCalculator.GetStatistics(processedLog);
				var statsTime = sw.Elapsed;

				var sb = new StringBuilder();
				sb.AppendLine($"Parsed in {parseTime}");
				sb.AppendLine($"Processed in {processTime}");
				sb.AppendLine($"Statistics generated in {statsTime}");
				sb.AppendLine(
					$"Build version: {parsedLog.LogVersion.BuildVersion}, revision {parsedLog.LogVersion.Revision}");
				sb.AppendLine(
					$"Parsed: {parsedLog.ParsedAgents.Length} agents, {parsedLog.ParsedSkills.Length} skills, {parsedLog.ParsedCombatItems.Length} combat items.");
				sb.AppendLine(
					$"Processed: {processedLog.Events.Count()} events, {processedLog.Agents.Count()} agents.");
				parsedStateLabel.Text = sb.ToString();

				agentItemGridView.DataStore = parsedLog.ParsedAgents.ToArray();
				skillsGridView.DataStore = parsedLog.ParsedSkills.ToArray();
				combatItemsGridView.DataStore = parsedLog.ParsedCombatItems.ToArray();

				eventCollection.Clear();
				eventCollection.AddRange(processedLog.Events);
				eventNameDropDown.DataStore =
					new[] {""}.Concat(
						eventCollection.Select(x => x.GetType().Name).Distinct().OrderBy(x => x)).ToArray();
				eventNameDropDown.SelectedIndex = 0;
				agentsGridView.DataStore = processedLog.Agents;

				agentControl.Events = processedLog.Events.ToArray();

				statisticsJsonControl.Object = stats;
			}
		}
	}
}