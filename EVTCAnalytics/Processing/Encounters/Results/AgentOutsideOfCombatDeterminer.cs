using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	public class AgentOutsideOfCombatDeterminer : IResultDeterminer
	{
		private readonly Agent agent;

		public AgentOutsideOfCombatDeterminer(Agent agent)
		{
			this.agent = agent;
		}

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			bool outOfCombat = true;
			long? lastChangeTime = null;
			foreach (var e in events)
			{
				switch (e)
				{
					case AgentEnterCombatEvent enter when enter.Agent == agent:
						outOfCombat = false;
						lastChangeTime = enter.Time;
						break;
					case AgentExitCombatEvent exit when exit.Agent == agent:
						outOfCombat = true;
						lastChangeTime = exit.Time;
						break;
				}
			}

			var result = outOfCombat ? EncounterResult.Success : EncounterResult.Failure;
			return new ResultDeterminerResult(result, lastChangeTime);
		}
	}
}
