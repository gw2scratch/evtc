using System;
using System.Collections.ObjectModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Data;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Sections.Players;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class PlayerList : DynamicLayout
	{
		private ApiData ApiData { get; }
		private ImageProvider ImageProvider { get; }

		private ObservableCollection<PlayerData> playerData;
		private SelectableFilterCollection<PlayerData> filtered;

		private readonly GridView<PlayerData> playerGridView;
		private readonly Label accountCountLabel = new Label();
		private readonly Label characterCountLabel = new Label();

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
				}

				UpdateCountLabels();
			}
		}

		private string PlayerFilter { get; set; } = "";

		public PlayerList(ApiData apiData, ImageProvider imageProvider)
		{
			ApiData = apiData;
			ImageProvider = imageProvider;

			var playerDetailPanel = ConstructPlayerDetailPanel();
			playerGridView = ConstructPlayerGridView(playerDetailPanel);

			DataStore = new ObservableCollection<PlayerData>();

			var playerFilterBox = new TextBox();
			playerFilterBox.TextBinding.Bind(this, x => x.PlayerFilter);
			playerFilterBox.TextChanged += (sender, args) =>
			{
				playerGridView.UnselectAll();
				Refresh();
			};

			BeginVertical(spacing: new Size(5, 5), padding: new Padding(5));
			BeginHorizontal();
			Add(new Label
				{Text = "Filter by character or account name", VerticalAlignment = VerticalAlignment.Center});
			Add(playerFilterBox);
			Add(null, true);
			BeginVertical(xscale: true);
			Add(accountCountLabel);
			Add(characterCountLabel);
			EndVertical();
			EndHorizontal();
			EndVertical();
			BeginVertical(yscale: true);
			BeginHorizontal();
			Add(playerGridView, true);
			Add(playerDetailPanel);
			EndHorizontal();
			EndVertical();
		}

		public void Refresh()
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
			return new PlayerDetailPanel(ApiData, ImageProvider);
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