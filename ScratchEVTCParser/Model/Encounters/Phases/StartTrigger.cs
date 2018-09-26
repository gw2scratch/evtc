using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Model.Encounters.Phases
{
	public class StartTrigger : IPhaseTrigger
	{
		public PhaseDefinition PhaseDefinition { get; }

		public StartTrigger(PhaseDefinition phaseDefinition)
		{
			PhaseDefinition = phaseDefinition;
		}

		public bool IsTrigger(Event e) => e is LogStartEvent;
	}
}