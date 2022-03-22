using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.GameData.Encounters
{
	/// <summary>
	/// Provides categories for <see cref="Encounter"/>s.
	/// </summary>
	public static class EncounterCategories
	{
		private static readonly IReadOnlyDictionary<Encounter, EncounterCategory> Categories =
			new Dictionary<Encounter, EncounterCategory>
			{
				{Encounter.Other, EncounterCategory.Other},
				{Encounter.ValeGuardian, EncounterCategory.RaidWing1},
				{Encounter.Gorseval, EncounterCategory.RaidWing1},
				{Encounter.Sabetha, EncounterCategory.RaidWing1},
				{Encounter.Slothasor, EncounterCategory.RaidWing2},
				{Encounter.BanditTrio, EncounterCategory.RaidWing2},
				{Encounter.Matthias, EncounterCategory.RaidWing2},
				{Encounter.Escort, EncounterCategory.RaidWing3},
				{Encounter.KeepConstruct, EncounterCategory.RaidWing3},
				{Encounter.TwistedCastle, EncounterCategory.RaidWing3},
				{Encounter.Xera, EncounterCategory.RaidWing3},
				{Encounter.Cairn, EncounterCategory.RaidWing4},
				{Encounter.MursaatOverseer, EncounterCategory.RaidWing4},
				{Encounter.Samarog, EncounterCategory.RaidWing4},
				{Encounter.Deimos, EncounterCategory.RaidWing4},
				{Encounter.SoullessHorror, EncounterCategory.RaidWing5},
				{Encounter.RiverOfSouls, EncounterCategory.RaidWing5},
				{Encounter.BrokenKing, EncounterCategory.RaidWing5},
				{Encounter.EaterOfSouls, EncounterCategory.RaidWing5},
				{Encounter.Eyes, EncounterCategory.RaidWing5},
				{Encounter.Dhuum, EncounterCategory.RaidWing5},
				{Encounter.ConjuredAmalgamate, EncounterCategory.RaidWing6},
				{Encounter.TwinLargos, EncounterCategory.RaidWing6},
				{Encounter.Qadim, EncounterCategory.RaidWing6},
				{Encounter.Adina, EncounterCategory.RaidWing7},
				{Encounter.Sabir, EncounterCategory.RaidWing7},
				{Encounter.QadimThePeerless, EncounterCategory.RaidWing7},
				{Encounter.MAMA, EncounterCategory.Fractal},
				{Encounter.SiaxTheCorrupted, EncounterCategory.Fractal},
				{Encounter.EnsolyssOfTheEndlessTorment, EncounterCategory.Fractal},
				{Encounter.Skorvald, EncounterCategory.Fractal},
				{Encounter.Artsariiv, EncounterCategory.Fractal},
				{Encounter.Arkk, EncounterCategory.Fractal},
#pragma warning disable 618
				{Encounter.AiKeeperOfThePeak, EncounterCategory.Fractal},
#pragma warning restore 618
				{Encounter.AiKeeperOfThePeakDayOnly, EncounterCategory.Fractal},
				{Encounter.AiKeeperOfThePeakNightOnly, EncounterCategory.Fractal},
				{Encounter.AiKeeperOfThePeakDayAndNight, EncounterCategory.Fractal},
				{Encounter.Freezie, EncounterCategory.StrikeMissionFestival},
				{Encounter.StandardKittyGolem, EncounterCategory.SpecialForcesTrainingArea},
				{Encounter.MediumKittyGolem, EncounterCategory.SpecialForcesTrainingArea},
				{Encounter.LargeKittyGolem, EncounterCategory.SpecialForcesTrainingArea},
				{Encounter.MassiveKittyGolem, EncounterCategory.SpecialForcesTrainingArea},
				{Encounter.WorldVersusWorld, EncounterCategory.WorldVersusWorld},
				{Encounter.ShiverpeaksPass, EncounterCategory.StrikeMissionIcebroodSaga},
				{Encounter.VoiceAndClawOfTheFallen, EncounterCategory.StrikeMissionIcebroodSaga},
				{Encounter.FraenirOfJormag, EncounterCategory.StrikeMissionIcebroodSaga},
				{Encounter.Boneskinner, EncounterCategory.StrikeMissionIcebroodSaga},
				{Encounter.WhisperOfJormag, EncounterCategory.StrikeMissionIcebroodSaga},
				{Encounter.VariniaStormsounder, EncounterCategory.StrikeMissionIcebroodSaga},
				{Encounter.Mordremoth, EncounterCategory.Other},
				{Encounter.AetherbladeHideout, EncounterCategory.StrikeMissionEndOfDragons},
				{Encounter.XunlaiJadeJunkyard, EncounterCategory.StrikeMissionEndOfDragons},
				{Encounter.KainengOverlook, EncounterCategory.StrikeMissionEndOfDragons},
				{Encounter.HarvestTemple, EncounterCategory.StrikeMissionEndOfDragons},
			};

		private static readonly HashSet<EncounterCategory> RaidCategories =
			new HashSet<EncounterCategory>()
			{
				EncounterCategory.RaidWing1,
				EncounterCategory.RaidWing2,
				EncounterCategory.RaidWing3,
				EncounterCategory.RaidWing4,
				EncounterCategory.RaidWing5,
				EncounterCategory.RaidWing6,
				EncounterCategory.RaidWing7,
			};
		
		private static readonly HashSet<EncounterCategory> StrikeMissionCategories =
			new HashSet<EncounterCategory>()
			{
				EncounterCategory.StrikeMissionIcebroodSaga,
				EncounterCategory.StrikeMissionEndOfDragons,
				EncounterCategory.StrikeMissionFestival,
			};

		/// <summary>
		/// Provides the category for an encounter.
		/// </summary>
		/// <param name="encounter">The encounter.</param>
		/// <returns>The category for an encounter.</returns>
		/// <exception cref="ArgumentException">Thrown for encounters with no specified category. This should never happen for values defined in <see cref="Encounter"/>.</exception>
		public static EncounterCategory GetEncounterCategory(this Encounter encounter)
		{
			if (Categories.TryGetValue(encounter, out var category))
			{
				return category;
			}

			throw new ArgumentException("Category not defined for the specified encounter", nameof(encounter));
		}

		/// <summary>
		/// Checks if this is a raid category.
		/// </summary>
		/// <returns>A value indicating whether the provided category is a raid category.</returns>
		public static bool IsRaid(this EncounterCategory category)
		{
			return RaidCategories.Contains(category);
		}
		
		/// <summary>
		/// Checks if this is a raid encounter.
		/// </summary>
		/// <returns>A value indicating whether the provided encounter is a raid encounter.</returns>
		public static bool IsRaid(this Encounter encounter)
		{
			var category = encounter.GetEncounterCategory();
			return IsRaid(category);
		}
		
		/// <summary>
		/// Checks if this is a strike mission category.
		/// </summary>
		/// <returns>A value indicating whether the provided category is a strike mission category.</returns>
		public static bool IsStrikeMission(this EncounterCategory category)
		{
			return StrikeMissionCategories.Contains(category);
		}
		
		/// <summary>
		/// Checks if this is a strike mission category.
		/// </summary>
		/// <returns>A value indicating whether the provided category is a strike mission category.</returns>
		public static bool IsStrikeMission(this Encounter encounter)
		{
			var category = encounter.GetEncounterCategory();
			return IsStrikeMission(category);
		}
	}
}