using GW2Scratch.ArcdpsLogManager.Configuration;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters
{
	public class SettingsFilters : ILogFilter
	{
		public bool FilterLog(LogData log)
		{
			if (!Settings.MinimumLogDurationSeconds.HasValue)
			{
				return true;
			}

			return log.EncounterDuration.TotalSeconds > Settings.MinimumLogDurationSeconds;
		}
	}
}