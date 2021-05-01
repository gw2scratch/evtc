using System;
using Eto.Drawing;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager
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
		private Lazy<Image> TinyIconRaid { get; } = new Lazy<Image>(Resources.GetTinyIconRaid);
		private Lazy<Image> TinyIconFractals { get; } = new Lazy<Image>(Resources.GetTinyIconFractals);
		private Lazy<Image> TinyIconLog { get; } = new Lazy<Image>(Resources.GetTinyIconGuildRegistrar);
		private Lazy<Image> TinyIconCommander { get; } = new Lazy<Image>(Resources.GetTinyIconCommander);
		private Lazy<Image> TinyIconStrike { get; } = new Lazy<Image>(Resources.GetTinyIconStrike);
		private Lazy<Image> TinyIconTrainingArea { get; } = new Lazy<Image>(Resources.GetTinyIconTrainingArea);
		private Lazy<Image> TinyIconWorldVersusWorld { get; } = new Lazy<Image>(Resources.GetTinyIconWorldVersusWorld);
		private Lazy<Image> TinyIconUncategorized { get; } = new Lazy<Image>(Resources.GetTinyIconUncategorized);
		private Lazy<Image> TinyIconFestival { get; } = new Lazy<Image>(Resources.GetTinyIconFestival);
		private Lazy<Image> GenericRaidWing { get; } = new Lazy<Image>(Resources.GetGenericRaidWingIcon);
		private Lazy<Image> FreezieIcon { get; } = new Lazy<Image>(Resources.GetFreezieIcon);
		private Lazy<Image> StandardKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetStandardKittyGolemIcon);
		private Lazy<Image> MediumKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetMediumKittyGolemIcon);
		private Lazy<Image> LargeKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetLargeKittyGolemIcon);

		public Image GetTinyLogIcon() => TinyIconLog.Value;
		public Image GetTinyFractalsIcon() => TinyIconFractals.Value;
		public Image GetTinyRaidIcon() => TinyIconRaid.Value;
		public Image GetTinyCommanderIcon() => TinyIconCommander.Value;
		public Image GetTinyStrikeIcon() => TinyIconStrike.Value;
		public Image GetTinyTrainingAreaIcon() => TinyIconTrainingArea.Value;
		public Image GetTinyWorldVersusWorldIcon() => TinyIconWorldVersusWorld.Value;
		public Image GetTinyUncategorizedIcon() => TinyIconUncategorized.Value;
		public Image GetTinyFestivalIcon() => TinyIconFestival.Value;

		public Image GetTinyProfessionIcon(Profession profession)
		{
			return profession switch {
				Profession.Warrior => TinyIconWarrior.Value,
				Profession.Guardian => TinyIconGuardian.Value,
				Profession.Revenant => TinyIconRevenant.Value,
				Profession.Ranger => TinyIconRanger.Value,
				Profession.Thief => TinyIconThief.Value,
				Profession.Engineer => TinyIconEngineer.Value,
				Profession.Necromancer => TinyIconNecromancer.Value,
				Profession.Elementalist => TinyIconElementalist.Value,
				Profession.Mesmer => TinyIconMesmer.Value,
				_ => throw new ArgumentOutOfRangeException(nameof(profession)),
			};
		}

		public Image GetTinyProfessionIcon(EliteSpecialization specialization)
		{
			return specialization switch {
				EliteSpecialization.Berserker => TinyIconBerserker.Value,
				EliteSpecialization.Spellbreaker => TinyIconSpellbreaker.Value,
				EliteSpecialization.Dragonhunter => TinyIconDragonhunter.Value,
				EliteSpecialization.Firebrand => TinyIconFirebrand.Value,
				EliteSpecialization.Herald => TinyIconHerald.Value,
				EliteSpecialization.Renegade => TinyIconRenegade.Value,
				EliteSpecialization.Druid => TinyIconDruid.Value,
				EliteSpecialization.Soulbeast => TinyIconSoulbeast.Value,
				EliteSpecialization.Daredevil => TinyIconDaredevil.Value,
				EliteSpecialization.Deadeye => TinyIconDeadeye.Value,
				EliteSpecialization.Scrapper => TinyIconScrapper.Value,
				EliteSpecialization.Holosmith => TinyIconHolosmith.Value,
				EliteSpecialization.Reaper => TinyIconReaper.Value,
				EliteSpecialization.Scourge => TinyIconScourge.Value,
				EliteSpecialization.Tempest => TinyIconTempest.Value,
				EliteSpecialization.Weaver => TinyIconWeaver.Value,
				EliteSpecialization.Chronomancer => TinyIconChronomancer.Value,
				EliteSpecialization.Mirage => TinyIconMirage.Value,
				_ => throw new ArgumentOutOfRangeException(nameof(specialization)),
			};
		}

		public Image GetTinyProfessionIcon(LogPlayer player)
		{
			if (player.EliteSpecialization == EliteSpecialization.None)
			{
				return GetTinyProfessionIcon(player.Profession);
			}

			return GetTinyProfessionIcon(player.EliteSpecialization);
		}

		public Image GetTinyRaidBossIcon(Encounter encounter)
		{
			return encounter switch {
				Encounter.Freezie => FreezieIcon.Value,
				Encounter.StandardKittyGolem => StandardKittyGolemIcon.Value,
				Encounter.MediumKittyGolem => MediumKittyGolemIcon.Value,
				Encounter.LargeKittyGolem => LargeKittyGolemIcon.Value,
				_ => null
			};
		}

		public Image GetRaidWingIcon()
		{
			return GenericRaidWing.Value;
		}
	}
}