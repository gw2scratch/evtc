using System;

namespace GW2Scratch.EVTCAnalytics.GameData.Encounters
{
	/// <summary>
	/// Represents an encounter.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Has values for commonly logged encounters, even some that are not logged by default.
	/// </para>
	/// <para>
	/// Note that the values do not correspond to species ids of encounter bosses, there is no 1 to 1 mapping.
	/// </para>
	/// </remarks>
	public enum Encounter
	{
		/*
		 * This is a list of commonly logged encounters.
		 *
		 * Their numbers are generally in order within an instance, however if any encounter
		 * has been added later, this might not be true.
		 *
		 * Do not confuse the numbers with species ids of encounter bosses, they do not have a 1 to 1 mapping.
		 *
		 * When adding a new encounter, make sure to also:
		 *   - Add an English name in EncounterNames
		 *   - Add a category in EncounterCategories
		 */

		Other = 0,

		// Raids - Wing 1
		ValeGuardian = 11,
		Gorseval = 13,
		Sabetha = 14,

		// Raids - Wing 2
		Slothasor = 21,
		BanditTrio = 22,
		Matthias = 23,

		// Raids - Wing 3
		Escort = 31,
		KeepConstruct = 32,
		TwistedCastle = 33,
		Xera = 34,

		// Raids - Wing 4
		Cairn = 41,
		MursaatOverseer = 42,
		Samarog = 43,
		Deimos = 44,

		// Raids - Wing 5
		SoullessHorror = 51,
		RiverOfSouls = 52,
		BrokenKing = 53,
		EaterOfSouls = 54,
		Eyes = 55,
		Dhuum = 56,

		// Raids - Wing 6
		ConjuredAmalgamate = 61,
		TwinLargos = 62,
		Qadim = 63,

		// Raids - Wing 7
		Adina = 71,
		Sabir = 72,
		QadimThePeerless = 73,

		// Fractals - Nightmare CM
		MAMA = 10001,
		SiaxTheCorrupted = 10002,
		EnsolyssOfTheEndlessTorment = 10003,

		// Fractals - Shattered Observatory CM
		Skorvald = 10011,
		Artsariiv = 10012,
		Arkk = 10013,

		// Fractals - Sunqua Peak
		[Obsolete("This was one shared encounter for all possible included phases. " +
		          "Use the values for individual encounter contents instead.")]
		AiKeeperOfThePeak = 10021,
		AiKeeperOfThePeakDayOnly = 10022,
		AiKeeperOfThePeakNightOnly = 10023,
		AiKeeperOfThePeakDayAndNight = 10024,

		// Festivals - Wintersday
		Freezie = 20001,

		// Special Forces Training Area
		StandardKittyGolem = 30001,
		MediumKittyGolem = 30002,
		LargeKittyGolem = 30003,
		MassiveKittyGolem = 30004,
		// TODO: Check if there are more golem types

		// Strike Missions - Icebrood Saga
		ShiverpeaksPass = 40001,
		VoiceAndClawOfTheFallen = 40002,
		FraenirOfJormag = 40003,
		Boneskinner = 40004,
		WhisperOfJormag = 40005,
		VariniaStormsounder = 40006, // Last enemy in Strike Mission: Cold War
		
		// Strike Missions - End of Dragons
		AetherbladeHideout = 41001,
		XunlaiJadeJunkyard = 41002,
		KainengOverlook = 41003,
		HarvestTemple = 41004,
		OldLionsCourt = 42001,
		
		// Story
		Mordremoth = 50001,

		// Other
		WorldVersusWorld = 100000,
	}
}