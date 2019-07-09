using System;
using System.Collections.Generic;
using GW2Scratch.ArcdpsLogManager.GW2Api.V2;

namespace GW2Scratch.ArcdpsLogManager
{
	public class ApiData
	{
		private readonly GuildEndpoint guildEndpoint;

		private readonly Dictionary<string, ApiGuild> guildDataCache = new Dictionary<string, ApiGuild>();

		public ApiData(GuildEndpoint guildEndpoint)
		{
			this.guildEndpoint = guildEndpoint;
		}

		private ApiGuild GetApiGuild(string guid)
		{
			if (guid == null)
			{
				throw new ArgumentNullException(nameof(guid));
			}

			if (!guildDataCache.TryGetValue(guid, out var data))
			{
                var apiGuild = guildEndpoint.GetGuild(guid);
                guildDataCache[guid] = apiGuild;
                data = apiGuild;
			}

			return data;
		}

		public string GetGuildName(string guid)
		{
			return GetApiGuild(guid)?.Name;
		}

		public string GetGuildTag(string guid)
		{
			return GetApiGuild(guid)?.Tag;
		}
	}
}