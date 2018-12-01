using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ArcdpsLogManager.Annotations;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using Eto.Forms;
using ScratchEVTCParser.Model.Agents;

namespace ArcdpsLogManager.Controls
{
	public class PlayerDetailPanel : DynamicLayout, INotifyPropertyChanged
	{
		private PlayerData playerData = new PlayerData(":", new LogData[0]);
		public ImageProvider ImageProvider { get; }

		public PlayerData PlayerData
		{
			get => playerData;
			set
			{
				if (value == null)
				{
                    value = new PlayerData(":", new LogData[0]);
				}

				if (Equals(value, playerData)) return;
				playerData = value;
				OnPropertyChanged();
			}
		}

		public PlayerDetailPanel(ImageProvider imageProvider)
		{
			ImageProvider = imageProvider;

			Padding = new Padding(10);
			Width = 300;

			var accountName = new Label()
			{
				Font = Fonts.Sans(16, FontStyle.Bold)
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

				logCountLabel.Text = $"Appears in {PlayerData.Logs.Count} {(PlayerData.Logs.Count == 1 ? "log" : "logs")}";
			};

			var knownCharacters = new DynamicLayout();
			PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName != nameof(PlayerData)) return;

				var characters = new Dictionary<string, (Profession profession, int count)>();
				foreach (var log in PlayerData.Logs)
				{
					var player = log.Players.First(x => x.AccountName == PlayerData.AccountName);

					if (!characters.ContainsKey(player.Name))
					{
						characters[player.Name] = (player.Profession, 0);
					}

					characters[player.Name] = (player.Profession, characters[player.Name].count + 1);
				}


				knownCharacters.Clear();
				knownCharacters.BeginVertical(spacing: new Size(5, 5));

				knownCharacters.BeginHorizontal();
				knownCharacters.Add("", false);
				knownCharacters.Add("Character name", false);
				knownCharacters.Add("Log count", false);
				knownCharacters.Add(null, true);
				knownCharacters.EndHorizontal();
				foreach (var character in characters.OrderByDescending(x => x.Value.count))
				{
                    knownCharacters.BeginHorizontal();
                    knownCharacters.Add(ImageProvider.GetTinyProfessionIcon(character.Value.profession), false);
                    knownCharacters.Add(character.Key, false);
                    knownCharacters.Add($"{character.Value.count}", false);
					knownCharacters.Add(null, true);
                    knownCharacters.EndHorizontal();
				}

				knownCharacters.AddRow(null);
				knownCharacters.EndVertical();
				knownCharacters.Create();
			};

			BeginVertical(spacing: new Size(0, 30));

			BeginVertical();
			Add(accountName);
			Add(logCountLabel);
			EndVertical();
			BeginVertical(yscale: true);
			Add(knownCharacters);
			EndVertical();

			EndVertical();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}