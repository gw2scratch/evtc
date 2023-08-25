using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that results in success if an agent gains a specified buff.
	/// </summary>
	public class AgentBuffGainedDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;
		private readonly uint buffId;
		private readonly bool ignoreInitial;

		public AgentBuffGainedDeterminer(Agent agent, uint buffId, bool ignoreInitial = true)
		{
			this.agent = agent;
			this.buffId = buffId;
			this.ignoreInitial = ignoreInitial;
		}
		
		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(BuffApplyEvent), typeof(InitialBuffEvent) };
		public override IReadOnlyList<uint> RequiredBuffSkillIds => new List<uint> { buffId };

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			if (ignoreInitial)
			{
				return events
					.OfType<BuffApplyEvent>()
					.Where(x => x is not InitialBuffEvent)
					.FirstOrDefault(x => x.Agent == agent && x.Buff.Id == buffId);
			}
			else
			{
				return events
					.OfType<BuffApplyEvent>()
					.FirstOrDefault(x => x.Agent == agent && x.Buff.Id == buffId);
			}
		}
	}
}
