using System.Collections.Generic;
using System.Linq;
using ScratchEVTCParser.Model.Agents;

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

		public float? PlayerAverageMightOnHit { get; } = null;
		public float? PlayerAverageTargetVulnerabilityOnHit { get; } = null;
		public float? PlayerQuicknessCastRatio { get; } = null;

		public SquadDamageData(long timeMs, IEnumerable<DamageData> damageData)
		{
			TimeMs = timeMs;
			DamageData = damageData.ToArray();

			TotalConditionDamage = DamageData.Sum(x => x.ConditionDamage);
			TotalPhysicalDamage = DamageData.Sum(x => x.PhysicalDamage);
			TotalDamage = DamageData.Sum(x => x.TotalDamage);

			var playerDamageData = DamageData.Where(x => x.Attacker is Player).ToArray();

			if (playerDamageData.All(x => x.MightDataAvailable))
			{
				var mightSum = playerDamageData.Sum(x => x.MightSum);
				var hitCountSum = playerDamageData.Sum(x => x.DamageHitCount);
				PlayerAverageMightOnHit = hitCountSum == 0 ? 0 : mightSum / (float) hitCountSum;
			}

			if (playerDamageData.All(x => x.VulnerabilityDataAvailable))
			{
				var vulnerabilitySum = playerDamageData.Sum(x => x.VulnerabilitySum);
				var hitCountSum = playerDamageData.Sum(x => x.DamageHitCount);
				PlayerAverageTargetVulnerabilityOnHit = hitCountSum == 0 ? 0 : vulnerabilitySum / (float) hitCountSum;
			}

			if (playerDamageData.All(x => x.QuicknessDataAvailable))
			{
				var quicknessCastCountSum = playerDamageData.Sum(x => x.QuicknessCastCount);
				var castCountSum = playerDamageData.Sum(x => x.CastCount);
				PlayerQuicknessCastRatio = castCountSum == 0 ? 0 : quicknessCastCountSum / (float) castCountSum;
			}
		}
	}
}