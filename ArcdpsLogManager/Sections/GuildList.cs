using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Data;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Sections.Guilds;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class GuildList : DynamicLayout
	{
		private ApiData ApiData { get; }
		private LogDataProcessor LogProcessor { get; }
		private UploadProcessor UploadProcessor { get; }
		private ImageProvider ImageProvider { get; }

		private ObservableCollection<GuildData> guildData;
		private SelectableFilterCollection<GuildData> filtered;

		private readonly GridViewSorter<GuildData> sorter;
		private readonly GridView<GuildData> guildGridView;
		private readonly Label guildCountLabel = new Label {VerticalAlignment = VerticalAlignment.Center};

		public ObservableCollection<GuildData> DataStore
		{
			get => guildData;
			set
			{
				if (value == null)
				{
					value = new ObservableCollection<GuildData>();
				}

				guildData = value;
				filtered = new SelectableFilterCollection<GuildData>(guildGridView, guildData)
					{Filter = FilterGuildData};
				if (guildGridView != null)
				{
					guildGridView.DataStore = filtered;
					sorter.UpdateDataStore();
				}

				UpdateCountLabels();
			}
		}

		private string GuildFilter { get; set; } = "";

		public GuildList(ApiData apiData, LogDataProcessor logProcessor, UploadProcessor uploadProcessor,
			ImageProvider imageProvider)
		{
			ApiData = apiData;
			LogProcessor = logProcessor;
			UploadProcessor = uploadProcessor;
			ImageProvider = imageProvider;

			var playerDetailPanel = ConstructGuildDetailPanel();
			guildGridView = ConstructGuildGridView(playerDetailPanel);
			sorter = new GridViewSorter<GuildData>(guildGridView);
			sorter.EnableSorting();

			DataStore = new ObservableCollection<GuildData>();

			var playerFilterBox = new TextBox();
			playerFilterBox.TextBinding.Bind(this, x => x.GuildFilter);
			playerFilterBox.TextChanged += (sender, args) =>
			{
				guildGridView.UnselectAll();
				Refresh();
			};

			BeginVertical(spacing: new Size(5, 5), padding: new Padding(5));
			{
				BeginHorizontal();
				{
					Add(new Label
						{Text = "Filter by guild or member name", VerticalAlignment = VerticalAlignment.Center});
					Add(playerFilterBox);
					Add(null, true);
					BeginVertical(xscale: true);
					{
						Add(guildCountLabel, true, true);
					}
					EndVertical();
				}
				EndHorizontal();
			}
			EndVertical();
			BeginVertical(yscale: true);
			{
				BeginHorizontal();
				{
					Add(guildGridView, true);
					Add(playerDetailPanel);
				}
				EndHorizontal();
			}
			EndVertical();
		}

		public void UpdateDataFromLogs(IEnumerable<LogData> logs)
		{
			var dataByGuild = new Dictionary<string, (List<LogData> Logs, List<LogPlayer> Players)>();
			foreach (var log in logs)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed) continue;

				foreach (var player in log.Players)
				{
					if (player.GuildGuid == null) continue;

					if (!dataByGuild.ContainsKey(player.GuildGuid))
					{
						dataByGuild[player.GuildGuid] = (new List<LogData>(), new List<LogPlayer>());
					}

					var (guildLogs, guildPlayers) = dataByGuild[player.GuildGuid];

					guildLogs.Add(log);
					guildPlayers.Add(player);
				}
			}

			DataStore = new ObservableCollection<GuildData>(dataByGuild
				.Select(x => new GuildData(x.Key, x.Value.Logs.Distinct(), x.Value.Players))
				.OrderByDescending(x => x.Logs.Count));

			Refresh();
		}

		private void Refresh()
		{
			filtered.Refresh();
			UpdateCountLabels();
		}

		private void UpdateCountLabels()
		{
			guildCountLabel.Text = $"{filtered.Count} guilds";
		}

		private bool FilterGuildData(GuildData data)
		{
			if (string.IsNullOrWhiteSpace(GuildFilter))
			{
				return true;
			}

			return GetName(data).IndexOf(GuildFilter, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
			       GetTag(data).IndexOf(GuildFilter, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
			       data.Characters.Any(member =>
				       member.Name.IndexOf(GuildFilter, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
			       data.Accounts.Any(account =>
				       account.Name.IndexOf(GuildFilter, StringComparison.CurrentCultureIgnoreCase) >= 0);
		}

		private string GetName(GuildData data)
		{
			if (data.Guid == null) return "null";
			return ApiData.GetGuildName(data.Guid) ?? "(Unknown)";
		}

		private string GetTag(GuildData data)
		{
			if (data.Guid == null) return "null";
			return ApiData.GetGuildTag(data.Guid) ?? "???";
		}

		private GuildDetailPanel ConstructGuildDetailPanel()
		{
			return new GuildDetailPanel(ApiData, LogProcessor, UploadProcessor, ImageProvider);
		}

		private GridView<GuildData> ConstructGuildGridView(GuildDetailPanel guildDetailPanel)
		{
			var gridView = new GridView<GuildData>();
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Tag",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildData, string>(GetTag)}
			});
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildData, string>(GetName)}
			});
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Logs",
				DataCell = new TextBoxCell {Binding = new DelegateBinding<GuildData, string>(x => $"{x.Logs.Count}")}
			});
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Players",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildData, string>(x => $"{x.Accounts.Count}")}
			});
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Characters",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildData, string>(x => $"{x.Characters.Count}")}
			});

			gridView.SelectionChanged += (sender, args) =>
			{
				if (gridView.SelectedItem != null)
				{
					guildDetailPanel.GuildData = gridView.SelectedItem;
				}
			};

			return gridView;
		}
	}
}