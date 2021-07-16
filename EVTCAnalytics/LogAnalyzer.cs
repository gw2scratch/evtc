using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics
{
	/// <summary>
	/// The Log Analyzer calculates potentially performance-heavy statistics in a <see cref="Log"/>.
	/// All calculations are cached within the object and not repeated.
	/// </summary>
	public class LogAnalyzer
	{
		private readonly Log log;
		private IReadOnlyList<Player> logPlayers = null;
		private ResultDeterminerResult logResult = null;
		private EncounterMode? encounterMode = null;
		private long? logEncounterStart = null;
		private long? logEncounterEnd = null;

		// Since the health fraction result is float?, we cannot rely on the value being null
		// meaning that we haven't calculated the health fraction yet
		private bool healthFractionCalculated = false;
		private float? healthFraction = null;

		/// <summary>
		/// Creates a new instance of a log analyzer for a provided log.
		/// </summary>
		/// <param name="log">The processed log that will be analyzed.</param>
		public LogAnalyzer(Log log)
		{
			this.log = log;
		}

		/// <summary>
		/// Calculates the duration of the encounter.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is not the duration of the log, an encounter may start after log started and end before logging stops.
		/// </para>
		/// <para>
		/// This method caches its result, calculating the result only once.
		/// </para>
		/// </remarks>
		/// <returns>The duration of the encounter.</returns>
		public TimeSpan GetEncounterDuration()
		{
			return new TimeSpan(0, 0, 0, 0, (int)(GetEncounterEnd() - GetEncounterStart()));
		}

		/// <summary>
		/// Calculates the encounter start time (in system time milliseconds).
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method caches its result, calculating the result only once.
		/// </para>
		/// </remarks>
		/// <returns>The start time of the encounter in milliseconds since system boot.</returns>
		public long GetEncounterStart()
		{
			logEncounterStart ??= log.StartTime?.TimeMilliseconds ?? log.Events[0].Time;
			return logEncounterStart.Value;
		}

		/// <summary>
		/// Calculates the encounter end time (in system time milliseconds).
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method caches its result, calculating the result only once.
		/// </para>
		/// </remarks>
		/// <returns>The end time of the encounter in milliseconds since system boot.</returns>
		public long GetEncounterEnd()
		{
			if (logEncounterEnd == null)
			{
				logResult ??= log.EncounterData.ResultDeterminer.GetResult(log.Events);
				logEncounterEnd = logResult.Time ?? log.EndTime?.TimeMilliseconds ?? log.Events[^1].Time;
			}

			return logEncounterEnd.Value;
		}

		/// <summary>
		/// Provides a list of all players.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method caches its result, calculating the result only once.
		/// </para>
		/// </remarks>
		public IReadOnlyList<Player> GetPlayers()
		{
			logPlayers ??= log.Agents.OfType<Player>().ToList();
			return logPlayers;
		}

		/// <summary>
		/// Calculates the result of the encounter.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method caches its result, calculating the result only once.
		/// </para>
		/// </remarks>
		public EncounterResult GetResult()
		{
			logResult ??= log.EncounterData.ResultDeterminer.GetResult(log.Events);
			return logResult.EncounterResult;
		}

		/// <summary>
		/// Calculates the mode of the encounter.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method caches its result, calculating the result only once.
		/// </para>
		/// </remarks>
		public EncounterMode GetMode()
		{
			encounterMode ??= log.EncounterData.ModeDeterminer.GetMode(log);
			return encounterMode.Value;
		}

		/// <summary>
		/// Calculates the final health percentage of the main enemy or enemies in an encounter.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This value may have different semantics depending on the encounter.
		/// For example, for encounters with multiple boss enemies, the higher
		/// of their health values may be used.
		/// </para>
		/// <para>
		/// The returned value may be higher than 1 in case there are multiple enemies
		/// in sequence in the encounter, each extra 1 corresponds to a health bar in that case.
		/// </para>
		/// <para>
		/// This method caches its result, calculating the result only once.
		/// </para>
		/// </remarks>
		/// <returns>A non-negative health fraction value where 1 corresponds to 100% or <see langword="null"/> if not applicable for this encounter.</returns>
		public float? GetMainEnemyHealthFraction()
		{
			if (healthFractionCalculated)
			{
				return healthFraction;
			}

			healthFraction = log.EncounterData.HealthDeterminer.GetMainEnemyHealthFraction(log);
			healthFractionCalculated = true;

			return healthFraction;
		}

		/// <summary>
		/// Provides the encounter this log recorded.
		/// </summary>
		/// <returns>The identified encounter.</returns>
		public Encounter GetEncounter()
		{
			return log.EncounterData.Encounter;
		}
	}
}