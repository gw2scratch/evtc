using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Model
{
	public class Log
	{
		private readonly Event[] events;
		private readonly Agent[] agents;
		private readonly Skill[] skills;

		public IEnumerable<Event> Events => events;
		public IEnumerable<Agent> Agents => agents;
		public IEnumerable<Skill> Skills => skills;

		public IEncounter Encounter { get; }

		public Log(IEncounter encounter, IEnumerable<Event> events, IEnumerable<Agent> agents, IEnumerable<Skill> skills)
		{
			Encounter = encounter;
			this.events = events.ToArray();
			this.agents = agents.ToArray();
			this.skills = skills.ToArray();
		}

	}
}