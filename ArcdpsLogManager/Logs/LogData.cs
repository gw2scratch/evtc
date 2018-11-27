using System;
using System.Collections.Generic;
using System.IO;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;

namespace ArcdpsLogManager.Logs
{
	public class LogData
	{
		public FileInfo FileInfo { get; }

		public IEnumerable<Player> Players { get; }
		public EncounterResult EncounterResult { get; }
		public Agent Boss { get; }

		public DateTimeOffset FightStartTime { get; }
		public TimeSpan EncounterDuration { get; }


		public LogData(FileInfo fileInfo, IEnumerable<Player> players, Agent boss, EncounterResult encounterResult,
			DateTimeOffset fightStartTime, TimeSpan encounterDuration)
		{
			FileInfo = fileInfo;
			Players = players;
			EncounterResult = encounterResult;
			Boss = boss;
			FightStartTime = fightStartTime;
			EncounterDuration = encounterDuration;
		}
	}
}