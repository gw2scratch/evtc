using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health;
using GW2Scratch.EVTCAnalytics.Processing.Steps;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters
{
	/// <summary>
	/// Encounter-specific definitions and data.
	/// </summary>
	/// <remarks>
	/// This is the mechanism used to define custom logic for detecting success/failure,
	/// encounter modes, the final health of enemies and others.
	/// </remarks>
	public interface IEncounterData
	{
		/// <summary>
		/// Gets the encounter.
		/// </summary>
		Encounter Encounter { get; }

		/// <summary>
		/// Gets the result determiner for this encounter.
		/// </summary>
		IResultDeterminer ResultDeterminer { get; }

		/// <summary>
		/// Gets the mode determiner for this encounter.
		/// </summary>
		IModeDeterminer ModeDeterminer { get; }

		/// <summary>
		/// Gets the health determiner for this encounter.
		/// </summary>
		IHealthDeterminer HealthDeterminer { get; }

		/// <summary>
		/// Gets the post-processing steps for this encounter.
		/// </summary>
		IReadOnlyList<IPostProcessingStep> ProcessingSteps { get; }

		// TODO: Remove Agents and replace with immutable agent queries instead
		/// <summary>
		/// Gets the main targets (typically enemies) for this encounter.
		/// </summary>
		List<Agent> Targets { get; }
	}
}