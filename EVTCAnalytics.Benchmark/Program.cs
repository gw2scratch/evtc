using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Parsed;

namespace GW2Scratch.EVTCAnalytics.Benchmark
{
	internal class Program
	{
		public static EVTCParser Parser { get; set; } = new EVTCParser();
		public static LogProcessor Processor { get; set; } = new LogProcessor();
		public static StatisticsCalculator StatisticsCalculator { get; set; } = new StatisticsCalculator();
		public static GW2ApiData ApiData { get; set; } = null;

		public static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.Error.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} [directory with html files]");
				return;
			}

			var directory = args[0];
			if (!Directory.Exists(directory))
			{
				Console.Error.WriteLine("Directory doesn't exist.");
				return;
			}

			var firstFilename = Directory.EnumerateFiles(directory)
				.FirstOrDefault(x => x.EndsWith(".evtc") || x.EndsWith(".evtc.zip") || x.EndsWith(".zevtc"));
			if (firstFilename == null)
			{
				Console.Error.WriteLine("No logs found.");
				return;
			}

			foreach (string filename in Directory.EnumerateFiles(directory))
			{
				if (!filename.EndsWith(".evtc", StringComparison.InvariantCultureIgnoreCase) &&
				    !filename.EndsWith(".evtc.zip", StringComparison.InvariantCultureIgnoreCase) &&
				    !filename.EndsWith(".zevtc", StringComparison.InvariantCultureIgnoreCase))
				{
					Console.Error.WriteLine($"Ignoring file: {filename}");
					continue;
				}

				GC.Collect();

				MeasureTimes(filename, Parser, Processor, StatisticsCalculator, ApiData, Console.Out);
			}
		}

		private static void MeasureTimes(string filename, EVTCParser parser, LogProcessor processor, StatisticsCalculator calculator, GW2ApiData apiData, TextWriter outputWriter)
		{
			var stopwatch = Stopwatch.StartNew();
			var log = parser.ParseLog(filename);

			var parsedTime = stopwatch.Elapsed;

			stopwatch.Restart();
			var processedLog = processor.GetProcessedLog(log);

			var processedTime = stopwatch.Elapsed;

			stopwatch.Restart();
			var statistics = calculator.GetStatistics(processedLog, apiData);

			var statisticsTime = stopwatch.Elapsed;

			var totalTime = parsedTime + processedTime + statisticsTime;
			outputWriter.WriteLine(
				$"{filename},{parsedTime.TotalMilliseconds},{processedTime.TotalMilliseconds},{statisticsTime.TotalMilliseconds},{totalTime.TotalMilliseconds}");
			outputWriter.Flush();
		}
	}
}