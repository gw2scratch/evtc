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
using GW2Scratch.EVTCAnalytics.Model.Effects;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Processing;
using System.Globalization;

namespace GW2Scratch.EVTCInspector
{
	public sealed class InspectorForm : Form
	{
		private const string WindowTitle = "Scratch EVTC Inspector";
		private static readonly Padding MainTabPadding = new Padding(2);

		private string OpenedLogFile { get; set; } = null;
		private bool SkipParsing { get; set; } = false;
		private bool PruneForEncounterData { get; set; } = false;

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
		
		// Processed skills
		private readonly FilterCollection<Skill> skills = new FilterCollection<Skill>();
		private readonly SkillControl skillControl;
		
		// Processed effects
		private readonly FilterCollection<Effect> effects = new FilterCollection<Effect>();
			
		// Processed markers
		private readonly FilterCollection<Marker> markers = new FilterCollection<Marker>();

		// Processed species
		private readonly FilterCollection<Species> species = new FilterCollection<Species>();

		// Statistics
		private readonly PropertyGrid statisticsPropertyGrid = new PropertyGrid();

		public InspectorForm()
		{
			Title = WindowTitle;
			ClientSize = new Size(800, 600);
			var formLayout = new DynamicLayout();
			Content = formLayout;

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

			var agentSplitter = new Splitter {Panel1 = agentsGridView, Panel2 = agentsDetailLayout, Position = 400};

			skillControl = new SkillControl();
			var skillSplitter = new Splitter {Panel1 = ConstructSkillGridView(), Panel2 = skillControl, Position = 400};

			var processedTabControl = new TabControl();
			processedTabControl.Pages.Add(new TabPage(eventsDetailLayout) {Text = "Events"});
			processedTabControl.Pages.Add(new TabPage(agentSplitter) {Text = "Agents"});
			processedTabControl.Pages.Add(new TabPage(skillSplitter) {Text = "Skills"});
			processedTabControl.Pages.Add(new TabPage(ConstructEffectGridView()) {Text = "Effects"});
			processedTabControl.Pages.Add(new TabPage(ConstructMarkerGridView()) {Text = "Markers"});
			processedTabControl.Pages.Add(new TabPage(ConstructSpeciesGridView()) {Text = "Species"});

			var statisticsLayout = new DynamicLayout();
			statisticsLayout.Add(statisticsPropertyGrid);

			mainTabControl.Pages.Add(new TabPage(parsedTabControl) {Text = "Parsed data", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(processedTabControl) {Text = "Processed data", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(parsedStateLabel) {Text = "Log", Padding = MainTabPadding});
			mainTabControl.Pages.Add(new TabPage(statisticsLayout) {Text = "Statistics", Padding = MainTabPadding});

			formLayout.BeginVertical();
			formLayout.AddRow(mainTabControl);
			formLayout.EndVertical();
			
			var openFileMenuItem = new ButtonMenuItem {Text = "&Open EVTC log"};
			openFileMenuItem.Click += OpenFileButtonOnClick;
			openFileMenuItem.Shortcut = Application.Instance.CommonModifier | Keys.O;
			
			var reprocessMenuItem = new ButtonMenuItem {Text = "&Reprocess opened log"};
			reprocessMenuItem.Click += ReprocessButtonOnClick;
			reprocessMenuItem.Shortcut = Application.Instance.CommonModifier | Keys.R;

			var fileMenuItem = new ButtonMenuItem {Text = "&File"};
			fileMenuItem.Items.Add(openFileMenuItem);
			fileMenuItem.Items.Add(reprocessMenuItem);
			
			var skipParsingMenuItem = new CheckMenuItem {Text = "Merge parsing and processing into one step"};
			skipParsingMenuItem.Checked = SkipParsing;
			skipParsingMenuItem.CheckedChanged += (sender, args) => SkipParsing = skipParsingMenuItem.Checked;
			
			var pruneForEncounterDataMenuItem = new CheckMenuItem {Text = "Prune for encounter data (requires merged parsing and processing)"};
			pruneForEncounterDataMenuItem.Checked = PruneForEncounterData;
			pruneForEncounterDataMenuItem.CheckedChanged += (sender, args) => PruneForEncounterData = pruneForEncounterDataMenuItem.Checked;
			
			var showTimeSinceStartOfLogMenuItem = new CheckMenuItem {Text = "Show times since start of log"};
			showTimeSinceStartOfLogMenuItem.Checked = eventListControl.ShowTimeSinceFirstEvent;
			showTimeSinceStartOfLogMenuItem.CheckedChanged += (sender, args) =>
			{
				eventListControl.ShowTimeSinceFirstEvent = showTimeSinceStartOfLogMenuItem.Checked;
				agentControl.ShowTimeSinceFirstEvent = showTimeSinceStartOfLogMenuItem.Checked;
			};
			
			var optionsMenuItem = new ButtonMenuItem {Text = "&Options"};
			optionsMenuItem.Items.Add(skipParsingMenuItem);
			optionsMenuItem.Items.Add(pruneForEncounterDataMenuItem);
			optionsMenuItem.Items.Add(showTimeSinceStartOfLogMenuItem);

			Menu = new MenuBar(fileMenuItem, optionsMenuItem);
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
		
		private void ReprocessButtonOnClick(object s, EventArgs e)
		{
			if (OpenedLogFile != null) {
				SelectLog(OpenedLogFile);
			}
			else
			{
				MessageBox.Show("No log opened.", MessageBoxType.Error);
			}
		}

		public void SelectLog(string logFilePath)
		{
			OpenedLogFile = logFilePath;
			var filename = System.IO.Path.GetFileName(logFilePath);
			Title = $"{WindowTitle} – {filename}";
			var statusStringBuilder = new StringBuilder();

			var parser = new EVTCParser { SinglePassFilteringOptions = { PruneForEncounterData = PruneForEncounterData } };
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
					parsedLog = parser.ParseLog(logFilePath);
					var parseTime = sw.Elapsed;

					statusStringBuilder.AppendLine($"Parsed in {parseTime}");
					statusStringBuilder.AppendLine($"Boss ID: {parsedLog.ParsedBossData.ID}");

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
					processedLog = processor.ProcessLog(logFilePath, parser);
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
					eventListControl.TimeOfOldestEvent = eventList.Where(x => x is not UnknownEvent).Select(x => x.Time).DefaultIfEmpty().Min();
					eventListControl.Agents = processedLog.Agents.ToArray();
					agents.Clear();
					agents.AddRange(new FilterCollection<Agent>(processedLog.Agents));
					agents.Refresh();
					agentControl.Events = processedLog.Events.ToArray();
					skills.Clear();
					skills.AddRange(processedLog.Skills);
					effects.Clear();
					effects.AddRange(processedLog.Effects);
					markers.Clear();
					markers.AddRange(processedLog.Markers);
					species.Clear();
					species.AddRange(processedLog.Species);
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
				stats = new Statistics(
					processedLog.StartTime.LocalTime,
					processedLog.PointOfView,
					analyzer.GetResult(),
					analyzer.GetMode(),
					analyzer.GetEncounter(),
					processedLog.EvtcVersion,
					analyzer.GetEncounterDuration(),
					processedLog.GameBuild,
					processedLog.GameLanguage,
					processedLog.GameShardId,
					processedLog.MapId,
					processedLog.FractalScale,
					processedLog.Errors,
					processedLog.ArcdpsBuild
				);

				var statsTime = sw.Elapsed;

				statusStringBuilder.AppendLine($"Statistics generated in {statsTime}");

				Application.Instance.Invoke(() => { statisticsPropertyGrid.SelectedObject = stats; });
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
					$"Processed: {processedLog?.Events?.Count} events, {processedLog?.Agents?.Count} agents, {processedLog?.Skills?.Count} skills, {processedLog?.Effects?.Count} effects.");
				parsedStateLabel.Text = statusStringBuilder.ToString();
			});
		}

