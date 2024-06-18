using GW2Scratch.ArcdpsLogManager.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Uploads;

namespace GW2Scratch.ArcdpsLogManager.Processing
{
	public class UploadProcessor : BackgroundProcessor<UploadProcessor.QueuedUpload>
	{
		public abstract class QueuedUpload
		{
			public LogData LogData { get; }

			protected QueuedUpload(LogData logData)
			{
				LogData = logData ?? throw new ArgumentNullException(nameof(logData));
			}

			public abstract Task Upload(UploadProcessor processor, CancellationToken cancellationToken);
		}

		private class DpsReportUpload : QueuedUpload
		{
			public DpsReportUpload(LogData logData) : base(logData)
			{
			}

			// Not thread-safe due to setting the token if none has been acquired yet.
			public override async Task Upload(UploadProcessor processor, CancellationToken cancellationToken)
			{
				LogData.DpsReportEIUpload.UploadState = UploadState.Uploading;

				try
				{
					string uploadToken = !string.IsNullOrWhiteSpace(processor.DpsReportUserToken)
						? processor.DpsReportUserToken
						: null;
					var response =
						await processor.DpsReportUploader.UploadLogAsync(LogData, cancellationToken, uploadToken);
					if (string.IsNullOrWhiteSpace(processor.DpsReportUserToken))
					{
						processor.SetDpsReportUserToken(response.UserToken);
					}

					if (response.Error == null)
					{
						LogData.DpsReportEIUpload.Url = response.Permalink;
						LogData.DpsReportEIUpload.UploadState = UploadState.Uploaded;
					}
					else
					{
						LogData.DpsReportEIUpload.ProcessingError = response.Error;
						if (response.Permalink != null)
						{
							// With some errors, the upload may still go through.
							LogData.DpsReportEIUpload.Url = response.Permalink;
							LogData.DpsReportEIUpload.UploadState = UploadState.Uploaded;
						}
						else
						{
							LogData.DpsReportEIUpload.UploadState = UploadState.ProcessingError;
						}
					}
				}
				catch (Exception e)
				{
					LogData.DpsReportEIUpload.UploadState = UploadState.UploadError;
					LogData.DpsReportEIUpload.UploadError = e.Message;
				}

				processor.LogCache.CacheLogData(LogData);
			}
		}

		private DpsReportUploader DpsReportUploader { get; }
		private LogCache LogCache { get; }

		private string DpsReportUserToken { get; set; }

		public UploadProcessor(DpsReportUploader dpsReportUploader, LogCache logCache)
		{
			DpsReportUploader = dpsReportUploader ?? throw new ArgumentNullException(nameof(dpsReportUploader));
			LogCache = logCache ?? throw new ArgumentNullException(nameof(logCache));

			DpsReportUserToken = Settings.DpsReportUserToken;
			Settings.DpsReportUserTokenChanged += (sender, args) => DpsReportUserToken = Settings.DpsReportUserToken;
		}

		public void ScheduleDpsReportEIUpload(LogData logData)
		{
			var upload = new DpsReportUpload(logData);

			logData.DpsReportEIUpload.UploadState = UploadState.Queued;
			Schedule(upload);
		}

		public void CancelDpsReportEIUpload(IEnumerable<LogData> logData)
		{
			var logSet = logData.ToHashSet();
			Unschedule(x => x is DpsReportUpload upload && logSet.Contains(upload.LogData),
				x => x.LogData.DpsReportEIUpload.UploadState = UploadState.NotUploaded);
		}

		protected override Task Process(QueuedUpload item, CancellationToken cancellationToken)
		{
			return item.Upload(this, cancellationToken);
		}

		private void SetDpsReportUserToken(string newToken)
		{
			Settings.DpsReportUserToken = newToken;
		}
	}
}