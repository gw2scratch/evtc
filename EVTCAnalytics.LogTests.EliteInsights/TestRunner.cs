using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIGW2API;

namespace GW2Scratch.EVTCAnalytics.LogTests.EliteInsights
{
	public class TestRunner
	{
		public bool PrintUnchecked { get; set; } = false;
		public EliteInsightsLogChecker Checker { get; set; } = new EliteInsightsLogChecker(new GW2APIController());

		public void TestLogs(string directory, TextWriter writer)
		{
			var results = new List<CheckResult>();

			foreach (string filename in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
			{
				if (!IsLikelyEvtcLog(filename))
				{
					Console.Error.WriteLine($"Ignoring file: {filename}");
					continue;
				}

				var result = Checker.CheckLog(filename);
				results.Add(result);

				if (result.Ignored)
				{
					writer.WriteLine($"IGNORED {filename}");
					continue;
				}

				if (result.ProcessingFailed)
				{
					writer.WriteLine($"FAILED {filename}");
					writer.WriteLine($"\tException: {result.ProcessingException}");
					continue;
				}

				writer.WriteLine($"{(result.Correct ? "OK" : "WRONG")} {filename}");
				if (!result.Correct)
				{
					PrintResult(result.Encounter, "Encounter", writer);
					PrintResult(result.Result, "Result", writer);
					PrintResult(result.Mode, "Mode", writer);
					PrintResult(result.Duration, "Duration", writer);
					PrintPlayerResult(result.Players, "Players", writer);
				}
			}

			writer.WriteLine("");
			writer.WriteLine($"{results.Count(x => x.Correct)}/{results.Count} OK");
			writer.WriteLine($"{results.Count(x => !x.Correct && !x.ProcessingFailed)}/{results.Count} WRONG");
			writer.WriteLine($"{results.Count(x => x.ProcessingFailed)}/{results.Count} FAILED");
		}

		private void PrintPlayerResult(Result<List<LogPlayer>> result, string name, TextWriter writer)
		{
			if (result.Checked)
			{
				writer.WriteLine($"\t{(result.Correct ? "OK" : "WRONG")} {name}");
				writer.WriteLine($"\t\tExpected ({result.ExpectedValue.Count}):");
				foreach (var player in result.ExpectedValue)
				{
					writer.WriteLine($"\t\t\t{player.CharacterName} {player.AccountName} {player.Profession} {player.EliteSpecialization} {player.Subgroup}");
				}

				writer.WriteLine($"\t\tActual ({result.ActualValue.Count}):");
				foreach (var player in result.ActualValue)
				{
					writer.WriteLine($"\t\t\t{player.CharacterName} {player.AccountName} {player.Profession} {player.EliteSpecialization} {player.Subgroup}");
				}
			}
			else if (PrintUnchecked)
			{
				writer.WriteLine($"\tNot checked {name}");
				writer.WriteLine($"\t\tActual ({result.ActualValue.Count}):");
				foreach (var player in result.ActualValue)
				{
					writer.WriteLine($"\t\t\t{player.CharacterName} {player.AccountName} {player.Profession} {player.EliteSpecialization} {player.Subgroup}");
				}
			}
		}

		private void PrintResult<T>(Result<T> result, string name, TextWriter writer)
		{
			if (result.Checked)
			{
				writer.WriteLine($"\t{(result.Correct ? "OK" : "WRONG")} {name} Expected: {result.ExpectedValue}, Actual: {result.ActualValue}");
			}
			else if (PrintUnchecked)
			{
				writer.WriteLine($"\tNot checked {name}, Actual: {result.ActualValue}");
			}
		}

		private bool IsLikelyEvtcLog(string filename)
		{
			if (filename.EndsWith(".evtc") || filename.EndsWith(".evtc.zip") || filename.EndsWith(".zevtc"))
			{
				return true;
			}

			try
			{
				using var reader = new StreamReader(filename);
				var buffer = new char[4];
				reader.Read(buffer, 0, buffer.Length);

				return buffer[0] == 'E' && buffer[1] == 'V' && buffer[2] == 'T' && buffer[3] == 'C';
			}
			catch
			{
				return false;
			}
		}
	}
}