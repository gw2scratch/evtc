using System;
using System.Threading;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Analytics;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Processing
{
	public class LogDataProcessor : BackgroundProcessor<LogData>
	{
		private readonly LogAnalytics analytics;
		private readonly LogCache logCache;
		private readonly ApiProcessor apiProcessor;

		public LogDataProcessor(LogCache logCache, ApiProcessor apiProcessor, LogAnalytics analytics)
		{
			this.analytics = analytics ?? throw new ArgumentNullException(nameof(analytics));
			this.logCache = logCache ?? throw new ArgumentNullException(nameof(logCache));
			this.apiProcessor = apiProcessor ?? throw new ArgumentNullException(nameof(apiProcessor));
		}

		// not really sure on the best way to expose this to the Panels, since they now need to be able to
		// save their data after modifying tags.
		public void CacheData(LogData data)
		{
			logCache.CacheLogData(data);
		}

		protected override Task Process(LogData item, CancellationToken cancellationToken)
		{
			return Task.Run(() =>
			{
				item.ParseData(analytics);
				logCache.CacheLogData(item);
				if (item.ParsingStatus == ParsingStatus.Parsed)
				{
					apiProcessor.RegisterLog(item);
				}
			}, cancellationToken);
		}
	}
}