using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.GameData
{
	public static class Characters
	{
		public static readonly Profession[] Professions =
		{
			Profession.Guardian,
			Profession.Warrior,
			Profession.Engineer,
			Profession.Ranger,
			Profession.Thief,
			Profession.Elementalist,
			Profession.Mesmer,
			Profession.Necromancer,
			Profession.Revenant,
		};

		private static readonly Dictionary<Profession, EliteSpecialization> HeartOfThornsSpecializationsByProfession =
			new Dictionary<Profession, EliteSpecialization>
			{
				{Profession.Guardian, EliteSpecialization.Dragonhunter},
				{Profession.Warrior, EliteSpecialization.Berserker},
				{Profession.Engineer, EliteSpecialization.Scrapper},
				{Profession.Ranger, EliteSpecialization.Druid},
				{Profession.Thief, EliteSpecialization.Daredevil},
				{Profession.Elementalist, EliteSpecialization.Tempest},
				{Profession.Mesmer, EliteSpecialization.Chronomancer},
				{Profession.Necromancer, EliteSpecialization.Reaper},
				{Profession.Revenant, EliteSpecialization.Herald}
			};

		private static readonly IReadOnlyDictionary<Profession, EliteSpecialization> PathOfFireSpecializationsByProfession =
			new Dictionary<Profession, EliteSpecialization>
			{
				{Profession.Guardian, EliteSpecialization.Firebrand},
				{Profession.Warrior, EliteSpecialization.Spellbreaker},
				{Profession.Engineer, EliteSpecialization.Holosmith},
				{Profession.Ranger, EliteSpecialization.Soulbeast},
				{Profession.Thief, EliteSpecialization.Deadeye},
				{Profession.Elementalist, EliteSpecialization.Weaver},
				{Profession.Mesmer, EliteSpecialization.Mirage},
				{Profession.Necromancer, EliteSpecialization.Scourge},
				{Profession.Revenant, EliteSpecialization.Renegade}
			};

		/// <summary>
		/// Ids of elite specializations as used internally in-game and publicly in the official API.
		/// </summary>
		private static readonly IReadOnlyDictionary<uint, EliteSpecialization> SpecializationsById =
			new Dictionary<uint, EliteSpecialization>
			{
				{5, EliteSpecialization.Druid},
				{7, EliteSpecialization.Daredevil},
				{18, EliteSpecialization.Berserker},
				{27, EliteSpecialization.Dragonhunter},
				{34, EliteSpecialization.Reaper},
				{40, EliteSpecialization.Chronomancer},
				{43, EliteSpecialization.Scrapper},
				{48, EliteSpecialization.Tempest},
				{52, EliteSpecialization.Herald},
				{55, EliteSpecialization.Soulbeast},
				{56, EliteSpecialization.Weaver},
				{57, EliteSpecialization.Holosmith},
				{58, EliteSpecialization.Deadeye},
				{59, EliteSpecialization.Mirage},
				{60, EliteSpecialization.Scourge},
				{61, EliteSpecialization.Spellbreaker},
				{62, EliteSpecialization.Firebrand},
				{63, EliteSpecialization.Renegade}
			};

		/// <summary>
		/// Gets the elite specialization with a specific game id.
		/// The ids are both used internally in-game and publicly in the official API.
		/// </summary>
		/// <param name="specializationId">The id of the elite specialization</param>
		/// <returns>
		/// The corresponding specialization or <see cref="EliteSpecialization.None"/>
		/// if the id does not correspond to an elite specialization.
		/// </returns>
		public static EliteSpecialization GetEliteSpecializationFromId(uint specializationId)
		{
			if (SpecializationsById.TryGetValue(specializationId, out var specialization))
			{
				return specialization;
			}

			return EliteSpecialization.None;
		}

		public static EliteSpecialization GetHeartOfThornsEliteSpecialization(Profession profession)
		{
			return HeartOfThornsSpecializationsByProfession[profession];
		}

		public static EliteSpecialization GetPathOfFireEliteSpecialization(Profession profession)
		{
			return PathOfFireSpecializationsByProfession[profession];
		}
	}
}