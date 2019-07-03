using System;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Statistics
{
	public class DamageData
	{
		public Agent Attacker { get; }
		public long TimeMs { get; }
		public float ConditionDamage { get; private set; }
		public float PhysicalDamage { get; private set; }
		public float TotalDamage => ConditionDamage + PhysicalDamage;

		public float ConditionDps => ConditionDamage * 1000 / TimeMs;
		public float PhysicalDps => PhysicalDamage * 1000 / TimeMs;
		public float TotalDps => TotalDamage * 1000 / TimeMs;

		public bool MightDataAvailable { get; internal set; } = true;
		public bool VulnerabilityDataAvailable { get; internal set; } = true;
		public bool QuicknessDataAvailable { get; internal set; } = true;

		public float AverageMight => DamageHitCount == 0 ? 0 : MightSum / (float) DamageHitCount;
		public float AverageVulnerability => DamageHitCount == 0 ? 0 : VulnerabilitySum / (float) DamageHitCount;
		public float QuicknessCastRatio => CastCount == 0 ? 0 : QuicknessCastCount / (float) CastCount;

		internal int QuicknessCastCount { get; private set; } = 0;
		internal int CastCount { get; private set; } = 0;

		internal int DamageHitCount { get; private set; } = 0;
		internal long MightSum { get; private set; } = 0;
		internal long VulnerabilitySum { get; private set; } = 0;

		public DamageData(Agent attacker, long timeMs)
		{
			TimeMs = timeMs;
			Attacker = attacker;
		}

		internal void AddSkillCast(bool quickness)
		{
			if (quickness) QuicknessCastCount++;
			CastCount++;
		}

		internal void AddConditionDamage(float damage, int mightStacks, int vulnerabilityStacks)
		{
			DamageHitCount++;
			MightSum += mightStacks;
			VulnerabilitySum += vulnerabilityStacks;
			ConditionDamage += damage;
		}

		internal void AddPhysicalDamage(float damage, int mightStacks, int vulnerabilityStacks)
		{
			DamageHitCount++;
			MightSum += mightStacks;
			VulnerabilitySum += vulnerabilityStacks;
			PhysicalDamage += damage;
		}
	}
}