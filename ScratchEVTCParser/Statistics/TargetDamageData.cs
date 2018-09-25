using System.Collections.Generic;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics
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