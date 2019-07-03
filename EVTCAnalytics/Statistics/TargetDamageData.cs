using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics
{
	public class TargetSquadDamageData : SquadDamageData
	{
		public Agent Target { get; }

		public TargetSquadDamageData(Agent target, long timeMs, IEnumerable<DamageData> damageData) : base(timeMs,
			damageData)
		{
			Target = target;
		}
	}
}