using System;
using GW2Scratch.EVTCAnalytics;

namespace GW2Scratch.ArcdpsLogManager.Analytics
{
	public class LogAnalytics
	{
		public EVTCParser Parser { get; }
		public LogProcessor Processor { get; }
		public LogAnalyser Analyser { get; }

		public LogAnalytics(EVTCParser parser, LogProcessor processor, LogAnalyser analyser)
		{
			Parser = parser ?? throw new ArgumentNullException(nameof(parser));
			Processor = processor ?? throw new ArgumentNullException(nameof(processor));
			Analyser = analyser ?? throw new ArgumentNullException(nameof(analyser));
		}
	}
}