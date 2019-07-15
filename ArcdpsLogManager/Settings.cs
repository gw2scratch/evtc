using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace GW2Scratch.ArcdpsLogManager
{
	public class Settings
	{
		public const string AppDataDirectoryName = "ArcdpsLogManager";
		public const string CacheFilename = "LogDataCache.json";

		private static ISettings AppSettings => CrossSettings.Current;

		public static string LogRootPath
		{
			get => AppSettings.GetValueOrDefault(nameof(LogRootPath), string.Empty);

			set
			{
				if (AppSettings.AddOrUpdateValue(nameof(LogRootPath), value))
				{
					OnLogRootPathChanged();
				}
			}
		}

		public static bool ShowDebugData
		{
			get => AppSettings.GetValueOrDefault(nameof(ShowDebugData), false);

			set
			{
				if (AppSettings.AddOrUpdateValue(nameof(ShowDebugData), value))
				{
					OnShowDebugDataChanged();
				}
			}
		}

		public static bool ShowSquadCompositions
		{
			get => AppSettings.GetValueOrDefault(nameof(ShowSquadCompositions), false);

			set
			{
				if (AppSettings.AddOrUpdateValue(nameof(ShowSquadCompositions), value))
				{
					OnShowSquadCompositionsChanged();
				}
			}
		}

		public static event EventHandler<EventArgs> LogRootPathChanged;
		public static event EventHandler<EventArgs> ShowDebugDataChanged;
		public static event EventHandler<EventArgs> ShowSquadCompositionsChanged;

		private static void OnLogRootPathChanged()
		{
			LogRootPathChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnShowDebugDataChanged()
		{
			ShowDebugDataChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnShowSquadCompositionsChanged()
		{
			ShowSquadCompositionsChanged?.Invoke(null, EventArgs.Empty);
		}
	}
}