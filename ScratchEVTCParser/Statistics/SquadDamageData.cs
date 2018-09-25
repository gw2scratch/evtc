using System.Collections.Generic;
using System.Linq;

namespace ScratchEVTCParser.Statistics
{
	public class SquadDamageData
	{
		public long TimeMs { get; }

		public IEnumerable<DamageData> DamageData { get; }

		public float TotalConditionDamage { get; }
		public float TotalPhysicalDamage { get; }
		public float TotalDamage { get; }

		public float TotalConditionDps => TotalConditionDamage * 1000 / TimeMs;
		public float TotalPhysicalDps => TotalPhysicalDamage * 1000 / TimeMs;
		public float TotalDps => TotalDamage * 1000 / TimeMs;

		public SquadDamageData(long timeMs, IEnumerable<DamageData> damageData)
		{
			TimeMs = timeMs;
			DamageData = damageData.ToArray();

			TotalConditionDamage = DamageData.Sum(x => x.ConditionDamage);
			TotalPhysicalDamage = DamageData.Sum(x => x.PhysicalDamage);
			TotalDamage = DamageData.Sum(x => x.TotalDamage);
		}
	}
}