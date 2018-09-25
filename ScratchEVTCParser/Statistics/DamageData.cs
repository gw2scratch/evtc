using System;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics
{
	public class DamageData
	{
		public Agent Attacker { get; }
		public long TimeMs { get; }
		public float ConditionDamage { get; }
		public float PhysicalDamage { get; }
		public float TotalDamage { get; }

		public float ConditionDps => ConditionDamage * 1000 / TimeMs;
		public float PhysicalDps => PhysicalDamage * 1000 / TimeMs;
		public float TotalDps => TotalDamage * 1000 / TimeMs;

		public DamageData(Agent attacker, long timeMs, float physicalDamage, float conditionDamage) : this(attacker,
			timeMs, physicalDamage, conditionDamage, conditionDamage + physicalDamage)
		{
		}

		public DamageData(Agent attacker, long timeMs, float physicalDamage, float conditionDamage, float totalDamage)
		{
			TimeMs = timeMs;
			ConditionDamage = conditionDamage;
			PhysicalDamage = physicalDamage;
			TotalDamage = totalDamage;
			Attacker = attacker;
		}

		public static DamageData operator +(DamageData first, DamageData second)
		{
			if (first.TimeMs != second.TimeMs)
			{
				throw new ArgumentException("Time period over which damage was done differs.", nameof(second));
			}

			if (first.Attacker != second.Attacker)
			{
				throw new ArgumentException("The attacker differs.", nameof(second));
			}

			return new DamageData(first.Attacker, first.TimeMs, first.PhysicalDamage + second.PhysicalDamage,
				first.ConditionDamage + second.ConditionDamage);
		}
	}
}