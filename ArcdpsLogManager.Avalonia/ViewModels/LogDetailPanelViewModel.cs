using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Logs.Tagging;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the single-log detail panel (Avalonia counterpart of the Eto
	/// <c>Controls/LogDetailPanel.cs</c>). Shows encounter name, mode, result, timing, fractal
	/// instabilities, dps.report upload status, tags, the group composition and, for failed logs,
	/// the processing error.
	/// </summary>
	public partial class LogDetailPanelViewModel : ObservableObject
	{
		private readonly ImageProvider images;
		private readonly ILogNameProvider nameProvider;
		private readonly LogCacheService cacheService;
		private LogData? currentLog;
		private bool uploadProcessorAttached;

		[ObservableProperty] private bool hasSelection;
		[ObservableProperty] private string newTag = "";
		[ObservableProperty] private string name = "";
		[ObservableProperty] private string modeText = "";
		[ObservableProperty] private bool hasMode;
		[ObservableProperty] private string resultText = "";
		[ObservableProperty] private string fileName = "";
		[ObservableProperty] private string filePath = "";
		[ObservableProperty] private string parseStatus = "";

		[ObservableProperty] private bool hasInstabilities;
		[ObservableProperty] private bool hasTags;
		[ObservableProperty] private string uploadStatus = "";
		[ObservableProperty] private string? uploadUrl;
		[ObservableProperty] private bool hasUploadUrl;
		[ObservableProperty] private string uploadButtonText = "Upload";
		[ObservableProperty] private bool canUpload;
		[ObservableProperty] private bool isFailed;
		[ObservableProperty] private string failureText = "";

		/// <summary>The "Copy" button's icon, swapped between the enabled/disabled variant depending
		/// on <see cref="HasUploadUrl"/>.</summary>
		[ObservableProperty] private Bitmap? copyIcon;

		public List<PlayerSubgroupRow> PlayerGroups { get; private set; } = new();
		public IReadOnlyList<Bitmap> Instabilities { get; private set; } = new List<Bitmap>();
		public IReadOnlyList<string> Tags { get; private set; } = new List<string>();

		/// <summary>
		/// The background upload processor, assigned once available (the cache may still be loading,
		/// or read-only, when this view model is constructed). Enables the Upload button.
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

				RefreshUploadStatus();
				UploadCommand.NotifyCanExecuteChanged();
			}
		}

		private UploadProcessor? uploadProcessor;

		public LogDetailPanelViewModel(ImageProvider images, ILogNameProvider nameProvider, LogCacheService cacheService)
		{
			this.images = images;
			this.nameProvider = nameProvider;
			this.cacheService = cacheService;
			copyIcon = images.GetCopyButtonDisabledImage();
		}

		// Raised on a background thread by the upload processor; marshal to the UI thread.
		private void OnUploadProcessorUpdate(object? sender, System.EventArgs e)
		{
			Dispatcher.UIThread.Post(RefreshUploadStatus);
		}

		[RelayCommand(CanExecute = nameof(CanUpload))]
		private void Upload()
		{
			if (currentLog != null && UploadProcessor != null)
			{
				UploadProcessor.ScheduleDpsReportEIUpload(currentLog);
			}
		}

		[RelayCommand]
		private void AddTag()
		{
			string tag = NewTag.Trim();
			if (currentLog == null || string.IsNullOrWhiteSpace(tag))
			{
				return;
			}

			if (currentLog.Tags.Add(new TagInfo(tag)))
			{
				cacheService.NotifyChanged(currentLog);
				RefreshTags();
			}

			NewTag = "";
		}

		[RelayCommand]
		private void RemoveTag(string? tag)
		{
			if (currentLog == null || tag == null)
			{
				return;
			}

			if (currentLog.Tags.Remove(new TagInfo(tag)))
			{
				cacheService.NotifyChanged(currentLog);
				RefreshTags();
			}
		}

		private void RefreshTags()
		{
			Tags = currentLog == null ? new List<string>() : currentLog.Tags.Select(t => t.Name).OrderBy(t => t).ToList();
			OnPropertyChanged(nameof(Tags));
			HasTags = Tags.Count > 0;
		}

		public void Show(LogRow? row)
		{
			currentLog = row?.Log;
			var log = row?.Log;
			if (log is null)
			{
				HasSelection = false;
				return;
			}

			HasSelection = true;
			Name = nameProvider.GetName(log);

			string mode = log.EncounterMode switch
			{
				EncounterMode.Challenge => "Challenge Mode",
				EncounterMode.LegendaryChallenge => "Legendary Challenge Mode",
				EncounterMode.Emboldened1 => "Emboldened 1",
				EncounterMode.Emboldened2 => "Emboldened 2",
				EncounterMode.Emboldened3 => "Emboldened 3",
				EncounterMode.Emboldened4 => "Emboldened 4",
				EncounterMode.Emboldened5 => "Emboldened 5",
				EncounterMode.Quickplay => "Quickplay",
				_ => "",
			};

			int? scale = log.LogExtras?.FractalExtras?.FractalScale;
			if (scale != null)
			{
				mode += mode != "" ? ", " : "";
				mode += $"Scale {scale}";
			}

			ModeText = mode;
			HasMode = !string.IsNullOrWhiteSpace(mode);

			string result = log.EncounterResult switch
			{
				EncounterResult.Success => "Success",
				EncounterResult.Failure => log.HealthPercentage.HasValue
					? $"Failure ({log.HealthPercentage * 100:0.00}% health)"
					: "Failure",
				EncounterResult.Unknown => "Unknown",
				_ => log.EncounterResult.ToString(),
			};
			ResultText = $"{result} in {log.ShortDurationString}";

			FileName = log.FileName is null ? "" : Path.GetFileName(log.FileName);
			FilePath = log.FileName ?? "";
			ParseStatus = log.ParsingStatus.ToString();

			PlayerGroups = (log.Players ?? Enumerable.Empty<LogPlayer>())
				.OrderBy(p => p.Subgroup)
				.ThenBy(p => p.Profession)
				.ThenBy(p => p.EliteSpecialization)
				.GroupBy(p => p.Subgroup)
				.OrderBy(g => g.Key)
				.Select(g => new PlayerSubgroupRow(g.Key, g.Select(p => new PlayerRow(p, images)).ToList()))
				.ToList();
			OnPropertyChanged(nameof(PlayerGroups));

			// Fractal instabilities.
			Instabilities = (log.LogExtras?.FractalExtras?.MistlockInstabilities ?? Enumerable.Empty<MistlockInstability>())
				.OrderBy(GameNames.GetInstabilityName)
				.Select(images.GetMistlockInstabilityIcon)
				.ToList();
			OnPropertyChanged(nameof(Instabilities));
			HasInstabilities = Instabilities.Count > 0;

			// Tags.
			Tags = log.Tags.Select(t => t.Name).OrderBy(t => t).ToList();
			OnPropertyChanged(nameof(Tags));
			HasTags = Tags.Count > 0;

			// dps.report upload status.
			RefreshUploadStatus();

			// Failed processing.
			IsFailed = log.ParsingStatus == ParsingStatus.Failed;
			FailureText = IsFailed && log.ParsingException != null
				? $"{log.ParsingException.ExceptionName}: {log.ParsingException.Message}\n\n{log.ParsingException.StackTrace}"
				: "";
		}

		/// <summary>Recomputes the dps.report status text/button for the currently shown log
		/// (Avalonia counterpart of the Eto <c>LogDetailPanel.UpdateUploadStatus</c>). Called both
		/// when a new log is selected and whenever the upload processor reports a state change for
		/// any log (including ones not currently shown — cheap enough to just recompute).</summary>
		private void RefreshUploadStatus()
		{
			if (currentLog == null)
			{
				return;
			}

			const string reuploadButtonText = "Reupload";

			var upload = currentLog.DpsReportEIUpload;
			string buttonText;
			bool enabled;
			string text;
			switch (upload.UploadState)
			{
				case UploadState.NotUploaded:
					buttonText = "Upload";
					enabled = true;
					text = "Not uploaded";
					break;
				case UploadState.Queued:
					buttonText = "Uploading...";
					enabled = false;
					text = "Queued for upload";
					break;
				case UploadState.Uploading:
					buttonText = "Uploading...";
					enabled = false;
					text = "Uploading…";
					break;
				case UploadState.UploadError:
					buttonText = reuploadButtonText;
					enabled = true;
					text = $"Upload failed: {upload.UploadError ?? "No error"}";
					break;
				case UploadState.ProcessingError:
					buttonText = reuploadButtonText;
					enabled = true;
					text = $"dps.report error: {upload.ProcessingError ?? "No error"}";
					break;
				case UploadState.Uploaded:
					buttonText = reuploadButtonText;
					enabled = true;
					text = "Uploaded";
					break;
				default:
					buttonText = "Upload";
					enabled = true;
					text = upload.UploadState.ToString();
					break;
			}

			UploadButtonText = buttonText;
			CanUpload = enabled && UploadProcessor != null;
			UploadStatus = text;
			UploadUrl = upload.Url;
			HasUploadUrl = !string.IsNullOrEmpty(upload.Url);
			CopyIcon = HasUploadUrl ? images.GetCopyButtonEnabledImage() : images.GetCopyButtonDisabledImage();
			UploadCommand.NotifyCanExecuteChanged();
		}
	}
}
