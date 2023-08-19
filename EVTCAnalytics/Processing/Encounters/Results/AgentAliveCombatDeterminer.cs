using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that results in success if an agent never dies.
	/// </summary>
	public class AgentAliveDeterminer : EventFoundResultDeterminer
	{
		private readonly Agent agent;

		public AgentAliveDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public override IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(AgentDeadEvent) };
		protected override EncounterResult EventFound { get; } = EncounterResult.Failure;
		protected override EncounterResult EventNotFound { get; } = EncounterResult.Success;

		protected override Event GetEvent(IEnumerable<Event> events)
		{
			return events.OfType<AgentDeadEvent>().FirstOrDefault(x => x.Agent == agent);
		}
	}
}