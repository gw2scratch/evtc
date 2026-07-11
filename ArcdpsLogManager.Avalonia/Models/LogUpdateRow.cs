using GW2Scratch.ArcdpsLogManager.Logs.Updates;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// Flattened display projection of a <see cref="LogUpdateList"/> for the processing-update
	/// grid (log count + reason).
	/// </summary>
	public class LogUpdateRow
	{
		public int LogCount { get; }
		public string Reason { get; }

		public LogUpdateRow(LogUpdateList update)
		{
			LogCount = update.UpdateableLogs.Count;
			Reason = update.Update.Reason;
		}
	}
}
