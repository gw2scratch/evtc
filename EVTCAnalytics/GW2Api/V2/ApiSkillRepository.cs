using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace GW2Scratch.EVTCAnalytics.GW2Api.V2
{
	public class ApiSkillRepository
	{
		public async Task<IEnumerable<ApiSkill>> GetAllApiSkills()
		{
			var client = new RestClient("https://api.guildwars2.com/");

			var firstRequest = new RestRequest("v2/skills");
			firstRequest.AddQueryParameter("page", "0");
			firstRequest.AddQueryParameter("page_size", "200");

			//var firstResponse = client.Execute<List<ApiSkill>>(firstRequest);
			var firstResponse = await client.ExecuteTaskAsync<List<ApiSkill>>(firstRequest);

			int pageCount = int.Parse((string) firstResponse.Headers.First(x => x.Name == "X-Page-Total").Value);

			var tasks = new List<Task<IRestResponse<List<ApiSkill>>>>();

			for (int i = 1; i < pageCount; i++)
			{
				var request = new RestRequest("v2/skills", Method.GET);
				request.AddQueryParameter("page", $"{i}");
				request.AddQueryParameter("page_size", "200");

				tasks.Add(client.ExecuteTaskAsync<List<ApiSkill>>(request));
			}

			await Task.WhenAll(tasks);

			var skills = new List<ApiSkill>();
			skills.AddRange(firstResponse.Data);
			foreach (var task in tasks)
			{
				skills.AddRange(task.Result.Data);
			}

			return skills;
		}
	}
}