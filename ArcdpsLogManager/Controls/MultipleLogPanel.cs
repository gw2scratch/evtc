using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcdpsLogManager.Logs;
using ArcdpsLogManager.Uploaders;
using Eto.Drawing;
using Eto.Forms;

namespace ArcdpsLogManager.Controls
{
	public class MultipleLogPanel : DynamicLayout
	{
		public DpsReportUploader DpsReportUploader { get; } = new DpsReportUploader();

		private LogData[] logData;

		private readonly Label countLabel = new Label {Font = Fonts.Sans(12)};
		private readonly Label dpsReportNotUploadedLabel = new Label();
		private readonly Label dpsReportUploadingLabel = new Label();
		private readonly Label dpsReportUploadedLabel = new Label();
		private readonly TextArea dpsReportLinkTextArea = new TextArea {ReadOnly = true};
		private readonly Button dpsReportUploadButton = new Button();
		private readonly Button dpsReportCancelButton = new Button {Text = "Cancel"};
		private readonly ProgressBar dpsReportUploadProgressBar = new ProgressBar();

		private Task currentUploadTask = null;
		private CancellationTokenSource uploadTaskCancellationTokenSource = null;

		public IEnumerable<LogData> LogData
		{
			get => logData;
			set
			{
				SuspendLayout();
				logData = value?.ToArray();

				if (logData == null || logData.Length < 2)
				{
					Visible = false;
					return;
				}

				Visible = true;

				countLabel.Text = $"{logData.Length} logs selected";

				UpdateDpsReportUploadStatus();

				ResumeLayout();
			}
		}

		private void UpdateDpsReportUploadStatus()
		{
			int notUploadedCount = logData.Count(x => x.DpsReportEIUpload.UploadState == UploadState.NotUploaded);
			int uploadingCount = logData.Count(x => x.DpsReportEIUpload.UploadState == UploadState.Uploading);
			int uploadedCount = logData.Count(x => x.DpsReportEIUpload.UploadState == UploadState.Uploaded);

			bool currentTaskCompleted = currentUploadTask == null || currentUploadTask.IsCompleted;
			bool cancelRequested = uploadTaskCancellationTokenSource?.IsCancellationRequested ?? false;

			dpsReportUploadButton.Enabled = currentTaskCompleted;
			dpsReportCancelButton.Enabled = !currentTaskCompleted && !cancelRequested;
			dpsReportUploadButton.Text = $"Upload selected logs ({notUploadedCount})";
			dpsReportNotUploadedLabel.Text = notUploadedCount.ToString();
			dpsReportUploadingLabel.Text = uploadingCount.ToString();
			dpsReportUploadedLabel.Text = uploadedCount.ToString();
			dpsReportLinkTextArea.Text = string.Join(Environment.NewLine,
				logData.Where(x => x.DpsReportEIUpload.Url != null).Select(x => x.DpsReportEIUpload.Url));
		}

		public MultipleLogPanel()
		{
			Padding = new Padding(10);
			Width = 300;
			Visible = false;

			dpsReportUploadButton.Click += (sender, args) =>
			{
				Task.Run(
					() => DpsReportUploadFiles(
						logData.Where(x => x.DpsReportEIUpload.UploadState == UploadState.NotUploaded)
					)
				);
			};

			dpsReportCancelButton.Click += (sender, args) => uploadTaskCancellationTokenSource?.Cancel();

			BeginVertical(spacing: new Size(0, 30));
			{
				BeginVertical();
				{
					//Add(new Label {Text = "Batch log management", Font = Fonts.Sans(16, FontStyle.Bold)});
					Add(countLabel);
				}
				EndVertical();

				BeginVertical(spacing: new Size(0, 5));
				{
					BeginVertical(spacing: new Size(5, 5));
					{
						AddRow(new Label {Text = "Upload to dps.report (EI)", Font = Fonts.Sans(12)});
						AddRow("Not uploaded", dpsReportNotUploadedLabel);
						AddRow("Uploading", dpsReportUploadingLabel);
						AddRow("Uploaded", dpsReportUploadedLabel);
					}
					EndVertical();
					BeginVertical();
					{
						BeginHorizontal();
						{
							Add(dpsReportUploadButton, true);
							Add(dpsReportCancelButton);
						}
						EndHorizontal();
					}
					EndVertical();
					BeginVertical();
					{
						AddRow(dpsReportUploadProgressBar);
						AddRow(dpsReportLinkTextArea);
					}
					EndVertical();
				}
				EndVertical();
			}
			EndVertical();
		}

		private void DpsReportUploadFiles(IEnumerable<LogData> logsToUpload)
		{
			Debug.Assert(currentUploadTask == null || currentUploadTask.IsCompleted);

			uploadTaskCancellationTokenSource = new CancellationTokenSource();
			currentUploadTask = DpsReportUploadFilesAsync(
				logsToUpload,
				uploadTaskCancellationTokenSource.Token,
				new Progress<(int done, int logCount)>(progress =>
				{
					Application.Instance.AsyncInvoke(() =>
					{
						UpdateDpsReportUploadStatus();

						(int done, int logCount) = progress;
						dpsReportUploadProgressBar.MaxValue = logCount;
						dpsReportUploadProgressBar.Value = done;
					});
				})
			);
			currentUploadTask.ContinueWith(task => { Application.Instance.AsyncInvoke(UpdateDpsReportUploadStatus); });

			Application.Instance.AsyncInvoke(UpdateDpsReportUploadStatus);
		}

		private async Task DpsReportUploadFilesAsync(IEnumerable<LogData> logsToUpload,
			CancellationToken cancellationToken, IProgress<(int done, int logCount)> progress = null)
		{
			var logs = logsToUpload.ToArray();
			progress?.Report((0, logs.Length));

			for (var i = 0; i < logs.Length; i++)
			{
				cancellationToken.ThrowIfCancellationRequested();

				await logs[i].UploadDpsReportEliteInsights(DpsReportUploader, cancellationToken);
				progress?.Report((i + 1, logs.Length));
			}
		}
	}
}