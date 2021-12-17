using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Processing;

public class FractalInstabilityDetector
{
	private static readonly Dictionary<int, MistlockInstability> InstabilitiesById =
		new Dictionary<int, MistlockInstability>
		{
			{ SkillIds.Afflicted, MistlockInstability.Afflicted },
			{ SkillIds.NoPainNoGain, MistlockInstability.NoPainNoGain },
			{ SkillIds.LastLaugh, MistlockInstability.LastLaugh },
			{ SkillIds.SocialAwkwardness, MistlockInstability.SocialAwkwardness },
			{ SkillIds.ToxicTrail, MistlockInstability.ToxicTrail },
			{ SkillIds.AdrenalineRush, MistlockInstability.AdrenalineRush },
			{ SkillIds.FluxBomb, MistlockInstability.FluxBomb },
			{ SkillIds.Vengeance, MistlockInstability.Vengeance },
			{ SkillIds.ToxicSickness, MistlockInstability.ToxicSickness },
			{ SkillIds.Hamstrung, MistlockInstability.Hamstrung },
			{ SkillIds.FractalVindicators, MistlockInstability.FractalVindicators },
			{ SkillIds.BoonOverload, MistlockInstability.BoonOverload },
			{ SkillIds.StickTogether, MistlockInstability.StickTogether },
			{ SkillIds.Outflanked, MistlockInstability.Outflanked },
			{ SkillIds.SugarRush, MistlockInstability.SugarRush },
			{ SkillIds.Frailty, MistlockInstability.Frailty },
			{ SkillIds.WeBleedFire, MistlockInstability.WeBleedFire },
			{ SkillIds.MistsConvergence, MistlockInstability.MistsConvergence },
			{ SkillIds.Birds, MistlockInstability.Birds },
			{ SkillIds.SlipperySlope, MistlockInstability.SlipperySlope },
		};

	public IEnumerable<MistlockInstability> GetInstabilities(Log log)
	{
		foreach (var skill in log.Skills)
		{
			if (InstabilitiesById.TryGetValue((int) skill.Id, out var instability))
			{
				yield return instability;
			}
		}
	}
}