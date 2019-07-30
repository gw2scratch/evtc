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

			Shown += (sender, args) => Task.Run(LoadManager);
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