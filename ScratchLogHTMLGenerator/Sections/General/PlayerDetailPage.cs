using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using ScratchEVTCParser;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;
using ScratchEVTCParser.Statistics;
using ScratchEVTCParser.Statistics.PlayerDataParts;
using ScratchEVTCParser.Statistics.RotationItems;

namespace ScratchLogHTMLGenerator.Sections.General
{
	public class PlayerDetailPage : Page
	{
		private readonly IEnumerable<PlayerData> playerData;

		public PlayerDetailPage(IEnumerable<PlayerData> playerData) : base(true, "Players")
		{
			this.playerData = playerData;
		}

		public override void WriteStyleHtml(TextWriter writer)
		{
			writer.WriteLine(".player-skill-image {width: 32px; height: 32px;}");
			writer.WriteLine(".player-skill-image.cancel {outline: 3px solid red}");
			writer.WriteLine(".player-skill-image.interrupt {outline: 3px solid darkred}");
			writer.WriteLine(".player-trait-badge {display: none !important;}");
		}

		public override void WriteHtml(TextWriter writer)
		{
			int playerIndex = 0;
			foreach (var data in playerData)
			{
				var player = data.Player;

				// TODO: Proper alt for profession icon
				writer.WriteLine($@"
<div class='box'>
	<article class='media'>
		<div class='media-left'>
			<figure class='image is-64x64'>
				<img src='{Get200PxProfessionIconUrl(player)}' alt='Specialization icon'>
			</figure>");

				foreach (var badge in data.Badges)
				{
					if (badge.Type == BadgeType.Specialization)
					{
						writer.WriteLine($"<span class='tag is-rounded player-trait-badge'>{badge.Text}</span>");
					}
				}

				writer.WriteLine($@"
		</div>
		<div class='media-content'>
			<div class='content'>
				<p>
					<strong>{player.Name}</strong> <small>{player.AccountName.Substring(1)}</small> <small>(group {player.Subgroup})</small>
				</p>
				<p>");

				if (data.UtilitySkills != null && data.EliteSkills != null && data.HealingSkills != null)
				{
					var skillMatrix = GetSkillMatrix(data);

					for (int row = 0; row < skillMatrix.Length; row++)
					{
						foreach (var skillData in skillMatrix[row])
						{
							if (skillData == null)
							{
								writer.WriteLine(
									"<img class='player-skill-image' src='https://wiki.guildwars2.com/images/7/74/Skill.png' alt='Unknown skill' title='Unknown skill, unused or instant cast'>");
							}
							else
							{
								var encodedName = System.Web.HttpUtility.HtmlEncode(skillData.Name);
								writer.WriteLine(
									$"<img class='player-skill-image' src='{skillData.IconUrl}' alt='{encodedName}' title='{encodedName}'>");
							}
						}

						if (row != skillMatrix.Length - 1)
						{
							writer.WriteLine("<br>");
						}
					}
				}
				else
				{
					writer.WriteLine(
						$"No data on utility skills, perhaps API data was missing."); // TODO: Add reasons for missing data
				}

				writer.WriteLine($@"
                </p>
			</div>
			<div class='content is-hidden'>");

				// TODO: Tabs?
				if (data.Rotation != null)
				{
					foreach (var rotationItem in data.Rotation.Items.OrderBy(x => x.ItemTime))
					{
						if (rotationItem is SkillCastItem skillCast)
						{
							var encodedName = HttpUtility.HtmlEncode(skillCast.Skill.Name);

							var statusClass = skillCast.Type == SkillCastType.Cancel ? "cancel" :
								skillCast.Type == SkillCastType.Reset ? "interrupt" : "";

							if (skillCast.SkillData == null)
							{
								string title = $"No API data for skill {encodedName} ({skillCast.Skill.Id})";
								writer.WriteLine(
									$"<img class='player-skill-image {statusClass}' src='https://wiki.guildwars2.com/images/7/74/Skill.png' alt='Unknown skill' title='{title}'>");
							}
							else
							{
								writer.WriteLine(
									$"<img class='player-skill-image {statusClass}' src='{skillCast.SkillData.IconUrl}' alt='{encodedName}' title='{encodedName}'>");
							}
						}
						else if (rotationItem is WeaponSwapItem weaponSwap)
						{
							writer.WriteLine(
								$"<img class='player-skill-image' src='https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png' alt='Weapon Swap' title='Weapon Swap to ({weaponSwap.NewWeaponSet})'>");
							writer.WriteLine($"<br>");
						}
					}
				}
				else
				{
					writer.WriteLine("No rotation available.");
				}

				writer.WriteLine($@"
                </p>
			</div>
		</div>
		<div class='media-right'>
			<table class='table is-bordered'>
                <tr><th>Downs</th><td>{data.DownCount}</td></tr>
                <tr><th>Deaths</th><td>{data.DeathCount}</td></tr>
			</table>
		</div>
	</article>
</div>
");
				playerIndex++;
			}
		}

		private SkillData[][] GetSkillMatrix(PlayerData data)
		{
			const int weaponSlots = 5;
			const int healingSlots = 1;
			const int utilitySlots = 3;
			const int eliteSlots = 1;

			var healingSkills = data.HealingSkills.ToArray();
			var utilitySkills = data.UtilitySkills.ToArray();
			var eliteSkills = data.EliteSkills.ToArray();
			var land1Skills = data.LandSet1WeaponSkills.ToArray();
			var land2Skills = data.LandSet2WeaponSkills.ToArray();

			int requiredWeaponLines =
				(land1Skills.Any(x => x != null) ? 1 : 0) + (land2Skills.Any(x => x != null) ? 1 : 0);

			// Integer division with ceiling rounding
			int healingSkillRows = (healingSkills.Length - 1) / healingSlots + 1;
			int utilitySkillRows = (utilitySkills.Length - 1) / utilitySlots + 1;
			int eliteSkillRows = (eliteSkills.Length - 1) / eliteSlots + 1;

			int requiredNonWeaponLines = Math.Max(healingSkillRows, Math.Max(utilitySkillRows, eliteSkillRows));

			int lines = Math.Max(requiredWeaponLines, requiredNonWeaponLines);
			var matrix = new SkillData[lines][];

			for (int i = 0; i < lines; i++)
			{
				if (i < requiredNonWeaponLines)
				{
					matrix[i] = new SkillData[weaponSlots + healingSlots + utilitySlots + eliteSlots];
				}
				else
				{
					matrix[i] = new SkillData[weaponSlots];
				}
			}

			int weaponSkillRow = 0;
			if (land1Skills.Any(x => x != null))
			{
				for (int i = 0; i < weaponSlots; i++)
				{
					matrix[weaponSkillRow][i] = land1Skills[i];
				}

				weaponSkillRow++;
			}

			if (land2Skills.Any(x => x != null))
			{
				for (int i = 0; i < weaponSlots; i++)
				{
					matrix[weaponSkillRow][i] = land2Skills[i];
				}

				weaponSkillRow++;
			}

			for (int i = 0; i < healingSkills.Length; i++)
			{
				matrix[i / healingSlots][weaponSlots + i % healingSlots] = healingSkills[i];
			}

			for (int i = 0; i < utilitySkills.Length; i++)
			{
				matrix[i / utilitySlots][weaponSlots + healingSlots + i % utilitySlots] = utilitySkills[i];
			}

			for (int i = 0; i < eliteSkills.Length; i++)
			{
				matrix[i / eliteSlots][weaponSlots + healingSlots + utilitySlots + i % eliteSlots] = eliteSkills[i];
			}

			return matrix;
		}
	}
}