		private GridView<Skill> ConstructSkillGridView()
		{
			var grid = new GridView<Skill>();
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "ID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Skill, string>(x => x.Id.ToString())
				}
			});
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Skill, string>(x => x.Name)
				}
			});
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "Type",
				DataCell = new TextBoxCell
				{
					// TODO: Improve for older versions without SkillInfo
					Binding = new DelegateBinding<Skill, string>(x =>
					{
						return (x.SkillData != null, x.BuffData != null) switch
						{
							(true, true) => "Both?",
							(true, false) => "Ability",
							(false, true) => "Buff",
							(false, false) => "Unknown",
						};
					})
				}
			});
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "Content GUID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Skill, string>(x => GuidToString(x.ContentGuid))
				}
			});

			var menu = new ContextMenu();
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy ID",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = grid.SelectedItem.Id.ToString();
					}
				})
			});
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy Name",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = grid.SelectedItem.Name.ToString();
					}
				})
			});
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy GUID",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = GuidToString(grid.SelectedItem.ContentGuid);
					}
				})
			});
			grid.ContextMenu = menu;

			grid.SelectedItemsChanged += (_, _) => {skillControl.Skill = grid.SelectedItem;};
			grid.DataStore = skills;
			new GridViewSorter<Skill>(grid, skills).EnableSorting();

			return grid;
		}
		
		private GridView<Effect> ConstructEffectGridView()
		{
			var grid = new GridView<Effect>();
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "ID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Effect, string>(x => x.Id.ToString())
				}
			});
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "Content GUID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Effect, string>(x => GuidToString(x.ContentGuid))
				}
			});
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "Duration",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Effect, string>(x => x.DefaultDuration.ToString(CultureInfo.CurrentCulture))
				}
			});
			
			var menu = new ContextMenu();
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy ID",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = grid.SelectedItem.Id.ToString();
					}
				})
			});
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy GUID",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = GuidToString(grid.SelectedItem.ContentGuid);
					}
				})
			});
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy Duration",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = grid.SelectedItem.DefaultDuration.ToString();
					}
				})
			});
			grid.ContextMenu = menu;
			
			grid.DataStore = effects;
			new GridViewSorter<Effect>(grid, effects).EnableSorting();

			return grid;
		}
		
		private GridView<Marker> ConstructMarkerGridView()
		{
			var grid = new GridView<Marker>();
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "ID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Marker, string>(x => x.Id.ToString())
				}
			});
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "Content GUID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Marker, string>(x => GuidToString(x.ContentGuid))
				},
			});
			
			var menu = new ContextMenu();
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy ID",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = grid.SelectedItem.Id.ToString();
					}
				})
			});
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy GUID",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = GuidToString(grid.SelectedItem.ContentGuid);
					}
				})
			});
			grid.ContextMenu = menu;
			
			grid.DataStore = markers;
			new GridViewSorter<Marker>(grid, markers).EnableSorting();

			return grid;
		}

		private GridView<Species> ConstructSpeciesGridView()
		{
			var grid = new GridView<Species>();
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "ID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Species, string>(x => x.Id.ToString())
				}
			});
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "Content GUID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Species, string>(x => GuidToString(x.ContentGuid))
				},
			});

			var menu = new ContextMenu();
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy ID",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = grid.SelectedItem.Id.ToString();
					}
				})
			});
			menu.Items.Add(new ButtonMenuItem
			{
				Text = "Copy GUID",
				Command = new Command((_, _) =>
				{
					if (grid.SelectedItem != null)
					{
						Clipboard.Instance.Text = GuidToString(grid.SelectedItem.ContentGuid);
					}
				})
			});
			grid.ContextMenu = menu;

			grid.DataStore = species;
			new GridViewSorter<Species>(grid, species).EnableSorting();

			return grid;
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
				HeaderText = "Species ID",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Agent, string>(x =>
					{
						if (x is NPC npc)
						{
							return npc.SpeciesId.ToString();
						}
						return "N/A";
					})
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
				HeaderText = "Buff",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<Indexed<ParsedCombatItem>, string>(x => x.Item.Buff.ToString())
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

		private static string GuidToString(byte[] guidBytes)
		{
			string GetPart(byte[] bytes, int from, int to)
			{
				var builder = new StringBuilder();
				for (int i = from; i < to; i++)
				{
					builder.Append($"{bytes[i]:x2}");
				}

				return builder.ToString();
			}

			if (guidBytes == null) return null;
			if (guidBytes.Length != 16)
			{
				throw new ArgumentException("The GUID has to consist of 16 bytes", nameof(guidBytes));
			}

			return $"{GetPart(guidBytes, 0, 4)}-{GetPart(guidBytes, 4, 6)}-{GetPart(guidBytes, 6, 8)}" +
			       $"-{GetPart(guidBytes, 8, 10)}-{GetPart(guidBytes, 10, 16)}";
		}
	}
}