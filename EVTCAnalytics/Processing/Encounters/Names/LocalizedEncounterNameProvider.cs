using System;
using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Names
{
	/// <summary>
	/// An encounter name provider that uses the language of the log.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If no translation is available in <see cref="EncounterNames"/>
	/// for the language of the log, the <see cref="DefaultLanguage"/> is used instead.
	/// </para>
	/// <para>
	/// If no name is available even for the <see cref="DefaultLanguage"/>, this
	/// falls back to a <see cref="BossEncounterNameProvider"/>.
	/// </para>
	/// <see cref="EncounterNames"/>
	/// </remarks>
	public class LocalizedEncounterNameProvider : IEncounterNameProvider
	{
		public const GameLanguage DefaultLanguage = GameLanguage.English;

		public const string UnknownName = "Unknown Encounter";

		public string GetEncounterName(IEncounterData encounterData, GameLanguage logLanguage)
		{
			string name = null;

			// Get encounter name in the game language if names are available
			if (EncounterNames.TryGetNamesForLanguage(logLanguage, out var names))
			{
				names.TryGetValue(encounterData.Encounter, out name);
			}

			// If a translation is not available, try the default language
			if (name == null)
			{
				if (EncounterNames.TryGetNamesForLanguage(DefaultLanguage, out var englishNames))
				{
					englishNames.TryGetValue(encounterData.Encounter, out name);
				}
			}

			// If no translated name is available, default to using the name of the agent
			name ??= new BossEncounterNameProvider().GetEncounterName(encounterData, logLanguage);

			// If a name is still unavailable, fall back to a default name
			name ??= UnknownName;

			return name;
		}
	}
}