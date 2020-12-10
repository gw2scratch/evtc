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
	public class LogData
	{
		private const string UnknownMainTargetName = "Unknown";

		[JsonIgnore]
		public FileInfo FileInfo { get; }

		public string FileName => FileInfo.FullName;

		[JsonProperty]
		public IEnumerable<LogPlayer> Players { get; set; }

		[JsonProperty]
		public EncounterResult EncounterResult { get; set; } = EncounterResult.Unknown;

		[JsonProperty]
		public EncounterMode EncounterMode { get; set; } = EncounterMode.Unknown;

		[JsonProperty]
		public Encounter Encounter { get; set; } = Encounter.Other;

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
		/// </summary>
		[JsonProperty]
		public DateTimeOffset EncounterStartTime { get; set; }

		[JsonProperty]
		public TimeSpan EncounterDuration { get; set; }

		[JsonProperty]
		public LogUpload DpsReportEIUpload { get; set; } = new LogUpload();

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
		/// Indicates whether the start time of the log is precise, or if it's an approximation based on the file modification date.
		/// </summary>
		public bool IsEncounterStartTimePrecise => ParsingStatus == ParsingStatus.Parsed && !MissingEncounterStart;

		/// <summary>
		/// The tags (and info about tags) applied to this log. Set of <see cref="TagInfo"/> rather than Set of string for extensibility's sake.
		/// </summary>
		[JsonProperty]
		public ISet<TagInfo> Tags { get; set; } = new HashSet<TagInfo>();

		[JsonProperty]
		public bool IsFavorite { get; set; } = false;

		[JsonProperty]
		private bool MissingEncounterStart { get; set; } = false;

		[JsonConstructor]
		public LogData(string fileName) : this(new FileInfo(fileName))
		{
		}

		public LogData(FileInfo fileInfo)
		{
			FileInfo = fileInfo;
			EncounterStartTime = fileInfo.CreationTime;
		}

		internal LogData(FileInfo fileInfo, IEnumerable<LogPlayer> players, Encounter encounter,
			string mainTargetName, EncounterResult encounterResult,
			DateTimeOffset encounterStartTime, TimeSpan encounterDuration, long parseMilliseconds = -1)
		{
			FileInfo = fileInfo;
			Players = players.ToArray();
			EncounterResult = encounterResult;
			Encounter = encounter;
			MainTargetName = mainTargetName;
			EncounterStartTime = encounterStartTime;
			EncounterDuration = encounterDuration;

			ParseTime = DateTimeOffset.Now;
			ParseMilliseconds = parseMilliseconds;
		}

		public void ParseData(LogAnalytics logAnalytics)
		{
			try
			{
				var stopwatch = Stopwatch.StartNew();

				ParsingStatus = ParsingStatus.Parsing;

				var parsedLog = logAnalytics.Parser.ParseLog(FileInfo.FullName);
				var log = logAnalytics.Processor.ProcessLog(parsedLog);
				var analyzer = logAnalytics.AnalyzerFactory(log);

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