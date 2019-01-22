using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics.Encounters.Phases
{
	public class Phase
	{
		public Phase(long startTime, long endTime, int phaseOrder, string name, IEnumerable<Agent> importantEnemies, IEnumerable<Event> events)
		{
			Events = events.ToArray();
			ImportantEnemies = importantEnemies.ToArray();
			StartTime = startTime;
			EndTime = endTime;
			PhaseOrder = phaseOrder;
			Name = name;
		}

		public long StartTime { get; }
		public long EndTime { get; }
		public int PhaseOrder { get; }
		public string Name { get; }
		public IEnumerable<Agent> ImportantEnemies { get; }
		public IEnumerable<Event> Events { get; }
	}
}