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
	public class SquadRotationPage : Page
	{
		private readonly IEnumerable<PlayerData> playerData;

		public SquadRotationPage(IEnumerable<PlayerData> playerData) : base(true, "Squad rotation")
		{
			this.playerData = playerData;
		}

		public override void WriteStyleHtml(TextWriter writer)
		{
			writer.WriteLine(".vis-item {border: 0;}");
			writer.WriteLine(".vis-item {background-color: rgba(150, 150, 150, 0.2);}");
			writer.WriteLine(".vis-item.cancel {background-color: rgba(255, 0, 0, 0.2);}");
			writer.WriteLine(".vis-item.interrupt {background-color: rgba(140, 0, 0, 0.2);}");
			writer.WriteLine(".vis-item {line-height: 0;}");
			writer.WriteLine(".rotation-skill-image {width: 32px; height: 32px;}");
			writer.WriteLine(".rotation-skill-image.cancel {outline: 3px solid red}");
			writer.WriteLine(".rotation-skill-image.interrupt {outline: 3px solid darkred}");
		}

		public override void WriteHeadHtml(TextWriter writer)
		{
			writer.WriteLine(
				"<script defer src='https://cdnjs.cloudflare.com/ajax/libs/vis/4.21.0/vis.min.js'></script>");
			writer.WriteLine(
				"<link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/vis/4.21.0/vis.min.css'>");
		}

		public override void WriteHtml(TextWriter writer)
		{
			writer.WriteLine("<div id='squad-rotation'></div>");

			writer.WriteLine(@"
<script>
document.addEventListener('DOMContentLoaded', function() {
	var container = document.getElementById('squad-rotation');

	var groups = new vis.DataSet([");

			int playerIndex = 0;
			foreach (var data in playerData)
			{
				writer.WriteLine($@"{{id: {playerIndex++}, content: '{data.Player.Name}'}},");
			}

			writer.WriteLine(@"
	]);

	var items = new vis.DataSet([");

			// TODO: Optimize for HTML size

			playerIndex = 0;
			foreach (var data in playerData)
			{
				foreach (var rotationItem in data.Rotation.Items)
				{
					if (rotationItem is SkillCastItem skillCast)
					{
						string className = skillCast.Type == SkillCastType.Cancel ? "className: 'cancel'" :
							skillCast.Type == SkillCastType.Reset ? "className: 'interrupt'" : "";

						var htmlEncodedName = HttpUtility.HtmlEncode(skillCast.Skill.Name);

						var imageClass = skillCast.Type == SkillCastType.Cancel ? "cancel" :
							skillCast.Type == SkillCastType.Reset ? "interrupt" : "";

						string image;
						if (skillCast.SkillData == null)
						{
							string title = $"No API data for skill {htmlEncodedName} ({skillCast.Skill.Id})";
							image = $"<img class='rotation-skill-image {imageClass}' src='https://wiki.guildwars2.com/images/7/74/Skill.png' alt='Unknown skill' title='{title}'>";
						}
						else
						{
							image = $"<img class='rotation-skill-image {imageClass}' src='{skillCast.SkillData.IconUrl}' alt='{htmlEncodedName}' title='{htmlEncodedName}'>";
						}

						image = HttpUtility.JavaScriptStringEncode(image);

						writer.WriteLine(
							$"{{group: {playerIndex}, content: '{image}', start: {skillCast.ItemTime}, end: {skillCast.CastEndTime}, {className}}},");
					}
					else if (rotationItem is WeaponSwapItem weaponSwap)
					{
						/* TODO: Looks bad
						string image = $"<img class='rotation-skill-image' src='https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png' alt='Weapon Swap' title='Weapon Swap to ({weaponSwap.NewWeaponSet})'>";
						image = HttpUtility.JavaScriptStringEncode(image);
						writer.WriteLine($"{{group: {playerIndex}, content: '{image}', start: {weaponSwap.ItemTime}}},");
						*/
					}
				}

				playerIndex++;
			}

			writer.WriteLine(@"
	]);

	var options = {
		format: {
			minorLabels: function(date,scale,step) {
				return Math.floor(new Date(date).getTime() / 1000) + '';
			},
			majorLabels: function(date,scale,step) {
				return Math.floor(new Date(date).getTime() / 1000) + '';
			},
		},
		stack: false,
		selectable: false,
		min: 0,
		//margin: {item: {horizontal: -1}},
		//start: 0, // setting start results in all items in initial window being moved down
		end: 20000,
	};

	var timeline = new vis.Timeline(container, items, groups, options);
});
</script>
");
			foreach (var data in playerData)
			{
				var player = data.Player;
			}
		}
	}
}