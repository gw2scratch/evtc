using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Processing;
using GW2Scratch.EVTCAnalytics.Statistics;

namespace GW2Scratch.EVTCAnalytics.Benchmark
{
	internal class Program
	{
		public static EVTCParser Parser { get; set; } = new EVTCParser();
		public static LogProcessor Processor { get; set; } = new LogProcessor();
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

			Console.WriteLine("Filename,Parsing,Processing,Statistics,Total");
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

				MeasureTimes(filename, Parser, Processor, ApiData, Console.Out);
			}
		}

		private static void MeasureTimes(string filename, EVTCParser parser, LogProcessor processor, GW2ApiData apiData, TextWriter outputWriter)
		{
			var stopwatch = Stopwatch.StartNew();
			var log = parser.ParseLog(filename);

			var parsedTime = stopwatch.Elapsed;

			stopwatch.Restart();
			var processedLog = processor.ProcessLog(log);

			var processedTime = stopwatch.Elapsed;

			stopwatch.Restart();
			var analyzer = new LogAnalyzer(processedLog, apiData);
			var result = analyzer.GetResult();
			var duration = analyzer.GetEncounterDuration();
			var mode = analyzer.GetMode();

			var statisticsTime = stopwatch.Elapsed;

			var totalTime = parsedTime + processedTime + statisticsTime;
			outputWriter.WriteLine(
				$"{filename},{parsedTime.TotalMilliseconds},{processedTime.TotalMilliseconds},{statisticsTime.TotalMilliseconds},{totalTime.TotalMilliseconds}");
			outputWriter.Flush();
		}
	}
}