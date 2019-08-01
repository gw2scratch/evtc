using System.ComponentModel;
using System.Runtime.CompilerServices;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Analytics;
using GW2Scratch.ArcdpsLogManager.Data;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Properties;
using GW2Scratch.ArcdpsLogManager.Sections;
using GW2Scratch.ArcdpsLogManager.Sections.Guilds;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public sealed class GuildDetailPanel : DynamicLayout, INotifyPropertyChanged
	{
		private static readonly GuildData NullGuild = new GuildData(null, new LogData[0], new LogPlayer[0]);

		private GuildData guildData = NullGuild;

		private ApiData ApiData { get; }
		private LogAnalytics LogAnalytics { get; }
		private UploadProcessor UploadProcessor { get; }
		private ImageProvider ImageProvider { get; }

		public GuildData GuildData
		{
			get => guildData;
			set
			{
				if (value == null)
				{
					value = NullGuild;
				}

				if (Equals(value, guildData)) return;
				guildData = value;
				OnPropertyChanged();
			}
		}

		public GuildDetailPanel(LogCache logCache, ApiData apiData, LogAnalytics logAnalytics,
			UploadProcessor uploadProcessor, ImageProvider imageProvider)
		{
			ImageProvider = imageProvider;
			ApiData = apiData;
			LogAnalytics = logAnalytics;
			UploadProcessor = uploadProcessor;

			Padding = new Padding(10);
			Width = 300;
			Visible = false;

			var guildName = new Label()
			{
				Font = Fonts.Sans(16, FontStyle.Bold)
			};
			PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName != nameof(GuildData)) return;

				var name = GuildData.Guid != null ? ApiData.GetGuildName(GuildData.Guid) ?? "(Unknown)" : "(Unknown)";
				var tag = GuildData.Guid != null ? ApiData.GetGuildTag(GuildData.Guid) ?? "???" : "???";
				guildName.Text = $"{name} [{tag}]";
			};

			var memberCountLabel = new Label
			{
				Font = Fonts.Sans(12)
			};
			PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName != nameof(GuildData)) return;
				if (GuildData == null) return;

				var members = $"{GuildData.Accounts.Count} {(GuildData.Accounts.Count == 1 ? "member" : "members")}";
				var characters =
					$"{GuildData.Characters.Count} {(GuildData.Characters.Count == 1 ? "character" : "characters")}";
				memberCountLabel.Text = $"{members}, {characters}";
			};

			var accountGridView = new GridView<GuildMember>();
			accountGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Logs",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildMember, string>(x => $"{x.Logs.Count}")}
			});
			accountGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Account",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildMember, string>(x => x.Name.Substring(1))}
			});
			accountGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Characters",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildMember, string>(x => $"{x.Characters.Count}")}
			});
			// TODO: Add log button
			var characterGridView = new GridView<GuildCharacter>();
			characterGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Logs",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildCharacter, string>(x => $"{x.Logs.Count}")}
			});
			characterGridView.Columns.Add(new GridColumn {
				HeaderText = "",
				DataCell = new ImageViewCell
				{
					Binding = new DelegateBinding<GuildCharacter, Image>(x =>
						ImageProvider.GetTinyProfessionIcon(x.Profession))
				}
			});
			characterGridView.Columns.Add(new GridColumn {
				HeaderText = "Character",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildCharacter, string>(x => x.Name)}
			});
			characterGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Account",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<GuildCharacter, string>(x => x.Account.Name.Substring(1))}
			});
			// TODO: Add log button

			var tabs = new TabControl();
			tabs.Pages.Add(new TabPage(accountGridView) {Text = "Accounts"});
			tabs.Pages.Add(new TabPage(characterGridView) {Text = "Characters"});

			PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName != nameof(GuildData)) return;

				Visible = GuildData != NullGuild;
				accountGridView.DataStore = GuildData?.Accounts;
				characterGridView.DataStore = GuildData?.Characters;
			};

			BeginVertical(spacing: new Size(0, 30));

			BeginVertical();
			Add(guildName);
			Add(memberCountLabel);
			EndVertical();
			BeginVertical(yscale: true);
			Add(tabs);
			EndVertical();

			var logListButton = new Button {Text = "Show logs with this guild"};
			logListButton.Click += (sender, args) =>
			{
				var form = new Form
				{
					Content = new LogList(logCache, apiData, logAnalytics, uploadProcessor, imageProvider)
					{
						DataStore = new FilterCollection<LogData>(GuildData.Logs)
					},
					Width = 900,
					Height = 700,
					Title =
						$"arcdps Log Manager: logs with a member in {ApiData.GetGuildName(guildData.Guid) ?? "(Unknown)"}"
				};
				form.Show();
			};
			BeginVertical();
			Add(logListButton);
			EndVertical();

			EndVertical();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}