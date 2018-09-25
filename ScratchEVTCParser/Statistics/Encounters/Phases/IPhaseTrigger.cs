using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Statistics.Encounters.Phases
{
	public interface IPhaseTrigger
	{
		bool IsTrigger(Event e);
		string PhaseName { get; }
	}
}