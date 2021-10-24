using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Processing;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;

namespace GW2Scratch.EVTCAnalytics.Model
{
	/// <summary>
	/// Represents an arcdps log.
	/// </summary>
	public class Log
	{
		/// <summary>
		/// Provides a list of all <see cref="Event"/>s in the order they occured in the log.
		/// </summary>
		public IReadOnlyList<Event> Events { get; }
		
		/// <summary>
		/// Provides a list of all <see cref="Agent"/>s in the order they appear in the log.
		/// </summary>
		public IReadOnlyList<Agent> Agents { get; }
		
		/// <summary>
		/// Provides a list of all <see cref="Skill"/>s that appear in the log.
		/// </summary>
		public IReadOnlyList<Skill> Skills { get; }

		/// <summary>
		/// Provides a string with the version of arcdps used to record this log, prefixed with "EVTC".
		/// </summary>
		public string EvtcVersion { get; }
		
		/// <summary>
		/// Provides the language of the game client used to record the log.
		/// </summary>
		public GameLanguage GameLanguage { get; }

		/// <summary>
		/// Provides the time of the log ending. This may be <see langword="null"/> in cases when the log is missing the start event.
		/// </summary>
		/// <remarks>
		/// Some versions of arcdps, such as 20181211, have had their log start events missing.
		/// </remarks>
		public LogTime StartTime { get; }
		/// <summary>
		/// Provides the time of the log ending. This may be <see langword="null"/> in cases when the log ends abruptly.
		/// </summary>
		/// <remarks>
		/// Older versions of arcdps have often had their log end events missing, which makes <see langword="null"/> values here common.
		/// </remarks>
		public LogTime EndTime { get; }

		/// <summary>
		/// Provides the type of the log.
		/// </summary>
		public LogType LogType { get; }
		
		/// <summary>
		/// Provides the <see cref="Agent"/> used to trigger the start of logging.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In most cases, this is the main enemy, but it may also be a friendly <see cref="NPC"/> and others.
		/// </para>
		/// <para>
		/// Agents used for triggering logs are configurable in arcdps and this may be used
		/// to add logging support for more encounters by a player.
		/// </para>
		/// </remarks>
		public Agent MainTarget { get; }
		
		/// <summary>
		/// Provides the <see cref="Player"/> who recorded the log.
		/// </summary>
		public Player PointOfView { get; }
		
		/// <summary>
		/// Provides the numeric ID of the language of the game client.
		/// </summary>
		public int? LanguageId { get; }
		
		/// <summary>
		/// Provides a numeric number corresponding to the version of Guild Wars 2 used to record this log.
		/// </summary>
		public int? GameBuild { get; }
		
		/// <summary>
		/// Provides a numeric ID of the shard (server instance) the contents of the log occured in.
		/// </summary>
		public int? GameShardId { get; }
		
		/// <summary>
		/// Provides a numeric ID of the map the encounter occured in.
		/// </summary>
		public int? MapId { get; }

		/// <summary>
		/// Provides access to encounter-specific data and definitions.
		/// </summary>
		public IEncounterData EncounterData { get; }

		/// <summary>
		/// Creates a new instance of a <see cref="Log"/>.
		/// </summary>
		/// <param name="state">The context of a <see cref="LogProcessor"/>.</param>
		internal Log(LogProcessorState state)
		{
			MainTarget = state.MainTarget;
			LogType = state.LogType;
			EncounterData = state.EncounterData;
			GameLanguage = state.GameLanguage;
			EvtcVersion = state.EvtcVersion;
			StartTime = state.LogStartTime;
			EndTime = state.LogEndTime;
			PointOfView = state.PointOfView;
			LanguageId = state.GameLanguageId;
			GameLanguage = state.GameLanguage;
			GameBuild = state.GameBuild;
			GameShardId = state.GameShardId;
			MapId = state.MapId;
			Events = state.Events;
			Agents = state.Agents;
			Skills = state.Skills;
		}

		/// <summary>
		/// Creates a new instance of a <see cref="Log"/> without requiring a <see cref="LogProcessorState"/>.
		/// </summary>
		internal Log(Agent mainTarget, LogType logType, IEnumerable<Event> events, IEnumerable<Agent> agents,
			IEnumerable<Skill> skills, IEncounterData encounterData,
			GameLanguage gameLanguage, string evtcVersion, LogTime startTime, LogTime endTime,
			Player pointOfView, int? language, int? gameBuild, int? gameShardId, int? mapId)
		{
			MainTarget = mainTarget;
			LogType = logType;
			EncounterData = encounterData;
			GameLanguage = gameLanguage;
			EvtcVersion = evtcVersion;
			StartTime = startTime;
			EndTime = endTime;
			PointOfView = pointOfView;
			LanguageId = language;
			GameBuild = gameBuild;
			GameShardId = gameShardId;
			MapId = mapId;
			Events = events as Event[] ?? events.ToArray();
			Agents = agents as Agent[] ?? agents.ToArray();
			Skills = skills as Skill[] ?? skills.ToArray();
		}
	}
}