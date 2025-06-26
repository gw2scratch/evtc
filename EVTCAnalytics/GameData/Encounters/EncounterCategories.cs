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
				{Encounter.SpiritRace, EncounterCategory.RaidWing1},
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
				{Encounter.Greer, EncounterCategory.RaidWing8},
				{Encounter.Decima, EncounterCategory.RaidWing8},
				{Encounter.Ura, EncounterCategory.RaidWing8},
				{Encounter.MAMA, EncounterCategory.FractalNightmare},
				{Encounter.SiaxTheCorrupted, EncounterCategory.FractalNightmare},
				{Encounter.EnsolyssOfTheEndlessTorment, EncounterCategory.FractalNightmare},
				{Encounter.Skorvald, EncounterCategory.FractalShatteredObservatory},
				{Encounter.Artsariiv, EncounterCategory.FractalShatteredObservatory},
				{Encounter.Arkk, EncounterCategory.FractalShatteredObservatory},
#pragma warning disable 618
				{Encounter.AiKeeperOfThePeak, EncounterCategory.FractalSunquaPeak},
#pragma warning restore 618
				{Encounter.AiKeeperOfThePeakDayOnly, EncounterCategory.FractalSunquaPeak},
				{Encounter.AiKeeperOfThePeakNightOnly, EncounterCategory.FractalSunquaPeak},
				{Encounter.AiKeeperOfThePeakDayAndNight, EncounterCategory.FractalSunquaPeak},
				{Encounter.Kanaxai, EncounterCategory.FractalSilentSurf},
				{Encounter.Eparch, EncounterCategory.FractalLonelyTower},
				{Encounter.WhisperingShadow, EncounterCategory.FractalKinfall},
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
				{Encounter.OldLionsCourt, EncounterCategory.StrikeMissionEndOfDragons},
				{Encounter.CosmicObservatory, EncounterCategory.StrikeMissionSecretsOfTheObscure},
				{Encounter.TempleOfFebe, EncounterCategory.StrikeMissionSecretsOfTheObscure},
				{Encounter.Map, EncounterCategory.Map},
			};

		private static readonly HashSet<EncounterCategory> RaidCategories =
			new HashSet<EncounterCategory>
			{
				EncounterCategory.RaidWing1,
				EncounterCategory.RaidWing2,
				EncounterCategory.RaidWing3,
				EncounterCategory.RaidWing4,
				EncounterCategory.RaidWing5,
				EncounterCategory.RaidWing6,
				EncounterCategory.RaidWing7,
				EncounterCategory.RaidWing8,
			};
		
		private static readonly HashSet<EncounterCategory> StrikeMissionCategories =
			new HashSet<EncounterCategory>
			{
				EncounterCategory.StrikeMissionIcebroodSaga,
				EncounterCategory.StrikeMissionEndOfDragons,
				EncounterCategory.StrikeMissionSecretsOfTheObscure,
				EncounterCategory.StrikeMissionFestival,
			};

		private static readonly HashSet<EncounterCategory> FractalCategories =
			new HashSet<EncounterCategory>
			{
				EncounterCategory.FractalNightmare,
				EncounterCategory.FractalShatteredObservatory,
				EncounterCategory.FractalSunquaPeak,
				EncounterCategory.FractalSilentSurf,
				EncounterCategory.FractalLonelyTower,
				EncounterCategory.FractalKinfall,
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

		/// <summary>
		/// Checks if this is a fractal category.
		/// </summary>
		/// <returns>A value indicating whether the provided category is a fractal category.</returns>
		public static bool IsFractal(this EncounterCategory category)
		{
			return FractalCategories.Contains(category);
		}

		/// <summary>
		/// Checks if this is a fractal encounter.
		/// </summary>
		/// <returns>A value indicating whether the provided encounter is a fractal encounter.</returns>
		public static bool IsFractal(this Encounter encounter)
		{
			var category = encounter.GetEncounterCategory();
			return IsFractal(category);
		}
	}
}