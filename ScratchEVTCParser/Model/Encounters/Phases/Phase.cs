using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;

namespace ScratchEVTCParser.Model.Encounters.Phases
{
	public class Phase
	{
		public Phase(long startTime, long endTime, int phaseOrder, string name, IEnumerable<Event> events)
		{
			Events = events.ToArray();
			StartTime = startTime;
			EndTime = endTime;
			PhaseOrder = phaseOrder;
			Name = name;
		}

		public long StartTime { get; }
		public long EndTime { get; }
		public int PhaseOrder { get; }
		public string Name { get; }
		public IEnumerable<Event> Events { get; }
	}
}