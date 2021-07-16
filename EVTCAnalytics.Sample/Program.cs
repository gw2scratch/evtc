using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Processing;
using System;
using System.Linq;

namespace EVTCAnalytics.Sample
{
	static class Program
	{
		static void Main()
		{
			string filename = "example.zevtc";

			var parser = new EVTCParser();      // Used to read a log file and get raw data out of it
			var processor = new LogProcessor(); // Used to process the raw data

			// The parsed log contains raw data from the EVTC file
			ParsedLog parsedLog = parser.ParseLog(filename);

			// The log after processing the raw data into structured events and agents.
			Log log = processor.ProcessLog(parsedLog);
			
			// At this point, we can do anything with the processed data, and use the LogAnalyzer
			// for easy access to most common results with caching.
			var analyzer = new LogAnalyzer(log);
			
			Encounter encounter = analyzer.GetEncounter();

			// Encounter names are available for some languages, we use the target name if it's not.
			if (EncounterNames.TryGetEncounterNameForLanguage(GameLanguage.English, encounter, out string name))
				Console.WriteLine($"Encounter: {name}");
			else
				Console.WriteLine($"Encounter: {log.MainTarget?.Name ?? "unknown target"}");
			
			Console.WriteLine($"Result: {analyzer.GetResult()}");
			Console.WriteLine($"Mode: {analyzer.GetMode()}");
			Console.WriteLine($"Duration: {analyzer.GetEncounterDuration()}");

			// The processed log allows easy access to data about agents
			foreach (var player in log.Agents.OfType<Player>())
			{
				Console.WriteLine($"{player.Name} - {player.AccountName} - {player.Profession} - {player.EliteSpecialization}");
			}

			// Events may be accessed as well
			foreach (var deadEvent in log.Events.OfType<AgentDeadEvent>())
			{
				if (deadEvent.Agent is Player player)
					Console.WriteLine($"{player.Name} died at {deadEvent.Time}.");
			}
		}
	}
}