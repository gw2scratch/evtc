using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Statistics.PlayerDataParts;

namespace GW2Scratch.EVTCAnalytics.Statistics
{
	public interface IRotationCalculator
	{
		PlayerRotation GetRotation(Log log, Player player);
	}
}