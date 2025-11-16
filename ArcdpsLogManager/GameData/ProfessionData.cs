using GW2Scratch.EVTCAnalytics.Model.Agents;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.GameData
{
	public static class ProfessionData
	{
		public static IReadOnlyList<(Profession Profession, EliteSpecialization HoT, EliteSpecialization PoF, EliteSpecialization EoD, EliteSpecialization VoE)>
			Professions { get; } = new[]
		{
			(Profession.Warrior, EliteSpecialization.Berserker, EliteSpecialization.Spellbreaker, EliteSpecialization.Bladesworn, EliteSpecialization.Paragon),
			(Profession.Guardian, EliteSpecialization.Dragonhunter, EliteSpecialization.Firebrand, EliteSpecialization.Willbender, EliteSpecialization.Luminary),
			(Profession.Revenant, EliteSpecialization.Herald, EliteSpecialization.Renegade, EliteSpecialization.Vindicator, EliteSpecialization.Conduit),
			(Profession.Ranger, EliteSpecialization.Druid, EliteSpecialization.Soulbeast, EliteSpecialization.Untamed, EliteSpecialization.Galeshot),
			(Profession.Thief, EliteSpecialization.Daredevil, EliteSpecialization.Deadeye, EliteSpecialization.Specter, EliteSpecialization.Antiquary),
			(Profession.Engineer, EliteSpecialization.Scrapper, EliteSpecialization.Holosmith, EliteSpecialization.Mechanist, EliteSpecialization.Amalgam),
			(Profession.Necromancer, EliteSpecialization.Reaper, EliteSpecialization.Scourge, EliteSpecialization.Harbinger, EliteSpecialization.Ritualist),
			(Profession.Elementalist, EliteSpecialization.Tempest, EliteSpecialization.Weaver, EliteSpecialization.Catalyst, EliteSpecialization.Evoker),
			(Profession.Mesmer, EliteSpecialization.Chronomancer, EliteSpecialization.Mirage, EliteSpecialization.Virtuoso, EliteSpecialization.Troubadour),
		};
	}
}