using System;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Logs
{
	public class LogUpload
	{
		private string url;
		private string processingError;

		public string Url
		{
			get => url;
			set
			{
				url = value;
				if (UploadState != UploadState.ProcessingError)
				{
					UploadState = url == null ? UploadState.NotUploaded : UploadState.Uploaded;
				}
			}
		}

		public string ProcessingError
		{
			get => processingError;
			set
			{
				processingError = value;
				if (processingError != null)
				{
					UploadState = UploadState.ProcessingError;
				}
			}
		}

		// Upload errors are not stored
		[JsonIgnore]
		public string UploadError { get; set; }

		public DateTimeOffset? UploadTime { get; set; }

		[JsonIgnore]
		public UploadState UploadState { get; set; } = UploadState.NotUploaded;
	}
}