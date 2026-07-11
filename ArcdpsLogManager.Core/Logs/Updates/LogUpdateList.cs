using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.ArcdpsLogManager.Logs.Updates
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