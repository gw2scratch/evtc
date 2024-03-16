using GW2Scratch.ArcdpsLogManager.Configuration.Migrations;
using GW2Scratch.ArcdpsLogManager.Sections.Clears;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GW2Scratch.ArcdpsLogManager.Configuration
{
	public static class Settings
	{
		public const string AppDataDirectoryName = "ArcdpsLogManager";
		public const string CacheFilename = "LogDataCache.json";
		public const string ApiCacheFilename = "ApiDataCache.json";
		public const string SettingsFilename = "Settings.json";

		private static readonly object WriteLock = new object();

		private static readonly List<ISettingsMigration> Migrations = new List<ISettingsMigration>()
		{
			// Null version is pre-1.4.0.0
			new NullVersionSettingsMigration(settings => settings.HiddenLogListColumns.Add("Instabilities"))
		};

		private static StoredSettings Values => Stored.Value;

		private static readonly Lazy<StoredSettings> Stored = new Lazy<StoredSettings>(() =>
		{
			LogRootPathChanged += (sender, args) => SaveToFile();
			ShowDebugDataChanged += (sender, args) => SaveToFile();
			HiddenLogListColumnsChanged += (sender, args) => SaveToFile();
			ShowGuildTagsInLogDetailChanged += (sender, args) => SaveToFile();
			ShowFailurePercentagesInLogListChanged += (sender, args) => SaveToFile();
			ShowFilterSidebarChanged += (sender, args) => SaveToFile();
			UseGW2ApiChanged += (sender, args) => SaveToFile();
			CheckForUpdatesChanged += (sender, args) => SaveToFile();
			DpsReportUserTokenChanged += (sender, args) => SaveToFile();
			DpsReportDomainChanged += (sender, args) => SaveToFile();
			MinimumLogDurationSecondsChanged += (sender, args) => SaveToFile();
			IgnoredUpdateVersionsChanged += (sender, args) => SaveToFile();
			DpsReportUploadDetailedWvwChanged += (sender, args) => SaveToFile();
			PlayerAccountNamesChanged += (sender, args) => SaveToFile();
			WeeklyClearGroupsChanged += (sender, args) => SaveToFile();

			return LoadFromFile();
		});

		public static StoredSettings LoadFromFile()
		{
			string filename = GetFilename();

			if (!File.Exists(filename))
			{
				return new StoredSettings();
			}

			using var reader = File.OpenText(filename);
			using var jsonReader = new JsonTextReader(reader);
			
			var serializer = new JsonSerializer { ObjectCreationHandling = ObjectCreationHandling.Replace };
			var settings = serializer.Deserialize<StoredSettings>(jsonReader);
			
			foreach (var migration in Migrations)
			{
				if (migration.Applies(settings.ManagerVersion))
				{
					migration.Apply(settings);
				}
			}
			
			settings.ManagerVersion = Assembly.GetExecutingAssembly().GetName().Version;
			
			return settings;
		}

		private static void SaveToFile()
		{
			string directory = GetDirectory();
			string filename = GetFilename();
			string tmpFilename = filename + ".tmp";

			lock (WriteLock)
			{
				if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

				using (var writer = new StreamWriter(tmpFilename))
				{
					var serializer = new JsonSerializer();
					serializer.Serialize(writer, Values);
				}

				if (File.Exists(filename))
				{
					File.Delete(filename);
				}

				File.Move(tmpFilename, filename);
			}
		}

		private static string GetDirectory()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				AppDataDirectoryName);
		}

		private static string GetFilename()
		{
			return Path.Combine(GetDirectory(), SettingsFilename);
		}

		public static IReadOnlyList<string> LogRootPaths
		{
			get => Values.LogRootPaths;
			set
			{
				if (!Equals(Values.LogRootPaths, value))
				{
					Values.LogRootPaths = value.ToList();
					OnLogRootPathsChanged();
				}
			}
		}

		public static bool ShowDebugData
		{
			get => Values.ShowDebugData;
			set
			{
				if (Values.ShowDebugData != value)
				{
					Values.ShowDebugData = value;
					OnShowDebugDataChanged();
				}
			}
		}

		public static IReadOnlyList<string> HiddenLogListColumns
		{
			get => Values.HiddenLogListColumns;
			set
			{
				if (!Equals(Values.HiddenLogListColumns, value))
				{
					Values.HiddenLogListColumns = value.ToList();
					OnHiddenLogListColumnsChanged();
				}
			}
		}

		public static bool ShowGuildTagsInLogDetail
		{
			get => Values.ShowGuildTagsInLogDetail;
			set
			{
				if (Values.ShowGuildTagsInLogDetail != value)
				{
					Values.ShowGuildTagsInLogDetail = value;
					OnShowGuildTagsInLogDetailChanged();
				}
			}
		}

		public static bool ShowFailurePercentagesInLogList
		{
			get => Values.ShowFailurePercentagesInLogList;
			set
			{
				if (Values.ShowFailurePercentagesInLogList != value)
				{
					Values.ShowFailurePercentagesInLogList = value;
					OnShowFailurePercentagesInLogListChanged();
				}
			}
		}

		public static bool ShowFilterSidebar
		{
			get => Values.ShowFilterSidebar;
			set
			{
				if (Values.ShowFilterSidebar != value)
				{
					Values.ShowFilterSidebar = value;
					OnShowFilterSidebarChanged();
				}
			}
		}

		public static bool UseGW2Api
		{
			get => Values.UseGW2Api;
			set
			{
				if (Values.UseGW2Api != value)
				{
					Values.UseGW2Api = value;
					OnUseGW2ApiChanged();
				}
			}
		}

		public static bool CheckForUpdates
		{
			get => Values.CheckForUpdates;
			set
			{
				if (Values.CheckForUpdates != value)
				{
					Values.CheckForUpdates = value;
					OnCheckForUpdatesChanged();
				}
			}
		}

		public static IReadOnlyList<string> IgnoredUpdateVersions
		{
			get => Values.IgnoredUpdateVersions;
			set
			{
				if (!Equals(Values.IgnoredUpdateVersions, value))
				{
					Values.IgnoredUpdateVersions = value.ToList();
					OnIgnoredUpdateVersionsChanged();
				}
			}
		}

		public static string DpsReportUserToken
		{
			get => Values.DpsReportUserToken;
			set
			{
				if (Values.DpsReportUserToken != value)
				{
					Values.DpsReportUserToken = value;
					OnDpsReportUserTokenChanged();
				}
			}
		}

		public static int? MinimumLogDurationSeconds
		{
			get => Values.MinimumLogDurationSeconds;
			set
			{
				if (Values.MinimumLogDurationSeconds != value)
				{
					Values.MinimumLogDurationSeconds = value;
					OnMinimumLogDurationSecondsChanged();
				}
			}
		}

		public static string DpsReportDomain
		{
			get => Values.DpsReportDomain;
			set
			{
				if (Values.DpsReportDomain != value)
				{
					Values.DpsReportDomain = value;
					OnDpsReportDomainChanged();
				}
			}
		}

		public static bool DpsReportUploadDetailedWvw
		{
			get => Values.DpsReportUploadDetailedWvw;
			set
			{
				if (Values.DpsReportUploadDetailedWvw != value)
				{
					Values.DpsReportUploadDetailedWvw = value;
					OnDpsReportUploadDetailedWvwChanged();
				}
			}
		}
		
		public static IReadOnlyList<string> PlayerAccountNames
		{
			get => Values.PlayerAccountNames;
			set
			{
				if (!Equals(Values.PlayerAccountNames, value))
				{
					Values.PlayerAccountNames = value.ToList();
					OnPlayerAccountNamesChanged();
				}
			}
		}
		
		public static IReadOnlyList<EncounterGroupId> WeeklyClearGroups
		{
			get => Values.WeeklyClearGroups;
			set
			{
				if (!Equals(Values.WeeklyClearGroups, value))
				{
					Values.WeeklyClearGroups = value.ToList();
					OnWeeklyClearGroupsChanged();
				}
			}
		}

		public static event EventHandler<EventArgs> LogRootPathChanged;
		public static event EventHandler<EventArgs> ShowDebugDataChanged;
		public static event EventHandler<EventArgs> HiddenLogListColumnsChanged;
		public static event EventHandler<EventArgs> ShowGuildTagsInLogDetailChanged;
		public static event EventHandler<EventArgs> ShowFailurePercentagesInLogListChanged;
		public static event EventHandler<EventArgs> ShowFilterSidebarChanged;
		public static event EventHandler<EventArgs> UseGW2ApiChanged;
		public static event EventHandler<EventArgs> CheckForUpdatesChanged;
		public static event EventHandler<EventArgs> DpsReportUserTokenChanged;
		public static event EventHandler<EventArgs> DpsReportDomainChanged;
		public static event EventHandler<EventArgs> MinimumLogDurationSecondsChanged;
		public static event EventHandler<EventArgs> IgnoredUpdateVersionsChanged;
		public static event EventHandler<EventArgs> DpsReportUploadDetailedWvwChanged;
		public static event EventHandler<EventArgs> PlayerAccountNamesChanged;
		public static event EventHandler<EventArgs> WeeklyClearGroupsChanged;

		private static void OnLogRootPathsChanged()
		{
			LogRootPathChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnShowDebugDataChanged()
		{
			ShowDebugDataChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnUseGW2ApiChanged()
		{
			UseGW2ApiChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnCheckForUpdatesChanged()
		{
			CheckForUpdatesChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnShowGuildTagsInLogDetailChanged()
		{
			ShowGuildTagsInLogDetailChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnShowFailurePercentagesInLogListChanged()
		{
			ShowFailurePercentagesInLogListChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnShowFilterSidebarChanged()
		{
			ShowFilterSidebarChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnDpsReportUserTokenChanged()
		{
			DpsReportUserTokenChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnMinimumLogDurationSecondsChanged()
		{
			MinimumLogDurationSecondsChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnHiddenLogListColumnsChanged()
		{
			HiddenLogListColumnsChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnDpsReportDomainChanged()
		{
			DpsReportDomainChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnIgnoredUpdateVersionsChanged()
		{
			IgnoredUpdateVersionsChanged?.Invoke(null, EventArgs.Empty);
		}

		private static void OnDpsReportUploadDetailedWvwChanged()
		{
			DpsReportUploadDetailedWvwChanged?.Invoke(null, EventArgs.Empty);
		}
		
		private static void OnPlayerAccountNamesChanged()
		{
			PlayerAccountNamesChanged?.Invoke(null, EventArgs.Empty);
		}
		
		private static void OnWeeklyClearGroupsChanged()
		{
			WeeklyClearGroupsChanged?.Invoke(null, EventArgs.Empty);
		}
	}
}