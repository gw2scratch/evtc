using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups;
using GW2Scratch.ArcdpsLogManager.Logs.Tagging;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters
{
	public sealed class LogFilters : ILogFilter, INotifyPropertyChanged
	{
		private bool showParseUnparsedLogs = true;
		private bool showParseParsingLogs = true;
		private bool showParseParsedLogs = true;
		private bool showParseFailedLogs = true;
		private IReadOnlyList<LogGroup> logGroups = new List<LogGroup> {new RootLogGroup(Enumerable.Empty<LogData>())};
		private IReadOnlyList<string> requiredTags = new List<string>();
		private bool showSuccessfulLogs = true;
		private bool showFailedLogs = true;
		private bool showUnknownLogs = true;
		private bool showNormalModeLogs = true;
		private bool showChallengeModeLogs = true;
		private bool showNonFavoriteLogs = true;
		private bool showFavoriteLogs = true;
		private DateTime? minDateTime = null;
		private DateTime? maxDateTime = null;
		private CompositionFilters compositionFilters = new CompositionFilters();
		private InstabilityFilters instabilityFilters = new InstabilityFilters();
		private readonly IReadOnlyList<ILogFilter> additionalFilters;

		public bool ShowParseUnparsedLogs
		{
			get => showParseUnparsedLogs;
			set
			{
				if (value == showParseUnparsedLogs) return;
				showParseUnparsedLogs = value;
				OnPropertyChanged();
			}
		}

		public bool ShowParseParsingLogs
		{
			get => showParseParsingLogs;
			set
			{
				if (value == showParseParsingLogs) return;
				showParseParsingLogs = value;
				OnPropertyChanged();
			}
		}

		public bool ShowParseParsedLogs
		{
			get => showParseParsedLogs;
			set
			{
				if (value == showParseParsedLogs) return;
				showParseParsedLogs = value;
				OnPropertyChanged();
			}
		}

		public bool ShowParseFailedLogs
		{
			get => showParseFailedLogs;
			set
			{
				if (value == showParseFailedLogs) return;
				showParseFailedLogs = value;
				OnPropertyChanged();
			}
		}

		public IReadOnlyList<LogGroup> LogGroups
		{
			get => logGroups;
			set
			{
				if (Equals(value, logGroups)) return;
				logGroups = value;
				OnPropertyChanged();
			}
		}

		public IReadOnlyList<string> RequiredTags
		{
			get => requiredTags;
			set
			{
				if (Equals(value, requiredTags)) return;
				requiredTags = value;
				OnPropertyChanged();
			}
		}

		public bool ShowSuccessfulLogs
		{
			get => showSuccessfulLogs;
			set
			{
				if (value == showSuccessfulLogs) return;
				showSuccessfulLogs = value;
				OnPropertyChanged();
			}
		}

		public bool ShowFailedLogs
		{
			get => showFailedLogs;
			set
			{
				if (value == showFailedLogs) return;
				showFailedLogs = value;
				OnPropertyChanged();
			}
		}

		public bool ShowUnknownLogs
		{
			get => showUnknownLogs;
			set
			{
				if (value == showUnknownLogs) return;
				showUnknownLogs = value;
				OnPropertyChanged();
			}
		}

		public bool ShowNormalModeLogs
		{
			get => showNormalModeLogs;
			set
			{
				if (value == showNormalModeLogs) return;
				showNormalModeLogs = value;
				OnPropertyChanged();
			}
		}

		public bool ShowChallengeModeLogs
		{
			get => showChallengeModeLogs;
			set
			{
				if (value == showChallengeModeLogs) return;
				showChallengeModeLogs = value;
				OnPropertyChanged();
			}
		}

		public bool ShowNonFavoriteLogs
		{
			get => showNonFavoriteLogs;
			set
			{
				if (value == showNonFavoriteLogs) return;
				showNonFavoriteLogs = value;
				OnPropertyChanged();
			}
		}

		public bool ShowFavoriteLogs
		{
			get => showFavoriteLogs;
			set
			{
				if (value == showFavoriteLogs) return;
				showFavoriteLogs = value;
				OnPropertyChanged();
			}
		}

		public DateTime? MinDateTime
		{
			get => minDateTime;
			set
			{
				if (Nullable.Equals(value, minDateTime)) return;
				minDateTime = value;
				OnPropertyChanged();
			}
		}

		public DateTime? MaxDateTime
		{
			get => maxDateTime;
			set
			{
				if (Nullable.Equals(value, maxDateTime)) return;
				maxDateTime = value;
				OnPropertyChanged();
			}
		}

		public CompositionFilters CompositionFilters
		{
			get => compositionFilters;
			set
			{
				if (compositionFilters.Equals(value)) return;
				compositionFilters = value;
				OnPropertyChanged();
			}
		}
		
		public InstabilityFilters InstabilityFilters
		{
			get => instabilityFilters;
			set
			{
				if (instabilityFilters.Equals(value)) return;
				instabilityFilters = value;
				OnPropertyChanged();
			}
		}

		public LogFilters(params ILogFilter[] additionalFilters)
		{
			this.additionalFilters = additionalFilters;
			CompositionFilters = new CompositionFilters();
			CompositionFilters.PropertyChanged += (_, _) => OnPropertyChanged(nameof(CompositionFilters));
			InstabilityFilters = new InstabilityFilters();
			InstabilityFilters.PropertyChanged += (_, _) => OnPropertyChanged(nameof(InstabilityFilters));
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
			       && FilterByEncounterMode(log)
			       && FilterByFavoriteStatus(log)
			       && FilterByTags(log)
			       && FilterByComposition(log)
			       && FilterByInstabilities(log);
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
			// We default to keeping everything in case no log groups are selected.
			// It mainly serves as a workaround for users accidentally deselecting everything
			// in the UI, and for issues with the selection in LogEncounterFilterTree being
			// reset when logs are updated.
			
			if (LogGroups.Count == 0)
			{
				return true;
			}
			
			return LogGroups.Any(x => x.FilterLog(log));
		}

		private bool FilterByTags(LogData log)
		{
			return requiredTags.All(tag => log.Tags.Contains(new TagInfo(tag)));
		}

		private bool FilterByTime(LogData log)
		{
			return (!MinDateTime.HasValue || log.EncounterStartTime.LocalDateTime >= MinDateTime) &&
			       (!MaxDateTime.HasValue || log.EncounterStartTime.LocalDateTime <= MaxDateTime);
		}

		private bool FilterByEncounterMode(LogData log)
		{
			return (log.EncounterMode == EncounterMode.Normal && ShowNormalModeLogs) ||
			       (log.EncounterMode == EncounterMode.Unknown && ShowNormalModeLogs) ||
			       (log.EncounterMode == EncounterMode.Challenge && ShowChallengeModeLogs);
		}

		private bool FilterByFavoriteStatus(LogData log)
		{
			return (log.IsFavorite && ShowFavoriteLogs) || (!log.IsFavorite && ShowNonFavoriteLogs);
		}

		private bool FilterByComposition(LogData log)
		{
			return CompositionFilters.FilterLog(log);
		}
		
		private bool FilterByInstabilities(LogData log)
		{
			return InstabilityFilters.FilterLog(log);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}