using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results
{
	/// <summary>
	/// Returns success if there was a killing blow towards an agent
	/// </summary>
	public class AgentKillingBlowDeterminer : IResultDeterminer
	{
		private readonly Agent agent;

		public AgentKillingBlowDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			bool agentDead = events
				.OfType<PhysicalDamageEvent>()
				.Any(x => x.Defender == agent && x.HitResult == PhysicalDamageEvent.Result.KillingBlow);

			return agentDead ? EncounterResult.Success : EncounterResult.Failure;
		}
	}
}