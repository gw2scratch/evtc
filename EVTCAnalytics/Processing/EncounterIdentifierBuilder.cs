using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health;
using GW2Scratch.EVTCAnalytics.Processing.Steps;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	public class EncounterIdentifierBuilder
	{
		private Encounter Encounter { get; set; }
		private IResultDeterminer ResultDeterminer { get; set; }
		private IModeDeterminer ModeDeterminer { get; set; }
		private IHealthDeterminer HealthDeterminer { get; set; }
		private List<IPostProcessingStep> ProcessingSteps { get; set; }
		private List<Agent> Targets { get; set; }

		public EncounterIdentifierBuilder(
			Encounter defaultEncounter,
			List<Agent> defaultTargets,
			IResultDeterminer defaultResultDeterminer,
			IModeDeterminer defaultModeDeterminer,
			IHealthDeterminer defaultHealthDeterminer,
			IEnumerable<IPostProcessingStep> defaultProcessingSteps = null
		)
		{
			Encounter = defaultEncounter;
			Targets = defaultTargets ?? throw new ArgumentNullException(nameof(defaultTargets));
			ResultDeterminer = defaultResultDeterminer ?? throw new ArgumentNullException(nameof(defaultResultDeterminer));
			ModeDeterminer = defaultModeDeterminer ?? throw new ArgumentNullException(nameof(defaultModeDeterminer));
			HealthDeterminer = defaultHealthDeterminer ?? throw new ArgumentNullException(nameof(defaultHealthDeterminer));
			ProcessingSteps = (defaultProcessingSteps ?? Enumerable.Empty<IPostProcessingStep>()).ToList();
		}

		public EncounterIdentifierBuilder WithEncounter(Encounter encounter)
		{
			Encounter = encounter;
			return this;
		}

		public EncounterIdentifierBuilder WithResult(IResultDeterminer determiner)
		{
			ResultDeterminer = determiner;
			return this;
		}

		public EncounterIdentifierBuilder WithModes(IModeDeterminer determiner)
		{
			ModeDeterminer = determiner;
			return this;
		}

		public EncounterIdentifierBuilder WithHealthDeterminer(IHealthDeterminer determiner)
		{
			HealthDeterminer = determiner;
			return this;
		}

		public EncounterIdentifierBuilder AddPostProcessingStep(IPostProcessingStep step)
		{
			ProcessingSteps.Add(step);
			return this;
		}

		public EncounterIdentifierBuilder WithPostProcessingSteps(params IPostProcessingStep[] steps)
		{
			ProcessingSteps = steps.ToList();
			return this;
		}

		public EncounterIdentifierBuilder WithPostProcessingSteps(IEnumerable<IPostProcessingStep> steps)
		{
			ProcessingSteps = steps.ToList();
			return this;
		}

		public EncounterIdentifierBuilder WithTargets(List<Agent> targets)
		{
			Targets = targets;
			return this;
		}

		public IEncounterData Build()
		{
			return new BaseEncounterData(
				Encounter,
				Targets,
				ResultDeterminer,
				ModeDeterminer,
				HealthDeterminer,
				ProcessingSteps
			);
		}
	}
}