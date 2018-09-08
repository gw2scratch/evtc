using ScratchEVTCParser.Model;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Events
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
		public int StacksRemoved { get; }

		protected BuffRemoveEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int stacksRemoved) : base(
			time, agent, buff, sourceAgent)
		{
			StacksRemoved = stacksRemoved;
		}
	}

	public class AllStacksRemovedBuffEvent : BuffRemoveEvent
	{
		public AllStacksRemovedBuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int stacksRemoved)
			: base(time, agent, buff, sourceAgent, stacksRemoved)
		{
		}
	}

	public class SingleStackRemovedBuffEvent : BuffRemoveEvent
	{
		public int RemainingDuration { get; }
		public int RemainingIntensity { get; }

		public SingleStackRemovedBuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent,
			int remainingDuration, int remainingIntensity, int stacksRemoved) : base(time, agent, buff,
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
		public ManualSingleStackRemovedBuffEvent(long time, Agent agent, Skill buff, Agent sourceAgent,
			int remainingDuration, int remainingIntensity, int stacksRemoved) : base(time, agent, buff,
			sourceAgent, remainingDuration, remainingIntensity, stacksRemoved)
		{
		}
	}

	public class BuffApplyEvent : BuffEvent
	{
		public int DurationApplied { get; }
		public int DurationOfRemovedStack { get; }

		public BuffApplyEvent(long time, Agent agent, Skill buff, Agent sourceAgent, int durationApplied,
			int durationOfRemovedStack) : base(time, agent, buff, sourceAgent)
		{
			DurationApplied = durationApplied;
			DurationOfRemovedStack = durationOfRemovedStack;
		}
	}
}