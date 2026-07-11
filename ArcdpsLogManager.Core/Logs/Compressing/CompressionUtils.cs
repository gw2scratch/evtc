using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Logs.Compressing;

public static class CompressionUtils
{
	private static readonly string[] ZippedExtensions = { ".zevtc", ".zip" };

	/// <summary>
	/// Returns a value indicating whether this log file has an extension indicating that it's a ZIP-compressed log.
	/// </summary>
	/// <remarks>
	/// Note that the log file might not actually be compressed.
	/// </remarks>
	/// <param name="filename">A log filename.</param>
	public static bool HasZipExtension(string filename)
	{
		return ZippedExtensions.Any(filename.EndsWith);
	}
}