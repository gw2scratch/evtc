using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.GameData
{
	public static class GameLanguageIds
	{
		private static readonly IReadOnlyDictionary<int, GameLanguage> LanguagesById = new Dictionary<int, GameLanguage>()
		{
			{0, GameLanguage.English},
			{2, GameLanguage.French},
			{3, GameLanguage.German},
			{4, GameLanguage.Spanish},
		};

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