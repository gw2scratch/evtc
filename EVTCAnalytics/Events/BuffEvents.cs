using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// An event related to <see cref="Agent"/> buffs.
	/// </summary>
	/// <remarks>
	/// Note that buffs are internally <see cref="Skill"/>s in the game.
	/// </remarks>
	public abstract class BuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent)
		: AgentEvent(time, agent), ISkillEvent
	{
		public Skill Buff { get; } = buff;
		public Skill Skill { get => Buff; }
		public Agent SourceAgent { get; internal set; } = sourceAgent;
	}

	/// <summary>
	/// An event emitted when a buff is removed from an <see cref="Agent"/>. This may be one or more stacks.
	/// </summary>
	public abstract class BuffRemoveEvent(long time, Agent agent, Skill buff, Agent sourceAgent)
		: BuffEvent(time, agent, buff, sourceAgent);

	/// <summary>
	/// An event emitted when a buff is fully removed from an <see cref="Agent"/>. Individual stack removes will also be present.
	/// </summary>
	public class AllStacksRemovedBuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int stacksRemoved)
		: BuffRemoveEvent(time, agent, buff, sourceAgent)
	{
		public int StacksRemoved { get; } = stacksRemoved;
	}

	/// <summary>
	/// An event emitted when a single stack is removed from an <see cref="Agent"/>.
	/// </summary>
	public class SingleStackRemovedBuffEvent(
		long time,
		Agent agent,
		Skill buff,
		Agent sourceAgent,
		int remainingDuration,
		int remainingIntensity,
		uint stackId)
		: BuffRemoveEvent(time, agent, buff, sourceAgent)
	{
		public int RemainingDuration { get; } = remainingDuration;
		public int RemainingIntensity { get; } = remainingIntensity;
		public uint StackId { get; } = stackId;
	}

	/// <summary>
	/// An event emitted when a single stack is removed from an <see cref="Agent"/>. This event is added in cases the server does not notify.
	/// </summary>
	/// <remarks>
	/// Happens when going out of combat or stack is full, to be ignored for strip/cleanse, to be used for in/out volume.
	/// </remarks>
	public class ManualStackRemovedBuffEvent(
		long time,
		Agent agent,
		Skill buff,
		Agent sourceAgent,
		int remainingDuration,
		int remainingIntensity)
		: BuffRemoveEvent(time, agent, buff, sourceAgent)
	{
		public int RemainingDuration { get; } = remainingDuration;
		public int RemainingIntensity { get; } = remainingIntensity;
	}

	/// <summary>
	/// An event emitted when a single stack of a buff is added to an <see cref="Agent"/>.
	/// </summary>
	public class BuffApplyEvent(
		long time,
		Agent agent,
		Skill buff,
		Agent sourceAgent,
		int durationApplied,
		uint durationOfRemovedStack,
		uint instanceId,
		bool isStackActive)
		: BuffEvent(time, agent, buff, sourceAgent)
	{
		public int DurationApplied { get; } = durationApplied;
		public uint DurationOfRemovedStack { get; } = durationOfRemovedStack;
		public uint StackId { get; } = instanceId;
		public bool IsStackActive { get; } = isStackActive;
	}
	
	/// <summary>
	/// An event emitted when a single stack of a buff is added to an <see cref="Agent"/>.
	/// </summary>
	public class BuffExtensionEvent(
		long time,
		Agent agent,
		Skill buff,
		Agent sourceAgent,
		int durationChange,
		uint newDuration,
		uint instanceId,
		bool isStackActive)
		: BuffEvent(time, agent, buff, sourceAgent)
	{
		public int DurationChange { get; } = durationChange;
		public uint NewDuration { get; } = newDuration;
		public uint StackId { get; } = instanceId;
		public bool IsStackActive { get; } = isStackActive;
	}

	/// <summary>
	/// An event emitted when a stack of a buff becomes the current active one.
	/// </summary>
	public class ActiveBuffStackEvent(long time, Agent agent, uint stackId) : AgentEvent(time, agent)
	{
		public uint StackId { get; } = stackId;
	}

	/// <summary>
	/// An event emitted when the duration of a stack of a buff is changed.
	/// </summary>
	/// <remarks>
	/// This is commonly caused by buff extension skills, which typically
	/// extend the current stack.
	/// </remarks>
	public class ResetBuffStackEvent(long time, Agent agent, uint stackId, int duration)
		: AgentEvent(time, agent)
	{
		public uint StackId { get; } = stackId;
		public int Duration { get; } = duration;
	}
}