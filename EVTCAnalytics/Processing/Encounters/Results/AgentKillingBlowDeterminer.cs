using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if there was a killing blow towards an agent
	/// </summary>
	public class AgentKillingBlowDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;

		public AgentKillingBlowDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(PhysicalDamageEvent) };
		
		protected override Event GetEvent(IEnumerable<Event> events)
		{
			return events.OfType<PhysicalDamageEvent>()
				.FirstOrDefault(x => x.Defender == agent && x.HitResult == PhysicalDamageEvent.Result.KillingBlow);
		}
	}
}