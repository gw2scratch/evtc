using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health
{
	/// <summary>
	/// A health determiner that always returns the same value.
	/// </summary>
	public class ConstantHealthDeterminer : IHealthDeterminer
	{
		private readonly float? constantValue;

		public ConstantHealthDeterminer(float? constantValue)
		{
			this.constantValue = constantValue;
		}

		public float? GetMainEnemyHealthFraction(Log log)
		{
			return constantValue;
		}
	}
}