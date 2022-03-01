using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Configuration;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using Image = Eto.Drawing.Image;
using Label = Eto.Forms.Label;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public class GroupCompositionControl : DynamicLayout
	{
		private readonly ApiData apiData;
		private readonly ImageProvider imageProvider;
		private LogPlayer[] players;

		public IEnumerable<LogPlayer> Players
		{
			get => players;
			set
			{
				players = value?.ToArray();
				RecreateLayout();
			}
		}

		public GroupCompositionControl(ApiData apiData, ImageProvider imageProvider)
		{
			this.apiData = apiData;
			this.imageProvider = imageProvider;
		}

		private void RecreateLayout()
		{
			if (players == null)
			{
				SuspendLayout();
				Clear();
				Create();
				ResumeLayout();
				return;
			}

			var playersByGroups = players.GroupBy(x => x.Subgroup).OrderBy(x => x.Key);
			bool anyPlayerTag = players.Any(x => x.Tag != PlayerTag.None);

			SuspendLayout();
			Clear();
			BeginVertical();
			{
				foreach (var group in playersByGroups)
				{
					BeginGroup($"Subgroup {group.Key}", new Padding(5), new Size(5, 5));
					foreach (var player in group)
					{
						var icon = imageProvider.GetTinyProfessionIcon(player);
						var imageView = new ImageView
						{
							Image = icon,
							ToolTip = GameNames.GetSpecializationName(player)
						};

						string guildTagSuffix = "";
						if (player.GuildGuid != null && Settings.ShowGuildTagsInLogDetail)
						{
							string guildTag = apiData.GetGuildTag(player.GuildGuid);
							if (guildTag != "")
							{
								guildTagSuffix = $" [{guildTag}]";
							}
						}

						string guildName = player.GuildGuid != null
							? apiData.GetGuildName(player.GuildGuid)
							: "No guild data";

						var nameLabel = new Label {Text = $"{player.Name}{guildTagSuffix}", ToolTip = guildName};

						if (anyPlayerTag)
						{
							Image tagIcon = player.Tag == PlayerTag.Commander ? imageProvider.GetTinyCommanderIcon() : null;
							string tagToolTip = player.Tag == PlayerTag.Commander ? "Commander" : null;
							var tagImageView = new ImageView
							{
								Image = tagIcon,
								ToolTip = tagToolTip,
								Size = new Size(16, 17)
							};

							BeginHorizontal();
							{
								Add(tagImageView);
								Add(imageView);
								Add(nameLabel, true);
								Add(player.AccountName.Substring(1), true);
							}
							EndHorizontal();
						}
						else
						{
							// If no player has a tag, we don't waste space on it and omit the empty image views.
							BeginHorizontal();
							{
								Add(imageView);
								Add(nameLabel, true);
								Add(player.AccountName.Substring(1), true);
							}
							EndHorizontal();
						}
					}

					Add(null);
					EndGroup();
				}
			}
			EndVertical();
			AddRow(null);
			Create();
			ResumeLayout();
		}
	}
}