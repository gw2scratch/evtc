using System.Collections.Generic;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Statistics.Buffs
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