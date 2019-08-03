using System.Linq;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Data;

namespace GW2Scratch.ArcdpsLogManager
{
	public sealed class LoadingForm : Form
	{
		public LoadingForm()
		{
			var layout = new DynamicLayout();

			Icon = Resources.GetProgramIcon();
			Title = "Loading - arcdps Log Manager";
			ClientSize = new Size(200, 80);
			Resizable = false;
			Content = layout;

			layout.BeginCentered(spacing: new Size(5, 5));
			{
				layout.Add(null, yscale: true);
				layout.AddCentered("Loading the cache");
				layout.AddCentered(new ProgressBar {Indeterminate = true});
				layout.Add(null, yscale: true);
			}
			layout.EndCentered();

			if (!Settings.LogRootPaths.Any())
			{
				LoadComplete += (sender, args) => Task.Run(ShowInitialConfiguration);
			}
			else
			{
				LoadComplete += (sender, args) => Task.Run(LoadManager);
			}
		}

		private void ShowInitialConfiguration()
		{
			Application.Instance.Invoke(() =>
			{
				var form = new SettingsForm(null);
				form.Show();
				Visible = false;
				form.SettingsSaved += (sender, args) =>
				{
					Visible = true;
					Task.Run(LoadManager);
				};

				form.Closed += (sender, args) =>
				{
					// If the form was closed without saving, this form is still invisible, and we want to close
					// the whole program
					if (!Visible)
					{
						Close();
					}
				};
			});
		}

		private void LoadManager()
		{
			var cache = LogCache.LoadFromFile();

			Application.Instance.Invoke(() =>
			{
				var managerForm = new ManagerForm(cache);
				Application.Instance.MainForm = managerForm;
				managerForm.Show();

				Close();
			});
		}
	}
}