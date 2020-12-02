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
	public class Log
	{
		public IReadOnlyList<Event> Events { get; }
		public IReadOnlyList<Agent> Agents { get; }
		public IReadOnlyList<Skill> Skills { get; }

		public string EvtcVersion { get; }
		public GameLanguage GameLanguage { get; }

		/// <summary>
		/// The time of the log ending. This may be <see langword="null"/> in cases when the log is missing the start event.
		/// </summary>
		/// <remarks>
		/// Some versions of arcdps, such as 20181211, have had their log start events missing.
		/// </remarks>
		public LogTime StartTime { get; }
		/// <summary>
		/// The time of the log ending. This may be <see langword="null"/> in cases when the log ends abruptly.
		/// </summary>
		/// <remarks>
		/// Older versions of arcdps have often had their log end events missing, which makes <see langword="null"/> values here common.
		/// </remarks>
		public LogTime EndTime { get; }

		public LogType LogType { get; }
		public Agent MainTarget { get; }
		public Player PointOfView { get; }
		public int? LanguageId { get; }
		public int? GameBuild { get; }
		public int? GameShardId { get; }
		public int? MapId { get; }

		public IEncounterData EncounterData { get; }

		public Log(Agent mainTarget, LogProcessorContext context)
		{
			MainTarget = mainTarget;
			LogType = context.LogType;
			EncounterData = context.EncounterData;
			GameLanguage = context.GameLanguage;
			EvtcVersion = context.EvtcVersion;
			StartTime = context.LogStartTime;
			EndTime = context.LogEndTime;
			PointOfView = context.PointOfView;
			LanguageId = context.GameLanguageId;
			GameLanguage = context.GameLanguage;
			GameBuild = context.GameBuild;
			GameShardId = context.GameShardId;
			MapId = context.MapId;
			Events = context.Events;
			Agents = context.Agents;
			Skills = context.Skills;
		}

		public Log(Agent mainTarget, LogType logType, IEnumerable<Event> events, IEnumerable<Agent> agents,
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