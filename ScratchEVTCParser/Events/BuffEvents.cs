using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;

namespace ScratchEVTCParser.Events
{
	public abstract class BuffEvent : AgentEvent
	{
		public int BuffId { get; }
		public Agent SourceAgent { get; }

		protected BuffEvent(long time, Agent agent, int buffId, Agent sourceAgent) : base(time, agent)
		{
			BuffId = buffId;
			SourceAgent = sourceAgent;
		}
	}

	public abstract class BuffRemoveEvent : BuffEvent
	{
		public int StacksRemoved { get; }

		protected BuffRemoveEvent(long time, Agent agent, int buffId, Agent sourceAgent, int stacksRemoved) : base(
			time, agent, buffId, sourceAgent)
		{
			StacksRemoved = stacksRemoved;
		}
	}

	public class AllStacksRemovedBuffEvent : BuffRemoveEvent
	{
		public AllStacksRemovedBuffEvent(long time, Agent agent, int buffId, Agent sourceAgent, int stacksRemoved)
			: base(time, agent, buffId, sourceAgent, stacksRemoved)
		{
		}
	}

	public class SingleStackRemovedBuffEvent : BuffRemoveEvent
	{
		public int RemainingDuration { get; }
		public int RemainingIntensity { get; }

		public SingleStackRemovedBuffEvent(long time, Agent agent, int buffId, Agent sourceAgent,
			int remainingDuration, int remainingIntensity, int stacksRemoved) : base(time, agent, buffId,
			sourceAgent, stacksRemoved)
		{
			RemainingDuration = remainingDuration;
			RemainingIntensity = remainingIntensity;
		}
	}

	/// <summary>
	/// Happens when going out of combat or stack is full, to be ignored for strip/cleanse, to be used for in/out volume.
	/// </summary>
	public class ManualSingleStackRemovedBuffEvent : SingleStackRemovedBuffEvent
	{
		public ManualSingleStackRemovedBuffEvent(long time, Agent agent, int buffId, Agent sourceAgent,
			int remainingDuration, int remainingIntensity, int stacksRemoved) : base(time, agent, buffId,
			sourceAgent, remainingDuration, remainingIntensity, stacksRemoved)
		{
		}
	}

	public class BuffApplyEvent : BuffEvent
	{
		public int DurationApplied { get; }
		public int DurationOfRemovedStack { get; }

		public BuffApplyEvent(long time, Agent agent, int buffId, Agent sourceAgent, int durationApplied,
			int durationOfRemovedStack) : base(time, agent, buffId, sourceAgent)
		{
			DurationApplied = durationApplied;
			DurationOfRemovedStack = durationOfRemovedStack;
		}
	}
}