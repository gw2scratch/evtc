using ScratchEVTCParser.Events;
using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.GameData.Detections
{
	/// <summary>
	/// If a specific buff is applied to self, then the core specialization has to be selected.
	/// </summary>
	public class DamageWithSkillDetection : IDetection
	{
		public CoreSpecialization CoreSpecialization { get; }
		public int SkillId { get; }

		public DamageWithSkillDetection(int skillId, CoreSpecialization coreSpecialization)
		{
			SkillId = skillId;
			CoreSpecialization = coreSpecialization;
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