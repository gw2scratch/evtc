using System.Runtime.Remoting.Messaging;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using Eto.Forms;

namespace ArcdpsLogManager
{
	public class ManagerForm : Form
	{
		public ManagerForm()
		{
			Title = "arcdps Log Manager";
			ClientSize = new Size(800, 600);
			var formLayout = new DynamicLayout();
			Content = formLayout;

			var logLocationMenuItem = new ButtonMenuItem {Text = "&Log location"};
			logLocationMenuItem.Click += (sender, args) => { new LogSettingsDialog().ShowModal(); };
			logLocationMenuItem.Shortcut = Application.Instance.CommonModifier | Keys.L;

			var settingsMenuItem = new ButtonMenuItem {Text = "&Settings"};
			settingsMenuItem.Items.Add(logLocationMenuItem);

			Menu = new MenuBar(settingsMenuItem);

			formLayout.BeginVertical(Padding = new Padding(5));
			formLayout.BeginGroup("Filters", new Padding(5));
			formLayout.BeginHorizontal();
			formLayout.Add("By encounter result:");
			formLayout.Add(new CheckBox {Text = "Success", Checked = true, Enabled = false}); // TODO: IMPLEMENT
			formLayout.Add(new CheckBox {Text = "Failure", Checked = true, Enabled = false}); // TODO: IMPLEMENT
			formLayout.Add(new CheckBox {Text = "Unknown", Checked = true, Enabled = false}); // TODO: IMPLEMENT
			formLayout.EndHorizontal();
			formLayout.EndGroup();
			formLayout.Add(new Scrollable {Content = ConstructLogList()});
			formLayout.EndVertical();
		}

		public Control ConstructLogList()
		{
			var panel = new Panel();

			var logCollection = LogCollection.GetTesting();

			var layout = new TableLayout();
			layout.Spacing = new Size(0, 5);
			foreach (var log in logCollection.Logs)
			{
				var row = new LogRow(log);
				layout.Rows.Add(new TableRow(row));
			}

			panel.Content = layout;

			return panel;
		}
	}
}