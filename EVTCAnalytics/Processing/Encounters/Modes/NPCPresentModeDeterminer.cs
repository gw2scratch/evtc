using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;
using System.Collections.Generic;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;

/// <summary>
/// Determines encounter mode according to whether an NPC is present in the log.
/// </summary>
public class NPCPresentModeDeterminer(int speciesId, EncounterMode npcPresentMode = EncounterMode.Challenge) : IModeDeterminer
{
	public IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type>();
	public IReadOnlyList<uint> RequiredBuffSkillIds { get; } = new List<uint>();
	public IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

	public EncounterMode? GetMode(Log log)
	{
		foreach (var agent in log.Agents)
		{
			if (agent is NPC npc && npc.SpeciesId == speciesId)
			{
				return npcPresentMode;
			}
		}

		return null;
	}
}