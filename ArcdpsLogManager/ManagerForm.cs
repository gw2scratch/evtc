using System;
using System.Globalization;
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

		public ManagerForm()
		{
			Title = "arcdps Log Manager";
			ClientSize = new Size(800, 600);
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

			formLayout.Add(ConstructLogGridView(GetLogList(), logDetailPanel), true);
			formLayout.Add(logDetailPanel);

			formLayout.EndHorizontal();

			formLayout.EndVertical();
		}

		public LogCollection GetLogList()
		{
			LogCollection logCollection;
			//logCollection = LogCollection.GetTesting();
			try
			{
				logCollection = LogCollection.GetFromDirectory(Settings.LogRootPath);
			}
			catch
			{
                // TODO: Handle exceptions properly
                logCollection = LogCollection.Empty;
			}

			return logCollection;
		}

		public LogDetailPanel ConstructLogDetailPanel()
		{
			return new LogDetailPanel(ImageProvider);
		}

		public GridView ConstructLogGridView(LogCollection logs, LogDetailPanel detailPanel)
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

			gridView.DataStore = new SelectableFilterCollection<LogData>(gridView, logs.Logs);

			gridView.SelectionChanged += (sender, args) => { detailPanel.LogData = gridView.SelectedItem; };

			return gridView;
		}
	}
}