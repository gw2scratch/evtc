using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// Returns success if there was a reward event with a specified reward id
	/// </summary>
	public class RewardDeterminer : IResultDeterminer
	{
		private readonly ulong rewardId;

		public RewardDeterminer(ulong rewardId)
		{
			this.rewardId = rewardId;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			bool rewardFound = events.OfType<RewardEvent>().Any(x => x.RewardId == rewardId);

			return rewardFound ? EncounterResult.Success : EncounterResult.Failure;
		}
	}
}