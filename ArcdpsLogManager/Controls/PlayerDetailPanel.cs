using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Sections;
using GW2Scratch.ArcdpsLogManager.Sections.Players;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public sealed class PlayerDetailPanel : DynamicLayout, INotifyPropertyChanged
	{
		private const string NullAccountName = "-"; // Account names start with :, so this should never appear.

		private PlayerData playerData = new PlayerData(NullAccountName, new LogData[0]);

		public PlayerData PlayerData
		{
			get => playerData;
			set
			{
				if (value == null)
				{
					value = new PlayerData(NullAccountName, new LogData[0]);
				}

				if (Equals(value, playerData)) return;
				playerData = value;
				OnPropertyChanged();
			}
		}

		public PlayerDetailPanel(LogCache logCache, ApiData apiData, LogDataProcessor logProcessor, UploadProcessor uploadProcessor,
			ImageProvider imageProvider, ILogNameProvider logNameProvider)
		{
			Padding = new Padding(10);
			Width = 350;
			Visible = false;

			var accountName = new Label()
			{
				Font = Fonts.Sans(16, FontStyle.Bold),
				Wrap = WrapMode.Word
			};
			PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName != nameof(PlayerData)) return;

				accountName.Text = PlayerData?.AccountName?.Substring(1) ?? "";
			};

			var logCountLabel = new Label()
			{
				Font = Fonts.Sans(12)
			};
			PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName != nameof(PlayerData)) return;
				if (PlayerData?.Logs == null) return;

				logCountLabel.Text =
					$"Appears in {PlayerData.Logs.Count} {(PlayerData.Logs.Count == 1 ? "log" : "logs")}";
			};

			var knownCharacters = new DynamicLayout();
			PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName != nameof(PlayerData)) return;

				Visible = PlayerData.AccountName != NullAccountName;

				var characters = new Dictionary<string, (Profession Profession, int Count)>();
				foreach (var log in PlayerData.Logs)
				{
					var player = log.Players.First(x => x.AccountName == PlayerData.AccountName);

					if (!characters.ContainsKey(player.Name))
					{
						characters[player.Name] = (player.Profession, 0);
					}

					characters[player.Name] = (player.Profession, characters[player.Name].Count + 1);
				}

				knownCharacters.Clear();
				knownCharacters.BeginHorizontal();
				{
					knownCharacters.BeginVertical(spacing: new Size(5, 2));
					{
						knownCharacters.AddRow("", "Character name", "Log count", null);
						foreach (var character in characters.OrderByDescending(x => x.Value.Count))
						{
							var name = character.Key;
							var showLogsButton = new LinkButton {Text = "Logs"};
							showLogsButton.Click += (o, eventArgs) =>
							{
								// We need to make sure that both the character name and the account name match,
								// as a player may delete a character and another player may claim that name.
								var characterLogs = PlayerData.Logs.Where(log => log.Players.Any(x => x.Name == name && x.AccountName == PlayerData.AccountName));
								var form = new Form
								{
									Content = new LogList(logCache, apiData, logProcessor, uploadProcessor, imageProvider, logNameProvider)
										{DataStore = new FilterCollection<LogData>(characterLogs)},
									Width = 900,
									Height = 700,
									Title = $"arcdps Log Manager: logs with character {name}"
								};
								form.Show();
							};

							knownCharacters.BeginHorizontal();
							{
								var icon = new ImageView
								{
									Image = imageProvider.GetTinyProfessionIcon(character.Value.Profession),
									ToolTip = GameNames.GetName(character.Value.Profession)
								};
								knownCharacters.Add(icon);
								knownCharacters.Add(new Label {Text = name, VerticalAlignment = VerticalAlignment.Center});
								knownCharacters.Add(new Label
								{
									Text = $"{character.Value.Count}",
									VerticalAlignment = VerticalAlignment.Center
								});
								knownCharacters.Add(showLogsButton);
								knownCharacters.Add(null);
							}
							knownCharacters.EndHorizontal();
						}
						knownCharacters.AddRow(null);
					}
					knownCharacters.EndVertical();
					knownCharacters.Add(null);
				}
				knownCharacters.EndHorizontal();
				knownCharacters.Create();
			};

			BeginVertical(spacing: new Size(0, 30));
			{
				BeginVertical();
				{
					Add(accountName);
					Add(logCountLabel);
				}
				EndVertical();
				BeginVertical(yscale: true);
				{
					Add(new Scrollable {Content = knownCharacters, Border = BorderType.None});
				}
				EndVertical();

				var logListButton = new Button {Text = "Show logs with this player"};
				logListButton.Click += (sender, args) =>
				{
					var form = new Form
					{
						Content = new LogList(logCache, apiData, logProcessor, uploadProcessor, imageProvider, logNameProvider)
						{
							DataStore = new FilterCollection<LogData>(PlayerData.Logs)
						},
						Width = 900,
						Height = 700,
						Title = $"arcdps Log Manager: logs with {PlayerData.AccountName.Substring(1)}"
					};
					form.Show();
				};
				BeginVertical();
				{
					Add(logListButton);
				}
				EndVertical();
			}
			EndVertical();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}