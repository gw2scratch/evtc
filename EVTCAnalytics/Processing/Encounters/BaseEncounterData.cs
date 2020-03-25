using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Names;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using GW2Scratch.EVTCAnalytics.Processing.Steps;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters
{
	public class BaseEncounterData : IEncounterData
	{
		public Encounter Encounter { get; }
		public PhaseSplitter PhaseSplitter { get; }
		public IResultDeterminer ResultDeterminer { get; }
		public IModeDeterminer ModeDeterminer { get; }
		public IReadOnlyList<IPostProcessingStep> ProcessingSteps { get; }
		public List<Agent> Targets { get; }

		public BaseEncounterData(
			Encounter encounter,
			IEnumerable<Agent> importantAgents,
			PhaseSplitter phaseSplitter,
			IResultDeterminer resultDeterminer,
			IModeDeterminer modeDeterminer,
			IReadOnlyList<IPostProcessingStep> processingSteps)
		{
			Targets = importantAgents.ToList();
			Encounter = encounter;
			PhaseSplitter = phaseSplitter;
			ResultDeterminer = resultDeterminer;
			ModeDeterminer = modeDeterminer;
			ProcessingSteps = processingSteps;
		}
	}
}