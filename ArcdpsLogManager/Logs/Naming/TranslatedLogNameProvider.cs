using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Naming
{
	public class TranslatedLogNameProvider : ILogNameProvider
	{
		private readonly GameLanguage language;

		public TranslatedLogNameProvider(GameLanguage language)
		{
			this.language = language;
		}

		public string GetName(LogData logData)
		{
			if (logData.Encounter != Encounter.Other)
			{
				if (EncounterNames.TryGetNamesForLanguage(language, out var names))
				{
					if (names.TryGetValue(logData.Encounter, out string name))
					{
						return name;
					}
				}
			}
			// We default to the name of the main target in case a translated name
			// for the encounter is not available or we don't know the encounter.
			return logData.MainTargetName;
		}
	}
}