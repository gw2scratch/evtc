using System;
using Eto.Drawing;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.ArcdpsLogManager
{
	public class ImageProvider
	{
		// PROFESSIONS
		private Lazy<Image> TinyIconWarrior { get; } = new Lazy<Image>(Resources.GetTinyIconWarrior);
		private Lazy<Image> TinyIconGuardian { get; } = new Lazy<Image>(Resources.GetTinyIconGuardian);
		private Lazy<Image> TinyIconRevenant { get; } = new Lazy<Image>(Resources.GetTinyIconRevenant);
		private Lazy<Image> TinyIconRanger { get; } = new Lazy<Image>(Resources.GetTinyIconRanger);
		private Lazy<Image> TinyIconThief { get; } = new Lazy<Image>(Resources.GetTinyIconThief);
		private Lazy<Image> TinyIconEngineer { get; } = new Lazy<Image>(Resources.GetTinyIconEngineer);
		private Lazy<Image> TinyIconNecromancer { get; } = new Lazy<Image>(Resources.GetTinyIconNecromancer);
		private Lazy<Image> TinyIconElementalist { get; } = new Lazy<Image>(Resources.GetTinyIconElementalist);
		private Lazy<Image> TinyIconMesmer { get; } = new Lazy<Image>(Resources.GetTinyIconMesmer);
		// SPECIALIZATIONS
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
		// CATEGORIES
		private Lazy<Image> TinyIconRaid { get; } = new Lazy<Image>(Resources.GetTinyIconRaid);
		private Lazy<Image> TinyIconFractals { get; } = new Lazy<Image>(Resources.GetTinyIconFractals);
		private Lazy<Image> TinyIconLog { get; } = new Lazy<Image>(Resources.GetTinyIconGuildRegistrar);
		private Lazy<Image> TinyIconCommander { get; } = new Lazy<Image>(Resources.GetTinyIconCommander);
		private Lazy<Image> TinyIconStrike { get; } = new Lazy<Image>(Resources.GetTinyIconStrike);
		private Lazy<Image> TinyIconTrainingArea { get; } = new Lazy<Image>(Resources.GetTinyIconTrainingArea);
		private Lazy<Image> TinyIconWorldVersusWorld { get; } = new Lazy<Image>(Resources.GetTinyIconWorldVersusWorld);
		private Lazy<Image> TinyIconUncategorized { get; } = new Lazy<Image>(Resources.GetTinyIconUncategorized);
		private Lazy<Image> TinyIconFestival { get; } = new Lazy<Image>(Resources.GetTinyIconFestival);
		// RAIDS
		private Lazy<Image> GenericRaidWing { get; } = new Lazy<Image>(Resources.GetGenericRaidWingIcon);
		// RAID BOSSES
		// WING 1
		private Lazy<Image> ValeGuardianIcon { get; } = new Lazy<Image>(Resources.GetValeGuardianIcon);
		private Lazy<Image> GorsevalIcon { get; } = new Lazy<Image>(Resources.GetGorsevalIcon);
		private Lazy<Image> SabethaIcon { get; } = new Lazy<Image>(Resources.GetSabethaIcon);
		// WING 2
		private Lazy<Image> SlothasorIcon { get; } = new Lazy<Image>(Resources.GetSlothasorIcon);
		private Lazy<Image> BanditTrioIcon { get; } = new Lazy<Image>(Resources.GetBanditTrioIcon);
		private Lazy<Image> MatthiasIcon { get; } = new Lazy<Image>(Resources.GetMatthiasIcon);
		// WING 3
		private Lazy<Image> EscortIcon { get; } = new Lazy<Image>(Resources.GetEscortIcon);
		private Lazy<Image> KeepConstructIcon { get; } = new Lazy<Image>(Resources.GetKeepConstructIcon);
		private Lazy<Image> TwistedCastleIcon { get; } = new Lazy<Image>(Resources.GetTwistedCastleIcon);
		private Lazy<Image> XeraIcon { get; } = new Lazy<Image>(Resources.GetXeraIcon);
		// WING 4
		private Lazy<Image> CairnIcon { get; } = new Lazy<Image>(Resources.GetCairnIcon);
		private Lazy<Image> MursaatOverseerIcon { get; } = new Lazy<Image>(Resources.GetMursaatOverseerIcon);
		private Lazy<Image> SamarogIcon { get; } = new Lazy<Image>(Resources.GetSamarogIcon);
		private Lazy<Image> DeimosIcon { get; } = new Lazy<Image>(Resources.GetDeimosIcon);
		// WING 5
		private Lazy<Image> SoullessHorrorIcon { get; } = new Lazy<Image>(Resources.GetDesminaIcon);
		private Lazy<Image> RiverOfSoulsIcon { get; } = new Lazy<Image>(Resources.GetRiverOfSoulsIcon);
		private Lazy<Image> BrokenKingIcon { get; } = new Lazy<Image>(Resources.GetBrokenKingIcon);
		private Lazy<Image> EaterOfSoulsIcon { get; } = new Lazy<Image>(Resources.GetEaterOfSoulsIcon);
		private Lazy<Image> EyesIcon { get; } = new Lazy<Image>(Resources.GetEyesIcon);
		private Lazy<Image> DhuumIcon { get; } = new Lazy<Image>(Resources.GetDhuumIcon);
		// WING 6
		private Lazy<Image> ConjuredAmalgamatedIcon { get; } = new Lazy<Image>(Resources.GetConjuredAmalgamatedIcon);
		private Lazy<Image> TwinLargosIcon { get; } = new Lazy<Image>(Resources.GetTwinLargosIcon);
		private Lazy<Image> QadimIcon { get; } = new Lazy<Image>(Resources.GetQadimIcon);
		// WING 7
		private Lazy<Image> CardinalAdinaIcon { get; } = new Lazy<Image>(Resources.GetCardinalAdinaIcon);
		private Lazy<Image> CardinalSabirIcon { get; } = new Lazy<Image>(Resources.GetCardinalSabirIcon);
		private Lazy<Image> QadimThePeerlessIcon { get; } = new Lazy<Image>(Resources.GetQadimThePeerlessIcon);
		// STRIKES
		private Lazy<Image> ShiverpeaksPassIcon { get; } = new Lazy<Image>(Resources.GetShiverpeaksPassIcon);
		private Lazy<Image> VoiceAndClawOfTheFallenIcon { get; } = new Lazy<Image>(Resources.GetVoiceAndClawOfTheFallenIcon);
		private Lazy<Image> FraenirOfJormagIcon { get; } = new Lazy<Image>(Resources.GetFraenirOfJormagIcon); 
		private Lazy<Image> BoneskinnerIcon { get; } = new Lazy<Image>(Resources.GetBoneskinnerIcon);
		private Lazy<Image> WhisperOfJormagIcon { get; } = new Lazy<Image>(Resources.GetWhisperOfJormagIcon);
		private Lazy<Image> VariniaStormsounderIcon { get; } = new Lazy<Image>(Resources.GetVariniaStormsounderIcon);
		// FRACTALS
		private Lazy<Image> MAMAIcon { get; } = new Lazy<Image>(Resources.GetMAMAIcon);
		private Lazy<Image> SiaxTheCorruptedIcon { get; } = new Lazy<Image>(Resources.GetSiaxTheCorruptedIcon);
		private Lazy<Image> EnsolyssOfTheEndlessTormentIcon { get; } = new Lazy<Image>(Resources.GetEnsolyssOfTheEndlessTormentIcon);
		private Lazy<Image> SkorvaldIcon { get; } = new Lazy<Image>(Resources.GetSkorvaldIcon);
		private Lazy<Image> ArtsariivIcon { get; } = new Lazy<Image>(Resources.GetArtsariivIcon);
		private Lazy<Image> ArkkIcon { get; } = new Lazy<Image>(Resources.GetArkkIcon);
		private Lazy<Image> AiKeeperOfThePeakIcon { get; } = new Lazy<Image>(Resources.GetAiKeeperOfThePeakIcon);
		// FESTIVALS
		private Lazy<Image> FreezieIcon { get; } = new Lazy<Image>(Resources.GetFreezieIcon);
		// TRAINING AREA
		private Lazy<Image> StandardKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetStandardKittyGolemIcon);
		private Lazy<Image> MediumKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetMediumKittyGolemIcon);
		private Lazy<Image> LargeKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetLargeKittyGolemIcon);
		private Lazy<Image> MassiveKittyGolemIcon { get; } = new Lazy<Image>(Resources.GetMassiveKittyGolemIcon);
		// WORLD VS WORLD
		private Lazy<Image> WorldVersusWorldIcon { get; } = new Lazy<Image>(Resources.GetWorldVersusWorldIcon);

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

		public Image GetTinyEncounterIcon(Encounter encounter)
		{
			return encounter switch {
				// RAIDS
				// W1
				Encounter.ValeGuardian => ValeGuardianIcon.Value,
				Encounter.Gorseval => GorsevalIcon.Value,
				Encounter.Sabetha => SabethaIcon.Value,
				// W2
				Encounter.Slothasor => SlothasorIcon.Value,
				Encounter.BanditTrio => BanditTrioIcon.Value,
				Encounter.Matthias => MatthiasIcon.Value,
				// W3
				Encounter.Escort => EscortIcon.Value,
				Encounter.KeepConstruct => KeepConstructIcon.Value,
				Encounter.TwistedCastle => TwistedCastleIcon.Value,
				Encounter.Xera => XeraIcon.Value,
				// W4
				Encounter.Cairn => CairnIcon.Value,
				Encounter.MursaatOverseer => MursaatOverseerIcon.Value,
				Encounter.Samarog => SamarogIcon.Value,
				Encounter.Deimos => DeimosIcon.Value,
				// W5
				Encounter.SoullessHorror => SoullessHorrorIcon.Value,
				Encounter.RiverOfSouls => RiverOfSoulsIcon.Value,
				Encounter.BrokenKing => BrokenKingIcon.Value,
				Encounter.EaterOfSouls => EaterOfSoulsIcon.Value,
				Encounter.Eyes => EyesIcon.Value,
				Encounter.Dhuum => DhuumIcon.Value,
				// W6
				Encounter.ConjuredAmalgamate => ConjuredAmalgamatedIcon.Value,
				Encounter.TwinLargos => TwinLargosIcon.Value,
				Encounter.Qadim => QadimIcon.Value,
				// W7
				Encounter.Adina => CardinalAdinaIcon.Value,
				Encounter.Sabir => CardinalSabirIcon.Value,
				Encounter.QadimThePeerless => QadimThePeerlessIcon.Value,
				// STRIKES
				Encounter.ShiverpeaksPass => ShiverpeaksPassIcon.Value,
				Encounter.VoiceAndClawOfTheFallen => VoiceAndClawOfTheFallenIcon.Value,
				Encounter.FraenirOfJormag => FraenirOfJormagIcon.Value,
				Encounter.Boneskinner => BoneskinnerIcon.Value,
				Encounter.WhisperOfJormag => WhisperOfJormagIcon.Value,
				Encounter.VariniaStormsounder => VariniaStormsounderIcon.Value,
				// FRACTALS
				Encounter.MAMA => MAMAIcon.Value,
				Encounter.SiaxTheCorrupted => SiaxTheCorruptedIcon.Value,
				Encounter.EnsolyssOfTheEndlessTorment => EnsolyssOfTheEndlessTormentIcon.Value,
				Encounter.Skorvald => SkorvaldIcon.Value,
				Encounter.Artsariiv => ArtsariivIcon.Value,
				Encounter.Arkk => ArkkIcon.Value,
				Encounter.AiKeeperOfThePeak => AiKeeperOfThePeakIcon.Value,
				// FESTIVALS
				Encounter.Freezie => FreezieIcon.Value,
				// TRAINING AREA
				Encounter.StandardKittyGolem => StandardKittyGolemIcon.Value,
				Encounter.MediumKittyGolem => MediumKittyGolemIcon.Value,
				Encounter.LargeKittyGolem => LargeKittyGolemIcon.Value,
				Encounter.MassiveKittyGolem => MassiveKittyGolemIcon.Value,
				// WORLD VS WORLD
				Encounter.WorldVersusWorld => WorldVersusWorldIcon.Value,
				_ => null
			};
		}

		public Image GetRaidWingIcon()
		{
			return GenericRaidWing.Value;
		}
	}
}