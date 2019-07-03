using System.Threading.Tasks;
using GW2Scratch.EVTCAnalytics;

namespace GW2Scratch.RotationComparison.GW2Api
{
	public interface IApiDataSource
	{
		Task<GW2ApiData> GetApiDataAsync();
	}
}