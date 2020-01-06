using System.Threading.Tasks;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Statistics;

namespace GW2Scratch.RotationComparison.GW2Api
{
	public interface IApiDataSource
	{
		Task<GW2ApiData> GetApiDataAsync();
	}
}