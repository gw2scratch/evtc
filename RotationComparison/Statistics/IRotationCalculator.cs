using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.RotationComparison.Statistics
{
	public interface IRotationCalculator
	{
		ScratchPlayerRotation GetRotation(Log log, Player player);
	}
}