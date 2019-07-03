using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics.Buffs
{
	public class BuffData
	{
		public IReadOnlyDictionary<Agent, AgentBuffData> AgentBuffData { get; }

		public BuffData(IReadOnlyDictionary<Agent, AgentBuffData> agentBuffData)
		{
			AgentBuffData = agentBuffData;
		}
	}
}