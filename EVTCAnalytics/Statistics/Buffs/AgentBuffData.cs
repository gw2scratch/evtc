using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Statistics.Buffs
{
	public class AgentBuffData
	{
		public Dictionary<Skill, IBuffStackCollection> StackCollectionsBySkills { get; }

		public AgentBuffData(Agent agent, Dictionary<Skill, IBuffStackCollection> stackCollectionsBySkills)
		{
			Agent = agent;
			StackCollectionsBySkills = stackCollectionsBySkills;
		}

		public Agent Agent { get; }
	}
}