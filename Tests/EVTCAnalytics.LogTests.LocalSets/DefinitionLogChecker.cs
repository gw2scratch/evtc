using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.LogTests.LocalSets.Extraction;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.LogTests.LocalSets
{
	public class DefinitionLogChecker
	{
		private readonly ILogDataExtractor logDataExtractor;

		public DefinitionLogChecker(ILogDataExtractor logDataExtractor)
		{
			this.logDataExtractor = logDataExtractor;
		}

		public CheckResult CheckLog(LogDefinition definition)
		{
			try
			{
				var (encounter, mode, result, rawPlayers) = logDataExtractor.ExtractData(definition.Filename);
				var players = rawPlayers
					.Select(p => new LogPlayer
					{
						CharacterName = p.Name,
						AccountName = p.AccountName,
						Profession = p.Profession,
						EliteSpecialization = p.EliteSpecialization,
						Subgroup = p.Subgroup
					}).ToList();

				var encounterResult = definition.Encounter.HasValue
					? Result<Encounter>.CheckedResult(definition.Encounter.Value, encounter)
					: Result<Encounter>.UncheckedResult(encounter);

				var resultResult = definition.Result.HasValue
					? Result<EncounterResult>.CheckedResult(definition.Result.Value, result)
					: Result<EncounterResult>.UncheckedResult(result);

				var modeResult = definition.Mode.HasValue
					? Result<EncounterMode>.CheckedResult(definition.Mode.Value, mode)
					: Result<EncounterMode>.UncheckedResult(mode);

				var playerResult = definition.Players != null
					? players.ToHashSet().SetEquals(definition.Players)
						? Result<List<LogPlayer>>.CorrectResult(players)
						: Result<List<LogPlayer>>.IncorrectResult(definition.Players, players)
					: Result<List<LogPlayer>>.UncheckedResult(players);

				bool correct = encounterResult.Correct && resultResult.Correct && modeResult.Correct && playerResult.Correct;

				return new CheckResult
				{
					Correct = correct,
					ProcessingFailed = false,
					Encounter = encounterResult,
					Mode = modeResult,
					Result = resultResult,
					Players = playerResult
				};
			}
			catch (Exception e)
			{
				return new CheckResult
				{
					Correct = false,
					ProcessingFailed = true,
					ProcessingException = e
				};
			}
		}
	}
}