using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;

namespace GW2Scratch.ArcdpsLogManager.Controls
{
	public class GroupCompositionControl : DynamicLayout
	{
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

		public GroupCompositionControl(ImageProvider imageProvider)
		{
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
					var imageView = new ImageView() {Image = icon};
					AddRow(imageView, player.Name, player.AccountName.Substring(1));
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