using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics.PlayerDataParts;

namespace ScratchEVTCParser.Statistics
{
	public interface IRotationCalculator
	{
		PlayerRotation GetRotation(Log log, Player player, GW2ApiData apiData);
	}
}