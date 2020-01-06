using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	public class AgentCombatExitDeterminer : IResultDeterminer
	{
		private readonly Agent agent;

		public AgentCombatExitDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			bool combatExited = events.OfType<AgentExitCombatEvent>().Any(x => x.Agent == agent);

			return combatExited ? EncounterResult.Success : EncounterResult.Failure;
		}
	}
}