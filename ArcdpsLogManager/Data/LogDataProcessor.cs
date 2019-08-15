using System.Threading;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Analytics;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Data
{
	public class LogDataProcessor : BackgroundProcessor<LogData>
	{
		private readonly LogAnalytics analytics;
		private readonly LogCache logCache;
		private readonly ApiProcessor apiProcessor;

		public LogDataProcessor(LogCache logCache, ApiProcessor apiProcessor, LogAnalytics analytics)
		{
			this.analytics = analytics;
			this.logCache = logCache;
			this.apiProcessor = apiProcessor;
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