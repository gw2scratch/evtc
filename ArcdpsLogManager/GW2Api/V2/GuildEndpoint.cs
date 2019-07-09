using RestSharp;

namespace GW2Scratch.ArcdpsLogManager.GW2Api.V2
{
	public class GuildEndpoint
	{
		public ApiGuild GetGuild(string guid)
		{
			var client = new RestClient("https://api.guildwars2.com/");

			var request = new RestRequest("v2/guild/{guid}");
			request.AddParameter("guid", guid, ParameterType.UrlSegment);

			var response = client.Execute<ApiGuild>(request);

			return response.IsSuccessful ? response.Data : null;
		}
	}
}