using GW2Scratch.EVTCAnalytics.GameData;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Names
{
	public interface IEncounterNameProvider
	{
		/// <summary>
		/// Provide the name of an encounter.
		/// </summary>
		/// <param name="encounterData">An encounter to name.</param>
		/// <param name="logLanguage">The language of the log.</param>
		/// <returns>A name of the encounter or null if unable to name.</returns>
		string GetEncounterName(IEncounterData encounterData, GameLanguage logLanguage);
	}
}