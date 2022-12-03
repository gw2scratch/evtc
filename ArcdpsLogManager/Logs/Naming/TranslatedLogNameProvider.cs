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
			if (logData.Encounter != Encounter.Other && logData.Encounter != Encounter.Map)
			{
				if (EncounterNames.TryGetNamesForLanguage(language, out var names))
				{
					if (names.TryGetValue(logData.Encounter, out string name))
					{
						return name;
					}
				}
			}

			if (logData.Encounter == Encounter.Map)
			{
				if (logData.MapId == null)
				{
					return "Unknown map";
				}
				return $"Map {logData.MapId}";
			}
			
			// We default to the name of the main target in case a translated name
			// for the encounter is not available or we don't know the encounter.
			return logData.MainTargetName;
		}
	}
}