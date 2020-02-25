using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Sections.Players;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class PlayerList : DynamicLayout
	{
		private ApiData ApiData { get; }
		private LogDataProcessor LogProcessor { get; }
		private UploadProcessor UploadProcessor { get; }
		private ImageProvider ImageProvider { get; }
		private ILogNameProvider LogNameProvider { get; }

		private ObservableCollection<PlayerData> playerData;
		private SelectableFilterCollection<PlayerData> filtered;

		private readonly GridView<PlayerData> playerGridView;
		private readonly GridViewSorter<PlayerData> sorter;
		private readonly Label accountCountLabel = new Label {VerticalAlignment = VerticalAlignment.Center};
		private readonly Label characterCountLabel = new Label {VerticalAlignment = VerticalAlignment.Center};

		public ObservableCollection<PlayerData> DataStore
		{
			get => playerData;
			set
			{
				if (value == null)
				{
					value = new ObservableCollection<PlayerData>();
				}

				playerData = value;
				filtered = new SelectableFilterCollection<PlayerData>(playerGridView, playerData)
					{Filter = FilterPlayerData};
				if (playerGridView != null)
				{
					playerGridView.DataStore = filtered;
					sorter.UpdateDataStore();
				}

				UpdateCountLabels();
			}
		}

		private string PlayerFilter { get; set; } = "";

		public PlayerList(ApiData apiData, LogDataProcessor logProcessor, UploadProcessor uploadProcessor,
			ImageProvider imageProvider, ILogNameProvider logNameProvider)
		{
			ApiData = apiData;
			LogProcessor = logProcessor;
			ImageProvider = imageProvider;
			LogNameProvider = logNameProvider;
			UploadProcessor = uploadProcessor;

			var playerDetailPanel = ConstructPlayerDetailPanel();
			playerGridView = ConstructPlayerGridView(playerDetailPanel);
			sorter = new GridViewSorter<PlayerData>(playerGridView);
			sorter.EnableSorting();

			DataStore = new ObservableCollection<PlayerData>();

			var playerFilterBox = new TextBox();
			playerFilterBox.TextBinding.Bind(this, x => x.PlayerFilter);
			playerFilterBox.TextChanged += (sender, args) =>
			{
				playerGridView.UnselectAll();
				Refresh();
			};

			BeginVertical(spacing: new Size(5, 5), padding: new Padding(5));
			{
				BeginHorizontal();
				{
					Add(new Label
						{Text = "Filter by character or account name", VerticalAlignment = VerticalAlignment.Center});
					Add(playerFilterBox);
					Add(null, true);
					Add(accountCountLabel, true, true);
					Add(characterCountLabel, true, true);
					EndVertical();
				}
				EndHorizontal();
			}
			EndVertical();
			BeginVertical(yscale: true);
			{
				BeginHorizontal();
				{
					Add(playerGridView, true);
					Add(playerDetailPanel);
				}
				EndHorizontal();
			}
			EndVertical();
		}

		public void UpdateDataFromLogs(IEnumerable<LogData> logs)
		{
			var logsByAccountName = new Dictionary<string, List<LogData>>();
			foreach (var log in logs)
			{
				if (log.ParsingStatus != ParsingStatus.Parsed) continue;

				foreach (var player in log.Players)
				{
					if (!logsByAccountName.ContainsKey(player.AccountName))
					{
						logsByAccountName[player.AccountName] = new List<LogData>();
					}

					logsByAccountName[player.AccountName].Add(log);
				}
			}

			DataStore = new ObservableCollection<PlayerData>(logsByAccountName
				.Select(x => new PlayerData(x.Key, x.Value)).OrderByDescending(x => x.Logs.Count));

			Refresh();
		}

		private void Refresh()
		{
			filtered.Refresh();
			UpdateCountLabels();
		}

		private void UpdateCountLabels()
		{
			accountCountLabel.Text = $"{filtered.Count} accounts";
			int characterCount = filtered
				.SelectMany(x =>
					x.Logs.SelectMany(log => log.Players).Where(player => player.AccountName == x.AccountName))
				.Select(x => x.Name)
				.Distinct()
				.Count();

			characterCountLabel.Text = $"{characterCount} characters";
		}

		private bool FilterPlayerData(PlayerData playerData)
		{
			if (string.IsNullOrWhiteSpace(PlayerFilter))
			{
				return true;
			}

			return playerData.AccountName.IndexOf(PlayerFilter, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
			       playerData.Logs.Any(log =>
				       log.Players.Any(player =>
					       player.AccountName == playerData.AccountName &&
					       player.Name.IndexOf(PlayerFilter, StringComparison.CurrentCultureIgnoreCase) >= 0
				       )
			       );
		}

		private PlayerDetailPanel ConstructPlayerDetailPanel()
		{
			return new PlayerDetailPanel(ApiData, LogProcessor, UploadProcessor, ImageProvider, LogNameProvider);
		}

		private GridView<PlayerData> ConstructPlayerGridView(PlayerDetailPanel playerDetailPanel)
		{
			var gridView = new GridView<PlayerData>();
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Account name",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<PlayerData, string>(x => x.AccountName.Substring(1))}
			});
			gridView.Columns.Add(new GridColumn()
			{
				HeaderText = "Log count",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<PlayerData, string>(x => x.Logs.Count.ToString())}
			});

			gridView.SelectionChanged += (sender, args) =>
			{
				if (gridView.SelectedItem != null)
				{
					playerDetailPanel.PlayerData = gridView.SelectedItem;
				}
			};

			return gridView;
		}
	}
}