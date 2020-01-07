using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.GameData.Encounters
{
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
				{Encounter.Freezie, EncounterCategory.Festival},
				{Encounter.StandardKittyGolem, EncounterCategory.SpecialForcesTrainingArea},
				{Encounter.MediumKittyGolem, EncounterCategory.SpecialForcesTrainingArea},
				{Encounter.LargeKittyGolem, EncounterCategory.SpecialForcesTrainingArea},
				{Encounter.MassiveKittyGolem, EncounterCategory.SpecialForcesTrainingArea},
				{Encounter.WorldVersusWorld, EncounterCategory.WorldVersusWorld},
				{Encounter.ShiverpeaksPass, EncounterCategory.StrikeMission},
				{Encounter.VoiceAndClawOfTheFallen, EncounterCategory.StrikeMission},
				{Encounter.FraenirOfJormag, EncounterCategory.StrikeMission},
				{Encounter.Boneskinner, EncounterCategory.StrikeMission},
			};

		public static EncounterCategory GetEncounterCategory(this Encounter encounter)
		{
			if (Categories.TryGetValue(encounter, out var category))
			{
				return category;
			}

			throw new ArgumentException("Category not defined for the specified encounter", nameof(encounter));
		}
	}
}