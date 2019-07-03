using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.GameData.Detections
{
	public class InitialBuffDetection : IDetection
	{
		public int SkillId { get; }

		public InitialBuffDetection(int skillId)
		{
			SkillId = skillId;
		}

		public bool IsDetected(Player player, Event e)
		{
			if (e is InitialBuffEvent buffEvent)
			{
				return buffEvent.Agent == player && buffEvent.Skill.Id == SkillId;
			}

			return false;
		}
	}
}