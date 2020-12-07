using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Updates;

namespace GW2Scratch.ArcdpsLogManager.Dialogs
{
	public class UpdateDialog : Dialog
	{
		public UpdateDialog(LogDataProcessor logProcessor, IReadOnlyList<LogUpdateList> updates)
		{
			Title = "Manager update - arcdps Log Manager";
			ClientSize = new Size(500, -1);
			var layout = new DynamicLayout();
			Content = layout;

			layout.BeginVertical(new Padding(10), new Size(10, 20));
			{
				layout.BeginVertical(spacing: new Size(0, 0));
				{
					layout.AddRow(new Label
					{
						Text = "This new version of the log manager brings improvements for processing some of the logs. " +
						       "They have to be processed again to correctly update the data. " +
						       "Affected logs are listed below. Do you wish to do so now?",
						Wrap = WrapMode.Word
					});
					layout.AddRow(null);
				}
				layout.EndVertical();
				layout.AddRow(ConstructGridView(updates));
			}
			layout.EndVertical();

			var later = new Button {Text = "Later"};
			var yes = new Button {Text = "Yes"};
			later.Click += (sender, args) => Close();
			yes.Click += (sender, args) =>
			{
				foreach (var update in updates)
				{
					foreach (var log in update.UpdateableLogs)
					{
						logProcessor.Schedule(log);
					}
				}
				Close();
			};

			AbortButton = later;
			DefaultButton = yes;
			PositiveButtons.Add(yes);
			NegativeButtons.Add(later);
		}

		private GridView<LogUpdateList> ConstructGridView(IReadOnlyList<LogUpdateList> updates)
		{
			var grid = new GridView<LogUpdateList>();
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "Logs",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogUpdateList, string>(x => x.UpdateableLogs.Count.ToString())
				}
			});
			grid.Columns.Add(new GridColumn
			{
				HeaderText = "Reason",
				DataCell = new TextBoxCell
				{
					Binding = new DelegateBinding<LogUpdateList, string>(x => x.Update.Reason)
				}
			});
			grid.DataStore = updates;
			grid.SelectedRow = -1;
			grid.Height = 300;

			return grid;
		}
	}
}