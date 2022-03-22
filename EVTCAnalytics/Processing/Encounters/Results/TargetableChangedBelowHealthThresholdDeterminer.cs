using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if any attack target changes targetability to a specified
	/// specified when the gadget's health is below a specified threshold.
	/// </summary>
	public class TargetableChangedBelowHealthThresholdDeterminer : EventFoundResultDeterminer
	{
		private readonly Gadget gadget;
		private readonly bool targetableState;
		private readonly float healthThreshold;

		/// <summary>
		/// Creates a new <see cref="TeamChangedBelowHealthThresholdDeterminer"/>.
		/// </summary>
		/// <param name="gadget">The gadget to be considered.</param>
		/// <param name="targetableState">The targetable state to be set.</param>
		/// <param name="healthThreshold">The health threshold that has to be reached,
		/// where 1 is 100% and 0 is 0% of maximum health of the <paramref name="gadget"/>.</param>
		public TargetableChangedBelowHealthThresholdDeterminer(Gadget gadget, bool targetableState,
			float healthThreshold)
		{
			this.gadget = gadget;
			this.targetableState = targetableState;
			this.healthThreshold = healthThreshold;
		}

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			bool belowThreshold = false;
			foreach (var e in events)
			{
				if (e is AgentHealthUpdateEvent healthUpdate && healthUpdate.Agent == gadget)
				{
					belowThreshold = healthUpdate.HealthFraction <= healthThreshold;
				}
				else if (belowThreshold && e is TargetableChangeEvent targetableChange &&
				         targetableChange.AttackTarget.Gadget == gadget &&
				         targetableChange.IsTargetable == targetableState)
				{
					return targetableChange;
				}
			}

			return null;
		}
	}
}