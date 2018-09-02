using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.Generator;
using ScratchEVTCParser;
using ScratchEVTCParser.Events;
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
		private readonly GridView<Event> eventsGridView;
		private readonly Panel eventPanel;

		public BrowserForm()
		{
			Title = "Scratch EVTC Browser";
			ClientSize = new Size(600, 400);
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

			eventPanel = new Panel();

			var mainTabControl = new TabControl();

			var parsedTabControl = new TabControl();
			parsedTabControl.Pages.Add(new TabPage(agentItemGridView) {Text = "Agents"});
			parsedTabControl.Pages.Add(new TabPage(skillsGridView) {Text = "Skills"});
			parsedTabControl.Pages.Add(new TabPage(combatItemsGridView) {Text = "Combat Items"});

			var eventsLayout = new DynamicLayout();
			eventsLayout.BeginVertical();
			eventsLayout.BeginHorizontal();
			eventsLayout.Add(eventsGridView);
			eventsLayout.BeginVertical();
			eventsLayout.Add(eventPanel);
			eventsLayout.EndVertical();
			eventsLayout.EndHorizontal();
			eventsLayout.EndVertical();

			var processedTabControl = new TabControl();
			processedTabControl.Pages.Add(new TabPage(eventsLayout) {Text = "Events"});

			mainTabControl.Pages.Add(new TabPage(parsedTabControl) {Text = "Parsed data"});
			mainTabControl.Pages.Add(new TabPage(processedTabControl) {Text = "Processed data"});

			var openFileButton = new Button {Text = "Open log"};

			openFileButton.Click += OnOpenFileButtonOnClick;

			formLayout.BeginVertical();
			formLayout.AddRow(openFileButton);
			formLayout.AddRow(parsedStateLabel);
			formLayout.AddRow();
			formLayout.AddRow(mainTabControl);
			formLayout.EndVertical();
		}

		private void EventsGridViewOnSelectedKeyChanged(object sender, EventArgs e)
		{
			var gridView = (GridView<Event>) sender;
			var ev = gridView.SelectedItem;
            var control = new EventControl {Event = ev};
            eventPanel.Content = control.Control;
		}

		private void OnOpenFileButtonOnClick(object s, EventArgs e)
		{
			var result = openFileDialog.ShowDialog((Control) s);
			if (result == DialogResult.Ok)
			{
				string logFilename = openFileDialog.Filenames.First();
				var parser = new EVTCParser();
				var sw = Stopwatch.StartNew();
				var log = parser.ParseLog(logFilename);
				var parseTime = sw.Elapsed;

				var processor = new LogProcessor();
				sw.Restart();
				var events = processor.GetEvents(log).ToArray();
				var processTime = sw.Elapsed;

				var sb = new StringBuilder();
				sb.AppendLine($"Parsed in {parseTime}");
				sb.AppendLine($"Processed in {processTime}");
				sb.AppendLine($"Build version: {log.LogVersion.BuildVersion}, revision {log.LogVersion.Revision}");
				sb.AppendLine(
					$"Parsed: {log.ParsedAgents.Length} agents, {log.ParsedSkills.Length} skills, {log.ParsedCombatItems.Length} combat items.");
				sb.AppendLine($"Processed: {events.Length} events.");
				parsedStateLabel.Text = sb.ToString();

				agentItemGridView.DataStore = log.ParsedAgents;
				skillsGridView.DataStore = log.ParsedSkills;
				combatItemsGridView.DataStore = log.ParsedCombatItems;

				eventsGridView.DataStore = events;
			}
		}
	}
}