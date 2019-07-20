using System;
using System.Collections.ObjectModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Analytics;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Data;
using GW2Scratch.ArcdpsLogManager.Sections.Guilds;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class GuildList : DynamicLayout
	{
		private ApiData ApiData { get; }
		private LogAnalytics LogAnalytics { get; }
		private ImageProvider ImageProvider { get; }

		private ObservableCollection<GuildData> guildData;
		private SelectableFilterCollection<GuildData> filtered;

		private readonly GridView<GuildData> guildGridView;
		private readonly Label guildCountLabel = new Label();

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
				}

				UpdateCountLabels();
			}
		}

		private string GuildFilter { get; set; } = "";

		public GuildList(ApiData apiData, LogAnalytics logAnalytics, ImageProvider imageProvider)
		{
			ApiData = apiData;
			LogAnalytics = logAnalytics;
			ImageProvider = imageProvider;

			var playerDetailPanel = ConstructGuildDetailPanel();
			guildGridView = ConstructGuildGridView(playerDetailPanel);

			DataStore = new ObservableCollection<GuildData>();

			var playerFilterBox = new TextBox();
			playerFilterBox.TextBinding.Bind(this, x => x.GuildFilter);
			playerFilterBox.TextChanged += (sender, args) =>
			{
				guildGridView.UnselectAll();
				Refresh();
			};

			BeginVertical(spacing: new Size(5, 5), padding: new Padding(5));
			BeginHorizontal();
			Add(new Label
				{Text = "Filter by guild or member name", VerticalAlignment = VerticalAlignment.Center});
			Add(playerFilterBox);
			Add(null, true);
			BeginVertical(xscale: true);
			Add(guildCountLabel);
			EndVertical();
			EndHorizontal();
			EndVertical();
			BeginVertical(yscale: true);
			BeginHorizontal();
			Add(guildGridView, true);
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
			return new GuildDetailPanel(ApiData, LogAnalytics, ImageProvider);
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