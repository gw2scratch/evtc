using System;
using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Names
{
	public class TranslatedEncounterNameProvider : IEncounterNameProvider
	{
		public const string UnknownName = "Unknown Encounter";

		private readonly GameLanguage language;

		public TranslatedEncounterNameProvider(GameLanguage language)
		{
			this.language = language;
		}

		public string GetEncounterName(IEncounterData encounterData, GameLanguage _)
		{
			string name = null;

			// Get encounter name in the game language if names are available
			if (EncounterNames.TryGetNamesForLanguage(language, out var names))
			{
				names.TryGetValue(encounterData.Encounter, out name);
			}

			// If a translated name is not available, default to using the name of the agent
			name ??= new BossEncounterNameProvider().GetEncounterName(encounterData, language);

			// If a name is still unavailable, fall back to a default name
			name ??= UnknownName;

			return name;
		}
	}
}