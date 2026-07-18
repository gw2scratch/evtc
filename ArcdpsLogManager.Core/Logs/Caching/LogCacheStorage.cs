using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Logs.Caching
{
	public class LogCacheStorage
	{
		public int Version { get; set; } = 2;
		public Dictionary<string, LogData> LogsByFilename { get; set; }
	}
}