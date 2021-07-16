using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData;
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

		/// <summary>
		/// Weapon skill data used for determining which skills a player is using.
		/// If replaced after used, the resulting statistics will not be updated.
		/// </summary>
		public WeaponSkillData WeaponSkillData { get; set; } = new WeaponSkillData();

		/// <summary>
		/// Definitions of skill detections used to determine which skills a player was using.
		/// If replaced after skills were detected, they will not be updated.
		/// </summary>
		public SkillDetections SkillDetections { get; set; } = new SkillDetections();

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

		public TimeSpan GetEncounterDuration()
		{
			return new TimeSpan(0, 0, 0, 0, (int)(GetEncounterEnd() - GetEncounterStart()));
		}

		public long GetEncounterStart()
		{
			logEncounterStart ??= log.StartTime?.TimeMilliseconds ?? log.Events[0].Time;
			return logEncounterStart.Value;
		}

		public long GetEncounterEnd()
		{
			if (logEncounterEnd == null)
			{
				logResult ??= log.EncounterData.ResultDeterminer.GetResult(log.Events);
				logEncounterEnd = logResult.Time ?? log.EndTime?.TimeMilliseconds ?? log.Events[^1].Time;
			}

			return logEncounterEnd.Value;
		}

		public IReadOnlyList<Player> GetPlayers()
		{
			logPlayers ??= log.Agents.OfType<Player>().ToList();
			return logPlayers;
		}

		public EncounterResult GetResult()
		{
			logResult ??= log.EncounterData.ResultDeterminer.GetResult(log.Events);
			return logResult.EncounterResult;
		}

		public EncounterMode GetMode()
		{
			encounterMode ??= log.EncounterData.ModeDeterminer.GetMode(log);
			return encounterMode.Value;
		}

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

		public Encounter GetEncounter()
		{
			return log.EncounterData.Encounter;
		}
	}
}