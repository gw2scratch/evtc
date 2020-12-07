using System.Linq;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Health
{
	/// <summary>
	/// This is a health determiner that adds a specified health fraction to the resulting health if a skill is present in the log.
	/// </summary>
	public class ExtraHealthIfSkillPresentHealthDeterminer : MaxMinHealthDeterminer
	{
		private readonly int skillId;
		private readonly float extraHealth;

		public ExtraHealthIfSkillPresentHealthDeterminer(int skillId, float extraHealth)
		{
			this.skillId = skillId;
			this.extraHealth = extraHealth;
		}

		public override float? GetMainEnemyHealthFraction(Log log)
		{
			var health = base.GetMainEnemyHealthFraction(log);

			bool skillPresent = log.Skills.Any(x => x.Id == skillId);
			if (!skillPresent && health != null)
			{
				health += extraHealth;
			}

			return health;
		}
	}
}