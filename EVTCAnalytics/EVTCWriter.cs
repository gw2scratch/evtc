using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GW2Scratch.EVTCAnalytics.Parsed;

namespace GW2Scratch.EVTCAnalytics
{
	public class EVTCWriter
	{
		private static readonly Encoding Encoding = Encoding.UTF8;

		/// <summary>
		/// Writes a parsed log out as a revision 1 evtc log.
		/// </summary>
		/// <param name="log">The log to be written.</param>
		/// <param name="writer">The writer to write to.</param>
		public void WriteLog(ParsedLog log, BinaryWriter writer)
		{
			WriteHeader(log.LogVersion, writer);
			WriteBossData(log.ParsedBossData, writer);
			WriteAgents(log.ParsedAgents, writer);
			WriteSkills(log.ParsedSkills, writer);
			WriteCombatItems(log.ParsedCombatItems, writer);
		}

		private void WriteHeader(LogVersion version, BinaryWriter writer)
		{
			writer.Write(Encode(version.BuildVersion, 12));
			// We output revision 1 regardless of original revision
			writer.Write((byte) 1);
		}

		private void WriteBossData(ParsedBossData bossData, BinaryWriter writer)
		{
			writer.Write(bossData.ID);
			// 1 currently unused byte
			writer.Write((byte) 0);
		}

		private void WriteAgents(IReadOnlyList<ParsedAgent> agents, BinaryWriter writer)
		{
			writer.Write(agents.Count);

			foreach (var agent in agents)
			{
				writer.Write(agent.Address);
				writer.Write(agent.Prof);
				writer.Write(agent.IsElite);
				writer.Write(agent.Toughness);
				writer.Write(agent.Concentration);
				writer.Write(agent.Healing);
				writer.Write(agent.HitboxWidth);
				writer.Write(agent.Condition);
				writer.Write(agent.HitboxHeight);
				writer.Write(Encode(agent.Name, 68));
			}
		}

		private void WriteSkills(IReadOnlyList<ParsedSkill> skills, BinaryWriter writer)
		{
			writer.Write(skills.Count);

			foreach (var skill in skills) {
				writer.Write(skill.SkillId);
				writer.Write(Encode(skill.Name, 64));
			}
		}

		private void WriteCombatItems(IEnumerable<ParsedCombatItem> combatItems, BinaryWriter writer)
		{
			foreach (var combatItem in combatItems)
			{
				writer.Write(combatItem.Time);
				writer.Write(combatItem.SrcAgent);
				writer.Write(combatItem.DstAgent);
				writer.Write(combatItem.Value);
				writer.Write(combatItem.BuffDmg);
				writer.Write(combatItem.OverstackValue);
				writer.Write(combatItem.SkillId);
				writer.Write(combatItem.SrcAgentId);
				writer.Write(combatItem.DstAgentId);
				writer.Write(combatItem.SrcMasterId);
				writer.Write(combatItem.DstMasterId);
				writer.Write((byte) combatItem.Iff);
				writer.Write(combatItem.Buff);
				writer.Write((byte) combatItem.Result);
				writer.Write((byte) combatItem.IsActivation);
				writer.Write((byte) combatItem.IsBuffRemove);
				writer.Write(combatItem.IsNinety);
				writer.Write(combatItem.IsFifty);
				writer.Write(combatItem.IsMoving);
				writer.Write((byte)combatItem.IsStateChange);
				writer.Write(combatItem.IsFlanking);
				writer.Write(combatItem.IsShields);
				writer.Write(combatItem.IsOffCycle);
				writer.Write(combatItem.Padding);
			}
		}


		private byte[] Encode(string str, int bufferSize)
		{
			var buffer = new byte[bufferSize];
			int length = Math.Min(bufferSize, str.Length);
			Encoding.GetBytes(str, 0, length, buffer, 0);

			return buffer;
		}
	}
}