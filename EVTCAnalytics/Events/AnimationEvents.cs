using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;

namespace GW2Scratch.EVTCAnalytics.Events;

public class AnimationEvent(long time, Agent agent, Skill skill) : AgentEvent(time, agent), ISkillEvent
{
	public Skill Skill { get; } = skill;
}

public class AnimationStartEvent(long time, Agent agent,
	Agent target,
	int value,
	int control,
	uint referenceId,
	Skill skill,
	AnimationStart result
	) : AnimationEvent(time, agent, skill), ISkillEvent
{
	public Agent TargetAgent { get; set; } = target;
	public int Value { get; set; } = value;
	public int Control { get; set; } = control;
	public uint ReferenceId { get; set; } = referenceId;
	public AnimationStart Result { get; set; } = result;
}

public class AnimationEndEvent(long time, Agent agent,
	int durationAnimationScaled,
	int durationAnimation,
	Activation activation,
	AnimationStop result,
	Skill skill
	) : AnimationEvent(time, agent, skill), ISkillEvent
{
	public Agent Source { get; set; } = agent;
	public int DurationAnimationScaled { get; set; } = durationAnimationScaled;
	public int DurationAnimation { get; set; } = durationAnimation;
	public Activation Activation { get; set; } = activation;
	public AnimationStop Result { get; set; } = result;
}