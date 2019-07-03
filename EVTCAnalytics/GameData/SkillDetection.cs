using GW2Scratch.EVTCAnalytics.GameData.Detections;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.GameData
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