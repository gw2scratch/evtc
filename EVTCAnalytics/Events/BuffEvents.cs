using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Events
{
	public abstract class BuffEvent : AgentEvent
	{
		public Skill Buff { get; }
		public Agent SourceAgent { get; }

		protected BuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent) : base(time, agent)
		{
			Buff = buff;
			SourceAgent = sourceAgent;
		}
	}

	public abstract class BuffRemoveEvent : BuffEvent
	{
		protected BuffRemoveEvent(long time, Agent agent, Skill buff, Agent sourceAgent) : base(
			time, agent, buff, sourceAgent)
		{
		}
	}

	public class AllStacksRemovedBuffEvent : BuffRemoveEvent
	{
		public int StacksRemoved { get; }

		public AllStacksRemovedBuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int stacksRemoved)
			: base(time, agent, buff, sourceAgent)
		{
			StacksRemoved = stacksRemoved;
		}
	}

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
	/// Happens when going out of combat or stack is full, to be ignored for strip/cleanse, to be used for in/out volume.
	/// </summary>
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

	public class BuffApplyEvent : BuffEvent
	{
		public int DurationApplied { get; }
		public uint DurationOfRemovedStack { get; }

		public BuffApplyEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int durationApplied,
			uint durationOfRemovedStack) : base(time, agent, buff, sourceAgent)
		{
			DurationApplied = durationApplied;
			DurationOfRemovedStack = durationOfRemovedStack;
		}
	}

	public class ActiveBuffStackEvent : AgentEvent
	{
		public uint StackId { get; }

		public ActiveBuffStackEvent(long time, Agent agent, uint stackId) : base(time, agent)
		{
			StackId = stackId;
		}
	}

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