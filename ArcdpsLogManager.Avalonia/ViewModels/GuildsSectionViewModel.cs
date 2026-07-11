using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Sections.Guilds;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the Guilds section (Avalonia counterpart of the Eto <c>Sections/GuildList.cs</c>):
	/// a grid of guilds aggregated from the logs, a name/tag filter, and a count label.
	/// </summary>
	public partial class GuildsSectionViewModel : ObservableObject
	{
		private readonly ApiData apiData;
		private IReadOnlyList<GuildRow> allGuilds = Array.Empty<GuildRow>();

		[ObservableProperty] private string filter = "";
		[ObservableProperty] private GuildRow? selectedGuild;
		[ObservableProperty] private string countText = "";

		public ObservableCollection<GuildRow> Guilds { get; } = new();

		/// <summary>The detail panel for the selected guild (Avalonia counterpart of the Eto
		/// <c>Controls/GuildDetailPanel.cs</c>), showing accounts/characters and the log list.</summary>
		public GuildDetailPanelViewModel Detail { get; }

		public GuildsSectionViewModel(ApiData apiData, ImageProvider images, ILogNameProvider nameProvider)
		{
			this.apiData = apiData;
			Detail = new GuildDetailPanelViewModel(images, nameProvider);
		}

		partial void OnSelectedGuildChanged(GuildRow? value) => Detail.Show(value);

		partial void OnFilterChanged(string value) => ApplyFilter();

		public void UpdateFromLogs(IEnumerable<LogData> logs)
		{
			var dataByGuild = new Dictionary<string, (List<LogData> Logs, List<LogPlayer> Players)>();
			foreach (var log in logs)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed || log.Players == null)
				{
					continue;
				}

				foreach (var player in log.Players)
				{
					if (player.GuildGuid == null)
					{
						continue;
					}

					if (!dataByGuild.TryGetValue(player.GuildGuid, out var entry))
					{
						entry = (new List<LogData>(), new List<LogPlayer>());
						dataByGuild[player.GuildGuid] = entry;
					}

					entry.Logs.Add(log);
					entry.Players.Add(player);
				}
			}

			allGuilds = dataByGuild
				.Select(x => new GuildRow(new GuildData(x.Key, x.Value.Logs.Distinct(), x.Value.Players), apiData))
				.OrderByDescending(x => x.LogCount)
				.ToList();

			ApplyFilter();
		}

		private void ApplyFilter()
		{
			Guilds.Clear();
			IEnumerable<GuildRow> shown = allGuilds;
			if (!string.IsNullOrWhiteSpace(Filter))
			{
				shown = shown.Where(g =>
					g.Name.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) ||
					g.Tag.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) ||
					g.Members.Any(m => m.Contains(Filter, StringComparison.CurrentCultureIgnoreCase)));
			}

			var list = shown.ToList();
			foreach (var row in list)
			{
				Guilds.Add(row);
			}

			CountText = $"{list.Count} guilds";
		}
	}
}
