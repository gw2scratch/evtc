using System;
using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing;

namespace GW2Scratch.EVTCAnalytics.Tests.Processing.Steps
{
	public class MergeSimpletonNPCTests
	{
		public void ProcessMergesAgentEvents()
		{
			// TODO: Use reflection here
			throw new NotImplementedException();
		}

		public void ProcessRemovesAllReferencesToAgent()
		{
			// TODO: Use reflection here
			throw new NotImplementedException();
		}

		public void ProcessUpdatesEncounterDataAgents()
		{
			throw new NotImplementedException();
			/*
			var agent1 = new NPC(1, 1, "", 42, 0, 0, 0, 0, 0, 0);
			var agent2 = new NPC(2, 2, "", 1, 0, 0, 0, 0, 0, 0);
			var agent3 = new NPC(3, 3, "", 42, 0, 0, 0, 0, 0, 0);
			var logProcess
			var context = new LogProcessorContext()
			{
				Agents = new List<Agent> {
					agent1, agent2, agent3
				},
				Events = new List<Event>()
				{
					new AgentSpawnEvent(0, agent1),
					new AgentSpawnEvent(0, agent2),
					new AgentSpawnEvent(0, agent3)
				},
			}
			*/
		}

		public void ProcessUpdatesMasters()
		{
			throw new NotImplementedException();
		}

		public void ProcessUpdatesMinionLists()
		{
			throw new NotImplementedException();
		}

		public void ProcessUpdatesAwarenessTimesForMergedAgent()
		{
			throw new NotImplementedException();
		}

	}
}