using System;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Processing;

namespace GW2Scratch.ArcdpsLogManager.Analytics
{
	public class LogAnalytics
	{
		public EVTCParser Parser { get; }
		public LogProcessor Processor { get; }
		public Func<Log, LogAnalyzer> AnalyzerFactory { get; }

		public LogAnalytics(EVTCParser parser, LogProcessor processor, Func<Log, LogAnalyzer> analyzerFactory)
		{
			Parser = parser ?? throw new ArgumentNullException(nameof(parser));
			Processor = processor ?? throw new ArgumentNullException(nameof(processor));
			AnalyzerFactory = analyzerFactory ?? throw new ArgumentNullException(nameof(analyzerFactory));
		}
	}
}