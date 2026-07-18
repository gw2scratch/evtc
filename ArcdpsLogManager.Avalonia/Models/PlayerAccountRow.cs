using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Sections.Players;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// A display-ready projection of a player account (Avalonia counterpart of the Eto
	/// <c>PlayerList</c> grid rows), backed by <see cref="Sections.Players.PlayerData"/>.
	/// </summary>
	public sealed class PlayerAccountRow
	{
		public PlayerData Data { get; }

		public string AccountName { get; }
		public int LogCount { get; }

		/// <summary>Characters seen for this account (for the detail list).</summary>
		public IReadOnlyList<CharacterRow> Characters { get; }

		public PlayerAccountRow(PlayerData data, ImageProvider images)
		{
			Data = data;
			// Account names are stored with a leading ':'; trim it for display, matching the Eto UI.
			AccountName = data.AccountName.TrimStart(':');
			LogCount = data.Logs.Count;

			Characters = data.FindCharacters()
				.OrderByDescending(c => c.Logs.Count)
				.Select(c => new CharacterRow(c.Name, images.GetTinyProfessionIcon(c.Profession), c.Logs.Count))
				.ToList();
		}

		/// <summary>
		/// Aggregates a set of logs into one row per account seen among their (parsed) players,
		/// ordered by log count descending. Shared by <c>PlayersSectionViewModel</c> and
		/// <c>PlayerSelectWindowViewModel</c> (the Avalonia counterpart of the Eto
		/// <c>Sections/PlayerList.cs</c>'s <c>UpdateDataFromLogs</c>) so the aggregation logic is
		/// not duplicated between the Players tab and the player-picker dialog.
		/// </summary>
		public static List<PlayerAccountRow> BuildFromLogs(IEnumerable<LogData> logs, ImageProvider images)
		{
			var logsByAccount = new Dictionary<string, List<LogData>>();
			foreach (var log in logs)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed || log.Players == null)
				{
					continue;
				}

				foreach (var player in log.Players)
				{
					if (!logsByAccount.TryGetValue(player.AccountName, out var list))
					{
						list = new List<LogData>();
						logsByAccount[player.AccountName] = list;
					}

					list.Add(log);
				}
			}

			return logsByAccount
				.Select(x => new PlayerAccountRow(new PlayerData(x.Key, x.Value), images))
				.OrderByDescending(x => x.LogCount)
				.ToList();
		}
	}

	/// <summary>A character row (name + profession icon + log count) used in detail lists.</summary>
	public sealed class CharacterRow
	{
		public string Name { get; }
		public Bitmap? Icon { get; }
		public int LogCount { get; }

		public CharacterRow(string name, Bitmap? icon, int logCount)
		{
			Name = name;
			Icon = icon;
			LogCount = logCount;
		}
	}
}
