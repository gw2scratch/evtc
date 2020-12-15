using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.Exceptions;
using GW2EIGW2API;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.LogTests.EliteInsights
{
	public class EliteInsightsLogChecker
	{
		public bool CheckPlayers { get; set; } = true;
		public bool CheckMode { get; set; } = true;
		public bool CheckResult { get; set; } = true;
		public bool CheckDuration { get; set; } = true;

		public TimeSpan DurationEpsilon { get; set; } = TimeSpan.FromMilliseconds(10);
		private readonly GW2APIController eiApiController;

		public EliteInsightsLogChecker(GW2APIController eiApiController)
		{
			this.eiApiController = eiApiController;
		}

		private class EIController : ParserController
		{
		}

		public CheckResult CheckLog(string filename)
		{
			try
			{
				var parser = new EVTCParser();
				var processor = new LogProcessor();

				var bytes = ReadLogFileBytes(filename);

				var parsedLog = parser.ParseLog(bytes);
				var log = processor.ProcessLog(parsedLog);
				var analyzer = new LogAnalyzer(log);

				var encounter = log.EncounterData.Encounter;
				var mode = analyzer.GetMode();
				var result = analyzer.GetResult();
				var players = analyzer.GetPlayers()
					.Select(p => new LogPlayer
					{
						CharacterName = p.Name,
						AccountName = p.AccountName,
						Profession = p.Profession,
						EliteSpecialization = p.EliteSpecialization,
						Subgroup = p.Subgroup
					}).ToList();
				var duration = analyzer.GetEncounterDuration();

				// This combination of builds resulted in logs that did not contain NPCs other than the main target.
				// There is not much of a point in checking these.
				// This outdated version of arcdps was commonly used for extended periods of time due to it being
				// the last version that had working arcdps build templates.
				if (log.EvtcVersion == "EVTC20191001" && (log.GameBuild ?? 0) >= 100565)
				{
					return new CheckResult
					{
						Ignored = true,
						Correct = false,
						ProcessingFailed = false,
						Encounter = Result<Encounter>.UncheckedResult(encounter),
						Mode = Result<EncounterMode>.UncheckedResult(mode),
						Result = Result<EncounterResult>.UncheckedResult(result),
						Players = Result<List<LogPlayer>>.UncheckedResult(players),
						Duration = Result<TimeSpan>.UncheckedResult(duration)
					};
				}

				var eiSettings = new EvtcParserSettings(false, false, true, false, false, 0);
				var eiParser = new EvtcParser(eiSettings, eiApiController);
				var eiLog = eiParser.ParseLog(new EIController(), new MemoryStream(bytes), out var eiFailureReason);
				if (eiLog == null)
				{
					eiFailureReason.Throw();
				}

				var eiDuration = TimeSpan.FromMilliseconds(eiLog.FightData.FightEnd - eiLog.FightData.FightStart);
				var eiResult = eiLog.FightData.Success ? EncounterResult.Success : EncounterResult.Failure;
				var eiPlayers = eiLog.PlayerList
					.Where(p => !p.IsFakeActor)
					.Select(p =>
					{
						Profession profession;
						if (Enum.TryParse(p.Prof, out EliteSpecialization specialization))
						{
							profession = GameData.Characters.GetProfession(specialization);
						}
						else
						{
							specialization = EliteSpecialization.None;
							if (!Enum.TryParse(p.Prof, out profession))
							{
								throw new Exception($"Unknown profession {p.Prof} found in Elite Insights data.");
							}
						}

						return new LogPlayer
						{
							CharacterName = p.Character,
							// EI strips the leading : in account names, so we re-add it
							AccountName = $":{p.Account}",
							Profession = profession,
							EliteSpecialization = specialization,
							Subgroup = p.Group
						};
					}).ToList();

				var eiMode = eiLog.FightData.IsCM ? EncounterMode.Challenge : EncounterMode.Normal;

				// There is no reasonable way to compare EI and Analytics encounters
				var encounterResult = Result<Encounter>.UncheckedResult(encounter);

				var resultResult = CheckResult
					? Result<EncounterResult>.CheckedResult(eiResult, result)
					: Result<EncounterResult>.UncheckedResult(result);

				var modeResult = CheckMode
					? Result<EncounterMode>.CheckedResult(eiMode, mode)
					: Result<EncounterMode>.UncheckedResult(mode);

				var playerResult = CheckPlayers
					? players.ToHashSet().SetEquals(eiPlayers)
						? Result<List<LogPlayer>>.CorrectResult(players)
						: Result<List<LogPlayer>>.IncorrectResult(eiPlayers, players)
					: Result<List<LogPlayer>>.UncheckedResult(players);

				var durationResult = CheckDuration
					? (eiDuration - duration) < DurationEpsilon
						? Result<TimeSpan>.CorrectResult(duration)
						: Result<TimeSpan>.IncorrectResult(eiDuration, duration)
					: Result<TimeSpan>.UncheckedResult(duration);

				bool correct = encounterResult.Correct && resultResult.Correct && modeResult.Correct && playerResult.Correct && durationResult.Correct;

				return new CheckResult
				{
					Correct = correct,
					ProcessingFailed = false,
					Encounter = encounterResult,
					Mode = modeResult,
					Result = resultResult,
					Players = playerResult,
					Duration = durationResult
				};
			}
			catch (TooShortException)
			{
				return new CheckResult
				{
					Ignored = true,
					Correct = false
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

		private byte[] ReadLogFileBytes(string filename)
		{
			if (filename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ||
			    filename.EndsWith(".zevtc", StringComparison.OrdinalIgnoreCase))
			{
				using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (var arch = new ZipArchive(fileStream, ZipArchiveMode.Read))
				using (var data = arch.Entries[0].Open())
				{
					var bytes = new byte[arch.Entries[0].Length];
					data.Read(bytes, 0, bytes.Length);
					return bytes;
				}
			}

			return File.ReadAllBytes(filename);
		}
	}
}