namespace ArcdpsLogManager.Logs
{
	public class LogUpload
	{
		public string Url { get; set; } = null;
		public UploadState UploadState { get; set; } = UploadState.NotUploaded;
	}
}