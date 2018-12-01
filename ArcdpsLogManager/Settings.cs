using System;
using System.Dynamic;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace ArcdpsLogManager
{
	public class Settings
	{
		private static ISettings AppSettings => CrossSettings.Current;

		public static string LogRootPath
		{
			get => AppSettings.GetValueOrDefault(nameof(LogRootPath), string.Empty);

			set
			{
				if (value == LogRootPath) return;

				AppSettings.AddOrUpdateValue(nameof(LogRootPath), value);

                OnLogRootPathChanged();
			}
		}

		public static bool ShowDebugData
		{
			get => AppSettings.GetValueOrDefault(nameof(ShowDebugData), false);

			set
			{
				if (value == ShowDebugData) return;

				AppSettings.AddOrUpdateValue(nameof(ShowDebugData), value);
				OnShowDebugDataChanged();
			}
		}

		public static event EventHandler<EventArgs> LogRootPathChanged;
		public static event EventHandler<EventArgs> ShowDebugDataChanged;

		private static void OnLogRootPathChanged()
		{
			LogRootPathChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnShowDebugDataChanged()
		{
			ShowDebugDataChanged?.Invoke(null, EventArgs.Empty);
		}
	}
}