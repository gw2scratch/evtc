using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if there was a reward event with a specified reward id
	/// </summary>
	public class RewardDeterminer : EventFoundResultDeterminer
	{
		private readonly ulong rewardId;

		public RewardDeterminer(ulong rewardId)
		{
			this.rewardId = rewardId;
		}

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			return events.OfType<RewardEvent>().FirstOrDefault(x => x.RewardId == rewardId);
		}
	}
}