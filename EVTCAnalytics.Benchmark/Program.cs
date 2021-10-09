using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Processing;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;

namespace GW2Scratch.EVTCAnalytics.Benchmark
{
	public class EvtcAnalyticsBenchmark
	{
		private readonly List<byte[]> logs = new List<byte[]>();
		
		[GlobalSetup]
		public void LoadLogs()
		{
			foreach (var resourceName in Assembly.GetExecutingAssembly().GetManifestResourceNames())
			{
				if (resourceName.EndsWith(".zevtc"))
				{
					var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
					using var arch = new ZipArchive(stream, ZipArchiveMode.Read);

					if (arch.Entries.Count == 0)
					{
						throw new Exception("No EVTC file in ZIP archive.");
					}

					using var data = arch.Entries[0].Open();

					var bytes = new byte[arch.Entries[0].Length];
					int read;
					int offset = 0;
					while ((read = data.Read(bytes, offset, bytes.Length - offset)) > 0) {
						offset += read;
					}
					logs.Add(bytes);
				}
			}

			if (logs.Count == 0)
			{
				throw new Exception("No logs found. Make sure that you have a logs directory next to the .csproj file when compiling, and only use .zevtc logs.");
			}
		}

		[GlobalCleanup]
		public void RemoveLogs()
		{
			logs.Clear();
		}

		[Benchmark]
		public void ParseAll()
		{
			var parser = new EVTCParser();
			foreach (var log in logs)
			{
				var parsed = parser.ParseLog(log);
			}
		}
		
		[Benchmark]
		public void ParseAndProcessAll()
		{
			var parser = new EVTCParser();
			var processor = new LogProcessor();
			foreach (var log in logs)
			{
				var parsed = parser.ParseLog(log);
				var processed = processor.ProcessLog(parsed);
			}
		}

		[Benchmark]
		public void ParseAndProcessAndAnalyze()
		{
			var parser = new EVTCParser();
			var processor = new LogProcessor();
			foreach (var log in logs)
			{
				var parsed = parser.ParseLog(log);
				var processed = processor.ProcessLog(parsed);
				var analyzer = new LogAnalyzer(processed);
				analyzer.GetEncounter();
				analyzer.GetMode();
				analyzer.GetResult();
				analyzer.GetEncounterDuration();
				analyzer.GetMainEnemyHealthFraction();
			}
		}
	}

	internal class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkRunner.Run<EvtcAnalyticsBenchmark>();
		}
	}
}