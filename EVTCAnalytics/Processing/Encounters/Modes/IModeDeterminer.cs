using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	/// <summary>
	/// Represents a determiner used for identifying the mode of an encounter.
	/// </summary>
	public interface IModeDeterminer
	{
		/// <summary>
		/// Determines the mode of the encounter recorded by a log.
		/// </summary>
		/// <param name="log">The log of the encounter.</param>
		EncounterMode GetMode(Log log);
	}
}