using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using System;

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
		
		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(RewardEvent) };
		public override IReadOnlyList<uint> RequiredBuffSkillIds { get; } = new List<uint>();
		public override IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			return events.OfType<RewardEvent>().FirstOrDefault(x => x.RewardId == rewardId);
		}
	}
}