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
		private Func<Log, LogAnalyzer> AnalyzerFactory { get; }
		public FractalInstabilityDetector FractalInstabilityDetector { get; }

		public LogAnalytics(EVTCParser parser, LogProcessor processor, FractalInstabilityDetector fractalInstabilityDetector, Func<Log, LogAnalyzer> analyzerFactory)
		{
			Parser = parser ?? throw new ArgumentNullException(nameof(parser));
			Processor = processor ?? throw new ArgumentNullException(nameof(processor));
			FractalInstabilityDetector = fractalInstabilityDetector ?? throw new ArgumentNullException(nameof(fractalInstabilityDetector));
			AnalyzerFactory = analyzerFactory ?? throw new ArgumentNullException(nameof(analyzerFactory));
		}

		public LogAnalyzer CreateAnalyzer(Log log)
		{
			return AnalyzerFactory(log);
		}
	}
}