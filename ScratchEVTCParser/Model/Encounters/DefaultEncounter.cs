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
			new PhaseSplitter(new StartTrigger("Default phase")), new AgentDeathResultDeterminer(boss),
			new AgentNameEncounterNameProvider(boss))
		{
		}
	}

	public class BaseEncounter : IEncounter
	{
		private readonly PhaseSplitter phaseSplitter;
		private readonly IResultDeterminer resultDeterminer;
		private readonly IEncounterNameProvider nameProvider;
		private readonly Event[] events;

		public BaseEncounter(IEnumerable<Agent> importantAgents, IEnumerable<Event> events, PhaseSplitter phaseSplitter,
			IResultDeterminer resultDeterminer, IEncounterNameProvider nameProvider)
		{
			ImportantAgents = importantAgents.ToArray();
			this.phaseSplitter = phaseSplitter;
			this.resultDeterminer = resultDeterminer;
			this.nameProvider = nameProvider;
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

		public string GetName()
		{
			return nameProvider.GetEncounterName();
		}
	}
}