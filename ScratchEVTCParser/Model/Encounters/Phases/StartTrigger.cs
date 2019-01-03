using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Model.Encounters.Phases
{
	public class StartTrigger : IPhaseTrigger
	{
		public PhaseDefinition PhaseDefinition { get; }
		public bool Triggered = false;

		public StartTrigger(PhaseDefinition phaseDefinition)
		{
			PhaseDefinition = phaseDefinition;
		}

		public bool IsTrigger(Event e)
		{
			if (Triggered) return false;

			Triggered = true;
			return true;
		}
	}
}