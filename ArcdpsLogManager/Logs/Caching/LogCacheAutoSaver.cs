using System;
using System.Threading;

namespace GW2Scratch.ArcdpsLogManager.Logs.Caching
{
	public class LogCacheAutoSaver
	{
		private readonly LogCache logCache;
		private Timer timer;

		private LogCacheAutoSaver(LogCache logCache)
		{
			this.logCache = logCache;
		}

		public static LogCacheAutoSaver Started(LogCache cache, TimeSpan savePeriod)
		{
			var saver = new LogCacheAutoSaver(cache);
			var timer = new Timer(saver.Callback);
			timer.Change(savePeriod, savePeriod);
			saver.timer = timer;

			return saver;
		}

		private void Callback(object state)
		{
			logCache.SaveToFile();
		}
	}
}