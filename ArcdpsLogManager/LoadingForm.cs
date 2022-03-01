using System;
using System.Linq;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Caching;

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
				var form = new SettingsForm();
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
			LogCache cache = null;
			ApiData apiData = null;

			do
			{
				try
				{
					cache = LogCache.LoadFromFile();
				}
				catch (CacheLockedException)
				{
					Application.Instance.Invoke(() =>
					{
						MessageBox.Show("Only one instance of the log manager can be running at a time.", MessageBoxType.Error);
						Close();
					});
					return;
				}
				catch (Exception e)
				{
					bool abort = false;

					Application.Instance.Invoke(() =>
					{
						var result = MessageBox.Show("An error has occured while loading the stored log data. " +
						                             "This can be automatically resolved by deleting the cache file, " +
						                             "however, all logs will have to be processed again. Delete the cache?" +
						                             $"\n\nError: {e.Message}.", "Error loading the log cache.",
							MessageBoxButtons.YesNo, MessageBoxType.Error);

						if (result != DialogResult.Yes)
						{
							abort = true;
						}
						else
						{
							try
							{
								LogCache.DeleteFile();
							}
							catch (Exception deletionException)
							{
								MessageBox.Show("An error has occured while deleting the log cache. " +
								                $"\nError: {deletionException.Message}.", "Error deleting the log cache.", MessageBoxType.Error);
								abort = true;
							}
						}
					});

					if (abort)
					{
						Application.Instance.Invoke(Close);
						return;
					}
				}
			} while (cache == null);

			do
			{
				try
				{
					apiData = ApiData.LoadFromFile();
				}
				catch (Exception e)
				{
					bool abort = false;

					Application.Instance.Invoke(() =>
					{
						var result = MessageBox.Show("An error has occured while loading the stored API data. " +
						                             "This can be automatically resolved by deleting the API cache file. " +
						                             " Delete the cache?" +
						                             $"\n\nError: {e.Message}.", "Error loading the API cache.",
							MessageBoxButtons.YesNo, MessageBoxType.Error);

						if (result != DialogResult.Yes)
						{
							abort = true;
						}
						else
						{
							try
							{
								ApiData.DeleteFile();
							}
							catch (Exception deletionException)
							{
								MessageBox.Show("An error has occured while deleting the API cache. " +
								                $"\nError: {deletionException.Message}.", "Error deleting the API cache.", MessageBoxType.Error);
								abort = true;
							}
						}
					});

					if (abort)
					{
						Application.Instance.Invoke(Close);
						return;
					}
				}
			} while (apiData == null);

			Application.Instance.Invoke(() =>
			{
				var managerForm = new ManagerForm(cache, apiData);
				Application.Instance.MainForm = managerForm;
				managerForm.Show();

				Close();
			});
		}
	}
}