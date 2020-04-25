using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters
{
	public class LogFilters : ILogFilter
	{
		public static readonly DateTime GuildWars2ReleaseDate = new DateTime(2012, 8, 28);

		public bool ShowParseUnparsedLogs { get; set; } = true;
		public bool ShowParseParsingLogs { get; set; } = true;
		public bool ShowParseParsedLogs { get; set; } = true;
		public bool ShowParseFailedLogs { get; set; } = true;
		public List<LogGroup> LogGroups { get; set; } = new List<LogGroup> {new RootLogGroup(Enumerable.Empty<LogData>())};

		public bool ShowSuccessfulLogs { get; set; } = true;
		public bool ShowFailedLogs { get; set; } = true;
		public bool ShowUnknownLogs { get; set; } = true;

		public bool ShowNormalModeLogs { get; set; } = true;
		public bool ShowChallengeModeLogs { get; set; } = true;

		public DateTime? MinDateTimeFilter { get; set; } = GuildWars2ReleaseDate;
		public DateTime? MaxDateTimeFilter { get; set; } = DateTime.Now.Date.AddDays(1);

		private readonly ILogNameProvider nameProvider;
		private readonly IReadOnlyList<ILogFilter> additionalFilters;

		public LogFilters(ILogNameProvider nameProvider, params ILogFilter[] additionalFilters)
		{
			this.nameProvider = nameProvider;
			this.additionalFilters = additionalFilters;
		}

		public bool FilterLog(LogData log)
		{
			foreach (var filter in additionalFilters)
			{
				if (!filter.FilterLog(log))
				{
					return false;
				}
			}

			return FilterByEncounterName(log)
			       && FilterByResult(log)
			       && FilterByParsingStatus(log)
			       && FilterByTime(log)
			       && FilterByEncounterMode(log);
		}

		private bool FilterByParsingStatus(LogData log)
		{
			return (ShowParseUnparsedLogs || log.ParsingStatus != ParsingStatus.Unparsed) &&
			       (ShowParseParsedLogs || log.ParsingStatus != ParsingStatus.Parsed) &&
			       (ShowParseParsingLogs || log.ParsingStatus != ParsingStatus.Parsing) &&
			       (ShowParseFailedLogs || log.ParsingStatus != ParsingStatus.Failed);
		}

		private bool FilterByResult(LogData log)
		{
			return (ShowFailedLogs || log.EncounterResult != EncounterResult.Failure) &&
			       (ShowUnknownLogs || log.EncounterResult != EncounterResult.Unknown) &&
			       (ShowSuccessfulLogs || log.EncounterResult != EncounterResult.Success);
		}

		private bool FilterByEncounterName(LogData log)
		{
			return LogGroups.Any(x => x.FilterLog(log))
			       && log.ParsingStatus == ParsingStatus.Parsed;
		}

		private bool FilterByTime(LogData log)
		{
			return !(log.EncounterStartTime.LocalDateTime < MinDateTimeFilter) &&
			       !(log.EncounterStartTime.LocalDateTime > MaxDateTimeFilter);
		}

		private bool FilterByEncounterMode(LogData log)
		{
			return (log.EncounterMode == EncounterMode.Normal && ShowNormalModeLogs) ||
			       (log.EncounterMode == EncounterMode.Unknown && ShowNormalModeLogs) ||
			       (log.EncounterMode == EncounterMode.Challenge && ShowChallengeModeLogs);
		}
	}
}