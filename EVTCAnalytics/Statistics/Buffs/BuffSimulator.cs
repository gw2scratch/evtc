using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Statistics.Buffs
{
	public class BuffSimulator
	{
		private readonly Dictionary<Skill, (BuffSimulationType type, int limit)> simulationTypeBySkill =
			new Dictionary<Skill, (BuffSimulationType type, int limit)>();

		public BuffData SimulateBuffs(IEnumerable<Agent> agents, IEnumerable<BuffEvent> events, long timeEnd)
		{
			var agentBuffData = agents.ToDictionary(a => a, a => new AgentBuffData(a, GetEmptyBuffStackCollections()));

			foreach (var buffEvent in events)
			{
				var source = buffEvent.SourceAgent;
				var agent = buffEvent.Agent;
				var buff = buffEvent.Buff;

				if (agent == null) continue;

				if (!agentBuffData.TryGetValue(agent, out var buffData))
				{
					continue;
				}

				// TODO: Make InitialBuffEvent into a BuffEvent

				if (buffEvent is BuffApplyEvent applyEvent)
				{
					var start = applyEvent.Time;
					var end = applyEvent.Time + applyEvent.DurationApplied;
					if (buffData.StackCollectionsBySkills.TryGetValue(buff, out var collection))
					{
						collection.AddStack(start, end, source);
					}
				}
				else if (buffEvent is AllStacksRemovedBuffEvent allStacksRemovedBuffEvent)
				{
					if (buffData.StackCollectionsBySkills.TryGetValue(buff, out var collection))
					{
						collection.RemoveAllStacks(allStacksRemovedBuffEvent.Time);
					}
				}
				else if (buffEvent is SingleStackRemovedBuffEvent stackRemovedBuffEvent)
				{
					if (buffData.StackCollectionsBySkills.TryGetValue(buff, out var collection))
					{
						collection.RemoveStack(stackRemovedBuffEvent.Time, stackRemovedBuffEvent.RemainingDuration);
					}
				}
			}

			foreach (var pair in agentBuffData)
			{
				var agent = pair.Key;
				var collections = pair.Value;
				foreach (var collection in collections.StackCollectionsBySkills.Values)
				{
					collection.FinalizeCollection(timeEnd);
				}
			}

			return new BuffData(agentBuffData);
		}

		private Dictionary<Skill, IBuffStackCollection> GetEmptyBuffStackCollections()
		{
			var collections = new Dictionary<Skill, IBuffStackCollection>();
			foreach (var pair in simulationTypeBySkill)
			{
				var skill = pair.Key;
				(var type, int limit) = pair.Value;
				if (type == BuffSimulationType.Intensity)
				{
					collections[skill] = new IntensityBuffStackCollection(skill, limit);
				}
				else if (type != BuffSimulationType.None)
				{
					throw new NotImplementedException();
				}
			}

			return collections;
		}

		public void TrackBuff(Skill buff, BuffSimulationType type, int limit)
		{
			if (simulationTypeBySkill.ContainsKey(buff))
			{
				throw new ArgumentException("The buff is already tracked.", nameof(buff));
			}

			simulationTypeBySkill[buff] = (type, limit);
			// TODO: use limit
		}
	}
}