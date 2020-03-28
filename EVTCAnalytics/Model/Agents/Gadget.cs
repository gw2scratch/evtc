using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	public class Gadget : Agent
	{
		public int VolatileId { get; }

		private readonly List<AttackTarget> attackTargets = new List<AttackTarget>();

		public IReadOnlyList<AttackTarget> AttackTargets => attackTargets;

		public Gadget(AgentOrigin agentOrigin, int volatileId, string name, int hitboxWidth, int hitboxHeight)
			: base(agentOrigin, name, hitboxWidth, hitboxHeight)
		{
			VolatileId = volatileId;
		}

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