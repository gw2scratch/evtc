using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Statistics;
using ScratchEVTCParser.Statistics.PlayerDataParts;

namespace RotationComparison.Logs
{
	public class FileLogSource : ILogSource
	{
		private readonly string filename;
		private string[] characterNames;

		public FileLogSource(string filename)
		{
			this.filename = filename;
		}

		public void SetCharacterNameFilter(string[] names)
		{
			characterNames = names;
		}

		public IEnumerable<PlayerRotation> GetRotations()
		{
			var parser = new EVTCParser();
			var processor = new LogProcessor();
			var parsedLog = parser.ParseLog(filename);
			var log = processor.GetProcessedLog(parsedLog);

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
	}
}