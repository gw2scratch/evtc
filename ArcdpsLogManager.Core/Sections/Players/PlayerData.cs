using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager.Sections.Players
{
	public class PlayerData
	{
		public string AccountName { get; }
		public IReadOnlyList<LogData> Logs { get; }

		public PlayerData(string accountName, IEnumerable<LogData> logs)
		{
			AccountName = accountName;
			Logs = logs.ToArray();
		}

		/// <summary>
		/// Extracts a list of characters from the logs.
		/// </summary>
		public IEnumerable<PlayerCharacter> FindCharacters()
		{
			var characters = new Dictionary<(string Name, Profession profession), List<LogData>>();
			foreach (var log in Logs)
			{
				var player = log.Players.First(x => x.AccountName == AccountName);

				var key = (player.Name, player.Profession);
				if (!characters.ContainsKey(key))
				{
					characters[key] = new List<LogData>();
				}

				characters[key].Add(log);
			}

			foreach (((string name, Profession profession), List<LogData> logs) in characters)
			{
				yield return new PlayerCharacter(name, profession, logs);
			}
		}
	}
}