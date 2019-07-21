using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results
{
	/// <summary>
	/// Is successful if the health of the agent drops below the provided health fraction.
	/// </summary>
	public class AgentBelowHealthFractionDeterminer : IResultDeterminer
	{
		private readonly Agent agent;
		private readonly float healthFraction;

		public AgentBelowHealthFractionDeterminer(Agent agent, float healthFraction)
		{
			this.agent = agent;
			this.healthFraction = healthFraction;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			float lowestHealthFraction = events
				.OfType<AgentHealthUpdateEvent>()
				.Where(x => x.Agent == agent)
				.DefaultIfEmpty(new AgentHealthUpdateEvent(-1, agent, 1.0f))
				.Min(x => x.HealthFraction);

			return lowestHealthFraction <= healthFraction ? EncounterResult.Success : EncounterResult.Failure;
		}
	}
}