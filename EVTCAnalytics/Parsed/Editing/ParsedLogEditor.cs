using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;

namespace GW2Scratch.EVTCAnalytics.Parsed.Editing
{
	/// <summary>
	/// Represents an editor that can work with raw parsed items from EVTC logs.
	/// </summary>
	public class ParsedLogEditor
	{
		public static readonly IReadOnlyList<byte> SupportedRevisions = new List<byte> {0, 1};

		/// <summary>
		/// Update the names of players in order to hide their identity. Also removes guild data.
		/// </summary>
		/// <param name="log">The log data that will be updated.</param>
		/// <exception cref="NotSupportedException">Thrown if the log is not a supported revision.</exception>
		public void AnonymizePlayers(ParsedLog log)
		{
			EnsureRevisionIsSupported(log.LogVersion);

			int playerIndex = 1;
			for (int i = 0; i < log.ParsedAgents.Count; i++)
			{
				var agent = log.ParsedAgents[i];
				if (agent.IsElite == 0xFFFFFFFF)
				{
					// This agent is not a player
					continue;
				}

				// The subgroup is encoded within the name, so we need to reconstruct this.
				// It is also possible that some parts may be missing in case the players in
				// the log are enemies, in that case we maintain that.
				var nameParts = agent.Name.Split(new[] {'\0'}, StringSplitOptions.RemoveEmptyEntries);
				string characterName = nameParts[0];
				string accountName = nameParts.Length > 1 ? nameParts[1] : null;
				string subgroupLiteral = nameParts.Length > 2 ? nameParts[2] : null;

				if (accountName != null)
				{
					accountName = $":Anonymous.{playerIndex:0000}";
				}

				characterName = $"Player {playerIndex}";

				string updatedName = $"{characterName}\0{accountName ?? ""}\0{subgroupLiteral ?? ""}";

				var updatedAgent = new ParsedAgent(agent.Address, updatedName, agent.Prof, agent.IsElite,
					agent.Toughness, agent.Concentration, agent.Healing, agent.Condition, agent.HitboxWidth,
					agent.HitboxHeight);

				log.ParsedAgents[i] = updatedAgent;
				playerIndex++;
			}

			for (int i = 0; i < log.ParsedCombatItems.Count; i++)
			{
				var item = log.ParsedCombatItems[i];
				if (item.IsStateChange != StateChange.Guild)
				{
					continue;
				}
				// We cannot just remove the events as that would break expectations for present for a specific log version.
				// Instead we set the guild guid to zero, which corresponds to having no guild.

				// dst, value and buffdmg have to be zeroed
				var updatedItem = new ParsedCombatItem(item.Time, item.SrcAgent, 0, 0, 0, item.OverstackValue, item.SkillId, item.SrcAgentId,
					item.DstAgentId, item.SrcMasterId, item.DstMasterId, item.Iff, item.Buff, item.Result, item.IsActivation, item.IsBuffRemove, item.IsNinety,
					item.IsFifty, item.IsMoving, item.IsStateChange, item.IsFlanking, item.IsShields, item.IsOffCycle, item.Padding);

				log.ParsedCombatItems[i] = updatedItem;
			}
		}

		/// <summary>
		/// Removes all combat items with a specific state change from a log.
		/// </summary>
		/// <param name="log">The log data that will be updated.</param>
		/// <param name="stateChange">The state change to be removed.</param>
		/// <exception cref="NotSupportedException">Thrown if the log is not a supported revision.</exception>
		public void RemoveStateChanges(ParsedLog log, StateChange stateChange)
		{
			EnsureRevisionIsSupported(log.LogVersion);

			var keptItems = new List<ParsedCombatItem>();
			foreach (var combatItem in log.ParsedCombatItems)
			{
				if (combatItem.IsStateChange != stateChange)
				{
					keptItems.Add(combatItem);
				}
			}

			log.ParsedCombatItems.Clear();
			log.ParsedCombatItems.AddRange(keptItems);
		}

		private void EnsureRevisionIsSupported(LogVersion version)
		{
			if (!SupportedRevisions.Contains(version.Revision))
			{
				// Defensively check the log revision - it is possible
				// the meaning of values may change between revisions
				throw new NotSupportedException("Only revisions 0 and and 1 of the EVTC format are supported.");
			}
		}
	}
}