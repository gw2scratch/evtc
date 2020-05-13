using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Uploads;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager
{
	public static class Settings
	{
		public const string AppDataDirectoryName = "ArcdpsLogManager";
		public const string CacheFilename = "LogDataCache.json";
		public const string ApiCacheFilename = "ApiDataCache.json";
		public const string SettingsFilename = "Settings.json";

		/// <summary>
		/// A class that is serialized to store the settings. Also used to define default values.
		/// </summary>
		private class StoredSettings
		{
			public List<string> LogRootPaths { get; set; } = new List<string>();
			public bool ShowDebugData { get; set; } = false;
			public bool ShowGuildTagsInLogDetail { get; set; } = false;
			public bool ShowFilterSidebar { get; set; } = true;
			public bool UseGW2Api { get; set; } = true;
			public string DpsReportUserToken { get; set; } = string.Empty;
			public string DpsReportDomain { get; set; } = DpsReportUploader.DefaultDomain.Domain;
			public int? MinimumLogDurationSeconds { get; set; } = null;
			public List<string> HiddenLogListColumns { get; set; } = new List<string>() {"Character", "Map ID", "Game Version", "arcdps Version"};

			public static StoredSettings LoadFromFile()
			{
				string filename = GetFilename();

				if (!File.Exists(filename))
				{
					return new StoredSettings();
				}

				using (var reader = File.OpenText(filename))
				{
					var serializer = new JsonSerializer();
					return (StoredSettings) serializer.Deserialize(reader, typeof(StoredSettings));
				}
			}
		}

		private static readonly object WriteLock = new object();

		private static StoredSettings Values => Stored.Value;

		private static readonly Lazy<StoredSettings> Stored = new Lazy<StoredSettings>(() =>
		{
			LogRootPathChanged += (sender, args) => SaveToFile();
			ShowDebugDataChanged += (sender, args) => SaveToFile();
			HiddenLogListColumnsChanged += (sender, args) => SaveToFile();
			ShowGuildTagsInLogDetailChanged += (sender, args) => SaveToFile();
			ShowFilterSidebarChanged += (sender, args) => SaveToFile();
			UseGW2ApiChanged += (sender, args) => SaveToFile();
			DpsReportUserTokenChanged += (sender, args) => SaveToFile();
			DpsReportDomainChanged += (sender, args) => SaveToFile();
			MinimumLogDurationSecondsChanged += (sender, args) => SaveToFile();

			return StoredSettings.LoadFromFile();
		});

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

		public static event EventHandler<EventArgs> LogRootPathChanged;
		public static event EventHandler<EventArgs> ShowDebugDataChanged;
		public static event EventHandler<EventArgs> HiddenLogListColumnsChanged;
		public static event EventHandler<EventArgs> ShowGuildTagsInLogDetailChanged;
		public static event EventHandler<EventArgs> ShowFilterSidebarChanged;
		public static event EventHandler<EventArgs> UseGW2ApiChanged;
		public static event EventHandler<EventArgs> DpsReportUserTokenChanged;
		public static event EventHandler<EventArgs> DpsReportDomainChanged;
		public static event EventHandler<EventArgs> MinimumLogDurationSecondsChanged;

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

		private static void OnShowGuildTagsInLogDetailChanged()
		{
			ShowGuildTagsInLogDetailChanged?.Invoke(null, EventArgs.Empty);
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
	}
}