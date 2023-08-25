using GW2Scratch.EVTCAnalytics.Model;
using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health
{
	/// <summary>
	/// An interface for determiners that can find the health fraction of the main enemy or enemies of an encounter.
	/// </summary>
	public interface IHealthDeterminer : IDeterminer
	{
		/// <summary>
		/// Finds the final health percentage of the main enemy or enemies in an encounter.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This value may have different semantics depending on the encounter.
		/// For example, for encounters with multiple boss enemies, the higher
		/// of their health values may be used.
		/// </para>
		/// <para>
		/// The returned value may be higher than 1 in case there are multiple enemies
		/// in sequence in the encounter, each extra 1 corresponds to a health bar in that case.
		/// </para>
		/// </remarks>
		/// <param name="log">An encounter log.</param>
		/// <returns>A non-negative health fraction value where 1 corresponds to 100% or <see langword="null"/> if not applicable for this encounter.</returns>
		float? GetMainEnemyHealthFraction(Log log);
	}
}