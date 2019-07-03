using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.GameData.Detections
{
	public class SelfBuffApplyDetection : IDetection
	{
		public int SkillId { get; }

		public SelfBuffApplyDetection(int skillId)
		{
			SkillId = skillId;
		}

		public bool IsDetected(Player player, Event e)
		{
			if (e is BuffApplyEvent applyEvent)
			{
				return applyEvent.SourceAgent == player && applyEvent.Agent == player && applyEvent.Buff.Id == SkillId;
			}

			return false;
		}
	}
}