using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.RotationComparison.Statistics.RotationItems;
using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.RotationComparison.Statistics
{
	public class RotationCalculator : IRotationCalculator
	{
		private interface ITemporaryEffect
		{
			TemporaryStatus TemporaryStatus { get; }
			bool StartTriggered(AgentEvent agentEvent);
			bool EndTriggered(AgentEvent agentEvent);
		}

		private class BuffTemporaryEffect : ITemporaryEffect
		{
			private readonly int buffId;
			public TemporaryStatus TemporaryStatus { get; }

			public BuffTemporaryEffect(int buffId, TemporaryStatus temporaryStatus)
			{
				this.buffId = buffId;
				TemporaryStatus = temporaryStatus;
			}

			public bool StartTriggered(AgentEvent agentEvent)
			{
				switch (agentEvent)
				{
					case BuffApplyEvent buffApplyEvent when buffApplyEvent.Buff.Id == buffId:
					case InitialBuffEvent initialBuffEvent when initialBuffEvent.Skill.Id == buffId:
						return true;
					default:
						return false;
				}
			}

			public bool EndTriggered(AgentEvent agentEvent)
			{
				return agentEvent is BuffRemoveEvent buffRemoveEvent && buffRemoveEvent.Buff.Id == buffId;
			}
		}

		private class EventTemporaryEffect<TStarting, TEnding> : ITemporaryEffect
			where TStarting : AgentEvent
			where TEnding : AgentEvent
		{
			public TemporaryStatus TemporaryStatus { get; }

			public EventTemporaryEffect(TemporaryStatus temporaryStatus)
			{
				TemporaryStatus = temporaryStatus;
			}

			public bool StartTriggered(AgentEvent agentEvent)
			{
				return agentEvent is TStarting;
			}

			public bool EndTriggered(AgentEvent agentEvent)
			{
				return agentEvent is TEnding;
			}
		}

		private static readonly ITemporaryEffect[] TemporaryEffects =
		{
			new BuffTemporaryEffect(SkillIds.TimeAnchored, TemporaryStatus.ContinuumSplit),
			new EventTemporaryEffect<AgentDownedEvent, AgentRevivedEvent>(TemporaryStatus.Downed),
		};

		public ScratchPlayerRotation GetRotation(Log log, Player player)
		{
			var rotation = new List<RotationItem>();

			long startTime = log.StartTime.TimeMilliseconds;

			var effectStarts = new long[TemporaryEffects.Length];
			for (int i = 0; i < effectStarts.Length; i++)
			{
				effectStarts[i] = -1;
			}

			long castStart = 0;
			foreach (var logEvent in log.Events.OfType<AgentEvent>().Where(x => x.Agent == player))
			{
				long time = logEvent.Time - startTime;
				if (logEvent is StartSkillCastEvent startSkillCastEvent)
				{
					castStart = logEvent.Time - startTime;
				}
				else if (logEvent is ResetSkillCastEvent resetSkillCastEvent)
				{
					var skill = resetSkillCastEvent.Skill;
					rotation.Add(new SkillCastItem(castStart, time, SkillCastType.Reset, skill));
					castStart = logEvent.Time - startTime;
				}
				else if (logEvent is EndSkillCastEvent cancelledSkillCastEvent)
				{
					var skill = cancelledSkillCastEvent.Skill;
					var type = cancelledSkillCastEvent.EndType == EndSkillCastEvent.SkillEndType.Cancel
						? SkillCastType.Cancel
						: SkillCastType.Success;
					rotation.Add(new SkillCastItem(castStart, time, type, skill));
				}
				else if (logEvent is AgentWeaponSwapEvent weaponSwapEvent)
				{
					rotation.Add(new WeaponSwapItem(time, weaponSwapEvent.NewWeaponSet));
				}

				for (int i = 0; i < TemporaryEffects.Length; i++)
				{
					var effect = TemporaryEffects[i];
					if (effectStarts[i] != -1)
					{
						if (effect.EndTriggered(logEvent))
						{
							rotation.Add(new TemporaryStatusItem(effectStarts[i], time, effect.TemporaryStatus));
							effectStarts[i] = -1;
						}
					}
					else
					{
						if (effect.StartTriggered(logEvent))
						{
							effectStarts[i] = time;
						}
					}
				}

				// TODO: Add death
			}

			return new ScratchPlayerRotation(player, rotation);
		}
	}
}