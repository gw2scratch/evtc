using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	/// <summary>
	/// An interface for providing encounter-specific data for an encounter.
	/// </summary>
	public interface IEncounterDataProvider
	{
		/// <summary>
		/// Get encounter-specific data for a log while processing the log.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is intended to be used before a <see cref="Log"/> is created,
		/// which is why passing in individual parts of the log is required.
		/// </para>
		/// </remarks>
		/// <param name="encounter">The encounter of the log.</param>
		/// <param name="mainTarget">The main target of the log.</param>
		/// <param name="agents">The agents of the log.</param>
		/// <param name="gameBuild">The game build number.</param>
		/// <param name="logType">The log type.</param>
		/// <returns>The encounter data for this log.</returns>
		IEncounterData GetEncounterData(Encounter encounter, Agent mainTarget, IReadOnlyList<Agent> agents, int? gameBuild, LogType logType);
	}
}