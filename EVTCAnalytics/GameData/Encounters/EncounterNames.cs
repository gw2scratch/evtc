using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.GameData.Encounters
{
	/// <summary>
	/// Provides names for encounters.
	/// </summary>
	public static class EncounterNames
	{
		public static IReadOnlyDictionary<Encounter, string> EnglishNames { get; } = new Dictionary<Encounter, string>
		{
			{Encounter.ValeGuardian, "Vale Guardian"},
			{Encounter.Gorseval, "Gorseval the Multifarious"},
			{Encounter.Sabetha, "Sabetha the Saboteur"},
			{Encounter.Slothasor, "Slothasor"},
			{Encounter.BanditTrio, "Bandit Trio"},
			{Encounter.Matthias, "Matthias Gabrel"},
			{Encounter.Escort, "Escort"},
			{Encounter.KeepConstruct, "Keep Construct"},
			{Encounter.TwistedCastle, "Twisted Castle"},
			{Encounter.Xera, "Xera"},
			{Encounter.Cairn, "Cairn the Indomitable"},
			{Encounter.MursaatOverseer, "Mursaat Overseer"},
			{Encounter.Samarog, "Samarog"},
			{Encounter.Deimos, "Deimos"},
			{Encounter.SoullessHorror, "Soulless Horror"},
			{Encounter.RiverOfSouls, "River of Souls"},
			{Encounter.BrokenKing, "Broken King"},
			{Encounter.EaterOfSouls, "Eater of Souls"},
			{Encounter.Eyes, "Statue of Darkness"},
			{Encounter.Dhuum, "Dhuum"},
			{Encounter.ConjuredAmalgamate, "Conjured Amalgamate"},
			{Encounter.TwinLargos, "Twin Largos"},
			{Encounter.Qadim, "Qadim"},
			{Encounter.Adina, "Cardinal Adina"},
			{Encounter.Sabir, "Cardinal Sabir"},
			{Encounter.QadimThePeerless, "Qadim the Peerless"},
			{Encounter.MAMA, "MAMA"},
			{Encounter.SiaxTheCorrupted, "Siax the Corrupted"},
			{Encounter.EnsolyssOfTheEndlessTorment, "Ensolyss of the Endless Torment"},
			{Encounter.Skorvald, "Skorvald the Shattered"},
			{Encounter.Artsariiv, "Artsariiv"},
			{Encounter.Arkk, "Arkk"},
#pragma warning disable 618
			{Encounter.AiKeeperOfThePeak, "Ai, Keeper of the Peak"},
#pragma warning restore 618
			{Encounter.AiKeeperOfThePeakDayOnly, "Ai, Keeper of the Peak – Elemental"},
			{Encounter.AiKeeperOfThePeakNightOnly, "Ai, Keeper of the Peak – Dark"},
			{Encounter.AiKeeperOfThePeakDayAndNight, "Ai, Keeper of the Peak – Both Phases"},
			{Encounter.Kanaxai, "Kanaxai, Scythe of House Aurkus"},
			{Encounter.Freezie, "Freezie"},
			{Encounter.StandardKittyGolem, "Standard Kitty Golem"},
			{Encounter.MediumKittyGolem, "Medium Kitty Golem"},
			{Encounter.LargeKittyGolem, "Large Kitty Golem"},
			{Encounter.MassiveKittyGolem, "Massive Kitty Golem"},
			{Encounter.WorldVersusWorld, "World versus World"},
			{Encounter.ShiverpeaksPass, "Icebrood Construct"},
			{Encounter.VoiceAndClawOfTheFallen, "The Voice and the Claw"},
			{Encounter.FraenirOfJormag, "Fraenir of Jormag"},
			{Encounter.Boneskinner, "Boneskinner"},
			{Encounter.WhisperOfJormag, "Whisper of Jormag"},
			{Encounter.VariniaStormsounder, "Varinia Stormsounder"},
			{Encounter.Mordremoth, "Mordremoth"},
			{Encounter.AetherbladeHideout, "Aetherblade Hideout"},
			{Encounter.XunlaiJadeJunkyard, "Xunlai Jade Junkyard"},
			{Encounter.KainengOverlook, "Kaineng Overlook"},
			{Encounter.HarvestTemple, "Harvest Temple"},
			{Encounter.OldLionsCourt, "Old Lion's Court"},
			{Encounter.CosmicObservatory, "Cosmic Observatory"},
			{Encounter.TempleOfFebe, "Temple of Febe"},
		};

		/// <summary>
		/// Provides a dictionary of names for encounters in the specified language if it is available.
		/// </summary>
		/// <param name="language">The language of the encounter name.</param>
		/// <param name="namesByEncounter">When this value returns, contains the name dictionary; otherwise, <see langword="null"/>.</param>
		/// <returns><see langword="true" /> if there are names available; otherwise, <see langword="false" />.</returns>
		public static bool TryGetNamesForLanguage(GameLanguage language, out IReadOnlyDictionary<Encounter, string> namesByEncounter)
		{
			// TODO: Add translated names for other languages as well
			switch (language)
			{
				case GameLanguage.English:
					namesByEncounter = EnglishNames;
					return true;
				case GameLanguage.French:
				case GameLanguage.German:
				case GameLanguage.Spanish:
				case GameLanguage.Chinese:
				case GameLanguage.Other:
					namesByEncounter = null;
					return false;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Provides a name for an encounter in the specified language if it is available.
		/// </summary>
		/// <param name="language">The language of the encounter name.</param>
		/// <param name="encounter">The encounter.</param>
		/// <param name="encounterName">When this value returns, contains the name; otherwise, <see langword="null"/>.</param>
		/// <returns><see langword="true" /> if there is a name available; otherwise, <see langword="false" />.</returns>
		public static bool TryGetEncounterNameForLanguage(GameLanguage language, Encounter encounter, out string encounterName)
		{
			if (TryGetNamesForLanguage(language, out var names) && names.TryGetValue(encounter, out string name))
			{
				encounterName = name;
				return true;
			}

			encounterName = null;
			return false;
		}
	}
}