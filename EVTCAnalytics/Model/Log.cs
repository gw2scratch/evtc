using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;

namespace GW2Scratch.EVTCAnalytics.Model
{
	public class Log
	{
		public IReadOnlyList<Event> Events { get; }
		public IReadOnlyList<Agent> Agents { get; }
		public IReadOnlyList<Skill> Skills { get; }

		public string EVTCVersion { get; }
		public GameLanguage GameLanguage { get; }

		public LogTime StartTime { get; }
		public LogTime EndTime { get; }

		public LogType LogType { get; }
		public Agent MainTarget { get; }
		public Player PointOfView { get; }
		public int? Language { get; }
		public int? GameBuild { get; }
		public int? GameShardId { get; }
		public int? MapId { get; }

		public string EncounterName { get; }
		public IEncounterData EncounterData { get; }

		public Log(Agent mainTarget, LogType logType, IEnumerable<Event> events, IEnumerable<Agent> agents,
			IEnumerable<Skill> skills, IEncounterData encounterData, string encounterName,
			GameLanguage gameLanguage, string evtcVersion, LogTime startTime, LogTime endTime,
			Player pointOfView, int? language, int? gameBuild, int? gameShardId, int? mapId)
		{
			MainTarget = mainTarget;
			LogType = logType;
			EncounterData = encounterData;
			EncounterName = encounterName;
			GameLanguage = gameLanguage;
			EVTCVersion = evtcVersion;
			StartTime = startTime;
			EndTime = endTime;
			PointOfView = pointOfView;
			Language = language;
			GameBuild = gameBuild;
			GameShardId = gameShardId;
			MapId = mapId;
			Events = events as Event[] ?? events.ToArray();
			Agents = agents as Agent[] ?? agents.ToArray();
			Skills = skills as Skill[] ?? skills.ToArray();
		}
	}
}