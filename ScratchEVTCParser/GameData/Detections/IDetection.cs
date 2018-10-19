using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.GameData.Detections
{
	public interface IDetection
	{
		// TODO: Remove
		CoreSpecialization CoreSpecialization { get; }
		bool IsDetected(Player player, Event e);
	}
}