namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	public class AttackTarget : Agent
	{
		public int VolatileId { get; }
		public Gadget Gadget { get; internal set; }

		public AttackTarget(AgentOrigin agentOrigin, int volatileId, string name, int hitboxWidth, int hitboxHeight)
			: base(agentOrigin, name, hitboxWidth, hitboxHeight)
		{
			VolatileId = volatileId;
		}
	}
}