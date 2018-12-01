using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScratchEVTCParser;
using ScratchEVTCParser.GameData;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;

namespace ArcdpsLogManager.Logs
{
	public class LogCollection
	{
		public IEnumerable<LogData> Logs { get; }

		private LogCollection(LogData[] logs)
		{
			Logs = logs;
		}

		/// <summary>
		/// Creates a new log collection that contains all logs contained in a directory including subdirectories
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <returns></returns>
		public static LogCollection GetFromDirectory(string directoryPath)
		{
			var logData = new List<LogData>();

			var files = Directory.EnumerateFiles(directoryPath, "*.evtc*", SearchOption.AllDirectories).Where(x => x.EndsWith(".evtc") || x.EndsWith(".evtc.zip"));
			foreach (var file in files)
			{
				logData.Add(new LogData(new FileInfo(file)));
			}

			return new LogCollection(logData.ToArray());
		}

		public static LogCollection Empty => new LogCollection(new LogData[0]);

		public static LogCollection GetTesting()
		{
			var player1 = new Player(1, 1, "Testing player", 0, 0, 0, 0, 120, 120, ":Testing account.1234",
				Profession.Thief, EliteSpecialization.Daredevil, 1);
			var player2 = new Player(2, 2, "Testing player 2", 0, 0, 0, 0, 120, 120, ":Another account.1235",
				Profession.Ranger, EliteSpecialization.Druid, 1);
			var player3 = new Player(3, 3, "Testing player 3", 0, 0, 0, 0, 120, 120, ":Someone.1236",
				Profession.Mesmer, EliteSpecialization.Chronomancer, 2);
			var players = new[] {player1, player2, player3};

			var dateStart1 = new DateTimeOffset(2018, 06, 15, 12, 13, 14, TimeSpan.Zero);
			var dateStart2 = new DateTimeOffset(2018, 06, 15, 13, 14, 15, TimeSpan.Zero);
			var dateStart3 = new DateTimeOffset(2018, 06, 15, 14, 15, 16, TimeSpan.Zero);

			var duration1 = new TimeSpan(0, 1, 0);
			var duration2 = new TimeSpan(0, 2, 0);
			var duration3 = new TimeSpan(0, 6, 0);

			var log1 = new LogData(null, players, "Sabetha the Saboteur", EncounterResult.Failure, dateStart1, duration1);
			var log2 = new LogData(null, players, "Sabetha the Saboteur", EncounterResult.Failure, dateStart2, duration2);
			var log3 = new LogData(null, players, "Sabetha the Saboteur", EncounterResult.Success, dateStart3, duration3);

			return new LogCollection(new[] {log1, log2, log3, log1, log2, log3, log1, log2, log3, log1, log2, log3, log1, log2, log3, log1, log2, log3, log1, log2, log3, log1, log2, log3});
		}
	}
}