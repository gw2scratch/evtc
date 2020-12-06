﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GW2Scratch.EVTCAnalytics.LogTests.LocalSets
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

				var definitions = JsonConvert.DeserializeObject<List<LogDefinition>>(serialized, new StringEnumConverter());
				foreach (var definition in definitions)
				{
					// Paths in the config are relative to the location of the config.
					definition.Filename = Path.Combine(Path.GetDirectoryName(filename), definition.Filename);
				}

				logs.AddRange(definitions);
			}

			var testRunner = new TestRunner();
			testRunner.TestLogs(logs, Console.Out);
		}
	}
}