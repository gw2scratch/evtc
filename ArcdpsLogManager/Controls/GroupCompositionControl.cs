using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Data;
using GW2Scratch.ArcdpsLogManager.Logs;

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
				ResumeLayout();
				return;
			}

			var playersByGroups = players.GroupBy(x => x.Subgroup).OrderBy(x => x.Key);

			SuspendLayout();
			Clear();
			BeginVertical();
			foreach (var group in playersByGroups)
			{
				BeginGroup($"Subgroup {group.Key}", new Padding(5), new Size(5, 5));
				foreach (var player in group)
				{
					var icon = imageProvider.GetTinyProfessionIcon(player);
					var imageView = new ImageView {Image = icon};

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

					AddRow(imageView, nameLabel, player.AccountName.Substring(1));
				}

				Add(null);
				EndGroup();
			}

			EndVertical();
			AddRow(null);
			Create();
			ResumeLayout();
		}
	}
}