using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Dialogs;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Sections;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager
{
	/// <summary>
	/// Lists files before deleting them, the User can remove Files from the List as well before confirming.
	/// </summary>
	public class DeleteFilesForm : Form
	{
		private FilterCollection<LogData> dataStore;

		private readonly Button confirmDeleteButton = new Button { Text = "Delete permanently listed logs" };
		private readonly Button closeWindowButton = new Button { Text = "Close list" };
		private readonly Button removeSelectedButton = new Button { Text = "Remove selected from list" };
		private readonly LogList logList;

		public DeleteFilesForm(IEnumerable<LogData> files, LogCache logCache, ApiData apiData,
			LogDataProcessor logProcessor, UploadProcessor uploadProcessor, ImageProvider imageProvider,
			ILogNameProvider nameProvider)
		{
			dataStore = new FilterCollection<LogData>(files);
			var layout = new DynamicLayout();

			Icon = Resources.GetProgramIcon();
			Title = "Confirm Deletion - arcdps Log Manager";
			ClientSize = new Size(750, 500);
			Resizable = true;
			Content = layout;

			logList = new LogList(logCache, apiData, logProcessor, uploadProcessor, imageProvider, nameProvider, true);
			logList.DataStore = dataStore;

			closeWindowButton.Click += (sender, args) => Close();
			confirmDeleteButton.Click += ConfirmDeleteButtonClicked;
			removeSelectedButton.Click += (_, _) => RemoveItems(logList.SelectedItems);
			layout.BeginGroup("Files Locations", new Padding(5), new Size(0, 5));
			{
				layout.Add(logList, yscale: true);
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
		}

		private void ConfirmDeleteButtonClicked(object sender, EventArgs e)
		{
			var unreliablelogs = FindUnreliableLogs();
			if (unreliablelogs.Count() > 0)
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
			logList.DataStore = dataStore;
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