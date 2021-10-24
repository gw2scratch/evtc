using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Processing;

namespace GW2Scratch.EVTCInspector
{
	public sealed class InspectorForm : Form
	{
		private static readonly Padding MainTabPadding = new Padding(2);

		private bool SkipParsing { get; set; } = false;

		private readonly OpenFileDialog openFileDialog;
		private readonly Label parsedStateLabel;

		// Processed events
		private readonly EventListControl eventListControl;

		private readonly FilterCollection<Indexed<ParsedCombatItem>> parsedCombatItems = new FilterCollection<Indexed<ParsedCombatItem>>();
		private readonly FilterCollection<Indexed<ParsedAgent>> parsedAgents = new FilterCollection<Indexed<ParsedAgent>>();
		private readonly FilterCollection<Indexed<ParsedSkill>> parsedSkills = new FilterCollection<Indexed<ParsedSkill>>();

		// Processed event filtering
		private readonly List<Event> eventList = new List<Event>();

		// Processed agents
		private readonly FilterCollection<Agent> agents = new FilterCollection<Agent>();
		private readonly AgentControl agentControl;

		// Statistics
		private readonly JsonSerializationControl statisticsJsonControl = new JsonSerializationControl();

		public InspectorForm()
		{
			Title = "Scratch EVTC Inspector";
			ClientSize = new Size(800, 600);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			var openFileMenuItem = new ButtonMenuItem {Text = "&Open EVTC log"};
			openFileMenuItem.Click += OpenFileButtonOnClick;
			openFileMenuItem.Shortcut = Application.Instance.CommonModifier | Keys.O;

			var fileMenuItem = new ButtonMenuItem {Text = "&File"};
			fileMenuItem.Items.Add(openFileMenuItem);
			
			var skipParsingMenuItem = new CheckMenuItem {Text = "Merge parsing and processing into one step"};
			skipParsingMenuItem.Checked = SkipParsing;
			skipParsingMenuItem.CheckedChanged += (sender, args) => SkipParsing = skipParsingMenuItem.Checked;
			
			var optionsMenuItem = new ButtonMenuItem {Text = "&Options"};
			optionsMenuItem.Items.Add(skipParsingMenuItem);

			Menu = new MenuBar(fileMenuItem, optionsMenuItem);

			openFileDialog = new OpenFileDialog();
			openFileDialog.Filters.Add(new FileFilter("EVTC logs", ".evtc", ".evtc.zip", ".zevtc"));

			parsedStateLabel = new Label {Text = "No log parsed yet."};

			eventListControl = new EventListControl();

			var agentsGridView = ConstructAgentGridView();
			agentControl = new AgentControl();
			var mainTabControl = new TabControl();

			var parsedTabControl = new TabControl();
			parsedTabControl.Pages.Add(new TabPage(ConstructParsedAgentGridView()) {Text = "Agents"});
			parsedTabControl.Pages.Add(new TabPage(ConstructParsedSkillGridView()) {Text = "Skills"});
			parsedTabControl.Pages.Add(new TabPage(ConstructParsedCombatItemGridView()) {Text = "Combat Items"});

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

			var statisticsLayout = new DynamicLayout();
			statisticsLayout.Add(statisticsJsonControl);

			mainTabControl.Pages.Add(new TabPage(parsedTabControl) {Text = "Parsed data", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(processedTabControl)
				{Text = "Processed data", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(parsedStateLabel) {Text = "Log", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(statisticsLayout) {Text = "Statistics", Padding = MainTabPadding});

			formLayout.BeginVertical();
			formLayout.AddRow(mainTabControl);
			formLayout.EndVertical();
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
			var processor = new LogProcessor()
			{
				IgnoreUnknownEvents = false
			};

			// Parsing
			var sw = Stopwatch.StartNew();
			ParsedLog parsedLog = null;
			if (!SkipParsing)
			{
				try
				{
					parsedLog = parser.ParseLog(logFilename);
					var parseTime = sw.Elapsed;

					statusStringBuilder.AppendLine($"Parsed in {parseTime}");

					Application.Instance.Invoke(() =>
					{
						parsedAgents.Clear();
						parsedAgents.AddRange(parsedLog.ParsedAgents.Select((x, i) => new Indexed<ParsedAgent>(x, i)));
						parsedAgents.Refresh();
						parsedSkills.Clear();
						parsedSkills.AddRange(parsedLog.ParsedSkills.Select((x, i) => new Indexed<ParsedSkill>(x, i)));
						parsedSkills.Refresh();
						parsedCombatItems.Clear();
						parsedCombatItems.AddRange(
							parsedLog.ParsedCombatItems.Select((x, i) => new Indexed<ParsedCombatItem>(x, i)));
						parsedCombatItems.Refresh();
					});
				}
				catch (Exception ex)
				{
					statusStringBuilder.AppendLine($"Parsing failed: {ex.Message}\n{ex.StackTrace}");
				}
			}
			else
			{
				Application.Instance.Invoke(() =>
				{
					parsedAgents.Clear();
					parsedAgents.Refresh();
					parsedSkills.Clear();
					parsedSkills.Refresh();
					parsedCombatItems.Clear();
					parsedCombatItems.Refresh();
				});
				statusStringBuilder.AppendLine($"Parsing as a separate step skipped.");
			}

			// Processing
			Log processedLog = null;
			try
			{
				sw.Restart();
				if (!SkipParsing)
				{
					processedLog = processor.ProcessLog(parsedLog);
				}
				else
				{
					processedLog = processor.ProcessLog(logFilename, parser);
				}

				var processTime = sw.Elapsed;

				if (SkipParsing)
				{
					statusStringBuilder.AppendLine($"Parsed and processed in {processTime}.");
				}
				else
				{
					statusStringBuilder.AppendLine($"Processed in {processTime}");
				}

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
				statusStringBuilder.AppendLine($"Processing failed: {ex.Message}\n{ex.StackTrace}");
			}

			// Statistics
			Statistics stats = null;
			sw.Restart();
			try
			{
				var analyzer = new LogAnalyzer(processedLog);
				stats = new Statistics(processedLog.StartTime.LocalTime,
					processedLog.PointOfView,
					analyzer.GetResult(),
					analyzer.GetMode(),
					analyzer.GetEncounter(),
					processedLog.EvtcVersion,
					analyzer.GetEncounterDuration());

				var statsTime = sw.Elapsed;

				statusStringBuilder.AppendLine($"Statistics generated in {statsTime}");

				Application.Instance.Invoke(() => { statisticsJsonControl.Object = stats; });
			}
			catch (Exception ex)
			{
				statusStringBuilder.AppendLine($"Statistics generation failed: {ex.Message}\n{ex.StackTrace}");
			}

			Application.Instance.Invoke(() =>
			{
				statusStringBuilder.AppendLine(
					$"Build version: {parsedLog?.LogVersion?.BuildVersion}, revision {parsedLog?.LogVersion?.Revision}");
				statusStringBuilder.AppendLine(
					$"Parsed: {parsedLog?.ParsedAgents?.Count} agents, {parsedLog?.ParsedSkills?.Count} skills, {parsedLog?.ParsedCombatItems?.Count} combat items.");
				statusStringBuilder.AppendLine(
					$"Processed: {processedLog?.Events?.Count} events, {processedLog?.Agents?.Count} agents.");
				parsedStateLabel.Text = statusStringBuilder.ToString();
			});
		}

		private GridView<Agent> ConstructAgentGridView()
		{
			var agentsGridView = new GridView<Agent>();
			agentsGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Type",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<object, string>(x => x.GetType().Name)
				}
			});
			agentsGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Agent Origin",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Agent, string>(x => x.AgentOrigin.Merged
						? "Merged"
						: $"{x.AgentOrigin.OriginalAgentData[0].Address} | {x.AgentOrigin.OriginalAgentData[0].Id}")
				}
			});
			agentsGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Agent, string>(x => x.Name)
				}
			});
			agentsGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Hitbox Width",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Agent, string>(x => x.HitboxWidth.ToString())
				}
			});
			agentsGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Hitbox Height",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Agent, string>(x => x.HitboxHeight.ToString())
				}
			});
			agentsGridView.Columns.Add(new GridColumn
			{
				HeaderText = "First Aware Time",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Agent, string>(x => x.FirstAwareTime.ToString())
				}
			});
			agentsGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Last Aware Time",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Agent, string>(x => x.LastAwareTime.ToString())
				}
			});
			agentsGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Minions",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Agent, string>(x => x.Minions.Count.ToString())
				}
			});
			agentsGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Master",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Agent, string>(x => x.Master?.Name ?? "")
				}
			});

			agentsGridView.DataStore = agents;
			agentsGridView.SelectedItemsChanged += AgentGridViewOnSelectedKeyChanged;

			new GridViewSorter<Agent>(agentsGridView, agents).EnableSorting();

			return agentsGridView;
		}

		private GridView<Indexed<ParsedAgent>> ConstructParsedAgentGridView()
		{
			var gridView = new GridView<Indexed<ParsedAgent>>();
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Index",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Index.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Address",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Item.Address.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => $"{x.Item.Name.TrimEnd('\0').Replace("\0", "\\0")}")
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Prof",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Item.Prof.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsElite",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Item.IsElite.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Toughness",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Item.Toughness.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Concentration",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Item.Concentration.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Healing",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Item.Healing.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Condition",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Item.Condition.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Hitbox Width",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Item.HitboxWidth.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Hitbox Height",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedAgent>, string>(x => x.Item.HitboxHeight.ToString())
				}
			});
			
			gridView.DataStore = parsedAgents;
			new GridViewSorter<Indexed<ParsedAgent>>(gridView, parsedAgents).EnableSorting();

			return gridView;
		}
		
		private GridView<Indexed<ParsedSkill>> ConstructParsedSkillGridView()
		{
			var gridView = new GridView<Indexed<ParsedSkill>>();
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Index",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedSkill>, string>(x => x.Index.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "ID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedSkill>, string>(x => x.Item.SkillId.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedSkill>, string>(x => $"{x.Item.Name.TrimEnd('\0').Replace("\0", "\\0")}")
				}
			});
			
			gridView.DataStore = parsedSkills;
			new GridViewSorter<Indexed<ParsedSkill>>(gridView, parsedSkills).EnableSorting();
			
			return gridView;
		}
		
		private GridView<Indexed<ParsedCombatItem>> ConstructParsedCombatItemGridView()
		{
			var gridView = new GridView<Indexed<ParsedCombatItem>>();
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Index",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x => x.Index.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Time",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x => x.Item.Time.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "SrcAgent",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.SrcAgent.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "DstAgent",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.DstAgent.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Value",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.Value.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "BuffDmg",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.BuffDmg.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "OverstackValue",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.OverstackValue.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "SkillId",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.SkillId.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "SrcAgentId",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.SrcAgentId.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "DstAgentId",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.DstAgentId.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "SrcMasterId",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.SrcMasterId.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "DstMasterId",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.DstMasterId.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Iff",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x => x.Item.Iff.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "BuffDmg",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x => x.Item.BuffDmg.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Result",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x => x.Item.Result.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsActivation",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.IsActivation.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsBuffRemove",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.IsBuffRemove.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsNinety",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.IsNinety.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsFifty",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.IsFifty.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsMoving",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.IsMoving.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsStateChange",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.IsStateChange.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsFlanking",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.IsFlanking.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsShields",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.IsShields.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "IsOffCycle",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.IsOffCycle.ToString())
				}
			});
			gridView.Columns.Add(new GridColumn
			{
				HeaderText = "Padding",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x =>
						x.Item.Padding.ToString())
				}
			});
			
			gridView.DataStore = parsedCombatItems;
			new GridViewSorter<Indexed<ParsedCombatItem>>(gridView, parsedCombatItems).EnableSorting();
			
			return gridView;
		}
	}
}