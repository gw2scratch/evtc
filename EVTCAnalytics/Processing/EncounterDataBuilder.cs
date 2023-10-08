using GW2Scratch.EVTCAnalytics.Events;
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
	public class EncounterDataBuilder
	{
		private Encounter Encounter { get; set; }
		private IResultDeterminer ResultDeterminer { get; set; }
		private IModeDeterminer ModeDeterminer { get; set; }
		private IModeDeterminer FallbackModeDeterminer { get; set; }
		private IHealthDeterminer HealthDeterminer { get; set; }
		private List<IPostProcessingStep> ProcessingSteps { get; set; }
		private List<Agent> Targets { get; set; }

		public EncounterDataBuilder(
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

		public EncounterDataBuilder WithEncounter(Encounter encounter)
		{
			Encounter = encounter;
			return this;
		}

		public EncounterDataBuilder WithResult(IResultDeterminer determiner)
		{
			ResultDeterminer = determiner;
			return this;
		}

		public EncounterDataBuilder WithModes(IModeDeterminer determiner)
		{
			ModeDeterminer = determiner;
			return this;
		}

		public EncounterDataBuilder WithHealth(IHealthDeterminer determiner)
		{
			HealthDeterminer = determiner;
			return this;
		}

		public EncounterDataBuilder WithHealth(
			IReadOnlyList<Type> requiredEventTypes,
			IReadOnlyList<uint> requiredBuffSkillIds,
			IReadOnlyList<PhysicalDamageEvent.Result> requiredPhysicalDamageEventResults,
			Func<Log, float?> determinerFunc
		)
		{
			HealthDeterminer = new AdHocHealthDeterminer(determinerFunc, requiredEventTypes, requiredBuffSkillIds, requiredPhysicalDamageEventResults);
			return this;
		}

		public EncounterDataBuilder AddPostProcessingStep(IPostProcessingStep step)
		{
			ProcessingSteps.Add(step);
			return this;
		}

		public EncounterDataBuilder WithPostProcessingSteps(params IPostProcessingStep[] steps)
		{
			ProcessingSteps = steps.ToList();
			return this;
		}

		public EncounterDataBuilder WithPostProcessingSteps(IEnumerable<IPostProcessingStep> steps)
		{
			ProcessingSteps = steps.ToList();
			return this;
		}

		public EncounterDataBuilder WithTargets(List<Agent> targets)
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