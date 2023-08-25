using GW2Scratch.EVTCAnalytics.LogTests.LocalSets.Extraction;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.LogTests.LocalSets
{
	public class TestRunner
	{
		public bool PrintUnchecked { get; set; } = false;

		public bool TestLogs(IEnumerable<LogDefinition> logs, TextWriter writer, ILogDataExtractor extractor)
		{
			var checker = new DefinitionLogChecker(extractor);
			var results = new List<CheckResult>();

			bool success = true;

			foreach (var log in logs)
			{
				var result = checker.CheckLog(log);
				results.Add(result);

				if (result.Ignored)
				{
					writer.WriteLine($"IGNORED {log.Filename} {log.Comment}");
					continue;
				}

				if (result.ProcessingFailed)
				{
					success = false;
					writer.WriteLine($"FAILED {log.Filename} {log.Comment}");
					writer.WriteLine($"\tException: {result.ProcessingException}");
					continue;
				}

				writer.WriteLine($"{(result.Correct ? "OK" : "WRONG")} {log.Filename} {log.Comment}");
				if (!result.Correct)
				{
					success = false;
					PrintResult(result.Encounter, "Encounter", writer);
					PrintResult(result.Result, "Result", writer);
					PrintResult(result.Mode, "Mode", writer);
					PrintPlayerResult(result.Players, "Players", writer);
				}
			}

			writer.WriteLine("");
			writer.WriteLine($"{results.Count(x => x.Correct)}/{results.Count} OK");
			writer.WriteLine($"{results.Count(x => !x.Correct && !x.ProcessingFailed)}/{results.Count} WRONG");
			writer.WriteLine($"{results.Count(x => x.ProcessingFailed)}/{results.Count} FAILED");

			return success;
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
	}
}