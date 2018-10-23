using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.GameData.Detections
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