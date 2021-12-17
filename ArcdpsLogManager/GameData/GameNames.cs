using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;

namespace GW2Scratch.ArcdpsLogManager.GameData
{
	public static class GameNames
	{
		/// <summary>
		/// Provides the English name of a Profession.
		/// </summary>
		public static string GetName(Profession profession)
		{
			return profession switch
			{
				Profession.Guardian => "Guardian",
				Profession.Warrior => "Warrior",
				Profession.Engineer => "Engineer",
				Profession.Ranger => "Ranger",
				Profession.Thief => "Thief",
				Profession.Elementalist => "Elementalist",
				Profession.Mesmer => "Mesmer",
				Profession.Necromancer => "Necromancer",
				Profession.Revenant => "Revenant",
				Profession.None => "Unknown Profession",
				_ => throw new ArgumentOutOfRangeException(nameof(profession), profession, null)
			};
		}

		/// <summary>
		/// Provides the English name of an Elite Specialization.
		/// </summary>
		public static string GetName(EliteSpecialization specialization)
		{
			return specialization switch
			{
				EliteSpecialization.None => "None",
				EliteSpecialization.Dragonhunter => "Dragonhunter",
				EliteSpecialization.Berserker => "Berserker",
				EliteSpecialization.Scrapper => "Scrapper",
				EliteSpecialization.Druid => "Druid",
				EliteSpecialization.Daredevil => "Daredevil",
				EliteSpecialization.Tempest => "Tempest",
				EliteSpecialization.Chronomancer => "Chronomancer",
				EliteSpecialization.Reaper => "Reaper",
				EliteSpecialization.Herald => "Herald",
				EliteSpecialization.Soulbeast => "Soulbeast",
				EliteSpecialization.Weaver => "Weaver",
				EliteSpecialization.Holosmith => "Holosmith",
				EliteSpecialization.Deadeye => "Deadeye",
				EliteSpecialization.Mirage => "Mirage",
				EliteSpecialization.Scourge => "Scourge",
				EliteSpecialization.Spellbreaker => "Spellbreaker",
				EliteSpecialization.Firebrand => "Firebrand",
				EliteSpecialization.Renegade => "Renegade",
				EliteSpecialization.Willbender => "Willbender",
				EliteSpecialization.Bladesworn => "Bladesworn",
				EliteSpecialization.Mechanist => "Mechanist",
				EliteSpecialization.Untamed => "Untamed",
				EliteSpecialization.Specter => "Specter",
				EliteSpecialization.Catalyst => "Catalyst",
				EliteSpecialization.Virtuoso => "Virtuoso",
				EliteSpecialization.Harbinger => "Harbinger",
				EliteSpecialization.Vindicator => "Vindicator",
				_ => throw new ArgumentOutOfRangeException(nameof(specialization), specialization, null)
			};
		}

		/// <summary>
		/// Provides the English name of the Elite Specialization of a player, or the core Profession name
		/// if no Elite Specialization is used.
		/// </summary>
		/// <remarks>
		/// The returned name explicitly mentions that a profession is not elite.
		/// </remarks>
		public static string GetSpecializationName(LogPlayer player)
		{
			if (player.EliteSpecialization == EliteSpecialization.None)
			{
				return $"Core {GetName(player.Profession)}";
			}

			return GetName(player.EliteSpecialization);
		}

		/// <summary>
		/// Provides the English name of a Mistlock Instability.
		/// </summary>
		public static string GetInstabilityName(MistlockInstability instability)
		{
			return instability switch
			{
				MistlockInstability.AdrenalineRush => "Adrenaline Rush",
				MistlockInstability.Afflicted => "Afflicted",
				MistlockInstability.BoonOverload => "Boon Overload",
				MistlockInstability.FluxBomb => "Flux Bomb",
				MistlockInstability.FractalVindicators => "Fractal Vindicators",
				MistlockInstability.Frailty => "Frailty",
				MistlockInstability.Hamstrung => "Hamstrung",
				MistlockInstability.LastLaugh => "Last Laugh",
				MistlockInstability.MistsConvergence => "Mists Convergence",
				MistlockInstability.NoPainNoGain => "No Pain, No Gain",
				MistlockInstability.Outflanked => "Outflanked",
				MistlockInstability.SocialAwkwardness => "Social Awkwardness",
				MistlockInstability.StickTogether => "Stick Together",
				MistlockInstability.SugarRush => "Sugar Rush",
				MistlockInstability.ToxicSickness => "Toxic Sickness",
				MistlockInstability.ToxicTrail => "Toxic Trail",
				MistlockInstability.Vengeance => "Vengeance",
				MistlockInstability.WeBleedFire => "We Bleed Fire",
				MistlockInstability.Birds => "Birds",
				MistlockInstability.SlipperySlope => "Slippery Slope",
				_ => throw new ArgumentOutOfRangeException(nameof(instability), instability, null)
			};
		}
	}
}