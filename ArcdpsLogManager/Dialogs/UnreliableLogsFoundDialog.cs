using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GW2Scratch.ArcdpsLogManager.Dialogs
{
	class UnreliableLogsFoundDialog : Dialog
	{
		public Button ConfirmButton;
		public Button CancelButton;
		public Button RemoveUnreliableLogsButton;
		public UnreliableLogsFoundDialog(IEnumerable<LogData> unreliableLogs)
		{
			Title = "Unreliable Logs found";
			ClientSize = new Size(500, 300);
			ShowInTaskbar = true;
			var layout = new DynamicLayout();
			Content = layout;

			ConfirmButton = new Button() { Text = "Confirm Deletion" };
			ConfirmButton.Click += Button_Clicked;
			CancelButton = new Button() { Text = "Cancel" };
			CancelButton.Click += Button_Clicked;
			RemoveUnreliableLogsButton = new Button() { Text = "Remove unreliable Logs" };
			RemoveUnreliableLogsButton.Click += Button_Clicked;

			var unreliableLogGridView = new GridView<LogData>();
			unreliableLogGridView.DataStore = unreliableLogs;
			unreliableLogGridView.Columns.Add(new GridColumn() {
				DataCell = new TextBoxCell() {
					Binding = new DelegateBinding<LogData, string>(log => log.FileName)
				}
			});

			layout.BeginGroup("WARNING", new Padding(5), new Size(0, 5));
			{
				layout.AddRow("Artsariiv, Arkk and Bandit Trio (Narella) have a rare unreliable success detection due to the way the encounter ends.");
				layout.AddRow("These logs listed may display a failure but might be successful. Confirm Deletion?");
			}
			layout.EndGroup();
			layout.BeginGroup("Files Locations", new Padding(5), new Size(0, 5));
			{
				layout.Add(unreliableLogGridView, yscale: true);
				layout.Add(null, yscale: false);
				layout.BeginVertical();
				{
					layout.BeginHorizontal();
					{
						layout.Add(RemoveUnreliableLogsButton, xscale: true);
						layout.Add(ConfirmButton, xscale: true);						
						layout.Add(CancelButton, xscale: true);
					}
					layout.EndHorizontal();
				}
				layout.EndVertical();
			}
			layout.EndGroup();

		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			Close();
		}
	}
}
