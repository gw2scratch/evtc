namespace GW2Scratch.EVTCAnalytics.Model.Agents;

public class UnlistedAgent : Agent
{
	public UnlistedAgent(AgentOrigin agentOrigin) : base(agentOrigin, $"{agentOrigin} (not in agent list)", 0, 0)
	{
	}
}