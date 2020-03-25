using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using GW2Scratch.EVTCAnalytics.Processing.Steps;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	public class EncounterIdentifierBuilder
	{
		private Encounter Encounter { get; set; }
		private PhaseSplitter PhaseSplitter { get; set; }
		private IResultDeterminer ResultDeterminer { get; set; }
		private IModeDeterminer DefaultModeDeterminer { get; set; }
		private IReadOnlyList<IPostProcessingStep> DefaultProcessingSteps { get; set; }
		private List<Agent> DefaultTargets { get; set; }

		public EncounterIdentifierBuilder(
			Encounter defaultEncounter,
			List<Agent> defaultTargets,
			PhaseSplitter defaultPhaseSplitter,
			IResultDeterminer defaultResultDeterminer,
			IModeDeterminer defaultModeDeterminer,
			IReadOnlyList<IPostProcessingStep> defaultProcessingSteps = null
		)
		{
			Encounter = defaultEncounter;
			DefaultTargets = defaultTargets ?? throw new ArgumentNullException(nameof(defaultTargets));
			PhaseSplitter = defaultPhaseSplitter ?? throw new ArgumentNullException(nameof(defaultPhaseSplitter));
			ResultDeterminer = defaultResultDeterminer ?? throw new ArgumentNullException(nameof(defaultResultDeterminer));
			DefaultModeDeterminer = defaultModeDeterminer ?? throw new ArgumentNullException(nameof(defaultModeDeterminer));
			DefaultProcessingSteps = defaultProcessingSteps ?? new IPostProcessingStep[0];
		}

		public EncounterIdentifierBuilder WithEncounter(Encounter encounter)
		{
			Encounter = encounter;
			return this;
		}

		public EncounterIdentifierBuilder WithPhases(PhaseSplitter splitter)
		{
			PhaseSplitter = splitter;
			return this;
		}

		public EncounterIdentifierBuilder WithResult(IResultDeterminer determiner)
		{
			ResultDeterminer = determiner;
			return this;
		}

		public EncounterIdentifierBuilder WithModes(IModeDeterminer determiner)
		{
			DefaultModeDeterminer = determiner;
			return this;
		}

		public EncounterIdentifierBuilder WithPostProcessingSteps(params IPostProcessingStep[] steps)
		{
			DefaultProcessingSteps = steps;
			return this;
		}

		public EncounterIdentifierBuilder WithPostProcessingSteps(IEnumerable<IPostProcessingStep> steps)
		{
			DefaultProcessingSteps = steps.ToList();
			return this;
		}

		public EncounterIdentifierBuilder WithTargets(List<Agent> targets)
		{
			DefaultTargets = targets;
			return this;
		}

		public IEncounterData Build()
		{
			return new BaseEncounterData(
				Encounter,
				DefaultTargets,
				PhaseSplitter,
				ResultDeterminer,
				DefaultModeDeterminer,
				DefaultProcessingSteps ?? new IPostProcessingStep[0]
			);
		}
	}
}