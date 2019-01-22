using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics.Encounters.Phases
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