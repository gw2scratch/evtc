using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Data
{
	public class LogCache
	{
		private readonly Dictionary<string, LogData> logsByFilename;
		private readonly object dictionaryLock = new object();

		public bool ChangedSinceLastSave { get; private set; } = false;

		public int LogCount
		{
			get
			{
				lock (dictionaryLock)
				{
					return logsByFilename.Count;
				}
			}
		}


		private LogCache(Dictionary<string, LogData> logsByFilename)
		{
			this.logsByFilename = logsByFilename;
		}

		public static LogCache LoadFromFile()
		{
			string filename = GetCacheFilename();
			if (File.Exists(filename))
			{
				using (var reader = File.OpenText(filename))
				{
					var serializer = new JsonSerializer();
					var dictionary = (Dictionary<string, LogData>) serializer.Deserialize(reader,
						typeof(Dictionary<string, LogData>));

					return new LogCache(dictionary);
				}
			}

			return new LogCache(new Dictionary<string, LogData>());
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

				using (var writer = new StreamWriter(tmpFilename))
				{
					var serializer = new JsonSerializer();
					serializer.Serialize(writer, logsByFilename);
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
	}
}