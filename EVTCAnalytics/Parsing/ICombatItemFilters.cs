using GW2Scratch.EVTCAnalytics.Parsed.Enums;

namespace GW2Scratch.EVTCAnalytics.Parsing;

public interface ICombatItemFilters
{
	public bool IsStateChangeRequired(StateChange stateChange);
	public bool IsStateChangeRequired(byte stateChange);
	public bool IsBuffEventRequired(uint skillId);
	public bool IsPhysicalDamageResultRequired(byte result);
	public bool IsBuffDamageRequired();
	public bool IsSkillCastRequired();
}