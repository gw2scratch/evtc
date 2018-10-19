using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.GameData.Detections
{
	/// <summary>
	/// If a specific buff is applied to self, then the core specialization has to be selected.
	/// </summary>
	public class SelfBuffApplyDetection : IDetection
	{
		public CoreSpecialization CoreSpecialization { get; }
		public int SkillId { get; }

		public SelfBuffApplyDetection(int skillId, CoreSpecialization coreSpecialization)
		{
			SkillId = skillId;
			CoreSpecialization = coreSpecialization;
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