using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScratchEVTCParser;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics;
using ScratchEVTCParser.Statistics.PlayerDataParts;

namespace RotationComparison.Logs
{
	public class FileLogSource : ILogSource
	{
		private readonly string filename;
		private string[] characterNames;
		private Log processedLog;

		public FileLogSource(string filename)
		{
			this.filename = filename;
		}

		private Log GetLog()
		{
			if (processedLog == null)
			{
				var parser = new EVTCParser();
				var processor = new LogProcessor();
				var parsedLog = parser.ParseLog(filename);
				processedLog = processor.GetProcessedLog(parsedLog);
			}

			return processedLog;
		}

		public void SetCharacterNameFilter(string[] names)
		{
			characterNames = names;
		}

		public IEnumerable<PlayerRotation> GetRotations()
		{
			var log = GetLog();
			var players = log.Agents.OfType<Player>();
			if (characterNames != null)
			{
				players = players.Where(x => characterNames.Contains(x.Name));
			}

			var rotationCalculator = new RotationCalculator();
			foreach (var player in players)
			{
				yield return rotationCalculator.GetRotation(log, player);
			}
		}

		public string GetEncounterName()
		{
			var log = GetLog();
			var calculator = new StatisticsCalculator();
			return calculator.GetEncounter(log).GetName();
		}

		public string GetLogName()
		{
			return Path.GetFileName(filename);
		}
	}
}