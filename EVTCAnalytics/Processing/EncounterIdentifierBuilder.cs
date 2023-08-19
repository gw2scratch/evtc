using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health;
using GW2Scratch.EVTCAnalytics.Processing.Steps;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	/// <summary>
	/// A convenient builder for fluently building encounter data.
	/// </summary>
	public class EncounterIdentifierBuilder
	{
		private Encounter Encounter { get; set; }
		private IResultDeterminer ResultDeterminer { get; set; }
		private IModeDeterminer ModeDeterminer { get; set; }
		private IModeDeterminer FallbackModeDeterminer { get; set; }
		private IHealthDeterminer HealthDeterminer { get; set; }
		private List<IPostProcessingStep> ProcessingSteps { get; set; }
		private List<Agent> Targets { get; set; }

		public EncounterIdentifierBuilder(
			Encounter defaultEncounter,
			List<Agent> defaultTargets,
			IResultDeterminer defaultResultDeterminer,
			IModeDeterminer fallbackModeDeterminer,
			IHealthDeterminer defaultHealthDeterminer,
			IEnumerable<IPostProcessingStep> defaultProcessingSteps = null
		)
		{
			Encounter = defaultEncounter;
			Targets = defaultTargets ?? throw new ArgumentNullException(nameof(defaultTargets));
			ResultDeterminer = defaultResultDeterminer ?? throw new ArgumentNullException(nameof(defaultResultDeterminer));
			FallbackModeDeterminer = fallbackModeDeterminer ?? throw new ArgumentNullException(nameof(fallbackModeDeterminer));
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

		public EncounterIdentifierBuilder WithHealth(IHealthDeterminer determiner)
		{
			HealthDeterminer = determiner;
			return this;
		}
		
		public EncounterIdentifierBuilder WithHealth(IReadOnlyList<Type> requiredEventTypes, Func<Log, float?> determinerFunc)
		{
			HealthDeterminer = new AdHocHealthDeterminer(determinerFunc, requiredEventTypes);
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
			IModeDeterminer mode = ModeDeterminer != null
				? new FallbackModeDeterminer(ModeDeterminer, FallbackModeDeterminer)
				: FallbackModeDeterminer;

			return new BaseEncounterData(
				Encounter,
				Targets,
				ResultDeterminer,
				mode,
				HealthDeterminer,
				ProcessingSteps
			);
		}
	}
}