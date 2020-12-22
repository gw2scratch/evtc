using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Names
{
	/// <summary>
	/// An encounter name provider that uses the name of the main target of the log.
	/// </summary>
	/// <remarks>
	/// The name is always localized according to the <see cref="GameLanguage"/> used by the game client
	/// that recorded the log.
	/// </remarks>
	public class BossEncounterNameProvider : IEncounterNameProvider
	{
		private readonly int bossIndex;

		/// <summary>
		/// Creates a new encounter name provider that uses the name of a boss as name of the encounter.
		/// </summary>
		/// <remarks>Returns null if there are no bosses in the encounter.</remarks>
		/// <param name="bossIndex">If the encounter has multiple bosses, the index of the boss chosen.</param>
		public BossEncounterNameProvider(int bossIndex = 0)
		{
			this.bossIndex = bossIndex;
		}

		public string GetEncounterName(IEncounterData encounterData, GameLanguage logLanguage)
		{
			var bosses = encounterData.Targets.ToList();
			if (bosses.Count <= bossIndex)
			{
				return null;
			}

			return bosses[bossIndex].Name;
		}
	}
}