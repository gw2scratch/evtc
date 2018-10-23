using ScratchEVTCParser.GameData.Detections;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.GameData
{
	public class SkillDetection
	{
		public IDetection Detection { get; }
		public int SkillId { get; }
		public SkillSlot Slot { get; }

		public SkillDetection(IDetection detection, int skillId, SkillSlot slot)
		{
			Detection = detection;
			SkillId = skillId;
			Slot = slot;
		}
	}
}