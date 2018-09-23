using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters.Phases;

namespace ScratchEVTCParser.Model.Encounters
{
	public class DefaultEncounter : BaseEncounter
	{
		public DefaultEncounter(Agent boss, IEnumerable<Event> events) : base(new[] {boss}, events,
			new PhaseSplitter(boss, new StartTrigger("Default phase")), new AgentDeathResultDeterminer(boss))
		{
		}
	}

	public class BaseEncounter : IEncounter
	{
		private readonly PhaseSplitter phaseSplitter;
		private readonly IResultDeterminer resultDeterminer;
		private readonly Event[] events;

		public BaseEncounter(IEnumerable<Agent> importantAgents, IEnumerable<Event> events, PhaseSplitter phaseSplitter,
			IResultDeterminer resultDeterminer)
		{
			ImportantAgents = importantAgents.ToArray();
			this.phaseSplitter = phaseSplitter;
			this.resultDeterminer = resultDeterminer;
			this.events = events as Event[] ?? events.ToArray();
		}

		public IEnumerable<Agent> ImportantAgents { get; }

		public EncounterResult GetResult()
		{
			return resultDeterminer.GetResult(events);
		}

		public IEnumerable<Phase> GetPhases()
		{
			return phaseSplitter.GetEventsByPhases(events);
		}
	}
}