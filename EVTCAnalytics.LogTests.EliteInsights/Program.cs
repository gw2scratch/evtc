using System;
using System.IO;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIGW2API;
using GW2Scratch.EVTCAnalytics.Processing;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.LogTests.EliteInsights
{
	class Program
	{
		public static EVTCParser Parser { get; set; } = new EVTCParser();
		public static LogProcessor Processor { get; set; } = new LogProcessor();

		public static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.Error.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} <directory with evtc files>");
				return;
			}

			var directory = args[0];
			if (!Directory.Exists(directory))
			{
				Console.Error.WriteLine("Directory doesn't exist.");
				return;
			}


			// The EI GW2API static class uses a file. The following is duplicated from GW2APIController:
			var contentDirectoryName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/";
			if (!Directory.Exists(contentDirectoryName))
			{
				Directory.CreateDirectory(contentDirectoryName);

				GW2APIController.WriteAPISkillsToFile();
				GW2APIController.WriteAPISpecsToFile();
				GW2APIController.WriteAPITraitsToFile();
			}

			GW2APIController.InitAPICache();

			var testRunner = new TestRunner
			{
				Checker = {CheckPlayers = false}
			};
			testRunner.TestLogs(directory, Console.Out);
		}
	}
}