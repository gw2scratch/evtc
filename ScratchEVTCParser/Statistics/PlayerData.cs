using System;
using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Statistics
{
	public class PlayerData
	{
		public PlayerData(Player player, int downCount, int deathCount, IEnumerable<Skill> usedSkills)
		{
			Player = player;
			DownCount = downCount;
			DeathCount = deathCount;
			UsedSkills = usedSkills as Skill[] ?? usedSkills.ToArray();
		}

		public Player Player { get; }
		public int DownCount { get; }
		public int DeathCount { get; }
		public IEnumerable<Skill> UsedSkills { get; }
	}
}