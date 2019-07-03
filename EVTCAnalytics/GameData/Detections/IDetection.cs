using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.GameData.Detections
{
	public interface IDetection
	{
		bool IsDetected(Player player, Event e);
	}
}