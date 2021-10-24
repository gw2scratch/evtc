using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Processing;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Benchmark
{
	[SimpleJob(RuntimeMoniker.NetCoreApp31)]
	[SimpleJob(RuntimeMoniker.Net50)]
	[SimpleJob(RuntimeMoniker.Net60)]
	public class ProcessingBenchmark
	{
		private List<byte[]> logs;
		
		[GlobalSetup]
		public void LoadLogs()
		{
			logs = Logs.LoadEmbeddedLogs();
		}

		[GlobalCleanup]
		public void RemoveLogs()
		{
			logs.Clear();
		}

		[Benchmark]
		public void DirectProcess()
		{
			var parser = new EVTCParser();
			var processor = new LogProcessor();
			foreach (var log in logs)
			{
				var processed = processor.ProcessLog(log, parser);
			}
		}
		
		[Benchmark]
		public void ParseAndProcess()
		{
			var parser = new EVTCParser();
			var processor = new LogProcessor();
			foreach (var log in logs)
			{
				var parsed = parser.ParseLog(log);
				var processed = processor.ProcessLog(parsed);
			}
		}
	}

	[SimpleJob(RuntimeMoniker.NetCoreApp31)]
	[SimpleJob(RuntimeMoniker.Net50)]
	[SimpleJob(RuntimeMoniker.Net60)]
	public class AnalyzerBenchmarks
	{
		private List<Log> logs;
		
		[GlobalSetup]
		public void LoadLogs()
		{
			logs = new List<Log>();
			var logBytes = Logs.LoadEmbeddedLogs();
			var parser = new EVTCParser();
			var processor = new LogProcessor();
			foreach (var log in logBytes)
			{
				logs.Add(processor.ProcessLog(log, parser));
			}
		}

		[GlobalCleanup]
		public void RemoveLogs()
		{
			logs.Clear();
		}
		

		[Benchmark]
		public void Analyze()
		{
			foreach (var log in logs)
			{
				var analyzer = new LogAnalyzer(log);
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
			BenchmarkRunner.Run<ProcessingBenchmark>();
			//BenchmarkRunner.Run<AnalyzerBenchmarks>();
		}
	}
}