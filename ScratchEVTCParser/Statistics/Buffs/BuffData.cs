using System.Collections.Generic;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics.Buffs
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