using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.GameData.Detections
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