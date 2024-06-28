using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Steps
{
	/// <summary>
	/// A post-processing step that merges NPCs that are unique in an encounter,
	/// but were split into multiple raw agents because of game limitations.
	/// </summary>
	/// <remarks>
	/// The main reason for NPCs being split is leaving the reporting range of the recording player.
	/// </remarks>
	public class MergeSingletonNPC : IPostProcessingStep
	{
		public int SpeciesId { get; }

		public MergeSingletonNPC(int speciesId)
		{
			SpeciesId = speciesId;
		}

		public void Process(LogProcessorState state)
		{
			Debug.Assert(state.Agents != null);

			var agentsToMerge = new HashSet<Agent>();
			NPC resultingAgent = null;
			foreach (var agent in state.Agents)
			{
				if (agent is NPC npc && npc.SpeciesId == SpeciesId)
				{
					agentsToMerge.Add(npc);
					if (resultingAgent == null)
					{
						// We are going to reuse the first agent. It is the one most likely
						// to have full data in the case of merging an NPC that was split because
						// of going too far from it.
						resultingAgent = npc;
					}
				}
			}

			if (agentsToMerge.Count < 2)
			{
				return;
			}

			if (resultingAgent == null)
			{
				return;
			}

			// Update agent origins to properly reflect the merge
			resultingAgent.AgentOrigin.OriginalAgentData = agentsToMerge
				.SelectMany(x => x.AgentOrigin.OriginalAgentData)
				.ToList();

			// TODO: Consider finding all Agent properties by reflection and perhaps
			// compiling expressions to effectively update those. Newly introduced events
			// with agent properties now have to be manually added here.
			foreach (var ev in state.Events)
			{
				if (ev is AgentEvent agentEvent)
				{
					if (agentsToMerge.Contains(agentEvent.Agent))
					{
						agentEvent.Agent = resultingAgent;
					}
				}

				if (ev is BuffEvent buffEvent)
				{
					if (agentsToMerge.Contains(buffEvent.SourceAgent))
					{
						buffEvent.SourceAgent = resultingAgent;
					}
				}

				if (ev is DamageEvent damageEvent)
				{
					if (agentsToMerge.Contains(damageEvent.Attacker))
					{
						damageEvent.Attacker = resultingAgent;
					}

					if (agentsToMerge.Contains(damageEvent.Defender))
					{
						damageEvent.Defender = resultingAgent;
					}
				}

				if (ev is CrowdControlEvent crowdControlEvent)
				{
					if (agentsToMerge.Contains(crowdControlEvent.Attacker))
					{
						crowdControlEvent.Attacker = resultingAgent;
					}

					if (agentsToMerge.Contains(crowdControlEvent.Defender))
					{
						crowdControlEvent.Defender = resultingAgent;
					}
				}
				
				if (ev is EffectStartEvent effectStartEvent)
				{
					if (agentsToMerge.Contains(effectStartEvent.AgentTarget))
					{
						effectStartEvent.AgentTarget = resultingAgent;
					}
				}
			}

			foreach (var agent in state.Agents)
			{
				if (agentsToMerge.Contains(agent.Master))
				{
					agent.Master = resultingAgent;
				}

				var newMinions = new List<Agent>();
				bool resultingAgentAdded = false;
				foreach (var minion in agent.MinionList)
				{
					bool toReplace = agentsToMerge.Contains(minion);
					bool isResulting = minion == resultingAgent;
					if ((toReplace || isResulting) && !resultingAgentAdded)
					{
						newMinions.Add(resultingAgent);
						resultingAgentAdded = true;
					}
					else if (!toReplace && !isResulting)
					{
						newMinions.Add(minion);
					}
				}

				agent.MinionList.Clear();
				agent.MinionList.AddRange(newMinions);
			}

			// This should not be necessary once agent definitions are reworked
			for (int i = 0; i < state.EncounterData.Targets.Count; i++)
			{
				if (agentsToMerge.Contains(state.EncounterData.Targets[i]))
				{
					state.EncounterData.Targets[i] = resultingAgent;
				}
			}

			resultingAgent.FirstAwareTime = agentsToMerge.Min(x => x.FirstAwareTime);
			resultingAgent.LastAwareTime = agentsToMerge.Max(x => x.LastAwareTime);
			
			state.Agents.RemoveAll(x => agentsToMerge.Contains(x) && x != resultingAgent);
		}
	}
}