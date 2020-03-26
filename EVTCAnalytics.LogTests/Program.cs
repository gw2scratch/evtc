using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GW2Scratch.EVTCAnalytics.LogTests
{
	internal class Program
	{
		public static void Main(string[] args)
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
			foreach (var filename in configFilenames)
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

				var definitions = JsonConvert.DeserializeObject<List<LogDefinition>>(serialized, new StringEnumConverter());
				logs.AddRange(definitions);
			}

			var testRunner = new TestRunner();
			testRunner.TestLogs(logs, Console.Out);
		}
	}
}