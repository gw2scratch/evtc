namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	/// <summary>
	/// Represents an attack target of a <see cref="Gadget"/>.
	/// These typically appear as targetable crosshairs in the game.
	/// </summary>
	public class AttackTarget : Agent
	{
		/// <summary>
		/// Provides the volatile arcdps ID of this attack target.
		/// </summary>
		/// <remarks>
		/// As it is effectively a hash of parameters of the attack target, this is prone to collisions
		/// and some dynamically placed targets may not always have the same id.
		/// </remarks>
		public int VolatileId { get; }
		
		/// <summary>
		/// Provides the Gadget this attack target belongs to.
		/// </summary>
		public Gadget Gadget { get; internal set; }

		public AttackTarget(AgentOrigin agentOrigin, int volatileId, string name, int hitboxWidth, int hitboxHeight)
			: base(agentOrigin, name, hitboxWidth, hitboxHeight)
		{
			VolatileId = volatileId;
		}
	}
}