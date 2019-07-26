namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	public class AttackTarget : Agent
	{
		public int VolatileId { get; }
		public Gadget Gadget { get; internal set; }

		public AttackTarget(ulong address, int id, string name, int hitboxWidth, int hitboxHeight)
			: base(address, id, name, hitboxWidth, hitboxHeight)
		{
			VolatileId = id;
		}
	}
}