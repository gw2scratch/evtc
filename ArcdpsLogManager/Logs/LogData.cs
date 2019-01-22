using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcdpsLogManager.Uploaders;
using ScratchEVTCParser;
using ScratchEVTCParser.Statistics.Encounters.Results;

namespace ArcdpsLogManager.Logs
{
	public class LogData
	{
		public FileInfo FileInfo { get; }

		public IEnumerable<LogPlayer> Players { get; set; }
		public EncounterResult EncounterResult { get; set; } = EncounterResult.Unknown;
		public string EncounterName { get; set; } = "Unknown";
		/// <summary>
		/// Time when the encounter started.
		/// Is only an estimate if <see cref="ParsingStatus"/> is not <see cref="Logs.ParsingStatus.Parsed"/>.
		/// </summary>
		public DateTimeOffset EncounterStartTime { get; set; }
		public TimeSpan EncounterDuration { get; set; }

		public LogUpload DpsReportEIUpload { get; set; } = new LogUpload();

		public ParsingStatus ParsingStatus { get; set; } = ParsingStatus.Unparsed;

		/// <summary>
		/// Contains the time of when parsing of the log was finished, will be default unless <see cref="ParsingStatus"/> is <see cref="Logs.ParsingStatus.Parsed"/>
		/// </summary>
		public DateTimeOffset ParseTime { get; set; }

		/// <summary>
		/// The amount of milliseconds the parsing of the log took or -1 if <see cref="ParsingStatus"/> is not <see cref="Logs.ParsingStatus.Parsed"/>
		/// </summary>
		public long ParseMilliseconds { get; set; } = -1;

		/// <summary>
		/// An exception if one was thrown during parsing. Will be null unless <see cref="ParsingStatus"/> is <see cref="Logs.ParsingStatus.Failed"/>.
		/// </summary>
		public Exception ParsingException { get; set; }

		public LogData(FileInfo fileInfo)
		{
			FileInfo = fileInfo;
			EncounterStartTime = fileInfo.CreationTime;
		}

		internal LogData(FileInfo fileInfo, IEnumerable<LogPlayer> players, string encounterName,
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

		public void ParseData(EVTCParser parser, LogProcessor processor, StatisticsCalculator calculator)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();

				ParsingStatus = ParsingStatus.Parsing;

				var parsedLog = parser.ParseLog(FileInfo.FullName);
				var log = processor.GetProcessedLog(parsedLog);

				var encounter = calculator.GetEncounter(log);
				EncounterName = encounter.GetName();
				EncounterResult = encounter.GetResult();
				Players = calculator.GetPlayers(log).Select(x =>
					new LogPlayer(x.Name, x.AccountName, x.Subgroup, x.Profession, x.EliteSpecialization)
				).ToArray();

				EncounterStartTime = log.StartTime.ServerTime;
				EncounterDuration = calculator.GetEncounterDuration(encounter);

				stopwatch.Stop();

				ParseMilliseconds = stopwatch.ElapsedMilliseconds;
				ParseTime = DateTimeOffset.Now;
				ParsingStatus = ParsingStatus.Parsed;
			}
			catch (Exception e)
			{
				ParsingStatus = ParsingStatus.Failed;
				ParsingException = e;
			}
		}

		public Task UploadDpsReportEliteInsights(IUploader uploader, CancellationToken cancellationToken,
			bool reupload = false)
		{
			return DpsReportEIUpload.Upload(this, uploader, cancellationToken, reupload);
		}
	}
}