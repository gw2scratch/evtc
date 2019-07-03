using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager
{
	public static class GameData
	{
		public static IReadOnlyList<(Profession profession, EliteSpecialization hot, EliteSpecialization pof)>
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