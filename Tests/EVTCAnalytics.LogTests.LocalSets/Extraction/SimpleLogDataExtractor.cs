using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.LogTests.LocalSets.Extraction;

/// <summary>
/// A simple log data extractor that uses the default log processing pipeline.
/// Parse -> Process -> Analyze
/// </summary>
public class SimpleLogDataExtractor : ILogDataExtractor
{
	private readonly EVTCParser parser = new EVTCParser();
	private readonly LogProcessor processor = new LogProcessor();

	public (Encounter, EncounterMode, EncounterResult, IReadOnlyList<Player>) ExtractData(string filename)
	{
		var parsedLog = parser.ParseLog(filename);
		var log = processor.ProcessLog(parsedLog);
		var analyzer = new LogAnalyzer(log);

		var encounter = log.EncounterData.Encounter;
		var mode = analyzer.GetMode();
		var result = analyzer.GetResult();
		var players = analyzer.GetPlayers();

		return (encounter, mode, result, players);
	}
}