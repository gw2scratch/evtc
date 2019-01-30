using System.Threading.Tasks;
using ScratchEVTCParser;

namespace RotationComparison.GW2Api
{
	public interface IApiDataSource
	{
		Task<GW2ApiData> GetApiDataAsync();
	}
}