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

		private readonly Button ConfirmButton = new Button() { Text = "Confirm" };
		private readonly Button CancelButton = new Button() { Text = "Cancel" };
		private readonly Button RemoveSelectedButton = new Button() { Text = "Remove Selected" };
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


			LogGrid.DataStore = dataStore;
			LogGrid.AllowMultipleSelection = true;

			var fileNameColumn = new GridColumn()
			{
				HeaderText = "Files",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data => data.FileName)
				}
			};
			LogGrid.Columns.Add(fileNameColumn);

			var bossNameColumn = new GridColumn()
			{
				HeaderText = "Boss",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogData, string>(data => data.Encounter.ToString())
				}
			};
			LogGrid.Columns.Add(bossNameColumn);

			CancelButton.Click += (sender, args) => Close();
			ConfirmButton.Click += ConfirmButtonClicked;
			RemoveSelectedButton.Click += RemoveSelectedButtonClicked;


			layout.BeginGroup("Files Locations", new Padding(5), new Size(0, 5));
			{
				layout.Add(LogGrid, yscale: true);
				layout.Add(null, yscale: false);
				layout.BeginVertical();
				{
					layout.Add(RemoveSelectedButton, xscale: false);
					layout.BeginHorizontal();
					{
						layout.Add(ConfirmButton, xscale: true);
						layout.Add(CancelButton, xscale: true);
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

		private void ConfirmButtonClicked(object sender, EventArgs e)
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
