using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	/// <summary>
	/// Determines encounter mode according to whether a group of agents spawned within a time span.
	/// </summary>
	public class GroupedSpawnModeDeterminer : IModeDeterminer
	{
		private readonly Func<Agent, bool> agentCounted;
		private readonly int count;
		private readonly long timeSpan;
		private readonly EncounterMode spawnOccuredMode;

		public GroupedSpawnModeDeterminer(
			Func<Agent, bool> agentCounted,
			int count,
			long timeSpan,
			EncounterMode spawnOccuredMode = EncounterMode.Challenge)
		{
			this.agentCounted = agentCounted ?? throw new ArgumentNullException(nameof(agentCounted));
			this.count = count;
			this.timeSpan = timeSpan;
			this.spawnOccuredMode = spawnOccuredMode;
		}

		public EncounterMode? GetMode(Log log)
		{
			var countedEvents = new LinkedList<AgentSpawnEvent>();

			foreach (var current in log.Events.OfType<AgentSpawnEvent>().Where(x => agentCounted(x.Agent)))
			{
				while (countedEvents.Count > 0 && countedEvents.First.Value.Time < current.Time - timeSpan)
				{
					countedEvents.RemoveFirst();
				}

				countedEvents.AddLast(current);

				if (countedEvents.Count >= count)
				{
					return spawnOccuredMode;
				}
			}

			return null;
		}
	}
}
