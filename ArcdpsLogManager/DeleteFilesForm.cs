using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager
{
	public class DeleteFilesForm : Form
	{
		private FilterCollection<LogData> dataStore;

		private readonly Button ConfirmDeleteButton = new Button() { Text = "Delete files" };
		private readonly Button CloseWindowButton = new Button() { Text = "Close" };
		private readonly Button RemoveSelectedButton = new Button() { Text = "Remove selected from list" };
		private readonly GridView LogGrid = new GridView();

		public DeleteFilesForm(IEnumerable<LogData> files)
		{
			dataStore = new FilterCollection<LogData>(files);
			var layout = new DynamicLayout();

			Icon = Resources.GetProgramIcon();
			Title = "Confirm Deletion - arcdps Log Manager";
			ClientSize = new Size(500, 300);
			Resizable = true;
			Content = layout;

			var FileNameColumn = new GridColumn()
			{
				HeaderText = "Files",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data => data.FileName)
				}
			};

			var BossNameColumn = new GridColumn()
			{
				HeaderText = "Boss",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data => data.Encounter.ToString())
				}
			};

			var EncounterModeColumn = new GridColumn()
			{
				HeaderText = "Mode",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data => data.EncounterMode.ToString())
				}
			};

			var EncounterResultColumn = new GridColumn()
			{
				HeaderText = "Result",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data => data.EncounterResult.ToString())
				}
			};

			LogGrid.DataStore = dataStore;
			LogGrid.AllowMultipleSelection = true;
			LogGrid.Columns.Add(FileNameColumn);
			LogGrid.Columns.Add(BossNameColumn);
			LogGrid.Columns.Add(EncounterModeColumn);
			LogGrid.Columns.Add(EncounterResultColumn);

			CloseWindowButton.Click += (sender, args) => Close();
			ConfirmDeleteButton.Click += ConfirmDeleteButtonClicked;
			RemoveSelectedButton.Click += RemoveSelectedButtonClicked;

			layout.BeginGroup("Files Locations", new Padding(5), new Size(0, 5));
			{
				layout.Add(LogGrid, yscale: true);
				layout.Add(null, yscale: false);
				layout.BeginVertical();
				{
					layout.BeginHorizontal();
					{
						layout.Add(RemoveSelectedButton, xscale: true);
						layout.Add(ConfirmDeleteButton, xscale: true);
						layout.Add(CloseWindowButton, xscale: true);
					}
					layout.EndHorizontal();
				}
				layout.EndVertical();
			}
			layout.EndGroup();
			Show();
		}

		private void RemoveSelectedButtonClicked(object sender, EventArgs e)
		{
			dataStore = new FilterCollection<LogData>(dataStore.Where(log => !LogGrid.SelectedItems.Contains(log)));
			LogGrid.DataStore = dataStore;
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
