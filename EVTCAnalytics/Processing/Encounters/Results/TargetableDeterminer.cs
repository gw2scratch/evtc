using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// Return success if an attack target goes through a sequence of targetability changes.
	/// </summary>
	public class TargetableDeterminer : IResultDeterminer
	{
		private readonly Agent attackTarget;
		private readonly bool[] targetableStates;

		/// <param name="attackTarget">The attack target whose targetability changes will be tracked</param>
		/// <param name="targetableStates">
		/// The sequence of states that has to be set for the encounter
		/// to be successful. If a state is repeated, it is ignored.
		/// </param>
		public TargetableDeterminer(AttackTarget attackTarget, params bool[] targetableStates)
		{
			this.attackTarget = attackTarget;
			this.targetableStates = targetableStates;
		}

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			if (targetableStates.Length == 0)
			{
				return new ResultDeterminerResult(EncounterResult.Success, null);
			}

			int currentState = 0;
			foreach (var change in events.OfType<TargetableChangeEvent>().Where(x => x.AttackTarget == attackTarget))
			{
				if (targetableStates[currentState] == change.IsTargetable)
				{
					currentState++;
					if (currentState == targetableStates.Length)
					{
						return new ResultDeterminerResult(EncounterResult.Success, change.Time);
					}
				}
			}

			return new ResultDeterminerResult(EncounterResult.Failure, null);
		}
	}
}