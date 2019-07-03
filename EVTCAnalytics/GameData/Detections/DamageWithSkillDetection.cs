using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.GameData.Detections
{
	public class DamageWithSkillDetection : IDetection
	{
		public int SkillId { get; }

		public DamageWithSkillDetection(int skillId)
		{
			SkillId = skillId;
		}

		public bool IsDetected(Player player, Event e)
		{
			if (e is DamageEvent damageEvent)
			{
				return damageEvent.Attacker == player && damageEvent.Skill.Id == SkillId;
			}

			return false;
		}
	}
}