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
			var filteredEvents = stopAtDespawn ? TakeUntilDespawn(events) : events;
			
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
					.OfType<BuffApplyEvent>()
					.FirstOrDefault(x => x.Agent == agent && x.Buff.Id == buffId);
			}
		}

		private IEnumerable<Event> TakeUntilDespawn(IEnumerable<Event> events)
		{
			// Very rarely, arcdps has been seen to emit a despawn event instead of a spawn event
			// (specifically, this has been observed once with arcdps 20220330).
			// To work around this issue, we ignore despawn events that happen very soon after the first event.
			const long earlyDespawnThreshold = 200;
			
			long firstTime = long.MaxValue;
			foreach (var e in events)
			{
				firstTime = Math.Min(firstTime, e.Time);
				
				if (e is AgentDespawnEvent despawn && despawn.Agent == agent && despawn.Time - firstTime > earlyDespawnThreshold)
				{
					yield break;
				}
				
				yield return e;
			}
		}
	}
}
