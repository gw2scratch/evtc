using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Sections.Players
{
	public class PlayerData
	{
		public string AccountName { get; }
		public IReadOnlyList<LogData> Logs { get; }

		public PlayerData(string accountName, IEnumerable<LogData> logs)
		{
			AccountName = accountName;
			Logs = logs.ToArray();
		}
	}
}