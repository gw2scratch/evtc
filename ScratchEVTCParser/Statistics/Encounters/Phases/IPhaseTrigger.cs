using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Statistics.Encounters.Phases
{
	public interface IPhaseTrigger
	{
		bool IsTrigger(Event e);
		PhaseDefinition PhaseDefinition { get; }
	}
}