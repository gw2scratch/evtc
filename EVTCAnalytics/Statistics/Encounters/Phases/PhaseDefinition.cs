using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Phases
{
	public class PhaseDefinition
	{
		public string Name { get; }
		public IEnumerable<Agent> ImportantEnemies { get; }

		public PhaseDefinition(string name, params Agent[] importantAgents)
		{
			Name = name;
			ImportantEnemies = importantAgents.ToArray();
		}
	}
}