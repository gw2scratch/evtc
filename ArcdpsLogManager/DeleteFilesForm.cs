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
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
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

		private readonly Button confirmDeleteButton = new Button() { Text = "Delete files" };
		private readonly Button closeWindowButton = new Button() { Text = "Close" };
		private readonly Button removeSelectedButton = new Button() { Text = "Remove selected from list" };
		private readonly GridView gridView = new GridView();

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
						switch (data.EncounterMode)
						{
							case EncounterMode.Challenge:
								return "CM";
							case EncounterMode.Normal:
								return "";
							case EncounterMode.Unknown:
								return "?";
							default:
								throw new ArgumentOutOfRangeException();
						}
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

			gridView.RowHeight = Math.Max(gridView.RowHeight, PlayerIconSize + 2);
			gridView.DataStore = dataStore;
			gridView.AllowMultipleSelection = true;
			gridView.Columns.Add(fileNameColumn);
			gridView.Columns.Add(encounterNameColumn);
			gridView.Columns.Add(encounterModeColumn);
			gridView.Columns.Add(encounterResultColumn);
			gridView.Columns.Add(durationColumn);
			gridView.Columns.Add(playersColumn);
			gridView.CellDoubleClick += gridView_CellDoubleClick;

			closeWindowButton.Click += (sender, args) => Close();
			confirmDeleteButton.Click += ConfirmDeleteButtonClicked;
			removeSelectedButton.Click += RemoveSelectedButtonClicked;

			layout.BeginGroup("Files Locations", new Padding(5), new Size(0, 5));
			{
				layout.Add(gridView, yscale: true);
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

		private void gridView_CellDoubleClick(object sender, GridCellMouseEventArgs e)
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

		private void RemoveSelectedButtonClicked(object sender, EventArgs e)
		{
			dataStore = new FilterCollection<LogData>(dataStore.Where(log => !gridView.SelectedItems.Contains(log)));
			gridView.DataStore = dataStore;
		}

		private void ConfirmDeleteButtonClicked(object sender, EventArgs e)
		{
			//Call the delete function on the 
			DeleteFiles(dataStore.Select(log => log.FileName));
			Close();
		}

		private void DeleteFiles(IEnumerable<string> filesToDelete)
		{
			foreach (var file in filesToDelete)
			{
				// File.Delete doesnt throw an Error if the file doesnt exist
				//File.Delete(file);
			}
		}
	}
}
