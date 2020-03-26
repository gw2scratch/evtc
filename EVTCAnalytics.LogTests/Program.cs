using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EVTCAnalytics.LogTests
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			string filename = "log-config.json";
			string serialized = null;
			try
			{
				serialized = File.ReadAllText(filename);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine($"Failed to read log definitions {filename}:\n{e}");
			}
			var logs = JsonConvert.DeserializeObject<List<LogDefinition>>(serialized, new StringEnumConverter());

			var testRunner = new TestRunner();
			testRunner.TestLogs(logs, Console.Out);
		}
	}
}