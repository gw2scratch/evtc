using System;
using ArcdpsLogManager.Logs;
using Eto.Drawing;
using ScratchEVTCParser.Model.Agents;

namespace ArcdpsLogManager
{
	public class ImageProvider
	{
		private Lazy<Image> TinyIconWarrior { get; } = new Lazy<Image>(Resources.GetTinyIconWarrior);
		private Lazy<Image> TinyIconGuardian { get; } = new Lazy<Image>(Resources.GetTinyIconGuardian);
		private Lazy<Image> TinyIconRevenant { get; } = new Lazy<Image>(Resources.GetTinyIconRevenant);
		private Lazy<Image> TinyIconRanger { get; } = new Lazy<Image>(Resources.GetTinyIconRanger);
		private Lazy<Image> TinyIconThief { get; } = new Lazy<Image>(Resources.GetTinyIconThief);
		private Lazy<Image> TinyIconEngineer { get; } = new Lazy<Image>(Resources.GetTinyIconEngineer);
		private Lazy<Image> TinyIconNecromancer { get; } = new Lazy<Image>(Resources.GetTinyIconNecromancer);
		private Lazy<Image> TinyIconElementalist { get; } = new Lazy<Image>(Resources.GetTinyIconElementalist);
		private Lazy<Image> TinyIconMesmer { get; } = new Lazy<Image>(Resources.GetTinyIconMesmer);
		private Lazy<Image> TinyIconBerserker { get; } = new Lazy<Image>(Resources.GetTinyIconBerserker);
		private Lazy<Image> TinyIconSpellbreaker { get; } = new Lazy<Image>(Resources.GetTinyIconSpellbreaker);
		private Lazy<Image> TinyIconDragonhunter { get; } = new Lazy<Image>(Resources.GetTinyIconDragonhunter);
		private Lazy<Image> TinyIconFirebrand { get; } = new Lazy<Image>(Resources.GetTinyIconFirebrand);
		private Lazy<Image> TinyIconHerald { get; } = new Lazy<Image>(Resources.GetTinyIconHerald);
		private Lazy<Image> TinyIconRenegade { get; } = new Lazy<Image>(Resources.GetTinyIconRenegade);
		private Lazy<Image> TinyIconDruid { get; } = new Lazy<Image>(Resources.GetTinyIconDruid);
		private Lazy<Image> TinyIconSoulbeast { get; } = new Lazy<Image>(Resources.GetTinyIconSoulbeast);
		private Lazy<Image> TinyIconDaredevil { get; } = new Lazy<Image>(Resources.GetTinyIconDaredevil);
		private Lazy<Image> TinyIconDeadeye { get; } = new Lazy<Image>(Resources.GetTinyIconDeadeye);
		private Lazy<Image> TinyIconScrapper { get; } = new Lazy<Image>(Resources.GetTinyIconScrapper);
		private Lazy<Image> TinyIconHolosmith { get; } = new Lazy<Image>(Resources.GetTinyIconHolosmith);
		private Lazy<Image> TinyIconReaper { get; } = new Lazy<Image>(Resources.GetTinyIconReaper);
		private Lazy<Image> TinyIconScourge { get; } = new Lazy<Image>(Resources.GetTinyIconScourge);
		private Lazy<Image> TinyIconTempest { get; } = new Lazy<Image>(Resources.GetTinyIconTempest);
		private Lazy<Image> TinyIconWeaver { get; } = new Lazy<Image>(Resources.GetTinyIconWeaver);
		private Lazy<Image> TinyIconChronomancer { get; } = new Lazy<Image>(Resources.GetTinyIconChronomancer);
		private Lazy<Image> TinyIconMirage { get; } = new Lazy<Image>(Resources.GetTinyIconMirage);

		public Image GetTinyProfessionIcon(LogPlayer player)
		{
		    if (player.EliteSpecialization == EliteSpecialization.None)
		    {
			    switch (player.Profession)
			    {
				    case Profession.Warrior:
						return TinyIconWarrior.Value;
				    case Profession.Guardian:
						return TinyIconGuardian.Value;
				    case Profession.Revenant:
						return TinyIconRevenant.Value;
				    case Profession.Ranger:
						return TinyIconRanger.Value;
				    case Profession.Thief:
						return TinyIconThief.Value;
				    case Profession.Engineer:
						return TinyIconEngineer.Value;
				    case Profession.Necromancer:
						return TinyIconNecromancer.Value;
				    case Profession.Elementalist:
						return TinyIconElementalist.Value;
				    case Profession.Mesmer:
						return TinyIconMesmer.Value;
				    default:
					    throw new ArgumentOutOfRangeException(nameof(player.Profession));
			    }
		    }

			switch (player.EliteSpecialization)
			{
				case EliteSpecialization.Berserker:
					return TinyIconBerserker.Value;
				case EliteSpecialization.Spellbreaker:
					return TinyIconSpellbreaker.Value;
				case EliteSpecialization.Dragonhunter:
					return TinyIconDragonhunter.Value;
				case EliteSpecialization.Firebrand:
					return TinyIconFirebrand.Value;
				case EliteSpecialization.Herald:
					return TinyIconHerald.Value;
				case EliteSpecialization.Renegade:
					return TinyIconRenegade.Value;
				case EliteSpecialization.Druid:
					return TinyIconDruid.Value;
				case EliteSpecialization.Soulbeast:
					return TinyIconSoulbeast.Value;
				case EliteSpecialization.Daredevil:
					return TinyIconDaredevil.Value;
				case EliteSpecialization.Deadeye:
					return TinyIconDeadeye.Value;
				case EliteSpecialization.Scrapper:
					return TinyIconScrapper.Value;
				case EliteSpecialization.Holosmith:
					return TinyIconHolosmith.Value;
				case EliteSpecialization.Reaper:
					return TinyIconReaper.Value;
				case EliteSpecialization.Scourge:
					return TinyIconScourge.Value;
				case EliteSpecialization.Tempest:
					return TinyIconTempest.Value;
				case EliteSpecialization.Weaver:
					return TinyIconWeaver.Value;
				case EliteSpecialization.Chronomancer:
					return TinyIconChronomancer.Value;
				case EliteSpecialization.Mirage:
					return TinyIconMirage.Value;
				default:
					throw new ArgumentOutOfRangeException(nameof(player.EliteSpecialization));
			}
		}
	}
}
