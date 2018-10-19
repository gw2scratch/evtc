using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.GameData.Detections
{
	/// <summary>
	/// If a specific buff is applied to self, then the core specialization has to be selected.
	/// </summary>
	public class InitialBuffDetection : IDetection
	{
		public CoreSpecialization CoreSpecialization { get; }
		public int SkillId { get; }

		public InitialBuffDetection(int skillId, CoreSpecialization coreSpecialization)
		{
			SkillId = skillId;
			CoreSpecialization = coreSpecialization;
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