namespace GW2Scratch.ArcdpsLogManager.Logs.Filters
{
	public interface ILogFilter
	{
		/// <summary>
		/// Determine whether a log should be kept or filtered out.
		/// </summary>
		/// <returns>A value indicating whether a log is kept (<see langword="true"/>) or filtered out (<see langword="false"/>).</returns>
		bool FilterLog(LogData log);
	}
}