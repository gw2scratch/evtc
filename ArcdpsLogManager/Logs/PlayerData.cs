using System.Collections.Generic;
using System.Linq;

namespace ArcdpsLogManager.Logs
{
	public class PlayerData
	{
        public string AccountName { get; }
        public IReadOnlyCollection<LogData> Logs { get; }

        public PlayerData(string accountName, IEnumerable<LogData> logs)
        {
	        AccountName = accountName;
	        Logs = logs.ToArray();
        }
	}
}