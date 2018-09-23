using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Model.Encounters.Phases
{
	public interface IPhaseTrigger
	{
		bool IsTrigger(Event e);
		string PhaseName { get; }
	}
}