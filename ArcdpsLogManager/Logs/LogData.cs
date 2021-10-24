using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GW2Scratch.ArcdpsLogManager.Analytics;
using GW2Scratch.ArcdpsLogManager.Logs.Tagging;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Logs
{
	/// <summary>
	/// Contains the data extracted from a log file through processing and the state of this extraction.
	/// </summary>
	public class LogData
	{
		private const string UnknownMainTargetName = "Unknown";
		private DateTimeOffset encounterStartTime;
		private FileInfo fileInfo;

		/// <summary>
		/// The <see cref="FileInfo"/> of the corresponding log file.
		/// This will create a new <see cref="System.IO.FileInfo"/> if it doesn't exist and can throw every exception that <see cref="System.IO.FileInfo"/> can throw.
		/// </summary>
		[JsonIgnore]
		public FileInfo FileInfo
		{
			get
			{
				if (fileInfo == null)
				{
					fileInfo = new FileInfo(FileName);
				}

				return fileInfo;
			}
		}

		/// <summary>
		/// The name of the corresponding log file.
		/// </summary>
		public string FileName { get; }

		/// <summary>
		/// The players participating in the encounter recorded in this log.
		/// </summary>
		[JsonProperty]
		public IEnumerable<LogPlayer> Players { get; set; }

		/// <summary>
		/// The result of the encounter recorded in this log.
		/// </summary>
		[JsonProperty]
		public EncounterResult EncounterResult { get; set; } = EncounterResult.Unknown;

		/// <summary>
		/// The mode of the encounter recorded in this log.
		/// </summary>
		[JsonProperty]
		public EncounterMode EncounterMode { get; set; } = EncounterMode.Unknown;

		/// <summary>
		/// The <see cref="Encounter"/> that is recorded in this log.
		/// </summary>
		[JsonProperty]
		public Encounter Encounter { get; set; } = Encounter.Other;

		/// <summary>
		/// The ID of the game map this log was recorded on.
		/// </summary>
		[JsonProperty]
		public int? MapId { get; set; }

		/// <summary>
		/// The author of the log.
		/// </summary>
		[JsonProperty]
		public PointOfView PointOfView { get; set; }

		/// <summary>
		/// The game version (build) used to generate this log.
		/// </summary>
		[JsonProperty]
		public int? GameBuild { get; set; }

		/// <summary>
		/// The version of arcdps used to generate this log.
		/// </summary>
		[JsonProperty]
		public string EvtcVersion { get; set; }

		/// <summary>
		/// The language of the game used to generate the log.
		/// </summary>
		[JsonProperty]
		public GameLanguage GameLanguage { get; set; }

		/// <summary>
		/// The name of the main target of the encounter.
		/// </summary>
		[JsonProperty]
		public string MainTargetName { get; set; } = UnknownMainTargetName;

		/// <summary>
		/// The health percentage of the target of the encounter. If there are multiple targets,
		/// the highest percentage is provided.
		/// </summary>
		[JsonProperty]
		public float? HealthPercentage { get; set; }

		/// <summary>
		/// Time when the encounter started.
		/// Is only an estimate if <see cref="IsEncounterStartTimePrecise"/> is false.
		/// If it's an estimate it will create a <see cref="System.IO.FileInfo"/>.
		/// If the creation of <see cref="System.IO.FileInfo"/> fails it will return the default of <see cref="DateTimeOffset"/>.
		/// </summary>
		[JsonProperty]
		public DateTimeOffset EncounterStartTime
		{
			get
			{
				if (encounterStartTime == default && !IsEncounterStartTimePrecise)
				{
					try
					{
						encounterStartTime = FileInfo.CreationTime;
					}
					catch
					{
						encounterStartTime = default;
					}
				}

				return encounterStartTime;
			}

			set => encounterStartTime = value;
		}

		/// <summary>
		/// The duration of the encounter.
		/// </summary>
		[JsonProperty]
		public TimeSpan EncounterDuration { get; set; }

		/// <summary>
		/// The upload status for uploads to dps.report, using Elite Insights on dps.report.
		/// </summary>
		[JsonProperty]
		public LogUpload DpsReportEIUpload { get; set; } = new LogUpload();

		/// <summary>
		/// The current status of the data processing.
		/// </summary>
		[JsonProperty]
		public ParsingStatus ParsingStatus { get; set; } = ParsingStatus.Unparsed;

		/// <summary>
		/// Contains the time of when parsing of the log was finished, will be default unless <see cref="ParsingStatus"/> is <see cref="Logs.ParsingStatus.Parsed"/>
		/// </summary>
		[JsonProperty]
		public DateTimeOffset ParseTime { get; set; }

		/// <summary>
		/// The amount of milliseconds the parsing of the log took or -1 if <see cref="ParsingStatus"/> is not <see cref="Logs.ParsingStatus.Parsed"/>
		/// </summary>
		[JsonProperty]
		public long ParseMilliseconds { get; set; } = -1;

		/// <summary>
		/// An exception if one was thrown during parsing. Will be null unless <see cref="ParsingStatus"/> is <see cref="Logs.ParsingStatus.Failed"/>.
		/// </summary>
		[JsonProperty]
		public ExceptionData ParsingException { get; set; }

		/// <summary>
		/// The version of the program that was used to parse this log. Will be null unless <see cref="ParsingStatus"/>
		/// is <see cref="Logs.ParsingStatus.Parsed"/> or <see cref="Logs.ParsingStatus.Failed"/>.
		/// </summary>
		[JsonProperty]
		public Version ParsingVersion { get; set; }

		/// <summary>
		/// Indicates whether the start time of the log is precise, or if it's an approximation based on the file creation date.
		/// </summary>
		public bool IsEncounterStartTimePrecise => ParsingStatus == ParsingStatus.Parsed && !MissingEncounterStart;

		/// <summary>
		/// The tags (and info about tags) applied to this log. Set of <see cref="TagInfo"/> rather than Set of string for extensibility's sake.
		/// </summary>
		[JsonProperty]
		public ISet<TagInfo> Tags { get; set; } = new HashSet<TagInfo>();

		/// <summary>
		/// Indicates whether a log is a favorite.
		/// </summary>
		[JsonProperty]
		public bool IsFavorite { get; set; } = false;

		/// <summary>
		/// Indicates whether a log is missing the encounter start time
		/// </summary>
		[JsonProperty]
		private bool MissingEncounterStart { get; set; } = false;

		[JsonConstructor]
		public LogData(string fileName)
		{
			FileName = fileName;
		}

		internal LogData(FileInfo fileInfo, IEnumerable<LogPlayer> players, Encounter encounter,
			string mainTargetName, EncounterResult encounterResult,
			DateTimeOffset encounterStartTime, TimeSpan encounterDuration, long parseMilliseconds = -1)
		{
			this.fileInfo = fileInfo;
			Players = players.ToArray();
			EncounterResult = encounterResult;
			Encounter = encounter;
			MainTargetName = mainTargetName;
			EncounterStartTime = encounterStartTime;
			EncounterDuration = encounterDuration;

			ParseTime = DateTimeOffset.Now;
			ParseMilliseconds = parseMilliseconds;
		}

		/// <summary>
		/// Extracts data from the log file and stores it inside of this <see cref="LogData"/>.
		/// </summary>
		/// <param name="logAnalytics">The log analytics that will be used to process the log.</param>
		/// <remarks>
		/// Extracting the log data is fairly expensive in both file IO activity and CPU time.
		/// </remarks>
		public void ProcessLog(LogAnalytics logAnalytics)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();

				ParsingStatus = ParsingStatus.Parsing;

				var log = logAnalytics.Processor.ProcessLog(FileName, logAnalytics.Parser);
				var analyzer = logAnalytics.CreateAnalyzer(log);

				GameLanguage = log.GameLanguage;
				GameBuild = log.GameBuild;
				EvtcVersion = log.EvtcVersion;
				PointOfView = new PointOfView
				{
					AccountName = log.PointOfView?.AccountName ?? "Unknown",
					CharacterName = log.PointOfView?.Name ?? "Unknown"
				};
				Encounter = log.EncounterData.Encounter;
				MapId = log.MapId;
				MainTargetName = log.MainTarget?.Name ?? UnknownMainTargetName;
				EncounterResult = analyzer.GetResult();
				EncounterMode = analyzer.GetMode();
				HealthPercentage = analyzer.GetMainEnemyHealthFraction();
				if (EncounterResult == EncounterResult.Success)
				{
					HealthPercentage = 0;
				}

				var tagEvents = log.Events.OfType<AgentTagEvent>().Where(x => x.Id != 0 && x.Agent is Player);
				Players = analyzer.GetPlayers().Where(x => x.Identified).Select(p =>
					new LogPlayer(p.Name, p.AccountName, p.Subgroup, p.Profession, p.EliteSpecialization,
						GetGuildGuid(p.GuildGuid))
					{
						Tag = tagEvents.Any(e => e.Agent == p) ? PlayerTag.Commander : PlayerTag.None
					}
				).ToArray();

				if (log.StartTime != null)
				{
					EncounterStartTime = log.StartTime.ServerTime;
				}
				else
				{
					MissingEncounterStart = true;
				}

				EncounterDuration = analyzer.GetEncounterDuration();

				stopwatch.Stop();

				ParseMilliseconds = stopwatch.ElapsedMilliseconds;
				ParseTime = DateTimeOffset.Now;
				ParsingStatus = ParsingStatus.Parsed;
			}
			catch (Exception e)
			{
				ParsingStatus = ParsingStatus.Failed;
				ParsingException = new ExceptionData(e);
			}
			finally
			{
				ParsingVersion = typeof(LogAnalytics).Assembly.GetName().Version;
			}
		}

		private string GetGuildGuid(byte[] guidBytes)
		{
			string GetPart(byte[] bytes, int from, int to)
			{
				var builder = new StringBuilder();
				for (int i = from; i < to; i++)
				{
					builder.Append($"{bytes[i]:x2}");
				}

				return builder.ToString();
			}

			if (guidBytes == null) return null;
			if (guidBytes.Length != 16)
			{
				throw new ArgumentException("The GUID has to consist of 16 bytes", nameof(guidBytes));
			}

			return $"{GetPart(guidBytes, 0, 4)}-{GetPart(guidBytes, 4, 6)}-{GetPart(guidBytes, 6, 8)}" +
			       $"-{GetPart(guidBytes, 8, 10)}-{GetPart(guidBytes, 10, 16)}";
		}
	}
}