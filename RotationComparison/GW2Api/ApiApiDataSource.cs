using System.Threading.Tasks;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.GW2Api.V2;
using GW2Scratch.EVTCAnalytics.Statistics;

namespace GW2Scratch.RotationComparison.GW2Api
{
	public class ApiApiDataSource : IApiDataSource
	{
		public Task<GW2ApiData> GetApiDataAsync()
		{
			return GW2ApiData.LoadFromApiAsync(new ApiSkillRepository());
		}
	}
}