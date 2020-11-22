using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health
{
	public interface IHealthDeterminer
	{
		/// <summary>
		/// Find the final health percentage of the main enemy or enemies in an encounter.
		/// </summary>
		/// <remarks>
		/// This value may have different semantics depending on the encounter.
		/// For example, for encounters with multiple boss enemies, the higher
		/// of their health values may be used.
		/// </remarks>
		/// <param name="log">An encounter log.</param>
		/// <returns>A health fraction value in range 0-1 or <see langword="null"/> if not applicable for this encounter.</returns>
		float? GetMainEnemyHealthFraction(Log log);
	}
}