using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Model.Encounters.Phases
{
	public class Phase
	{
		public Phase(long startingTime, long endingTime, int phaseOrder, string name, IEnumerable<Event> events)
		{
			Events = events.ToArray();
			StartingTime = startingTime;
			EndingTime = endingTime;
			PhaseOrder = phaseOrder;
			Name = name;
		}

		public long StartingTime { get; }
		public long EndingTime { get; }
		public int PhaseOrder { get; }
		public string Name { get; }
		public IEnumerable<Event> Events { get; }
	}
}