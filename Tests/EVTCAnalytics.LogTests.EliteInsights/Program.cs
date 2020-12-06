using System;
using System.IO;

namespace GW2Scratch.EVTCAnalytics.LogTests.EliteInsights
{
	class Program
	{
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

			var testRunner = new TestRunner
			{
				Checker =
				{
					CheckPlayers = false,
					DurationEpsilon = TimeSpan.FromMilliseconds(200)
				},
				PrintUnchecked = true
			};
			testRunner.TestLogs(directory, Console.Out);
		}
	}
}