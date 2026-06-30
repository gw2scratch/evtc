using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GW2Scratch.ArcdpsLogManager.Analytics;
using GW2Scratch.ArcdpsLogManager.Logs.Extras;
using GW2Scratch.ArcdpsLogManager.Logs.Tagging;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using Newtonsoft.Json;
using System.Globalization;

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
		public string FileName { get; private set; }

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
		/// Extra data that is only relevant for some logs.
		/// </summary>
		[JsonProperty]
		public LogExtras LogExtras { get; set; } = null;

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
				PointOfView = new PointOfView { AccountName = log.PointOfView?.AccountName ?? "Unknown", CharacterName = log.PointOfView?.Name ?? "Unknown" };
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

				var tagEvents = log.Events.OfType<AgentMarkerEvent>().Where(x => x.Agent is Player).ToList();
				var players = analyzer.GetPlayers().Where(x => x.Identified).ToList();

				Players = [.. players.Select(p =>
				{
					var hasTag = tagEvents.Any(e => e.Agent == p && _commanderTags.Contains(ContentLocal.GuidToString(e.Marker.ContentGuid)));

					return new LogPlayer(p.Name, p.AccountName, p.Subgroup, p.Profession, p.EliteSpecialization, GetGuildGuid(p.GuildGuid))
					{
						Tag = hasTag ? PlayerTag.Commander : PlayerTag.None
					};
				})];

				if (log.StartTime != null)
				{
					EncounterStartTime = log.StartTime.ServerTime;
				}
				else
				{
					MissingEncounterStart = true;
				}

				LogExtras = new LogExtras();

				var mistlockInstabilities = logAnalytics.FractalInstabilityDetector.GetInstabilities(log).ToList();
				if (Encounter.GetEncounterCategory() == EncounterCategory.Fractal || mistlockInstabilities.Count > 0 || log.FractalScale != null)
				{
					LogExtras.FractalExtras = new FractalExtras { MistlockInstabilities = mistlockInstabilities, FractalScale = log.FractalScale, };
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

		/// <summary>
		/// Assigns this log data to a different file without doing any checks.
		/// </summary>
		/// <remarks>
		/// This is useful in case the file is moved, renamed, etc. and we want to avoid processing it again.
		/// 
		/// Note that we do not check whether the file exists and whether the data actually belongs to it.
		/// You may need to manually update caches that include this <see cref="LogData"/> as well.
		/// </remarks>
		/// <param name="newFileName">The new filename this log data belongs to.</param>
		public void AssignToFile(string newFileName)
		{
			fileInfo = null;
			FileName = newFileName;
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

		public string ShortDurationString
		{
			get
			{
				if (ParsingStatus is ParsingStatus.Unparsed or ParsingStatus.Parsing)
				{
					// We don't want to make this wider than actual times.
					return "?";
				}

				var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
				var totalSeconds = EncounterDuration.TotalSeconds;
				var minutes = (int) totalSeconds / 60;
				var seconds = (int) totalSeconds % 60;
				var milliseconds = (int) ((totalSeconds - Math.Floor(totalSeconds)) * 1000);

				if (minutes > 60)
				{
					var hours = minutes / 60;
					minutes -= hours * 60;
					return $"{hours}h {minutes:0}m {seconds}{separator}{milliseconds / 100}s";
				}

				return $"{minutes:0}m {seconds}{separator}{milliseconds / 100}s";
			}
		}

		private static readonly HashSet<string> _commanderTags =
			[
				"4242f370-667c-e54e-b3bf-22be8d06f986", // Red Commander Tag
				"e57aae9e-e7fc-5d45-8b0c-f16be4b096bf", // Orange Commander Tag
				"af9442a2-90c6-2145-96e0-b339eb3bde92", // Yellow Commander Tag
				"74ad480e-531f-4740-a407-879976c8ca91", // Green CommanderTag
				"96f4ab5c-dec5-2943-8837-5c7a03ab7614", // Cyan Commander Tag
				"ae714fc5-e4ea-464c-8961-cd78e86f9291", // Blue Commander Tag
				"1993fadb-6fb7-0e43-83a2-23a54d311f7d", // Purple Commander Tag
				"e911d8c0-ef2f-df4d-8d25-2e5fb1283c62", // Pink Commander Tag
				"a59678cd-fb57-3243-9d7f-cbf58d8bcec3", // White Commander Tag
				"ca76ab02-3593-b044-8f69-2fe29df03d17", // Red Catmander Tag
				"9fdf03e9-ba09-a245-8c1e-dda4d81bc34d", // Orange Catmander Tag
				"6bce90e9-9016-b448-969e-b317784a8334", // Yellow Catmander Tag
				"2ca226e0-7262-c743-ba19-3acf6f9d0af6", // Green CatmanderTag
				"a8072d65-ce35-924b-abba-c831b12019d7", // Cyan Catmander Tag
				"9b94f0fd-616e-7f4a-a58e-fdc8c59fb689", // Blue Catmander Tag
				"7224a4af-710e-4243-bfe0-32629e17ca6e", // Purple Catmander Tag
				"4387be61-46d4-3246-aa7b-333168ea58ea", // Pink Catmander Tag
				"a0b0ec07-6bc8-3b40-a293-c1cdec4a7de7", // White Catmander Tag
			];
	}
}