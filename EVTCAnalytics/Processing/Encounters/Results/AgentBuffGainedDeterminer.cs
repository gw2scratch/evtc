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
	public class AgentBuffGainedDeterminer(Agent agent, uint buffId, bool ignoreInitial = true, bool stopAtDespawn = true) : EventFoundResultDeterminer
	{
		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(BuffApplyEvent), typeof(InitialBuffEvent), typeof(AgentDespawnEvent) };
		public override IReadOnlyList<uint> RequiredBuffSkillIds => new List<uint> { buffId };
		public override IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			var filteredEvents = stopAtDespawn ? events.TakeWhile(x => !(x is AgentDespawnEvent despawn && despawn.Agent == agent)) : events;
			
			if (ignoreInitial)
			{
				return filteredEvents
					.OfType<BuffApplyEvent>()
					.Where(x => x is not InitialBuffEvent)
					.FirstOrDefault(x => x.Agent == agent && x.Buff.Id == buffId);
			}
			else
			{
				return filteredEvents
					.TakeWhile(x => x is not AgentDespawnEvent)
					.OfType<BuffApplyEvent>()
					.FirstOrDefault(x => x.Agent == agent && x.Buff.Id == buffId);
			}
		}
	}
}
