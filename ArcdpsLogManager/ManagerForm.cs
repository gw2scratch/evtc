using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ArcdpsLogManager.Controls;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using Eto.Forms;
using ScratchEVTCParser.Model.Encounters;

namespace ArcdpsLogManager
{
	public class ManagerForm : Form
	{
		private ImageProvider ImageProvider { get; } = new ImageProvider();
		private LogFinder LogFinder { get; } = new LogFinder();

		private List<LogData> logs = new List<LogData>();
		private SelectableFilterCollection<LogData> logsFiltered;

		public ManagerForm()
		{
			Title = "arcdps Log Manager";
			ClientSize = new Size(900, 700);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			var logLocationMenuItem = new ButtonMenuItem {Text = "&Log location"};
			logLocationMenuItem.Click += (sender, args) => { new LogSettingsDialog().ShowModal(); };
			logLocationMenuItem.Shortcut = Application.Instance.CommonModifier | Keys.L;

			var debugDataMenuItem = new CheckMenuItem {Text = "Show &Debug data"};
			debugDataMenuItem.Checked = Settings.ShowDebugData;
			debugDataMenuItem.CheckedChanged += (sender, args) =>
			{
				Settings.ShowDebugData = debugDataMenuItem.Checked;
			};

			var settingsMenuItem = new ButtonMenuItem {Text = "&Settings"};
			settingsMenuItem.Items.Add(logLocationMenuItem);
			settingsMenuItem.Items.Add(debugDataMenuItem);

			Menu = new MenuBar(settingsMenuItem);

			var logDetailPanel = ConstructLogDetailPanel();

			formLayout.BeginVertical(Padding = new Padding(5));

			formLayout.BeginGroup("Filters", new Padding(5));
			formLayout.BeginHorizontal();
			formLayout.Add("By encounter result:");
			formLayout.Add(new CheckBox {Text = "Success", Checked = true, Enabled = false}); // TODO: IMPLEMENT
			formLayout.Add(new CheckBox {Text = "Failure", Checked = true, Enabled = false}); // TODO: IMPLEMENT
			formLayout.Add(new CheckBox {Text = "Unknown", Checked = true, Enabled = false}); // TODO: IMPLEMENT
			formLayout.EndHorizontal();
			formLayout.EndGroup();

			formLayout.BeginHorizontal();

			var gridView = ConstructLogGridView(logDetailPanel);
			formLayout.Add(gridView, true);
			formLayout.Add(logDetailPanel);

			formLayout.EndHorizontal();

			formLayout.EndVertical();

			Task.Run(() => LoadLogs(gridView));
		}

		public async Task LoadLogs(GridView gridView)
		{
			await FindLogs();
			await ParseLogs(gridView);
		}

		public async Task FindLogs()
		{
			try
			{
				//foreach (var file in LogFinder.GetTesting())
				foreach (var file in LogFinder.GetFromDirectory(Settings.LogRootPath))
				{
					Application.Instance.Invoke(() => { logsFiltered.Add(file); });
				}
			}
			catch
			{
				// TODO: Handle exceptions properly
			}
		}

		public async Task ParseLogs(GridView gridView)
		{
			foreach (var log in logs)
			{
				log.ParseData();
				var index = logsFiltered.IndexOf(log);
				Application.Instance.Invoke(() => { gridView.ReloadData(index); });
			}
		}

		public LogDetailPanel ConstructLogDetailPanel()
		{
			return new LogDetailPanel(ImageProvider);
		}

		public GridView ConstructLogGridView(LogDetailPanel detailPanel)
		{
			var gridView = new GridView<LogData>();
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Encounter",
				DataCell = new TextBoxCell {Binding = new DelegateBinding<LogData, string>(x => x.EncounterName)}
			});

			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Result",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x =>
					{
						switch (x.EncounterResult)
						{
							case EncounterResult.Success:
								return "Success";
							case EncounterResult.Failure:
								return "Failure";
							case EncounterResult.Unknown:
								return "Unknown";
							default:
								throw new ArgumentOutOfRangeException();
						}
					})
				}
			});

			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Date",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x =>
					{
						if (x.EncounterStartTime == default)
						{
							return "Unknown";
						}

						return x.EncounterStartTime.ToLocalTime().DateTime.ToString(CultureInfo.CurrentCulture);
					})
				}
			});

			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Duration",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x =>
					{
						var seconds = x.EncounterDuration.TotalSeconds;
						return $"{seconds / 60:0}m {seconds % 60:00.0}s";
					})
				}
			});

			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Players",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(x => "Composition will be here")
				}
			});

			logsFiltered = new SelectableFilterCollection<LogData>(gridView, logs);

			gridView.DataStore = logsFiltered;

			gridView.SelectionChanged += (sender, args) => { detailPanel.LogData = gridView.SelectedItem; };

			return gridView;
		}
	}
}