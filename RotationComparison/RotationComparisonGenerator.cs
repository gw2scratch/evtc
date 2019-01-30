using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RotationComparison.Logs;
using ScratchEVTCParser;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Skills;
using ScratchEVTCParser.Statistics.RotationItems;

namespace RotationComparison
{
	public class RotationComparisonGenerator
	{
		private readonly GW2ApiData apiData;

		public RotationComparisonGenerator(GW2ApiData apiData)
		{
			this.apiData = apiData;
		}

		private enum RotationItemType
		{
			Unknown = 0,
			SkillCast = 1,
			WeaponSwap = 2,
		}

        private enum SkillCast
        {
	        Unknown = 0,
            Success = 1,
            Cancel = 2,
            Reset = 3,
        }

		private abstract class RotationItemBase
		{
			public abstract RotationItemType Type { get; }
			public abstract long Time { get; }
			public abstract long Duration { get; }
			public long TimeEnd => Time + Duration;
		}

		private class SkillRotationItem : RotationItemBase
		{
			public override RotationItemType Type { get; } = RotationItemType.SkillCast;
            public override long Time { get; }
            public override long Duration { get; }
			public SkillCast CastType { get; }
            public uint SkillId { get; }

            public SkillRotationItem(SkillCastItem skillCastItem)
            {
	            Time = skillCastItem.ItemTime;
	            SkillId = skillCastItem.Skill.Id;
	            Duration = skillCastItem.Duration;
	            switch (skillCastItem.Type)
	            {
		            case SkillCastType.Success:
			            CastType = SkillCast.Success;
			            break;
		            case SkillCastType.Cancel:
			            CastType = SkillCast.Cancel;
			            break;
		            case SkillCastType.Reset:
			            CastType = SkillCast.Reset;
			            break;
		            default:
			            CastType = SkillCast.Unknown;
			            break;
	            }
            }
		}

		private class WeaponSwapRotationItem : RotationItemBase
		{
			public override RotationItemType Type { get; } = RotationItemType.WeaponSwap;
            public override long Time { get; }
            public override long Duration { get; } = 0;
			public WeaponSet NewWeaponSet { get; }

            public WeaponSwapRotationItem(WeaponSwapItem item)
            {
	            Time = item.ItemTime;
	            NewWeaponSet = item.NewWeaponSet;
            }
		}


		public void WriteHtmlOutput(IEnumerable<ILogSource> logSources, TextWriter writer)
		{
			writer.Write(@"<!DOCTYPE html>
<html>
<head>
	<title>Rotation Comparison</title>
	<meta charset='utf-8'>
    <script defer src='https://cdnjs.cloudflare.com/ajax/libs/vis/4.21.0/vis.min.js'></script>
    <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/vis/4.21.0/vis.min.css'>
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
		.main {
			width: 80%;
			margin: 0 auto;
        }
	</style>
</head>
<body>
	<section class='main'>
		<h1>Rotation Comparison</h1>
        <div id='rotation'></div>
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

	let id = 0;
	for (let i = 0; i < rotations.length; i++) {
		groups.add({id: i, content: rotations[i].PlayerData.Name});
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

                let content = '<img class=\'rot-skill-image\' src=\'' + iconUrl + '\' alt=\'' + name + '\'> ';
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
	</section>
</body>
</html>
");
		}

		private class PlayerData
		{
			public string Name { get; }

			public PlayerData(string name)
			{
				Name = name;
			}
		}

		private class Rotation
		{
			public Rotation(PlayerData playerData, IEnumerable<RotationItemBase> items)
			{
				PlayerData = playerData;
				Items = items.ToArray();
			}

			public PlayerData PlayerData { get; }
			public IEnumerable<RotationItemBase> Items { get; }
		}

		public void WriteJsonModel(IEnumerable<ILogSource> logSources, TextWriter writer)
		{
            var rotationLists = new List<Rotation>();

            // This can't simply be a Hashset because Skills from different logs can have the same id,
            // but are in fact different instances
            var usedSkills = new Dictionary<uint, Skill>();

			foreach (var source in logSources)
			{
				var rotations = source.GetRotations();

				foreach (var rotation in rotations)
				{
					var outputItems = new List<RotationItemBase>();

					foreach (var item in rotation.Items)
					{
						switch (item)
						{
							case SkillCastItem skillCastItem:
                                outputItems.Add(new SkillRotationItem(skillCastItem));
                                usedSkills[skillCastItem.Skill.Id] = skillCastItem.Skill;
								break;
							case TemporaryStatusItem temporaryStatusItem:
								break;
							case WeaponSwapItem weaponSwapItem:
                                outputItems.Add(new WeaponSwapRotationItem(weaponSwapItem));
								break;
						}
					}

					var player = new PlayerData(rotation.Player.Name);

					rotationLists.Add(new Rotation(player, outputItems));
				}
			}

			var skillData = usedSkills.Values.Select(x => (Skill: x, Data: apiData.GetSkillData(x)))
				.ToDictionary(
					x => x.Skill.Id,
					x => x.Data == null
						? new {Name = x.Skill.Name, IconUrl = (string) null}
						: new {Name = x.Data.Name, IconUrl = x.Data.IconUrl}
				);

            writer.Write(JsonConvert.SerializeObject(new {Rotations = rotationLists, SkillData = skillData}));
		}
	}
}