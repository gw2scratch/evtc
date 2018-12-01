using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ScratchEVTCParser;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Encounters;

namespace ArcdpsLogManager.Logs
{
	public class LogData
	{
		public FileInfo FileInfo { get; }

		public IEnumerable<Player> Players { get; private set; }
		public EncounterResult EncounterResult { get; private set; } = EncounterResult.Unknown;
		public string EncounterName { get; private set; } = "Unknown";
		public DateTimeOffset EncounterStartTime { get; private set; }
		public TimeSpan EncounterDuration { get; private set; }

		public DateTimeOffset ParseTime { get; private set; }
		public long ParseMilliseconds { get; private set; }

		public LogData(FileInfo fileInfo)
		{
			FileInfo = fileInfo;
		}

		internal LogData(FileInfo fileInfo, IEnumerable<Player> players, string encounterName,
			EncounterResult encounterResult,
			DateTimeOffset encounterStartTime, TimeSpan encounterDuration, long parseMilliseconds = -1)
		{
			FileInfo = fileInfo;
			Players = players.ToArray();
			EncounterResult = encounterResult;
			EncounterName = encounterName;
			EncounterStartTime = encounterStartTime;
			EncounterDuration = encounterDuration;

			ParseTime = DateTimeOffset.Now;
			ParseMilliseconds = parseMilliseconds;
		}

		public void ParseData()
		{
			// TODO: Move out dependencies

			var parser = new EVTCParser();
			var processor = new LogProcessor();
			var calculator = new StatisticsCalculator();

			var stopwatch = Stopwatch.StartNew();

			var parsedLog = parser.ParseLog(FileInfo.FullName);
			var log = processor.GetProcessedLog(parsedLog);

			EncounterName = calculator.GetEncounterName(log);
			EncounterResult= calculator.GetResult(log);
			Players = calculator.GetPlayers(log).ToArray();

			EncounterStartTime = calculator.GetEncounterStartTime(log);
			EncounterDuration = calculator.GetEncounterDuration(log);

			stopwatch.Stop();

			ParseMilliseconds = stopwatch.ElapsedMilliseconds;
			ParseTime = DateTimeOffset.Now;
		}
	}
}