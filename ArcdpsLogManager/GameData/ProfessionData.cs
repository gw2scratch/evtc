using GW2Scratch.EVTCAnalytics.Model.Agents;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.GameData
{
	public static class ProfessionData
	{
		public static IReadOnlyList<(Profession Profession, EliteSpecialization HoT, EliteSpecialization PoF)>
			Professions { get; } = new[]
		{
			(Profession.Warrior, EliteSpecialization.Berserker, EliteSpecialization.Spellbreaker),
			(Profession.Guardian, EliteSpecialization.Dragonhunter, EliteSpecialization.Firebrand),
			(Profession.Revenant, EliteSpecialization.Herald, EliteSpecialization.Renegade),
			(Profession.Ranger, EliteSpecialization.Druid, EliteSpecialization.Soulbeast),
			(Profession.Thief, EliteSpecialization.Daredevil, EliteSpecialization.Deadeye),
			(Profession.Engineer, EliteSpecialization.Scrapper, EliteSpecialization.Holosmith),
			(Profession.Necromancer, EliteSpecialization.Reaper, EliteSpecialization.Scourge),
			(Profession.Elementalist, EliteSpecialization.Tempest, EliteSpecialization.Weaver),
			(Profession.Mesmer, EliteSpecialization.Chronomancer, EliteSpecialization.Mirage),
		};
	}
}