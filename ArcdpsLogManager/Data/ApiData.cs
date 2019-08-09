using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Data.Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using Gw2Sharp;
using Gw2Sharp.WebApi.Http;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;
using Emblem = GW2Scratch.ArcdpsLogManager.Data.Api.Emblem;
using File = System.IO.File;

namespace GW2Scratch.ArcdpsLogManager.Data
{
	public class ApiData : BackgroundProcessor<string>
	{
		private static readonly TimeSpan TooManyRequestsDelay = TimeSpan.FromMinutes(1);
		private const string NoGuildGuid = "00000000-0000-0000-0000-000000000000";

		public static readonly ApiGuild NoGuild = new ApiGuild {Tag = "", Name = "No guild"};

		private readonly Gw2Client apiClient;

		private readonly ConcurrentDictionary<string, ApiGuild> guildDataCache =
			new ConcurrentDictionary<string, ApiGuild>();

		public int CachedGuildCount => guildDataCache.Count;


		public ApiData(Gw2Client apiClient)
		{
			this.apiClient = apiClient;
		}

		public void LoadDataFromFile()
		{
			string filename = GetCacheFilename();

			if (File.Exists(filename))
			{
				using (var reader = File.OpenText(filename))
				{
					var serializer = new JsonSerializer();
					var dictionary = (Dictionary<string, ApiGuild>) serializer.Deserialize(reader,
						typeof(Dictionary<string, ApiGuild>));

					foreach (var pair in dictionary)
					{
						guildDataCache[pair.Key] = pair.Value;
					}
				}
			}
		}

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

		public FileInfo GetCacheFileInfo()
		{
			return new FileInfo(GetCacheFilename());
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

		protected override async Task Process(string item, CancellationToken cancellationToken)
		{
			Guild guild = null;
			bool retry;
			do
			{
				try
				{
					guild = await apiClient.WebApi.V2.Guild[item].GetAsync(cancellationToken);
					retry = false;
				}
				catch (TooManyRequestsException)
				{
					await Task.Delay(TooManyRequestsDelay, cancellationToken);
					retry = true;
				}
				catch (BadRequestException)
				{
					// Currently, for some guilds that do not exist the API return a 400 error.
					retry = false;
				}
				catch (NotFoundException)
				{
					retry = false;
				}
			} while (retry);

			if (guild != null)
			{
				Emblem emblem;
				if (guild.Emblem.Background.Id == 0 && guild.Emblem.Foreground.Id == 0)
				{
					// This is a workaround for Gw2Sharp defining the Emblem even if it doesn't exist.
					emblem = null;
				}
				else
				{
					emblem = new Emblem
					{
						Background = new EmblemPart
						{
							Id = guild.Emblem.Background.Id,
							Colors = guild.Emblem.Background.Colors.ToList()
						},
						Foreground = new EmblemPart
						{
							Id = guild.Emblem.Foreground.Id,
							Colors = guild.Emblem.Foreground.Colors.ToList()
						},
						Flags = guild.Emblem.Flags.List.Select(x => x.RawValue).ToList()
					};
				}

				var apiGuild = new ApiGuild
				{
					Id = item,
					Name = guild.Name,
					Tag = guild.Tag,
					Emblem = emblem
				};

				guildDataCache[item] = apiGuild;
			}
		}

		// Forgets all cached data.
		public void Clear()
		{
			guildDataCache.Clear();
			SaveDataToFile();
		}

		/// <summary>
		/// Register a guild GUID, potentially scheduling it for retrieval of data from the API.
		/// </summary>
		/// <param name="guid">A guild GUID</param>
		public void RegisterGuild(string guid)
		{
			if (guid == NoGuildGuid)
			{
				return;
			}

			if (!guildDataCache.ContainsKey(guid) && !IsScheduledOrBeingProcessed(guid))
			{
				Schedule(guid, false);
			}
		}

		/// <summary>
		/// Register a log, potentially scheduling all possible data for retrieval from the API.
		/// </summary>
		public void RegisterLog(LogData log)
		{
			foreach (var player in log.Players)
			{
				if (player.GuildGuid != null)
				{
					RegisterGuild(player.GuildGuid);
				}
			}
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