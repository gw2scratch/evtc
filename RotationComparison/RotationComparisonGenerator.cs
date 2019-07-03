using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.RotationComparison.JsonModel;
using GW2Scratch.RotationComparison.Logs;
using GW2Scratch.RotationComparison.Rotations;
using Newtonsoft.Json;

namespace GW2Scratch.RotationComparison
{
	public class RotationComparisonGenerator
	{
		private readonly GW2ApiData apiData;

		public RotationComparisonGenerator(GW2ApiData apiData)
		{
			this.apiData = apiData;
		}

		public void WriteHtmlOutput(IEnumerable<ILogSource> logSources, TextWriter writer)
		{
			writer.Write(@"<!DOCTYPE html>
<html>
<head>
	<title>Rotation Comparison</title>
	<meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <script defer src='https://cdnjs.cloudflare.com/ajax/libs/vis/4.21.0/vis.min.js'></script>
    <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/vis/4.21.0/vis.min.css'>
	<link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.4/css/bulma.min.css' integrity='sha256-8B1OaG0zT7uYA572S2xOxWACq9NXYPQ+U5kHPV1bJN4=' crossorigin='anonymous' />
	<style>
		.rot-skill-image {
			width: 32px;
			height: 32px;
		}
        .vis-item {
			border: 0;
			line-height: 0;
        }
        .vis-item .vis-item-content {
			padding: 0;
        }
		.vis-item.vis-range {
			border-radius: 0;
        }
        .vis-item.success {
            background-color: #80DA5F;
			border: 1px solid #005826;
        }
        .vis-item.cancel {
            background-color: #F26D7D;
			border: 1px solid #ED1C24;
        }
        .vis-item.reset {
            background-color: #AAAAAA;
			border: 1px solid black;
        }
        .vis-inner {
			padding: 5px;
        }
		.character-name {
			margin-bottom: 5px;
			display: flex;
        }
		.encounter-name {
			font-size: small;
			text-align: center;
			margin-bottom: 5px;
        }
		.log-name {
			font-size: x-small;
			font-style: italic;
			text-align: right;
        }
	</style>
</head>
<body>
	<section class='section'>
		<div class='container'>
			<h1 class='title'>Rotation Comparison</h1>
        	<div id='rotation'></div>
			<br>
			<div id='errors'></div>
        	<script>
document.addEventListener('DOMContentLoaded', function() {
	var model = ");
			WriteJsonModel(logSources, writer);
			writer.Write(@";
	let container = document.getElementById('rotation');
	let groups = new vis.DataSet({});
	let items = new vis.DataSet({});

	let rotations = model.Rotations;
	let skillData = model.SkillData;
	let errors = model.Errors;

	for (let i = 0; i < errors.length; i++) {
		let div = document.createElement('div');
		div.className = 'notification is-danger';
		div.innerHTML = errors[i];
		document.getElementById('errors').appendChild(div);
	}

	let id = 0;
	for (let i = 0; i < rotations.length; i++) {
		let groupContent = '<div class=\'character-name\'>';
		groupContent += '<img src=\'' + rotations[i].PlayerData.IconUrl + '\'>';
		groupContent += rotations[i].PlayerData.Name;
		groupContent += '</div>';
		groupContent += '<div class=\'encounter-name\'>';
		groupContent += rotations[i].PlayerData.EncounterName;
		groupContent += '</div>';
		groupContent += '<div class=\'log-name\'>';
		groupContent += rotations[i].PlayerData.LogName;
		groupContent += '</div>';

		groups.add({id: i, content: groupContent});

		for (let j = 0; j < rotations[i].Items.length; j++) {
			let item = rotations[i].Items[j];
			if (item.Type == 1) { // TODO: Use id from enum definition
				// Skill cast
                let displayClass = '';
				if (item.CastType == 1) {
					displayClass = 'success';
                } else if (item.CastType == 2) {
					displayClass = 'cancel';
                } else if (item.CastType == 3) {
                    displayClass = 'reset';
                }

                let data = skillData[item.SkillId];
                let name = data.Name;
                if (name == null) {
                    name = 'Unknown Skill Name';
                }
                let iconUrl = data.IconUrl;
                if (iconUrl == null) {
                    iconUrl = 'https://wiki.guildwars2.com/images/7/74/Skill.png';
                }

                let content = '<img class=\'rot-skill-image\' src=\'' + iconUrl + '\' alt=\'' + name + '\' title=\'' + name + '\'> ';
                items.add({id: id++, group: i, content: content, start: item.Time, end: item.TimeEnd, className: displayClass});
            }
        }
	}

	var options = {
		format: {
			minorLabels: function(date,scale,step) {
				return Math.floor(new Date(date).getTime() / 1000) + 's';
			},
			majorLabels: function(date,scale,step) {
				return Math.floor(new Date(date).getTime() / 1000) + 's';
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
		</div>
	</section>
</body>
</html>
");
		}

		public void WriteJsonModel(IEnumerable<ILogSource> logSources, TextWriter writer)
		{
			var errors = new List<string>();
			var jsonRotations = new List<JsonRotation>();

			var usedSkills = new Dictionary<uint, string>();

			foreach (var source in logSources)
			{
                IEnumerable<Rotation> rotations;
                try
                {
	                rotations = source.GetRotations();
                }
                catch (Exception e)
                {
                    errors.Add($"Failed to process {source.GetLogName()}: " + e.Message);
                    continue;
                }

				foreach (var rotation in rotations)
				{
					var player = new PlayerData(rotation.PlayerName,
						GetTinyProfessionIconUrl(rotation.Profession, rotation.Specialization),
						source.GetLogName(), source.GetEncounterName());

					foreach (var skillCast in rotation.Items.OfType<SkillCast>())
					{
						usedSkills[skillCast.SkillId] = skillCast.SkillName;
					}

					jsonRotations.Add(new JsonRotation(player, rotation.Items));
				}
			}

			var skillData = usedSkills
				.Select(x => (Skill: (Id: x.Key, Name: x.Value), Data: apiData.GetSkillData((int) x.Key)))
				.ToDictionary(
					x => x.Skill.Id,
					x => x.Data == null
						? new {Name = x.Skill.Name, IconUrl = (string) null}
						: new {Name = x.Data.Name, IconUrl = x.Data.IconUrl}
				);

			writer.Write(JsonConvert.SerializeObject(new {Rotations = jsonRotations, SkillData = skillData, Errors = errors}));
		}

		public string GetTinyProfessionIconUrl(Profession profession, EliteSpecialization specialization)
		{
			if (specialization == EliteSpecialization.None)
			{
				switch (profession)
				{
					case Profession.Warrior:
						return "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png";
					case Profession.Guardian:
						return "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png";
					case Profession.Revenant:
						return "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png";
					case Profession.Ranger:
						return "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png";
					case Profession.Thief:
						return "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png";
					case Profession.Engineer:
						return "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png";
					case Profession.Necromancer:
						return "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png";
					case Profession.Elementalist:
						return "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png";
					case Profession.Mesmer:
						return "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png";
					default:
						throw new ArgumentOutOfRangeException(nameof(profession));
				}
			}

			switch (specialization)
			{
				case EliteSpecialization.Berserker:
					return "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png";
				case EliteSpecialization.Spellbreaker:
					return "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png";
				case EliteSpecialization.Dragonhunter:
					return "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
				case EliteSpecialization.Firebrand:
					return "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png";
				case EliteSpecialization.Herald:
					return "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png";
				case EliteSpecialization.Renegade:
					return "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png";
				case EliteSpecialization.Druid:
					return "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png";
				case EliteSpecialization.Soulbeast:
					return "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png";
				case EliteSpecialization.Daredevil:
					return "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png";
				case EliteSpecialization.Deadeye:
					return "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png";
				case EliteSpecialization.Scrapper:
					return "https://wiki.guildwars2.com/images/b/be/Scrapper_tango_icon_20px.png";
				case EliteSpecialization.Holosmith:
					return "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png";
				case EliteSpecialization.Reaper:
					return "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png";
				case EliteSpecialization.Scourge:
					return "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png";
				case EliteSpecialization.Tempest:
					return "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png";
				case EliteSpecialization.Weaver:
					return "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png";
				case EliteSpecialization.Chronomancer:
					return "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png";
				case EliteSpecialization.Mirage:
					return "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png";
				default:
					throw new ArgumentOutOfRangeException(nameof(specialization));
			}
		}
	}
}