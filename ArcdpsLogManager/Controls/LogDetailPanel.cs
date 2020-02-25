using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
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
						result = "Failure";
						break;
					case EncounterResult.Unknown:
						result = "Unknown";
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				double seconds = logData.EncounterDuration.TotalSeconds;
				string duration = $"{(int) seconds / 60:0}m {seconds % 60:0.0}s";

				fileNameButton.Text = logData.FileInfo.Name;

				resultLabel.Text = $"{result} in {duration}";

				parseTimeLabel.Text = $"{logData.ParseMilliseconds} ms";
				parseStatusLabel.Text = logData.ParsingStatus.ToString();

				groupComposition.Players = logData.Players;

				UpdateUploadStatus();

				ResumeLayout();
			}
		}

		public LogDetailPanel(ApiData apiData, LogDataProcessor logProcessor, UploadProcessor uploadProcessor,
			ImageProvider imageProvider, ILogNameProvider nameProvider)
		{
			UploadProcessor = uploadProcessor;
			ImageProvider = imageProvider;
			NameProvider = nameProvider;

			Padding = new Padding(10, 10, 10, 2);
			Width = 350;
			Visible = false;

			groupComposition = new GroupCompositionControl(apiData, imageProvider);

			DynamicTable debugSection;
			var debugButton = new Button {Text = "Debug data"};
			var reparseButton = new Button {Text = "Reparse"};

			BeginVertical(spacing: new Size(0, 30), yscale: true);
			{
				BeginVertical();
				{
					Add(nameLabel);
					Add(resultLabel);
				}
				EndVertical();
				BeginVertical();
				{
					BeginHorizontal(true);
					{
						Add(new Scrollable {Content = groupComposition, Border = BorderType.None});
					}
					EndHorizontal();

					debugSection = BeginVertical();
					{
						BeginHorizontal();
						{
							BeginVertical(xscale: true, spacing: new Size(5, 0));
							{
								AddRow("Time spent parsing", parseTimeLabel);
								AddRow("Parsing status", parseStatusLabel);
							}
							EndVertical();
							BeginVertical();
							{
								AddRow(debugButton);
								AddRow(reparseButton);
							}
							EndVertical();
						}
						EndHorizontal();
					}
					EndVertical();

					dpsReportUploadButton = new Button();
					dpsReportTextBox = new TextBox {ReadOnly = true};
					dpsReportOpenButton = new Button {Text = "Open"};

					BeginVertical(spacing: new Size(0, 5));
					{
						BeginVertical(spacing: new Size(5, 5));
						{
							BeginHorizontal();
							{
								Add(dpsReportUploadButton);
							}
							EndHorizontal();
						}
						EndVertical();
						BeginVertical(spacing: new Size(5, 5));
						{
							BeginHorizontal();
							{
								Add(dpsReportTextBox, true);
								Add(dpsReportOpenButton);
							}
							EndHorizontal();
						}
						EndVertical();
					}
					EndVertical();
				}
				EndVertical();
			}
			EndVertical();
			BeginVertical(spacing: new Size(10, 0));
			{
				Add(null, true);
				BeginHorizontal();
				{
					Add(null, true);
					Add(fileNameButton);
				}
			}
			EndVertical();

			dpsReportUploadButton.Click += (sender, args) => { UploadProcessor.ScheduleDpsReportEIUpload(logData); };
			dpsReportOpenButton.Click += (sender, args) =>
			{
				Process.Start(logData.DpsReportEIUpload.Url);
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
					Process.Start(logData.FileInfo.DirectoryName);
				}
			};

			Settings.ShowDebugDataChanged += (sender, args) => { debugSection.Visible = Settings.ShowDebugData; };
			Shown += (sender, args) =>
			{
				// Assigning visibility in the constructor does not work
				debugSection.Visible = Settings.ShowDebugData;
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

			const string reuploadButtonText = "Reupload to dps.report (EI)";

			bool uploadEnabled = false;
			bool openEnabled = false;
			string text = "";
			string uploadButtonText;
			var upload = logData.DpsReportEIUpload;
			switch (upload.UploadState)
			{
				case UploadState.NotUploaded:
					uploadButtonText = "Upload to dps.report (EI)";
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
					text = $"Processing failed: {upload.ProcessingError ?? "No error"}";
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
		}

		private void OnUploadProcessorUpdate(object sender, EventArgs e)
		{
			Application.Instance.Invoke(UpdateUploadStatus);
		}
	}
}