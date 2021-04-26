using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Logs.Tagging;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Sections;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public sealed class LogDetailPanel : DynamicLayout
	{
		private UploadProcessor UploadProcessor { get; }
		private ImageProvider ImageProvider { get; }
		private ILogNameProvider NameProvider { get; }

		private LogData logData;

		private readonly Label nameLabel = new Label {Font = Fonts.Sans(16, FontStyle.Bold), Wrap = WrapMode.Word};
		private readonly Label resultLabel = new Label {Font = Fonts.Sans(12)};
		private readonly LinkButton fileNameButton = new LinkButton();
		private readonly GroupCompositionControl groupComposition;
		private readonly Label parseTimeLabel = new Label();
		private readonly Label parseStatusLabel = new Label();
		private readonly Button dpsReportUploadButton;
		private readonly TextBox dpsReportTextBox;
		private readonly Button dpsReportOpenButton;
		private readonly TagControl tagControl;
		private readonly DynamicTable groupCompositionSection;
		private readonly DynamicTable failedProcessingSection;
		private readonly TextArea exceptionTextArea = new TextArea {ReadOnly = true};
		private readonly MistlockInstabilityList instabilityList;
		private readonly Button deleteButton = new Button() { Text = "Delete Log" };

		public LogData LogData
		{
			get => logData;
			set
			{
				SuspendLayout();
				logData = value;

				if (logData == null)
				{
					Visible = false;
					return;
				}

				Visible = true;

				nameLabel.Text = NameProvider.GetName(logData);

				string result;
				switch (logData.EncounterResult)
				{
					case EncounterResult.Success:
						result = "Success";
						break;
					case EncounterResult.Failure:
						result = logData.HealthPercentage.HasValue
							? $"Failure ({logData.HealthPercentage * 100:0.00}% health)"
							: "Failure";
						break;
					case EncounterResult.Unknown:
						result = "Unknown";
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				double seconds = logData.EncounterDuration.TotalSeconds;
				string duration = $"{(int) seconds / 60:0}m {seconds % 60:0.0}s";

				fileNameButton.Text = System.IO.Path.GetFileName(logData.FileName);

				resultLabel.Text = $"{result} in {duration}";

				parseTimeLabel.Text = $"{logData.ParseMilliseconds} ms";
				parseStatusLabel.Text = logData.ParsingStatus.ToString();

				instabilityList.MistlockInstabilities = logData.LogExtras?.FractalExtras?.MistlockInstabilities;

				groupComposition.Players = logData.Players;

				UpdateUploadStatus();

				tagControl.Tags = logData.Tags;

				if (logData.ParsingStatus == ParsingStatus.Failed)
				{
					if (logData.ParsingException != null)
					{
						string exceptionText = $"{logData.ParsingException.Message}\n\n\n" +
						                       $"{logData.ParsingException.ExceptionName}: {logData.ParsingException.Message}\n" +
						                       $"{logData.ParsingException.StackTrace}";
						exceptionTextArea.Text = exceptionText;
					}

					if (logData.Players == null)
					{
						groupCompositionSection.Visible = false;
					}

					failedProcessingSection.Visible = true;
				}
				else
				{
					groupCompositionSection.Visible = true;
					failedProcessingSection.Visible = false;
				}

				ResumeLayout();
			}
		}

		public LogDetailPanel(LogCache logCache, ApiData apiData, LogDataProcessor logProcessor, UploadProcessor uploadProcessor,
			ImageProvider imageProvider, ILogNameProvider nameProvider)
		{
			UploadProcessor = uploadProcessor;
			ImageProvider = imageProvider;
			NameProvider = nameProvider;

			Padding = new Padding(10, 10, 10, 2);
			Width = 350;
			Visible = false;

			instabilityList = new MistlockInstabilityList(imageProvider);
			groupComposition = new GroupCompositionControl(apiData, imageProvider);
			tagControl = new TagControl();

			DynamicGroup debugSection;
			var debugButton = new Button {Text = "Debug data"};
			var reparseButton = new Button {Text = "Reprocess"};

			deleteButton.Click += (sender, args) => new DeleteFilesForm(new LogData[] { LogData }, nameProvider, imageProvider);

			tagControl.TagAdded += (sender, args) =>
			{
				if (logData.Tags.Add(new TagInfo(args.Name)))
				{
					logCache.CacheLogData(logData);
				}
			};

			tagControl.TagRemoved += (sender, args) =>
			{
				if (logData.Tags.Remove(new TagInfo(args.Name)))
				{
					logCache.CacheLogData(logData);
				}
			};

			BeginVertical(spacing: new Size(0, 15), yscale: true);
			{
				BeginVertical(spacing: new Size(0, 8));
				{
					BeginVertical();
					{
						Add(nameLabel);
						Add(resultLabel);
					}
					EndVertical();
					BeginVertical();
					{
						Add(instabilityList);
					}
					EndVertical();
				}
				EndVertical();
				BeginVertical(spacing: new Size(0, 5));
				{
					groupCompositionSection = BeginVertical(yscale: true);
					{
						AddRow(new Scrollable {Content = groupComposition, Border = BorderType.None});
					}
					EndVertical();

					// The WPF platform also considers invisible sections in layout when their yscale is set to true.
					// To work around this, we disable it for WPF. As the same issue also affects the group composition
					// section above, the layout does not fully collapse, and the failed processing section appears in
					// the lower half of the panel, which is good enough and doesn't even look that much out of place.
					bool yscaleFailedSection = !Application.Instance.Platform.IsWpf;

					failedProcessingSection = BeginVertical(spacing: new Size(10, 10), yscale: yscaleFailedSection);
					{
						AddRow("Processing of this log failed. This may be a malformed log, " +
						       "often caused by versions of arcdps incompatible with a specific Guild Wars 2 release.");
						AddRow("Reason for failed processing:");
						AddRow(exceptionTextArea);
					}
					EndVertical();

					debugSection = BeginGroup("Debug data", new Padding(5));
					{
						BeginHorizontal();
						{
							BeginVertical(xscale: true, spacing: new Size(5, 5));
							{
								AddRow("Time spent processing", parseTimeLabel);
								AddRow("Processing status", parseStatusLabel);
							}
							EndVertical();
							BeginVertical(spacing: new Size(5, 5));
							{
								AddRow(debugButton);
								AddRow(reparseButton);
							}
							EndVertical();
						}
						EndHorizontal();
					}
					EndGroup();
					BeginHorizontal();
					{
						Add(new Scrollable {Content = tagControl, Border = BorderType.None});
					}
					EndHorizontal();

					dpsReportUploadButton = new Button();
					dpsReportTextBox = new TextBox {ReadOnly = true};
					dpsReportOpenButton = new Button {Text = "Open"};

					BeginGroup("Upload to dps.report (Elite Insights)", new Padding(5), new Size(0, 5));
					{
						BeginVertical(spacing: new Size(5, 5));
						{
							BeginHorizontal();
							{
								Add(dpsReportTextBox, true);
								Add(dpsReportOpenButton);
								Add(dpsReportUploadButton);
							}
							EndHorizontal();
						}
						EndVertical();
					}
					EndGroup();
				}
				EndVertical();
			}
			EndVertical();
			BeginGroup("File Management", new Padding(5), new Size(0, 5));
			{
				BeginVertical(spacing: new Size(10, 0));
				{
					Add(null, true);
					BeginHorizontal();
					{
						Add(deleteButton);
						Add(fileNameButton);
					}
					EndHorizontal();
				}
				EndVertical();
			}
			EndGroup();

			dpsReportUploadButton.Click += (sender, args) => { UploadProcessor.ScheduleDpsReportEIUpload(logData); };
			dpsReportOpenButton.Click += (sender, args) =>
			{
				var processInfo = new ProcessStartInfo()
				{
					FileName = logData.DpsReportEIUpload.Url,
					UseShellExecute = true
				};
				Process.Start(processInfo);
			};

			debugButton.Click += (sender, args) =>
			{
				var debugData = new DebugData {LogData = LogData};
				var dialog = new Form {Content = debugData, Width = 500, Title = "Debug data"};
				dialog.Show();
			};

			reparseButton.Click += (sender, args) => logProcessor.Schedule(logData);

			fileNameButton.Click += (sender, args) =>
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					Process.Start("explorer.exe", $"/select,\"{logData.FileName}\"");
				}
				else
				{
					var processInfo = new ProcessStartInfo()
					{
						FileName = System.IO.Path.GetDirectoryName(logData.FileName),
						UseShellExecute = true
					};
					Process.Start(processInfo);
				}
			};

			Settings.ShowDebugDataChanged += (sender, args) =>
			{
				debugSection.Visible = Settings.ShowDebugData;
				debugSection.GroupBox.Visible = Settings.ShowDebugData;
			};
			Shown += (sender, args) =>
			{
				// Assigning visibility in the constructor does not work
				debugSection.Visible = Settings.ShowDebugData;
				debugSection.GroupBox.Visible = Settings.ShowDebugData;
			};

			uploadProcessor.Processed += OnUploadProcessorUpdate;
			uploadProcessor.Unscheduled += OnUploadProcessorUpdate;
			uploadProcessor.Scheduled += OnUploadProcessorUpdate;
		}

		private void UpdateUploadStatus()
		{
			if (logData == null)
			{
				return;
			}

			const string reuploadButtonText = "Reupload";

			bool uploadEnabled = false;
			bool openEnabled = false;
			string text = "";
			string uploadButtonText;
			var upload = logData.DpsReportEIUpload;
			switch (upload.UploadState)
			{
				case UploadState.NotUploaded:
					uploadButtonText = "Upload";
					uploadEnabled = true;
					break;
				case UploadState.Queued:
				case UploadState.Uploading:
					uploadButtonText = "Uploading...";
					break;
				case UploadState.UploadError:
					uploadButtonText = reuploadButtonText;
					uploadEnabled = true;
					text = $"Upload failed: {upload.UploadError ?? "No error"}";
					break;
				case UploadState.ProcessingError:
					uploadButtonText = reuploadButtonText;
					uploadEnabled = true;
					text = $"dps.report error: {upload.ProcessingError ?? "No error"}";
					break;
				case UploadState.Uploaded:
					uploadButtonText = reuploadButtonText;
					uploadEnabled = true;
					openEnabled = true;
					text = upload.Url;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			dpsReportUploadButton.Text = uploadButtonText;
			dpsReportUploadButton.Enabled = uploadEnabled;
			dpsReportOpenButton.Enabled = openEnabled;
			dpsReportTextBox.Text = text;
			dpsReportTextBox.Enabled = text != "";
		}

		private void OnUploadProcessorUpdate(object sender, EventArgs e)
		{
			Application.Instance.Invoke(UpdateUploadStatus);
		}
	}
}