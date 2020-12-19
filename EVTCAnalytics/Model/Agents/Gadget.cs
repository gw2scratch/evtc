using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	/// <summary>
	/// Represents a gadget in the game. Gadgets are a special type of NPCs that are immobile
	/// and can be damaged through <see cref="AttackTarget"/>s.
	/// </summary>
	/// <remarks>
	/// Not all combat events are available for gadgets. They are also exempt from maximum distance limits
	/// for reporting data, so logs often contain gadgets located in a far away part of a map.
	/// </remarks>
	public class Gadget : Agent
	{
		/// <summary>
		/// Provides the volatile arcdps ID of this gadget.
		/// </summary>
		/// <remarks>
		/// As it is effectively a hash of parameters of the gadget, this is prone to collisions
		/// and some dynamically placed gadgets may not always have the same id.
		/// </remarks>
		public int VolatileId { get; }

		private readonly List<AttackTarget> attackTargets = new List<AttackTarget>();

		/// <summary>
		/// Provides a list of attack targets of this gadget.
		/// </summary>
		public IReadOnlyList<AttackTarget> AttackTargets => attackTargets;

		/// <summary>
		/// Creates a new instance of a gadget.
		/// </summary>
		public Gadget(AgentOrigin agentOrigin, int volatileId, string name, int hitboxWidth, int hitboxHeight)
			: base(agentOrigin, name, hitboxWidth, hitboxHeight)
		{
			VolatileId = volatileId;
		}

		/// <summary>
		/// Adds a new attack target to this gadget.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the target is already assigned to a different <see cref="Gadget"/>.</exception>
		internal void AddAttackTarget(AttackTarget target)
		{
			if (target.Master != null && target.Master != this)
			{
				throw new ArgumentException("The target is already assigned to a different master gadget",
					nameof(target));
			}

			attackTargets.Add(target);
		}
	}
}