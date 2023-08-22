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
	public abstract class BuffEvent : AgentEvent
	{
		public Skill Buff { get; }
		public Agent SourceAgent { get; internal set; }

		protected BuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent) : base(time, agent)
		{
			Buff = buff;
			SourceAgent = sourceAgent;
		}
	}

	/// <summary>
	/// An event emitted when a buff is removed from an <see cref="Agent"/>. This may be one or more stacks.
	/// </summary>
	public abstract class BuffRemoveEvent : BuffEvent
	{
		protected BuffRemoveEvent(long time, Agent agent, Skill buff, Agent sourceAgent) : base(
			time, agent, buff, sourceAgent)
		{
		}
	}

	/// <summary>
	/// An event emitted when a buff is fully removed from an <see cref="Agent"/>. Individual stack removes will also be present.
	/// </summary>
	public class AllStacksRemovedBuffEvent : BuffRemoveEvent
	{
		public int StacksRemoved { get; }

		public AllStacksRemovedBuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int stacksRemoved)
			: base(time, agent, buff, sourceAgent)
		{
			StacksRemoved = stacksRemoved;
		}
	}

	/// <summary>
	/// An event emitted when a single stack is removed from an <see cref="Agent"/>.
	/// </summary>
	public class SingleStackRemovedBuffEvent : BuffRemoveEvent
	{
		public int RemainingDuration { get; }
		public int RemainingIntensity { get; }
		public uint StackId { get; }

		public SingleStackRemovedBuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent,
			int remainingDuration, int remainingIntensity, uint stackId) : base(time, agent, buff, sourceAgent)
		{
			RemainingDuration = remainingDuration;
			RemainingIntensity = remainingIntensity;
			StackId = stackId;
		}
	}

	/// <summary>
	/// An event emitted when a single stack is removed from an <see cref="Agent"/>. This event is added in cases the server does not notify.
	/// </summary>
	/// <remarks>
	/// Happens when going out of combat or stack is full, to be ignored for strip/cleanse, to be used for in/out volume.
	/// </remarks>
	public class ManualStackRemovedBuffEvent : BuffRemoveEvent
	{
		public int RemainingDuration { get; }
		public int RemainingIntensity { get; }

		public ManualStackRemovedBuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int remainingDuration,
			int remainingIntensity) : base(time, agent, buff, sourceAgent)
		{
			RemainingDuration = remainingDuration;
			RemainingIntensity = remainingIntensity;
		}
	}

	/// <summary>
	/// An event emitted when a single stack of a buff is added to an <see cref="Agent"/>.
	/// </summary>
	public class BuffApplyEvent : BuffEvent
	{
		public int DurationApplied { get; }
		public uint DurationOfRemovedStack { get; }
		public uint BuffInstanceId { get; }

		public BuffApplyEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int durationApplied,
			uint durationOfRemovedStack, uint instanceId) : base(time, agent, buff, sourceAgent)
		{
			DurationApplied = durationApplied;
			DurationOfRemovedStack = durationOfRemovedStack;
			BuffInstanceId = instanceId;
		}
	}
	
	/// <summary>
	/// An event emitted when a single stack of a buff is added to an <see cref="Agent"/>.
	/// </summary>
	public class BuffExtensionEvent : BuffEvent
	{
		public int DurationChange { get; }
		public uint NewDuration { get; }
		public uint BuffInstanceId { get; }

		public BuffExtensionEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int durationChange,
			uint newDuration, uint instanceId) : base(time, agent, buff, sourceAgent)
		{
			DurationChange = durationChange;
			NewDuration = newDuration;
			BuffInstanceId = instanceId;
		}
	}

	/// <summary>
	/// An event emitted when a stack of a buff becomes the current active one.
	/// </summary>
	public class ActiveBuffStackEvent : AgentEvent
	{
		public uint StackId { get; }

		public ActiveBuffStackEvent(long time, Agent agent, uint stackId) : base(time, agent)
		{
			StackId = stackId;
		}
	}

	/// <summary>
	/// An event emitted when the duration of a stack of a buff is changed.
	/// </summary>
	/// <remarks>
	/// This is commonly caused by buff extension skills, which typically
	/// extend the current stack.
	/// </remarks>
	public class ResetBuffStackEvent : AgentEvent
	{
		public uint StackId { get; }
		public int Duration { get; }

		public ResetBuffStackEvent(long time, Agent agent, uint stackId, int duration) : base(time, agent)
		{
			StackId = stackId;
			Duration = duration;
		}
	}
}