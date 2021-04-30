using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Dialogs;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager
{
	/// <summary>
	/// Lists files before deleting them, the User can remove Files from the List as well before confirming.
	/// </summary>
	public class DeleteFilesForm : Form
	{
		private FilterCollection<LogData> dataStore;

		private readonly Button confirmDeleteButton = new Button() { Text = "Delete permanently listed logs" };
		private readonly Button closeWindowButton = new Button() { Text = "Close list" };
		private readonly Button removeSelectedButton = new Button() { Text = "Remove selected from list" };
		private readonly GridView logGrid = new GridView();

		private const int PlayerIconSize = 20;
		private const int PlayerIconSpacing = 5;

		public DeleteFilesForm(IEnumerable<LogData> files, ILogNameProvider nameProvider, ImageProvider imageProvider)
		{
			dataStore = new FilterCollection<LogData>(files);
			var layout = new DynamicLayout();

			Icon = Resources.GetProgramIcon();
			Title = "Confirm Deletion - arcdps Log Manager";
			ClientSize = new Size(750, 500);
			Resizable = true;
			Content = layout;

			var fileNameColumn = new GridColumn()
			{
				HeaderText = "Files",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data => data.FileInfo.Name)
				}
			};

			var encounterNameColumn = new GridColumn()
			{
				HeaderText = "Encounter",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data => nameProvider.GetName(data))
				}
			};

			var encounterModeColumn = new GridColumn()
			{
				HeaderText = "CM",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data =>
					{
						return data.EncounterMode switch {
							EncounterMode.Challenge => "CM",
							EncounterMode.Normal => "",
							EncounterMode.Unknown => "?",
							_ => throw new ArgumentOutOfRangeException(),
						};
					})
				}
			};

			var encounterResultColumn = new GridColumn()
			{
				HeaderText = "Result",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data => 
					{
						switch (data.EncounterResult)
						{
							case EncounterResult.Success:
								return "Success";
							case EncounterResult.Failure:
								if (Settings.ShowFailurePercentagesInLogList && data.HealthPercentage.HasValue)
								{
									return $"Failure ({data.HealthPercentage * 100:0.00}%)";
								}
								else
								{
									return "Failure";
								}
							case EncounterResult.Unknown:
								return "Unknown";
							default:
								throw new ArgumentOutOfRangeException();
						}
					})
				}
			};

			var durationColumn = new GridColumn() {
				HeaderText = "Duration",
				DataCell = new TextBoxCell {
					Binding = new DelegateBinding<LogData, string>(x => {
						var seconds = x.EncounterDuration.TotalSeconds;
						return $"{(int)seconds / 60:0}m {seconds % 60:00.0}s";
					})
				}
			};

			var compositionCell = new DrawableCell();
			compositionCell.Paint += (sender, args) => {
				if (!(args.Item is LogData log)) return;
				if (log.ParsingStatus != ParsingStatus.Parsed) return;


				var players = log.Players.OrderBy(player => player.Profession)
					.ThenBy(player => player.EliteSpecialization).ToArray();
				var origin = args.ClipRectangle.Location;
				for (int i = 0; i < players.Length; i++)
				{
					var icon = imageProvider.GetTinyProfessionIcon(players[i]);
					var point = origin + new PointF(i * (PlayerIconSize + PlayerIconSpacing), 0);
					args.Graphics.DrawImage(icon, point);
				}
			};

			var playersColumn = new GridColumn() 
			{
				HeaderText = "Players",
				DataCell = compositionCell,
				Width = 11 * (PlayerIconSize + PlayerIconSpacing) // There are logs with 11 players here and there
			};

			logGrid.RowHeight = Math.Max(logGrid.RowHeight, PlayerIconSize + 2);
			logGrid.DataStore = dataStore;
			logGrid.AllowMultipleSelection = true;
			logGrid.Columns.Add(fileNameColumn);
			logGrid.Columns.Add(encounterNameColumn);
			logGrid.Columns.Add(encounterModeColumn);
			logGrid.Columns.Add(encounterResultColumn);
			logGrid.Columns.Add(durationColumn);
			logGrid.Columns.Add(playersColumn);
			logGrid.CellDoubleClick += LogGrid_CellDoubleClick;

			closeWindowButton.Click += (sender, args) => Close();
			confirmDeleteButton.Click += ConfirmDeleteButtonClicked;
			removeSelectedButton.Click += (_, _) => RemoveItems(logGrid.SelectedItems);
			layout.BeginGroup("Files Locations", new Padding(5), new Size(0, 5));
			{
				layout.Add(logGrid, yscale: true);
				layout.Add(null, yscale: false);
				layout.BeginVertical();
				{
					layout.BeginHorizontal();
					{
						layout.Add(removeSelectedButton, xscale: true);
						layout.Add(confirmDeleteButton, xscale: true);
						layout.Add(closeWindowButton, xscale: true);
					}
					layout.EndHorizontal();
				}
				layout.EndVertical();
			}
			layout.EndGroup();
			Show();
		}

		private void LogGrid_CellDoubleClick(object sender, GridCellMouseEventArgs e)
		{
			if (e.Item is not LogData log) return;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Process.Start("explorer.exe", $"/select,\"{log.FileName}\"");
				return;
			}
			var processInfo = new ProcessStartInfo() {
				FileName = log.FileInfo.DirectoryName,
				UseShellExecute = true
			};
			Process.Start(processInfo);
		}


		private void ConfirmDeleteButtonClicked(object sender, EventArgs e)
		{
			var unreliablelogs = FindUnreliableLogs();
			if(unreliablelogs.Count() > 0)
			{
				UnreliableLogsFoundDialog dialog = new UnreliableLogsFoundDialog(unreliablelogs);
				dialog.ConfirmButton.Click += (_, _) => DeleteLogsAndClose(dataStore);
				dialog.RemoveUnreliableLogsButton.Click += (_, _) => RemoveItems(unreliablelogs);
				dialog.ShowModal();
			}
			else
			{
				DeleteLogsAndClose(dataStore);
			}
		}

		private void RemoveItems(IEnumerable<object> logs)
		{
			dataStore = new FilterCollection<LogData>(dataStore.Where(log => !logs.Contains(log)));
			logGrid.DataStore = dataStore;
		}

		private IEnumerable<LogData> FindUnreliableLogs()
		{
			return dataStore.Where(log =>
							log.EncounterResult != EncounterResult.Success &&
							(
								log.Encounter == Encounter.Arkk ||
								log.Encounter == Encounter.Artsariiv ||
								log.Encounter == Encounter.BanditTrio
							)
						);
		}

		private void DeleteLogsAndClose(IEnumerable<LogData> logs)
		{
			DeleteLogs(logs);
			if (Application.Instance.MainForm is ManagerForm mainform) mainform.ReloadLogs();
			Close();
		}

		private void DeleteLogs(IEnumerable<LogData> logs)
		{
			foreach (var log in logs)
			{
				File.Delete(log.FileName);
			}
		}
	}
}
