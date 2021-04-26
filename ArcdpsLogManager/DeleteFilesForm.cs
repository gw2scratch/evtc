using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
		private ILogNameProvider nameProvider;
		private ImageProvider imageProvider;

		private readonly Button confirmDeleteButton = new Button() { Text = "Delete files" };
		private readonly Button closeWindowButton = new Button() { Text = "Close" };
		private readonly Button removeSelectedButton = new Button() { Text = "Remove selected from list" };
		private readonly GridView logGrid = new GridView();

		public int PlayerIconSize { get; private set; }
		public int PlayerIconSpacing { get; private set; }

		public DeleteFilesForm(IEnumerable<LogData> files, ILogNameProvider nameProvider, ImageProvider imageProvider)
		{
			this.nameProvider = nameProvider;
			this.imageProvider = imageProvider;
			dataStore = new FilterCollection<LogData>(files);
			var layout = new DynamicLayout();

			Icon = Resources.GetProgramIcon();
			Title = "Confirm Deletion - arcdps Log Manager";
			ClientSize = new Size(500, 300);
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

			var bossNameColumn = new GridColumn()
			{
				HeaderText = "Boss",
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

			logGrid.DataStore = dataStore;
			logGrid.AllowMultipleSelection = true;
			logGrid.Columns.Add(fileNameColumn);
			logGrid.Columns.Add(bossNameColumn);
			logGrid.Columns.Add(encounterModeColumn);
			logGrid.Columns.Add(encounterResultColumn);
			logGrid.Columns.Add(playersColumn);
			logGrid.CellDoubleClick += LogGrid_CellDoubleClick;

			closeWindowButton.Click += (sender, args) => Close();
			confirmDeleteButton.Click += ConfirmDeleteButtonClicked;
			removeSelectedButton.Click += RemoveSelectedButtonClicked;

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
			if(e.Item is LogData log)
			{
				Process.Start("explorer.exe", @log.FileInfo.DirectoryName);
			}
		}

		private void RemoveSelectedButtonClicked(object sender, EventArgs e)
		{
			dataStore = new FilterCollection<LogData>(dataStore.Where(log => !logGrid.SelectedItems.Contains(log)));
			logGrid.DataStore = dataStore;
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
