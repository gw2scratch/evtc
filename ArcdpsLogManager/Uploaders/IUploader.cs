using System.Threading;
using System.Threading.Tasks;
using ArcdpsLogManager.Logs;

namespace ArcdpsLogManager.Uploaders
{
	public interface IUploader
	{
		/// <summary>
		/// Uploads a log to a service and returns where it can be found
		/// </summary>
		/// <param name="log">The log to be uploaded</param>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> for cancelling the upload.</param>
		/// <returns>An URL of the log</returns>
		Task<string> UploadLogAsync(LogData log, CancellationToken cancellationToken);

		/// <summary>
		/// Uploads a log to a service and returns where it can be found
		/// </summary>
		/// <param name="log">The log to be uploaded</param>
		/// <returns>An URL of the log</returns>
		Task<string> UploadLogAsync(LogData log);
	}
}