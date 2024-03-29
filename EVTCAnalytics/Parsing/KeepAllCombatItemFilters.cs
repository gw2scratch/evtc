using GW2Scratch.EVTCAnalytics.Parsed.Enums;

namespace GW2Scratch.EVTCAnalytics.Parsing;

public class KeepAllCombatItemFilters : ICombatItemFilters {
	public bool IsStateChangeRequired(StateChange stateChange)
	{
		return true;
	}

	public bool IsStateChangeRequired(byte stateChange)
	{
		return true;
	}

	public bool IsBuffEventRequired(uint skillId)
	{
		return true;
	}

	public bool IsPhysicalDamageResultRequired(byte result)
	{
		return true;
	}

	public bool IsBuffDamageRequired()
	{
		return true;
	}

	public bool IsSkillCastRequired()
	{
		return true;
	}
}