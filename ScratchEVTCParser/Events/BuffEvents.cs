using ScratchEVTCParser.Model;

namespace ScratchEVTCParser.Events
{
	public abstract class BuffEvent : AgentEvent
	{
		public int BuffId { get; }
		public Agent CausingAgent { get; }

		protected BuffEvent(long time, Agent agent, int buffId, Agent causingAgent) : base(time, agent)
		{
			BuffId = buffId;
			CausingAgent = causingAgent;
		}
	}

	public abstract class BuffRemoveEvent : BuffEvent
	{
		public int StacksRemoved { get; }

		protected BuffRemoveEvent(long time, Agent agent, int buffId, Agent causingAgent, int stacksRemoved) : base(
			time, agent, buffId, causingAgent)
		{
			StacksRemoved = stacksRemoved;
		}
	}

	public class AllStacksRemovedBuffEvent : BuffRemoveEvent
	{
		public AllStacksRemovedBuffEvent(long time, Agent agent, int buffId, Agent causingAgent, int stacksRemoved)
			: base(time, agent, buffId, causingAgent, stacksRemoved)
		{
		}
	}

	public class SingleStackRemovedBuffEvent : BuffRemoveEvent
	{
		public int RemainingDuration { get; }
		public int RemainingIntensity { get; }

		public SingleStackRemovedBuffEvent(long time, Agent agent, int buffId, Agent causingAgent,
			int remainingDuration, int remainingIntensity, int stacksRemoved) : base(time, agent, buffId,
			causingAgent, stacksRemoved)
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
		public ManualSingleStackRemovedBuffEvent(long time, Agent agent, int buffId, Agent causingAgent,
			int remainingDuration, int remainingIntensity, int stacksRemoved) : base(time, agent, buffId,
			causingAgent, remainingDuration, remainingIntensity, stacksRemoved)
		{
		}
	}

	public class BuffApplyEvent : BuffEvent
	{
		public int DurationApplied { get; }
		public int DurationOfRemovedStack { get; }

		public BuffApplyEvent(long time, Agent agent, int buffId, Agent causingAgent, int durationApplied,
			int durationOfRemovedStack) : base(time, agent, buffId, causingAgent)
		{
			DurationApplied = durationApplied;
			DurationOfRemovedStack = durationOfRemovedStack;
		}
	}
}