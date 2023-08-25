using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if the agent has a death event.
	/// </summary>
	public class AgentDeadDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;

		public AgentDeadDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentDeadEvent) };
		public override IReadOnlyList<uint> RequiredBuffSkillIds => new List<uint>();
		public override IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			return events.OfType<AgentDeadEvent>().FirstOrDefault(x => x.Agent == agent);
		}
	}
}