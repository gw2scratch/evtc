using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Phases
{
	public interface IPhaseTrigger
	{
		bool IsTrigger(Event e);
		PhaseDefinition PhaseDefinition { get; }
	}
}