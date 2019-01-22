using System;
using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics.Encounters.Phases;
using ScratchEVTCParser.Statistics.Encounters.Results;

namespace ScratchEVTCParser.Statistics.Encounters
{
	public class BaseEncounter : IEncounter
	{
		private readonly PhaseSplitter phaseSplitter;
		private readonly IResultDeterminer resultDeterminer;
		private readonly IEncounterNameProvider nameProvider;
		private readonly Event[] events;

		private EncounterResult? result = null;
		private Phase[] phases = null;
		private string name = null;

		public BaseEncounter(IEnumerable<Agent> importantAgents, IEnumerable<Event> events, PhaseSplitter phaseSplitter,
			IResultDeterminer resultDeterminer, IEncounterNameProvider nameProvider)
		{
			ImportantEnemies = importantAgents.ToArray();
			this.phaseSplitter = phaseSplitter;
			this.resultDeterminer = resultDeterminer;
			this.nameProvider = nameProvider;
			this.events = events as Event[] ?? events.ToArray();
		}

		public IEnumerable<Agent> ImportantEnemies { get; }

		public EncounterResult GetResult()
		{
			if (result == null)
			{
				result = resultDeterminer.GetResult(events);
			}

			return result.Value;
		}

		public IEnumerable<Phase> GetPhases()
		{
			if (phases == null)
			{
				phases = phaseSplitter.GetEventsByPhases(events).ToArray();
			}

			return phases;
		}

		public string GetName()
		{
			if (name == null)
			{
				name = nameProvider.GetEncounterName();
			}

			return name;
		}
	}
}