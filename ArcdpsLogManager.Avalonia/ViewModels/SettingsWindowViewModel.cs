using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Configuration;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the Settings window (Avalonia counterpart of the Eto <c>SettingsForm</c> +
	/// its pages). Boolean/simple settings apply immediately through <see cref="ISettingsService"/>;
	/// the log directory list is applied via <see cref="ApplyDirectories"/>.
	/// </summary>
	public partial class SettingsWindowViewModel : ObservableObject
	{
		public ISettingsService Settings { get; }

		public ApplicationTheme[] Themes { get; } =
			{ ApplicationTheme.System, ApplicationTheme.Light, ApplicationTheme.Dark };

		public ObservableCollection<string> LogDirectories { get; }

		[ObservableProperty] private string minimumLogDurationText;

		public SettingsWindowViewModel(ISettingsService settings)
		{
			Settings = settings;
			LogDirectories = new ObservableCollection<string>(settings.LogRootPaths);
			minimumLogDurationText = settings.MinimumLogDurationSeconds?.ToString() ?? "";
		}

		public void AddDirectory(string path)
		{
			if (!string.IsNullOrWhiteSpace(path) && !LogDirectories.Contains(path))
			{
				LogDirectories.Add(path);
				ApplyDirectories();
			}
		}

		[RelayCommand]
		private void RemoveDirectory(string? path)
		{
			if (path != null && LogDirectories.Remove(path))
			{
				ApplyDirectories();
			}
		}

		private void ApplyDirectories()
		{
			Settings.LogRootPaths = LogDirectories.ToList();
		}

		partial void OnMinimumLogDurationTextChanged(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				Settings.MinimumLogDurationSeconds = null;
			}
			else if (int.TryParse(value, out int seconds) && seconds >= 0)
			{
				Settings.MinimumLogDurationSeconds = seconds;
			}
		}
	}
}
