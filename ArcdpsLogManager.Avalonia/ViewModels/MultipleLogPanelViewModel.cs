using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Tagging;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the multi-log-selection aggregate panel (Avalonia counterpart of the Eto
	/// <c>Controls/MultipleLogPanel.cs</c>), shown instead of the single-log
	/// <see cref="LogDetailPanelViewModel"/> when 2+ logs are selected in the log grid.
	/// </summary>
	public partial class MultipleLogPanelViewModel : ObservableObject
	{
		private readonly LogCacheService cacheService;
		private readonly ImageProvider images;
		private IReadOnlyList<LogData> logs = Array.Empty<LogData>();
		private readonly EventHandler<EventArgs> debugDataChangedHandler;
		private bool uploadProcessorAttached;

		[ObservableProperty] private bool hasSelection;
		[ObservableProperty] private string countText = "";
		[ObservableProperty] private string successCountText = "0";
		[ObservableProperty] private string failureCountText = "0";
		/// <summary>Sum of each selected log's own encounter duration (fight-clock time).</summary>
		[ObservableProperty] private string sumOfDurationsText = "";

		/// <summary>Wall-clock span from the earliest log's start to the latest log's end —
		/// i.e. how long the whole selected session took, including time between pulls.</summary>
		[ObservableProperty] private string sessionLengthText = "";

		[ObservableProperty] private bool showDebugData;
		[ObservableProperty] private string parseTimeText = "";

		[ObservableProperty] private string newTag = "";
		[ObservableProperty] private bool hasTags;

		/// <summary>Hides the Tags section entirely when off (Settings.ShowTags), not just the
		/// sidebar filter.</summary>
		[ObservableProperty] private bool showTags;
		private readonly EventHandler<EventArgs> showTagsChangedHandler;
		public IReadOnlyList<string> Tags { get; private set; } = new List<string>();

		// dps.report aggregate status (display-only; see remarks above).
		[ObservableProperty] private string notUploadedText = "0";
		[ObservableProperty] private string uploadingText = "0";
		[ObservableProperty] private string uploadedText = "0";
		[ObservableProperty] private string uploadFailedText = "0";
		[ObservableProperty] private bool hasUploadFailures;
		[ObservableProperty] private string processingFailedText = "0";
		[ObservableProperty] private bool hasProcessingFailures;
		[ObservableProperty] private double uploadProgressMax = 1;
		[ObservableProperty] private double uploadProgressValue;
		[ObservableProperty] private bool hasUploadedLogs;
		[ObservableProperty] private string uploadedUrlsText = "";
		[ObservableProperty] private string uploadButtonText = "Upload missing logs (0)";
		[ObservableProperty] private bool canUploadMissing;
		[ObservableProperty] private bool canCancelUploads;
		public IReadOnlyList<string> UploadedUrls { get; private set; } = new List<string>();

		/// <summary>The "Copy links" button's icon, swapped between the enabled/disabled variant
		/// depending on <see cref="HasUploadedLogs"/> (matches the Eto WPF-only icon-button behavior).</summary>
		[ObservableProperty] private Bitmap? copyIcon;

		/// <summary>
		/// The background processing service, assigned once available (the cache may still be
		/// loading, or read-only, when this view model is constructed). Enables "Reprocess all".
		/// </summary>
		[ObservableProperty] private LogProcessingService? processingService;

		/// <summary>
		/// The background upload processor, assigned alongside <see cref="ProcessingService"/>.
		/// Enables "Upload missing logs" / "Cancel".
		/// </summary>
		public UploadProcessor? UploadProcessor
		{
			get => uploadProcessor;
			set
			{
				if (ReferenceEquals(uploadProcessor, value))
				{
					return;
				}

				uploadProcessor = value;
				if (uploadProcessor != null && !uploadProcessorAttached)
				{
					uploadProcessorAttached = true;
					uploadProcessor.Processed += OnUploadProcessorUpdate;
					uploadProcessor.Scheduled += OnUploadProcessorUpdate;
					uploadProcessor.Unscheduled += OnUploadProcessorUpdate;
				}

				UpdateDpsReportUploadStatus();
				UploadMissingCommand.NotifyCanExecuteChanged();
				CancelUploadsCommand.NotifyCanExecuteChanged();
			}
		}

		private UploadProcessor? uploadProcessor;

		public bool CanReprocess => ProcessingService != null;

		public MultipleLogPanelViewModel(LogCacheService cacheService, ImageProvider images)
		{
			this.cacheService = cacheService;
			this.images = images;
			copyIcon = images.GetCopyButtonDisabledImage();

			showDebugData = Settings.ShowDebugData;
			debugDataChangedHandler = (_, _) =>
				Dispatcher.UIThread.Post(() => ShowDebugData = Settings.ShowDebugData);
			Settings.ShowDebugDataChanged += debugDataChangedHandler;

			showTags = Settings.ShowTags;
			showTagsChangedHandler = (_, _) =>
				Dispatcher.UIThread.Post(() => ShowTags = Settings.ShowTags);
			Settings.ShowTagsChanged += showTagsChangedHandler;
		}

		partial void OnProcessingServiceChanged(LogProcessingService? value)
		{
			OnPropertyChanged(nameof(CanReprocess));
			ReprocessAllCommand.NotifyCanExecuteChanged();
		}

		// Raised on a background thread by the upload processor; marshal to the UI thread.
		private void OnUploadProcessorUpdate(object? sender, EventArgs e)
		{
			Dispatcher.UIThread.Post(UpdateDpsReportUploadStatus);
		}

		/// <summary>Updates the panel for the given selection; called by the Logs section when the
		/// grid's multi-selection changes. An empty/singleton selection hides the panel.</summary>
		public void Show(IReadOnlyList<LogData> selectedLogs)
		{
			logs = selectedLogs;
			if (logs.Count < 2)
			{
				HasSelection = false;
				return;
			}

			HasSelection = true;
			CountText = $"{logs.Count} logs selected";

			var parsed = logs.Where(x => x.ParsingStatus == ParsingStatus.Parsed).ToList();
			var sumOfDurations = parsed.Aggregate(TimeSpan.Zero, (acc, log) => acc + log.EncounterDuration);
			SumOfDurationsText = FormatTimeSpan(sumOfDurations);

			var timed = parsed.Where(x => x.EncounterStartTime != default).ToList();
			SessionLengthText = timed.Count > 0
				? FormatTimeSpan(timed.Max(x => x.EncounterStartTime + x.EncounterDuration) - timed.Min(x => x.EncounterStartTime))
				: FormatTimeSpan(TimeSpan.Zero);

			SuccessCountText = parsed.Count(x => x.EncounterResult == EncounterResult.Success).ToString();
			FailureCountText = parsed.Count(x => x.EncounterResult == EncounterResult.Failure).ToString();
			ParseTimeText = $"{logs.Sum(x => x.ParseMilliseconds)} ms";

			UpdateTags();
			UpdateDpsReportUploadStatus();
		}

		private void UpdateTags()
		{
			if (logs.Count == 0)
			{
				Tags = new List<string>();
				HasTags = false;
				OnPropertyChanged(nameof(Tags));
				return;
			}

			var allTagSets = logs.Select(x => x.Tags).ToList();
			var commonTags = new HashSet<TagInfo>(allTagSets[0]);
			foreach (var tagSet in allTagSets.Skip(1))
			{
				commonTags.IntersectWith(tagSet);
			}

			Tags = commonTags.Select(t => t.Name).OrderBy(t => t).ToList();
			OnPropertyChanged(nameof(Tags));
			HasTags = Tags.Count > 0;
		}

		private void UpdateDpsReportUploadStatus()
		{
			int notUploaded = logs.Count(x => x.DpsReportEIUpload.UploadState == UploadState.NotUploaded);
			int queued = logs.Count(x => x.DpsReportEIUpload.UploadState == UploadState.Queued);
			int uploading = logs.Count(x => x.DpsReportEIUpload.UploadState == UploadState.Uploading);
			int uploaded = logs.Count(x => x.DpsReportEIUpload.UploadState == UploadState.Uploaded);
			int uploadFailed = logs.Count(x => x.DpsReportEIUpload.UploadState == UploadState.UploadError);
			int processingFailed = logs.Count(x => x.DpsReportEIUpload.UploadState == UploadState.ProcessingError);

			int finished = uploaded + uploadFailed + processingFailed;
			int totalRequested = queued + uploading + uploaded + uploadFailed + processingFailed;
			int missingUploads = notUploaded + uploadFailed + processingFailed;
			UploadProgressMax = totalRequested > 0 ? totalRequested : 1;
			UploadProgressValue = finished;

			NotUploadedText = notUploaded.ToString();
			UploadingText = (uploading + queued).ToString();
			UploadedText = uploaded.ToString();
			UploadFailedText = uploadFailed.ToString();
			HasUploadFailures = uploadFailed > 0;
			ProcessingFailedText = processingFailed.ToString();
			HasProcessingFailures = processingFailed > 0;

			UploadButtonText = $"Upload missing logs ({missingUploads})";
			CanUploadMissing = missingUploads > 0 && UploadProcessor != null;
			CanCancelUploads = queued > 0 && UploadProcessor != null;
			UploadMissingCommand.NotifyCanExecuteChanged();
			CancelUploadsCommand.NotifyCanExecuteChanged();

			UploadedUrls = logs.Where(x => x.DpsReportEIUpload.Url != null)
				.Select(x => x.DpsReportEIUpload.Url!)
				.ToList();
			OnPropertyChanged(nameof(UploadedUrls));
			HasUploadedLogs = UploadedUrls.Count > 0;
			UploadedUrlsText = string.Join(Environment.NewLine, UploadedUrls);
			CopyIcon = HasUploadedLogs ? images.GetCopyButtonEnabledImage() : images.GetCopyButtonDisabledImage();
		}

		[RelayCommand(CanExecute = nameof(CanReprocess))]
		private void ReprocessAll()
		{
			if (ProcessingService is not { } service)
			{
				return;
			}

			foreach (var log in logs)
			{
				service.ScheduleReprocessing(log);
			}
		}

		[RelayCommand(CanExecute = nameof(CanUploadMissing))]
		private void UploadMissing()
		{
			if (UploadProcessor is not { } processor)
			{
				return;
			}

			foreach (var log in logs)
			{
				var state = log.DpsReportEIUpload.UploadState;
				if (state is UploadState.NotUploaded or UploadState.UploadError or UploadState.ProcessingError)
				{
					processor.ScheduleDpsReportEIUpload(log);
				}
			}
		}

		[RelayCommand(CanExecute = nameof(CanCancelUploads))]
		private void CancelUploads()
		{
			UploadProcessor?.CancelDpsReportEIUpload(logs);
		}

		[RelayCommand]
		private void AddTag()
		{
			string tag = NewTag.Trim();
			if (logs.Count == 0 || string.IsNullOrWhiteSpace(tag))
			{
				return;
			}

			bool added = false;
			foreach (var log in logs)
			{
				if (log.Tags.Add(new TagInfo(tag)))
				{
					added = true;
					cacheService.NotifyChanged(log);
				}
			}

			if (added)
			{
				UpdateTags();
			}

			NewTag = "";
		}

		[RelayCommand]
		private void RemoveTag(string? tag)
		{
			if (logs.Count == 0 || tag == null)
			{
				return;
			}

			bool removed = false;
			foreach (var log in logs)
			{
				if (log.Tags.Remove(new TagInfo(tag)))
				{
					removed = true;
					cacheService.NotifyChanged(log);
				}
			}

			if (removed)
			{
				UpdateTags();
			}
		}

		private static string FormatTimeSpan(TimeSpan time)
		{
			var str = $@"{time:hh\h\ mm\m\ ss\s}";
			if (time.Days > 0)
			{
				str = $@"{time:%d\d} " + str;
			}

			return str;
		}
	}
}
