using System;
using System.Threading;
using System.Threading.Tasks;
using ArcdpsLogManager.Uploaders;
using Newtonsoft.Json;

namespace ArcdpsLogManager.Logs
{
	public class LogUpload
	{
		private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

		private string url;

		public string Url
		{
			get => url;
			set
			{
				url = value;
				UploadState = url == null ? UploadState.NotUploaded : UploadState.Uploaded;
			}
		}

		public DateTimeOffset? UploadTime { get; set; }

		[JsonIgnore]
		public UploadState UploadState { get; set; } = UploadState.NotUploaded;

		public async Task Upload(LogData logData, IUploader uploader, CancellationToken cancellationToken,
			bool reupload = false)
		{
			var stateAtStart = UploadState;

			await semaphore.WaitAsync(cancellationToken);
			try
			{
				// If this log was being uploaded as this method was called, it is not needed to upload it again.
				// This doesn't detect it in all cases, it is possible to start uploading between the variable
				// is saved and the semaphore wait. This will be enough to prevent human users from uploading
				// this log multiple times at once, however be aware it is not perfect.
				if (stateAtStart != UploadState.Uploading && (UploadState == UploadState.NotUploaded || reupload))
				{
					var previousState = UploadState;
					UploadState = UploadState.Uploading;
					string logUrl;
					try
					{
						logUrl = await uploader.UploadLogAsync(logData, cancellationToken);
					}
					catch (TaskCanceledException)
					{
						UploadState = previousState;
						throw;
					}

					Url = logUrl;
					UploadState = UploadState.Uploaded;
					UploadTime = DateTimeOffset.Now;
				}
			}
			finally
			{
				semaphore.Release();
			}
		}
	}
}