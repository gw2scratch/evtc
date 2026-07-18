using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Caching;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Services
{
	/// <summary>
	/// Owns the application's <see cref="LogCache"/>. Loads it through the real cache API (which
	/// takes a global mutex, so only one manager instance can own it at a time) and starts the
	/// periodic auto-saver. If the cache is already locked by another running manager instance,
	/// it degrades gracefully to a read-only load of the JSON so the grid still works, but
	/// persistence (e.g. favorite toggles) is disabled until the lock is free.
	/// </summary>
	public sealed class LogCacheService : IDisposable
	{
		private LogCache? cache;
		private LogCacheAutoSaver? autoSaver;

		/// <summary>True when the cache is owned by this instance and changes can be persisted.</summary>
		public bool IsWritable => cache != null;

		/// <summary>
		/// The writable cache instance, or null when the cache is read-only (locked by another
		/// manager instance). Used to look up cached data during a filesystem scan and to persist
		/// newly processed logs.
		/// </summary>
		public LogCache? Cache => cache;

		/// <summary>A human-readable note about the cache state (e.g. why it is read-only).</summary>
		public string? StateNote { get; private set; }

		/// <summary>
		/// Loads the cache. Returns all logs it contains. Never throws for the "already locked"
		/// case — it falls back to a read-only JSON load instead.
		/// </summary>
		public IReadOnlyList<LogData> Load()
		{
			try
			{
				cache = LogCache.LoadFromFile();
				autoSaver = LogCacheAutoSaver.StartNew(cache, TimeSpan.FromMinutes(1));
				return cache.GetAllLogs();
			}
			catch (CacheLockedException)
			{
				StateNote = "Log cache is in use by another manager instance — opened read-only " +
				            "(favorite changes will not be saved).";
				return LoadReadOnlyFromJson();
			}
		}

		private static IReadOnlyList<LogData> LoadReadOnlyFromJson()
		{
			string path = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Settings.AppDataDirectoryName,
				Settings.CacheFilename);

			if (!File.Exists(path))
			{
				return Array.Empty<LogData>();
			}

			var serializer = new JsonSerializer();
			serializer.Converters.Add(new VersionConverter());
			using var reader = File.OpenText(path);
			using var jsonReader = new JsonTextReader(reader);
			var storage = serializer.Deserialize<LogCacheStorage>(jsonReader);
			return storage?.LogsByFilename?.Values.ToList() ?? (IReadOnlyList<LogData>) Array.Empty<LogData>();
		}

		/// <summary>
		/// Records that a log's mutable data changed (e.g. a favorite toggle) so it is persisted by
		/// the auto-saver / on shutdown. No-op when the cache is read-only.
		/// </summary>
		public void NotifyChanged(LogData log)
		{
			cache?.CacheLogData(log);
		}

		public void Dispose()
		{
			try
			{
				if (cache is { ChangedSinceLastSave: true })
				{
					cache.SaveToFile();
				}
			}
			catch
			{
				// Best-effort save on shutdown; never throw from Dispose.
			}
			finally
			{
				cache?.Dispose();
				cache = null;
			}
		}
	}
}
