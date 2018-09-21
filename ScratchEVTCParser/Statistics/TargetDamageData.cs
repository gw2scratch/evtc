using System;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Statistics
{
	public class TargetDamageData
	{
		public float TimeMs { get; }
		public float ConditionDamage { get; }
		public float PhysicalDamage { get; }
		public float TotalDamage { get; }
		public Agent Target { get; }

		public float ConditionDps => ConditionDamage * 1000 / TimeMs;
		public float PhysicalDps => PhysicalDamage * 1000 / TimeMs;
		public float TotalDps => TotalDamage * 1000 / TimeMs;

		public TargetDamageData(float timeMs, float physicalDamage, float conditionDamage, Agent target) : this(timeMs,
			physicalDamage, conditionDamage, conditionDamage + physicalDamage, target)
		{
		}

		public TargetDamageData(float timeMs, float physicalDamage, float conditionDamage, float totalDamage,
			Agent target)
		{
			TimeMs = timeMs;
			ConditionDamage = conditionDamage;
			PhysicalDamage = physicalDamage;
			TotalDamage = totalDamage;
			Target = target;
		}

		public static TargetDamageData operator +(TargetDamageData first, TargetDamageData second)
		{
			if (first.TimeMs != second.TimeMs)
			{
				throw new ArgumentException("Time period over which damage was done differs.", nameof(second));
			}

			if (first.Target != second.Target)
			{
				throw new ArgumentException("Target differs, both have to have the same target.", nameof(second));
			}

			return new TargetDamageData(first.TimeMs, first.PhysicalDamage + second.PhysicalDamage,
				first.ConditionDamage + second.ConditionDamage, first.Target);
		}
	}
}