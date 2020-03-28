using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	public class AgentOrigin
	{
		public bool Merged => OriginalAgentData.Count > 1;
		public IReadOnlyList<OriginalAgentData> OriginalAgentData { get; internal set; }

		public AgentOrigin(OriginalAgentData originalAgentData)
		{
			OriginalAgentData = new List<OriginalAgentData> {originalAgentData};
		}

		public AgentOrigin(IEnumerable<OriginalAgentData> originalAgentData)
		{
			OriginalAgentData = originalAgentData.ToList();
		}
	}
}