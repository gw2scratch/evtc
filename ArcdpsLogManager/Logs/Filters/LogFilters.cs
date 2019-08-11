using System;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters
{
	public class LogFilters
	{
		public static readonly DateTime GuildWars2ReleaseDate = new DateTime(2012, 8, 28);
		public const string EncounterFilterAll = "All";

		public bool ShowParseUnparsedLogs { get; set; } = true;
		public bool ShowParseParsingLogs { get; set; } = true;
		public bool ShowParseParsedLogs { get; set; } = true;
		public bool ShowParseFailedLogs { get; set; } = true;
		public string EncounterFilter { get; set; } = EncounterFilterAll;

		public bool ShowSuccessfulLogs { get; set; } = true;
		public bool ShowFailedLogs { get; set; } = true;
		public bool ShowUnknownLogs { get; set; } = true;

		public DateTime? MinDateTimeFilter { get; set; } = GuildWars2ReleaseDate;
		public DateTime? MaxDateTimeFilter { get; set; } = DateTime.Now.Date.AddDays(1);

		public bool FilterLog(LogData log)
		{
			return FilterByEncounterName(log) && FilterByResult(log) && FilterByParsingStatus(log) && FilterByTime(log);
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
			return EncounterFilter == EncounterFilterAll ||
			       log.ParsingStatus == ParsingStatus.Parsed && log.EncounterName == EncounterFilter;
		}

		private bool FilterByTime(LogData log)
		{
			return !(log.EncounterStartTime.LocalDateTime < MinDateTimeFilter) &&
			       !(log.EncounterStartTime.LocalDateTime > MaxDateTimeFilter);
		}
	}
}