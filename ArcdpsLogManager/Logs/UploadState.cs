namespace GW2Scratch.ArcdpsLogManager.Logs
{
	public enum UploadState
	{
		/// <summary>
		/// Not uploaded, not queued for one.
		/// </summary>
		NotUploaded,
		/// <summary>
		/// Currently queued for upload.
		/// </summary>
		Queued,
		/// <summary>
		/// Currently uploading.
		/// </summary>
		Uploading,
		/// <summary>
		/// Uploaded successfully.
		/// </summary>
		Uploaded,
		/// <summary>
		/// Upload failed (network issues or host was not reachable).
		/// </summary>
		UploadError,
		/// <summary>
		/// Processing of the file failed on the server after a successful upload.
		/// </summary>
		ProcessingError,
	}
}