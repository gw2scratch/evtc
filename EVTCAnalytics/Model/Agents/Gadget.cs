using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	public class Gadget : Agent
	{
		public int VolatileId { get; }

		private readonly List<AttackTarget> attackTargets = new List<AttackTarget>();

		public IReadOnlyList<AttackTarget> AttackTargets => attackTargets;

		public Gadget(ulong address, int id, string name, int hitboxWidth, int hitboxHeight)
			: base(address, id, name, hitboxWidth, hitboxHeight)
		{
			VolatileId = id;
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