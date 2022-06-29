using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	/// <summary>
	/// Determines encounter mode according to whether a skill is present in the log.
	/// </summary>
	public class SkillPresentModeDeterminer : IModeDeterminer
	{
		private readonly int skillId;
		private readonly EncounterMode skillPresentMode;

		public SkillPresentModeDeterminer(
			int skillId,
			EncounterMode skillPresentMode = EncounterMode.Challenge)
		{
			this.skillId = skillId;
			this.skillPresentMode = skillPresentMode;
		}

		public EncounterMode? GetMode(Log log)
		{
			foreach (var skill in log.Skills)
			{
				if (skill.Id == skillId)
				{
					return skillPresentMode;
				}
			}

			return null;
		}
	}
}