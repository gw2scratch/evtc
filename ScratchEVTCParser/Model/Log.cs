using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Model
{
	public class Log
	{
		private readonly Event[] events;
		private readonly Agent[] agents;

		public IEnumerable<Event> Events => events;
		public IEnumerable<Agent> Agents => agents;

		public Log(IEnumerable<Event> events, IEnumerable<Agent> agents)
		{
			this.events = events.ToArray();
			this.agents = agents.ToArray();
		}

	}
}