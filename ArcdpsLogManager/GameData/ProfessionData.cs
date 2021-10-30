using GW2Scratch.EVTCAnalytics.Model.Agents;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.GameData
{
	public static class ProfessionData
	{
		public static IReadOnlyList<(Profession Profession, EliteSpecialization HoT, EliteSpecialization PoF, EliteSpecialization EoD)>
			Professions { get; } = new[]
		{
			(Profession.Warrior, EliteSpecialization.Berserker, EliteSpecialization.Spellbreaker, EliteSpecialization.Bladesworn),
			(Profession.Guardian, EliteSpecialization.Dragonhunter, EliteSpecialization.Firebrand, EliteSpecialization.Willbender),
			(Profession.Revenant, EliteSpecialization.Herald, EliteSpecialization.Renegade, EliteSpecialization.Vindicator),
			(Profession.Ranger, EliteSpecialization.Druid, EliteSpecialization.Soulbeast, EliteSpecialization.Untamed),
			(Profession.Thief, EliteSpecialization.Daredevil, EliteSpecialization.Deadeye, EliteSpecialization.Specter),
			(Profession.Engineer, EliteSpecialization.Scrapper, EliteSpecialization.Holosmith, EliteSpecialization.Mechanist),
			(Profession.Necromancer, EliteSpecialization.Reaper, EliteSpecialization.Scourge, EliteSpecialization.Harbinger),
			(Profession.Elementalist, EliteSpecialization.Tempest, EliteSpecialization.Weaver, EliteSpecialization.Catalyst),
			(Profession.Mesmer, EliteSpecialization.Chronomancer, EliteSpecialization.Mirage, EliteSpecialization.Virtuoso),
		};
	}
}