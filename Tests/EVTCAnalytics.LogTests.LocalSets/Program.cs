using GW2Scratch.EVTCAnalytics.LogTests.LocalSets.Extraction;
using System;
using System.Collections.Generic;
using System.IO;
using Tomlyn;

namespace GW2Scratch.EVTCAnalytics.LogTests.LocalSets
{
	internal class Program
	{
		public static int Main(string[] args)
		{
			List<string> configFilenames = new List<string>();
			for (int i = 0; i < args.Length - 1; i++)
			{
				if (args[i] == "--local")
				{
					configFilenames.Add(args[i + 1]);
					i++;
				}
			}

			var logs = new List<LogDefinition>();
			foreach (string filename in configFilenames)
			{
				string serialized = null;
				try
				{
					serialized = File.ReadAllText(filename);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine($"Failed to read log definitions {filename}:\n{e}");
				}

				var definitions = Toml.ToModel<LogList>(serialized).Logs;
				//var definitions = JsonConvert.DeserializeObject<List<LogDefinition>>(serialized, new StringEnumConverter());
				foreach (var definition in definitions)
				{
					// Paths in the config are relative to the location of the config.
					definition.Filename = Path.Combine(Path.GetDirectoryName(filename), definition.Filename);
				}

				logs.AddRange(definitions);
			}

			var testRunner = new TestRunner();
			Console.WriteLine("Simple data extraction (parse -> process -> analyze)");
			bool successSimple = testRunner.TestLogs(logs, Console.Out, new SimpleLogDataExtractor());
			Console.WriteLine("Merged data extraction (parse & process -> analyze)");
			bool successMerged = testRunner.TestLogs(logs, Console.Out, new MergedParseProcessLogDataExtractor());
			Console.WriteLine("Merged data extraction with pruning (parse & process -> analyze), some combat items removed during parsing");
			bool successPruned = testRunner.TestLogs(logs, Console.Out, new PrunedMergedParseProcessLogDataExtractor());
			
			if (!successSimple) Console.WriteLine("Simple data extraction failed");
			if (!successMerged) Console.WriteLine("Merged data extraction failed");
			if (!successPruned) Console.WriteLine("Merged data extraction with pruning failed");
			
			return successSimple && successMerged && successPruned ? 0 : 1;
		}
	}
}