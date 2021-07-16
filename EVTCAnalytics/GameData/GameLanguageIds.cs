using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.GameData
{
	/// <summary>
	/// Provides ids for game client languages.
	/// </summary>
	public static class GameLanguageIds
	{
		private static readonly IReadOnlyDictionary<int, GameLanguage> LanguagesById = new Dictionary<int, GameLanguage>()
		{
			{0, GameLanguage.English},
			{2, GameLanguage.French},
			{3, GameLanguage.German},
			{4, GameLanguage.Spanish},
			{5, GameLanguage.Chinese},
		};

		/// <summary>
		/// Gets the language from its internal id.
		/// </summary>
		/// <param name="id">The internal id of a game language.</param>
		/// <returns>The game language corresponding to the specified id.</returns>
		public static GameLanguage GetLanguageById(int id)
		{
			if (LanguagesById.TryGetValue(id, out var language))
			{
				return language;
			}

			return GameLanguage.Other;
		}
	}
}