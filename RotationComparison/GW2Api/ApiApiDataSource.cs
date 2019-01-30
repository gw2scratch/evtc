using System.Threading.Tasks;
using ScratchEVTCParser;
using ScratchEVTCParser.GW2Api.V2;

namespace RotationComparison.GW2Api
{
	public class ApiApiDataSource : IApiDataSource
	{
		public Task<GW2ApiData> GetApiDataAsync()
		{
			return GW2ApiData.LoadFromApiAsync(new ApiSkillRepository());
		}
	}
}