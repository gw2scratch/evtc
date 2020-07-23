using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	public class NPCSpawnDeterminer : IResultDeterminer
	{
		private readonly int speciesId;
		private readonly int count;

		public NPCSpawnDeterminer(int speciesId, int count = 1)
		{
			this.speciesId = speciesId;
			this.count = count;
		}

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			int agentCount = 0;
			foreach (var e in events)
			{
				if (e is AgentSpawnEvent x && x.Agent is NPC npc && npc.SpeciesId == speciesId)
				{
					agentCount++;
					if (agentCount >= count)
					{
						return new ResultDeterminerResult(EncounterResult.Success, e.Time);
					}
				}
			}

			return new ResultDeterminerResult(EncounterResult.Failure, null);
		}
	}
}