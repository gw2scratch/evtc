using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Updates
{
	public class LogUpdateList
	{
		public LogUpdate Update { get; }
		public IReadOnlyList<LogData> UpdateableLogs { get; }

		public LogUpdateList(LogUpdate update, IEnumerable<LogData> updateableLogs)
		{
			Update = update;
			UpdateableLogs = updateableLogs.ToList();
		}
	}
}