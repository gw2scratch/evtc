using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Data.Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using Gw2Sharp;
using Gw2Sharp.WebApi.Http;
using Gw2Sharp.WebApi.V2.Models;
using Emblem = GW2Scratch.ArcdpsLogManager.Data.Api.Emblem;

namespace GW2Scratch.ArcdpsLogManager.Data
{
	public class ApiProcessor : BackgroundProcessor<string>
	{
		private static readonly TimeSpan TooManyRequestsDelay = TimeSpan.FromMinutes(1);
		public ApiData ApiData { get; }
		private readonly Gw2Client apiClient;

		public ApiProcessor(ApiData apiData, Gw2Client apiClient)
		{
			ApiData = apiData ?? throw new ArgumentNullException(nameof(apiData));
			this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
		}
		protected override async Task Process(string item, CancellationToken cancellationToken)
		{
			Guild guild = null;
			bool notFound = false;
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
					notFound = true;
					retry = false;
				}
				catch (NotFoundException)
				{
					notFound = true;
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
				ApiData.AddGuildData(item, apiGuild);
			}
			else if (notFound)
			{
				ApiData.AddGuildData(item, null);
			}
		}


		/// <summary>
		/// Register a guild GUID, potentially scheduling it for retrieval of data from the API.
		/// </summary>
		/// <param name="guid">A guild GUID</param>
		public void RegisterGuild(string guid)
		{
			if (guid == ApiData.NoGuildGuid)
			{
				return;
			}

			if (!ApiData.IsCached(guid) && !IsScheduledOrBeingProcessed(guid))
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

	}
}