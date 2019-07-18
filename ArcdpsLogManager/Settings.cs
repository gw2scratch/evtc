using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace GW2Scratch.ArcdpsLogManager
{
	public static class Settings
	{
		public const string AppDataDirectoryName = "ArcdpsLogManager";
		public const string CacheFilename = "LogDataCache.json";
		public const string ApiCacheFilename = "ApiDataCache.json";

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
			get => AppSettings.GetValueOrDefault(nameof(ShowSquadCompositions), true);

			set
			{
				if (AppSettings.AddOrUpdateValue(nameof(ShowSquadCompositions), value))
				{
					OnShowSquadCompositionsChanged();
				}
			}
		}

		public static bool ShowGuildTagsInLogDetail
		{
			get => AppSettings.GetValueOrDefault(nameof(ShowGuildTagsInLogDetail), false);

			set
			{
				if (AppSettings.AddOrUpdateValue(nameof(ShowGuildTagsInLogDetail), value))
				{
					OnShowGuildTagsInLogDetailChanged();
				}
			}
		}

		public static bool UseGW2Api
		{
			get => AppSettings.GetValueOrDefault(nameof(UseGW2Api), false);

			set
			{
				if (AppSettings.AddOrUpdateValue(nameof(UseGW2Api), value))
				{
					OnUseGW2ApiChanged();
				}
			}
		}

		public static int ManagerFormWidth
		{
			get => AppSettings.GetValueOrDefault(nameof(ManagerFormWidth), 900);

			set
			{
				if (AppSettings.AddOrUpdateValue(nameof(ManagerFormWidth), value))
				{
					OnManagerFormWidthChanged();
				}
			}
		}

		public static int ManagerFormHeight
		{
			get => AppSettings.GetValueOrDefault(nameof(ManagerFormHeight), 700);

			set
			{
				if (AppSettings.AddOrUpdateValue(nameof(ManagerFormHeight), value))
				{
					OnManagerFormWidthChanged();
				}
			}
		}

		public static event EventHandler<EventArgs> LogRootPathChanged;
		public static event EventHandler<EventArgs> ShowDebugDataChanged;
		public static event EventHandler<EventArgs> ShowSquadCompositionsChanged;
		public static event EventHandler<EventArgs> ShowGuildTagsInLogDetailChanged;
		public static event EventHandler<EventArgs> UseGW2ApiChanged;
		public static event EventHandler<EventArgs> ManagerFormWidthChanged;
		public static event EventHandler<EventArgs> ManagerFormHeightChanged;

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

		private static void OnUseGW2ApiChanged()
		{
			UseGW2ApiChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnShowGuildTagsInLogDetailChanged()
		{
			ShowGuildTagsInLogDetailChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnManagerFormWidthChanged()
		{
			ManagerFormWidthChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnManagerFormHeightChanged()
		{
			ManagerFormHeightChanged?.Invoke(null, EventArgs.Empty);
		}
	}
}