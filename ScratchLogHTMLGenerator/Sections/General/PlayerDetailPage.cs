using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ScratchEVTCParser;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;
using ScratchEVTCParser.Statistics;

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
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine(
				"<script src='https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.js' integrity='sha256-J2sc79NPV/osLcIpzL3K8uJyAD7T5gaEFKlLDM18oxY=' crossorigin='anonymous'></script>");
			int playerIndex = 0;
			foreach (var data in playerData)
			{
				var player = data.Player;

				string usedSkills = string.Join("", data.UsedSkills.Select(x => $"<li>{x.Name} ({x.Id})</li>"));

				// TODO: Proper alt for profession icon
				writer.WriteLine($@"
<div class='box'>
	<article class='media'>
		<div class='media-left'>
			<figure class='image is-64x64'>
				<img src='{Get200PxProfessionIconUrl(player)}' alt='Specialization icon'>
			</figure>
		</div>
		<div class='media-content'>
			<div class='content'>
				<p>
					<strong>{player.Name}</strong> <small>{player.AccountName.Substring(1)}</small> <small>(group {player.Subgroup})</small>
				</p>
				<p>
				Down count: {data.DownCount}, death count: {data.DeathCount}
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
									"<img class='player-skill-image' src='https://wiki.guildwars2.com/images/7/74/Skill.png' alt='Unknown skill'>");
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
		</div>
		<div class='media-right'>
            <canvas id='player-statchart-{playerIndex}' width='200' height='165'></canvas>
            <script>
                var ctx = document.getElementById('player-statchart-{playerIndex}').getContext('2d');

                var radarChart = new Chart(ctx, {{
                    type: 'radar',
                    data: {{
                        labels: ['Power dmg', 'Condition dmg', 'Healing', 'Toughness', 'Concentration'],
                        datasets: [{{
                            data: [{(1 - data.ConditionDamageRatio) * 10}, {data.ConditionDamageRatio * 10}, {player.Healing}, {player.Toughness}, {player.Concentration}],
                            borderColor: '{GetProfessionColorMedium(player)}',
                            pointBorderColor: '{GetProfessionColorMedium(player)}',
                            backgroundColor: '{GetProfessionColorLightTransparent(player)}',
                        }}],
                    }},
                    options: {{
                        scale: {{
                            ticks: {{
                                beginAtZero: true,
                                max: 10,
                                display: false
                            }}
                        }},
                        legend: {{display: false}},
                        responsive: false
                    }}
                }});
            </script>
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
			var land2Skills = data.LandSEt2WeaponSkills.ToArray();

			int requiredWeaponLines =
				(land1Skills.Any(x => x != null) ? 1 : 0) + (land2Skills.Any(x => x != null) ? 1 : 0);
			int requiredUtilityLines = Math.Max(healingSkills.Length / healingSlots,
				Math.Max(utilitySkills.Length / utilitySlots, eliteSkills.Length / eliteSlots));

			int lines = Math.Max(requiredWeaponLines, requiredUtilityLines);
			var matrix = new SkillData[lines][];

			for (int i = 0; i < lines; i++)
			{
				if (i < requiredUtilityLines)
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

		private string GetProfessionColorMedium(Player player)
		{
			switch (player.Profession)
			{
				case Profession.Warrior:
					return "#FFD166";
				case Profession.Guardian:
					return "#72C1D9";
				case Profession.Revenant:
					return "#D16E5A";
				case Profession.Engineer:
					return "#D09C59";
				case Profession.Ranger:
					return "#8CDC82";
				case Profession.Thief:
					return "#C08F95";
				case Profession.Elementalist:
					return "#F68A87";
				case Profession.Necromancer:
					return "#52A76F";
				case Profession.Mesmer:
					return "#B679D5";
				default:
					throw new ArgumentOutOfRangeException(nameof(player.Profession));
			}
		}

		private string GetProfessionColorLight(Player player)
		{
			switch (player.Profession)
			{
				case Profession.Warrior:
					return "#FFF2A4";
				case Profession.Guardian:
					return "#BCE8FD";
				case Profession.Revenant:
					return "#E4AEA3";
				case Profession.Ranger:
					return "#D2F6BC";
				case Profession.Thief:
					return "#DEC6C9";
				case Profession.Engineer:
					return "#E8BC84";
				case Profession.Necromancer:
					return "#BFE6D0";
				case Profession.Elementalist:
					return "#F6BEBC";
				case Profession.Mesmer:
					return "#D09EEA";
				default:
					throw new ArgumentOutOfRangeException(nameof(player.Profession));
			}
		}

		private string GetProfessionColorLightTransparent(Player player)
		{
			switch (player.Profession)
			{
				case Profession.Warrior:
					return "rgba(255,242,164,0.4)";
				case Profession.Guardian:
					return "rgba(188,232,253,0.4)";
				case Profession.Revenant:
					return "rgba(228,174,163,0.4)";
				case Profession.Ranger:
					return "rgba(210,246,188,0.4)";
				case Profession.Thief:
					return "rgba(222,198,201,0.4)";
				case Profession.Engineer:
					return "rgba(232,188,132,0.4)";
				case Profession.Necromancer:
					return "rgba(191,230,208,0.4)";
				case Profession.Elementalist:
					return "rgba(246,190,188,0.4)";
				case Profession.Mesmer:
					return "rgba(208,158,234,0.4)";
				default:
					throw new ArgumentOutOfRangeException(nameof(player.Profession));
			}
		}


		private string Get200PxProfessionIconUrl(Player player)
		{
			if (player.EliteSpecialization == EliteSpecialization.None)
			{
				switch (player.Profession)
				{
					case Profession.Warrior:
						return "https://wiki.guildwars2.com/images/d/db/Warrior_tango_icon_200px.png";
					case Profession.Guardian:
						return "https://wiki.guildwars2.com/images/6/6c/Guardian_tango_icon_200px.png";
					case Profession.Revenant:
						return "https://wiki.guildwars2.com/images/a/a8/Revenant_tango_icon_200px.png";
					case Profession.Ranger:
						return "https://wiki.guildwars2.com/images/5/51/Ranger_tango_icon_200px.png";
					case Profession.Thief:
						return "https://wiki.guildwars2.com/images/1/19/Thief_tango_icon_200px.png";
					case Profession.Engineer:
						return "https://wiki.guildwars2.com/images/2/2f/Engineer_tango_icon_200px.png";
					case Profession.Necromancer:
						return "https://wiki.guildwars2.com/images/c/cd/Necromancer_tango_icon_200px.png";
					case Profession.Elementalist:
						return "https://wiki.guildwars2.com/images/a/a0/Elementalist_tango_icon_200px.png";
					case Profession.Mesmer:
						return "https://wiki.guildwars2.com/images/7/73/Mesmer_tango_icon_200px.png";
					default:
						throw new ArgumentOutOfRangeException(nameof(player.Profession));
				}
			}

			switch (player.EliteSpecialization)
			{
				case EliteSpecialization.Berserker:
					return "https://wiki.guildwars2.com/images/8/80/Berserker_tango_icon_200px.png";
				case EliteSpecialization.Spellbreaker:
					return "https://wiki.guildwars2.com/images/7/78/Spellbreaker_tango_icon_200px.png";
				case EliteSpecialization.Dragonhunter:
					return "https://wiki.guildwars2.com/images/1/1f/Dragonhunter_tango_icon_200px.png";
				case EliteSpecialization.Firebrand:
					return "https://wiki.guildwars2.com/images/7/73/Firebrand_tango_icon_200px.png";
				case EliteSpecialization.Herald:
					return "https://wiki.guildwars2.com/images/c/c7/Herald_tango_icon_200px.png";
				case EliteSpecialization.Renegade:
					return "https://wiki.guildwars2.com/images/b/bc/Renegade_tango_icon_200px.png";
				case EliteSpecialization.Druid:
					return "https://wiki.guildwars2.com/images/6/6d/Druid_tango_icon_200px.png";
				case EliteSpecialization.Soulbeast:
					return "https://wiki.guildwars2.com/images/f/f6/Soulbeast_tango_icon_200px.png";
				case EliteSpecialization.Daredevil:
					return "https://wiki.guildwars2.com/images/c/ca/Daredevil_tango_icon_200px.png";
				case EliteSpecialization.Deadeye:
					return "https://wiki.guildwars2.com/images/b/b0/Deadeye_tango_icon_200px.png";
				case EliteSpecialization.Scrapper:
					return "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png";
				case EliteSpecialization.Holosmith:
					return "https://wiki.guildwars2.com/images/a/ae/Holosmith_tango_icon_200px.png";
				case EliteSpecialization.Reaper:
					return "https://wiki.guildwars2.com/images/9/95/Reaper_tango_icon_200px.png";
				case EliteSpecialization.Scourge:
					return "https://wiki.guildwars2.com/images/8/8a/Scourge_tango_icon_200px.png";
				case EliteSpecialization.Tempest:
					return "https://wiki.guildwars2.com/images/9/90/Tempest_tango_icon_200px.png";
				case EliteSpecialization.Weaver:
					return "https://wiki.guildwars2.com/images/3/31/Weaver_tango_icon_200px.png";
				case EliteSpecialization.Chronomancer:
					return "https://wiki.guildwars2.com/images/8/8b/Chronomancer_tango_icon_200px.png";
				case EliteSpecialization.Mirage:
					return "https://wiki.guildwars2.com/images/a/a9/Mirage_tango_icon_200px.png";
				default:
					throw new ArgumentOutOfRangeException(nameof(player.EliteSpecialization));
			}
		}
	}
}