using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results
{
	public class TargetableDeterminer : IResultDeterminer
	{
		private readonly Agent attackTarget;
		private readonly bool targetableState;

		public TargetableDeterminer(AttackTarget attackTarget, bool targetableState)
		{
			this.attackTarget = attackTarget;
			this.targetableState = targetableState;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			bool targetableSet = events.OfType<TargetableChangeEvent>()
				.Any(x => x.AttackTarget == attackTarget && x.IsTargetable == targetableState);

			return targetableSet ? EncounterResult.Success : EncounterResult.Failure;
		}
	}
}