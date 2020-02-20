using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Gw2Api
{
	public class ApiData
	{
		public const string NoGuildGuid = "00000000-0000-0000-0000-000000000000";
		public static readonly ApiGuild NoGuild = new ApiGuild {Tag = "", Name = "No guild"};

		private readonly ConcurrentDictionary<string, ApiGuild> guildDataCache;

		public int CachedGuildCount => guildDataCache.Count;

		private ApiData()
		{
			guildDataCache = new ConcurrentDictionary<string, ApiGuild>();
		}

		private ApiData(IReadOnlyDictionary<string, ApiGuild> cache)
		{
			guildDataCache = new ConcurrentDictionary<string, ApiGuild>(cache);
		}

		/// <summary>
		/// Loads saved API data from the cache file.
		/// </summary>
		public static ApiData LoadFromFile()
		{
			string filename = GetCacheFilename();

			if (File.Exists(filename))
			{
				using (var reader = File.OpenText(filename))
				{
					var serializer = new JsonSerializer();
					var dictionary = (Dictionary<string, ApiGuild>) serializer.Deserialize(reader,
						typeof(Dictionary<string, ApiGuild>));

					return new ApiData(dictionary);
				}
			}

			return new ApiData();
		}

		/// <summary>
		/// Save data to the cache file.
		/// </summary>
		public void SaveDataToFile()
		{
			string directory = GetCacheDirectory();
			string filename = GetCacheFilename();
			string tmpFilename = filename + ".tmp";

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			using (var writer = new StreamWriter(tmpFilename))
			{
				var serializer = new JsonSerializer();
				serializer.Serialize(writer, guildDataCache);
			}

			if (File.Exists(filename))
			{
				File.Delete(filename);
			}

			File.Move(tmpFilename, filename);
		}

		/// <summary>
		/// Deletes the file used to store the cache.
		/// </summary>
		public static void DeleteFile()
		{
			File.Delete(GetCacheFilename());
		}

		/// <summary>
		/// Provides the <see cref="FileInfo"/> of the cache file.
		/// </summary>
		public FileInfo GetCacheFileInfo()
		{
			return new FileInfo(GetCacheFilename());
		}

		/// <summary>
		/// Saves guild data for a specific GUID.
		/// </summary>
		public void AddGuildData(string guid, ApiGuild guild)
		{
			guildDataCache[guid] = guild;
		}

		public bool IsCached(string guid) => guildDataCache.ContainsKey(guid);

		/// <summary>
		/// Forgets all cached data.
		/// </summary>
		public void Clear()
		{
			guildDataCache.Clear();
			SaveDataToFile();
		}

		/// <summary>
		/// Get a guild name of a guild. In case the guild data has not been retrieved from the API,
		/// returns a name based on the GUID.
		/// </summary>
		/// <param name="guid">A guild GUID</param>
		/// <returns>The guild name, or an guid-based identifier if API data is not available.</returns>
		public string GetGuildName(string guid)
		{
			return GetApiGuild(guid)?.Name ?? $"Unknown ({guid.Substring(0, 7)})";
		}

		/// <summary>
		/// Get a guild tag of a guild.
		/// </summary>
		/// <param name="guid">A guild GUID</param>
		/// <param name="unavailableDefault">A value to be returned if the API data is not available.</param>
		/// <returns>The guild tag, or <paramref name="unavailableDefault"/> API data is not available.</returns>
		public string GetGuildTag(string guid, string unavailableDefault = "???")
		{
			return GetApiGuild(guid)?.Tag ?? unavailableDefault;
		}

		private ApiGuild GetApiGuild(string guid)
		{
			if (guid == null)
			{
				throw new ArgumentNullException(nameof(guid));
			}

			if (guid == NoGuildGuid)
			{
				return NoGuild;
			}

			if (guildDataCache.TryGetValue(guid, out var data))
			{
				return data;
			}

			return null;
		}

		private static string GetCacheDirectory()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Settings.AppDataDirectoryName);
		}

		private static string GetCacheFilename()
		{
			return Path.Combine(GetCacheDirectory(), Settings.ApiCacheFilename);
		}
	}
}