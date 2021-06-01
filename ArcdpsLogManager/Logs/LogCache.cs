using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GW2Scratch.ArcdpsLogManager.Logs.Caching;
using Newtonsoft.Json;
using System.Text;

namespace GW2Scratch.ArcdpsLogManager.Logs
{
	public class LogCache : IDisposable
	{
		private const string MutexName = "ArcdpsLogManagerLogCache";
		private readonly Dictionary<string, LogData> logsByFilename;
		private readonly object dictionaryLock = new object();
		private readonly Mutex globalMutex;

		public bool ChangedSinceLastSave { get; private set; } = false;

		public int LogCount
		{
			get {
				lock (dictionaryLock)
				{
					return logsByFilename.Count;
				}
			}
		}

		private LogCache(Dictionary<string, LogData> logsByFilename, Mutex globalMutex)
		{
			this.logsByFilename = logsByFilename;
			this.globalMutex = globalMutex;
		}

		public static LogCache LoadFromFile()
		{
			var globalMutex = new Mutex(true, $"Global\\{MutexName}", out bool createdNew);
			if (!createdNew)
			{
				globalMutex.Dispose();
				throw new CacheLockedException();
			}

			string filename = GetCacheFilename();
			if (File.Exists(filename))
			{
				using var reader = File.OpenText(filename);
				using var jsonReader = new JsonTextReader(reader);

				var serializer = new JsonSerializer();
				serializer.Converters.Add(new VersionConverter());
				var data = serializer.Deserialize<LogCacheStorage>(jsonReader);

				// Deserialize will not fail with old version, will just assign null instead
				if (data.LogsByFilename == null)
				{
					globalMutex.Dispose();
					throw new NotSupportedException("No log data found. This may be caused by trying " +
					                                "to load data from an incompatible old version.");
				}

				if (data.Version == 2)
				{
					return new LogCache(data.LogsByFilename, globalMutex);
				}

				globalMutex.Dispose();
				throw new NotSupportedException("Only version 2 of the log cache is supported. " +
				                                "Are you sure you are not trying to load " +
				                                "data from a newer version?");
			}

			return new LogCache(new Dictionary<string, LogData>(), globalMutex);
		}

		public void SaveToFile()
		{
			lock (dictionaryLock)
			{
				ChangedSinceLastSave = false;

				var directory = GetCacheDirectory();

				var filename = GetCacheFilename();
				var tmpFilename = filename + ".tmp";

				if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

				// We need to use a non-throwing replacement fallback for encoding because of an arcdps issue.
				// arcdps 20210324 generated directories that often contain invalid Unicode (the names contain
				// raw memory). This would only occur if guild or character name folders were enabled.
				// This issue was fixed in arcdps 20210327. Since this only lasted 3 days and the configuration
				// is not very common among arcdps users, we make do with a simple solution - replacing invalid
				// characters and always processing those logs again as the filename will never match
				// after the replacement.
				// TODO: A better solution would be nice.
				var encoding = Encoding.GetEncoding(
					Encoding.UTF8.CodePage,
					new EncoderReplacementFallback("�"),
					new DecoderExceptionFallback()
				);
				
				using (var writer = new StreamWriter(tmpFilename, false, encoding))
				{
					var serializer = new JsonSerializer();
					var storage = new LogCacheStorage() {
						LogsByFilename = logsByFilename
					};
					serializer.Serialize(writer, storage);
				}

				if (File.Exists(filename))
				{
					File.Delete(filename);
				}

				File.Move(tmpFilename, filename);
			}
		}

		public int GetUnloadedLogCount(IEnumerable<LogData> loadedLogs)
		{
			lock (dictionaryLock)
			{
				int loadedCount = logsByFilename.Values.ToList().Intersect(loadedLogs).Count();
				return LogCount - loadedCount;
			}
		}

		public static void DeleteFile()
		{
			File.Delete(GetCacheFilename());
		}

		public int Prune(IEnumerable<LogData> keptLogData)
		{
			lock (dictionaryLock)
			{
				ChangedSinceLastSave = true;

				int previousCount = LogCount;

				var logs = logsByFilename.Values.Intersect(keptLogData).ToList();
				logsByFilename.Clear();
				foreach (var log in logs)
				{
					logsByFilename[log.FileInfo.FullName] = log;
				}

				return previousCount - LogCount;
			}
		}

		public FileInfo GetCacheFileInfo()
		{
			return new FileInfo(GetCacheFilename());
		}

		public void Clear()
		{
			lock (dictionaryLock)
			{
				logsByFilename.Clear();
			}
		}

		public bool TryGetLogData(FileInfo fileInfo, out LogData data)
		{
			return TryGetLogData(fileInfo.FullName, out data);
		}

		public bool TryGetLogData(string filename, out LogData data)
		{
			lock (dictionaryLock)
			{
				if (logsByFilename.TryGetValue(filename, out var cached))
				{
					data = cached;
					return true;
				}

				data = null;
				return false;
			}
		}

		/// <summary>
		/// Save the cache data. This has to be called even if the mutable data is modified.
		/// </summary>
		/// <param name="logData">The log data that will be cached</param>
		public void CacheLogData(LogData logData)
		{
			lock (dictionaryLock)
			{
				logsByFilename[logData.FileInfo.FullName] = logData;
				ChangedSinceLastSave = true;
			}
		}

		private static string GetCacheDirectory()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Settings.AppDataDirectoryName);
		}

		private static string GetCacheFilename()
		{
			return Path.Combine(GetCacheDirectory(), Settings.CacheFilename);
		}

		public void Dispose()
		{
			globalMutex?.Dispose();
		}
	}
}