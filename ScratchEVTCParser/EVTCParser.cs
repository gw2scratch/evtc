using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ScratchEVTCParser.Parsed;
using ScratchEVTCParser.Parsed.Enums;

namespace ScratchEVTCParser
{
	// Based on the Elite Insights parser
	public class EVTCParser
	{
		public ParsedLog ParseLog(string evtcFilename)
		{
			if (evtcFilename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
			{
				using (var fileStream = new FileStream(evtcFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (var arch = new ZipArchive(fileStream, ZipArchiveMode.Read))
				using (var data = arch.Entries[0].Open())
				{
					var bytes = new byte[arch.Entries[0].Length];
					data.Read(bytes, 0, bytes.Length);
					return ParseLog(bytes);
				}
			}

			var fileBytes = File.ReadAllBytes(evtcFilename);
			return ParseLog(fileBytes);
		}

		public ParsedLog ParseLog(byte[] bytes)
		{
			var reader = new ByteArrayBinaryReader(bytes, Encoding.UTF8);

			var logVersion = ParseLogData(reader);
			var bossData = ParseBossData(reader);
			var agents = ParseAgents(reader).ToArray();
			var skills = ParseSkills(reader).ToArray();
			var combatItems = ParseCombatItems(logVersion.Revision, reader).ToArray();

			return new ParsedLog(logVersion, bossData, agents, skills, combatItems);
		}

		private LogVersion ParseLogData(ByteArrayBinaryReader reader)
		{
			// 12 bytes: arc build version
			string buildVersion = reader.ReadString(12);

			// 1 byte: revision
			byte revision = reader.ReadByte();

			return new LogVersion(buildVersion, revision);
		}

		/// <summary>
		/// Parses boss related data
		/// </summary>
		private ParsedBossData ParseBossData(ByteArrayBinaryReader reader)
		{
			// 2 bytes: boss instance ID
			ushort id = reader.ReadUInt16();
			// 1 byte: position
			byte position = reader.ReadByte(); // ignored

			//Save
			return new ParsedBossData(id);
		}

		/// <summary>
		/// Parses agent related data
		/// </summary>
		private IEnumerable<ParsedAgent> ParseAgents(ByteArrayBinaryReader reader)
		{
			// 4 bytes: player count
			int agentCount = reader.ReadInt32();

			// 96 bytes: each player
			for (int i = 0; i < agentCount; i++)
			{
				// 8 bytes: agent address
				ulong address = reader.ReadUInt64();

				// 4 bytes: profession
				uint prof = reader.ReadUInt32();

				// 4 bytes: is_elite
				uint isElite = reader.ReadUInt32();

				// 2 bytes: toughness
				int toughness = reader.ReadInt16();
				// 2 bytes: concentration
				int concentration = reader.ReadInt16();
				// 2 bytes: healing
				int healing = reader.ReadInt16();
				// 2 bytes: hb_width
				int hitboxWidth = reader.ReadInt16();
				// 2 bytes: condition
				int condition = reader.ReadInt16();
				// 2 bytes: hb_height
				int hitboxHeight = reader.ReadInt16();
				// 68 bytes: name
				String name = reader.ReadString(68);

				ParsedAgent parsedAgent = new ParsedAgent(address, name, prof, isElite, toughness, concentration,
					healing, condition,
					hitboxWidth, hitboxHeight);

				yield return parsedAgent;
			}
		}

		/// <summary>
		/// Parses skill related data
		/// </summary>
		private IEnumerable<ParsedSkill> ParseSkills(ByteArrayBinaryReader reader)
		{
			// 4 bytes: player count
			int skillCount = reader.ReadInt32();

			// 68 bytes: each skill
			for (int i = 0; i < skillCount; i++)
			{
				// 4 bytes: skill ID
				int skillId = reader.ReadInt32();

				// 64 bytes: name
				var name = reader.ReadString(64);

				var skill = new ParsedSkill(skillId, name);
				yield return skill;
			}
		}

		/// <summary>
		/// Parses combat related data
		/// </summary>
		private IEnumerable<ParsedCombatItem> ParseCombatItems(int revision, ByteArrayBinaryReader reader)
		{
			switch (revision)
			{
				case 0:
					// 64 bytes: each combat item
					while (reader.Length - reader.Position >= 64)
					{
						ParsedCombatItem combatItem = ReadCombatItemRevision0(reader);
						yield return combatItem;
					}

					break;
				case 1:
					// 64 bytes: each combat item
					while (reader.Length - reader.Position >= 64)
					{
						ParsedCombatItem combatItem = ReadCombatItemRevision1(reader);
						yield return combatItem;
					}

					break;
				default:
					throw new NotSupportedException("Only EVTC revisions 0 and 1 are supported.");
			}
		}

		private static ParsedCombatItem ReadCombatItemRevision0(ByteArrayBinaryReader reader)
		{
			// 8 bytes: time
			long time = reader.ReadInt64();

			// 8 bytes: src_agent
			ulong srcAgent = reader.ReadUInt64();

			// 8 bytes: dst_agent
			ulong dstAgent = reader.ReadUInt64();

			// 4 bytes: value
			int value = reader.ReadInt32();

			// 4 bytes: buff_dmg
			int buffDmg = reader.ReadInt32();

			// 2 bytes: overstack_value
			ushort overstackValue = reader.ReadUInt16();

			// 2 bytes: skill_id
			ushort skillId = reader.ReadUInt16();

			// 2 bytes: src_instid
			ushort srcInstid = reader.ReadUInt16();

			// 2 bytes: dst_instid
			ushort dstInstid = reader.ReadUInt16();

			// 2 bytes: src_master_instid
			ushort srcMasterInstid = reader.ReadUInt16();

			// 9 bytes: garbage
			reader.Skip(9);

			// 1 byte: iff
			FriendOrFoe iff = GetFriendOrFoeFromByte(reader.ReadByte());

			// 1 byte: buff
			byte buff = reader.ReadByte();

			// 1 byte: result
			Result result = GetResultFromByte(reader.ReadByte());

			// 1 byte: is_activation
			Activation isActivation = GetActivationFromByte(reader.ReadByte());

			// 1 byte: is_buffremove
			BuffRemove isBuffRemove = GetBuffRemoveFromByte(reader.ReadByte());

			// 1 byte: is_ninety
			byte isNinety = reader.ReadByte();

			// 1 byte: is_fifty
			byte isFifty = reader.ReadByte();

			// 1 byte: is_moving
			byte isMoving = reader.ReadByte();

			// 1 byte: is_statechange
			StateChange isStateChange = GetStateChangeFromByte(reader.ReadByte());

			// 1 byte: is_flanking
			byte isFlanking = reader.ReadByte();

			// 1 byte: is_shields
			byte isShields = reader.ReadByte();

			// 1 byte: is_offcycle
			byte isOffcycle = reader.ReadByte();

			// 1 byte: garbage
			reader.Skip(1);

			//save
			// Add combat
			return new ParsedCombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId,
				srcInstid, dstInstid, srcMasterInstid, 0, iff, buff, result, isActivation, isBuffRemove,
				isNinety, isFifty, isMoving, isStateChange, isFlanking, isShields, isOffcycle);
		}

		private static ParsedCombatItem ReadCombatItemRevision1(ByteArrayBinaryReader reader)
		{
			// 8 bytes: time
			long time = reader.ReadInt64();

			// 8 bytes: src_agent
			ulong srcAgent = reader.ReadUInt64();

			// 8 bytes: dst_agent
			ulong dstAgent = reader.ReadUInt64();

			// 4 bytes: value
			int value = reader.ReadInt32();

			// 4 bytes: buff_dmg
			int buffDmg = reader.ReadInt32();

			// 4 bytes: overstack_value
			uint overstackValue = reader.ReadUInt32();

			// 4 bytes: skill_id
			uint skillId = reader.ReadUInt32();

			// 2 bytes: src_instid
			ushort srcInstid = reader.ReadUInt16();

			// 2 bytes: dst_instid
			ushort dstInstid = reader.ReadUInt16();

			// 2 bytes: src_master_instid
			ushort srcMasterInstid = reader.ReadUInt16();

			// 2 bytes: dst_master_instid
			ushort dstMasterInstid = reader.ReadUInt16();

			// 1 byte: iff
			FriendOrFoe iff = GetFriendOrFoeFromByte(reader.ReadByte());

			// 1 byte: buff
			byte buff = reader.ReadByte();

			// 1 byte: result
			Result result = GetResultFromByte(reader.ReadByte());

			// 1 byte: is_activation
			Activation isActivation = GetActivationFromByte(reader.ReadByte());

			// 1 byte: is_buffremove
			BuffRemove isBuffRemove = GetBuffRemoveFromByte(reader.ReadByte());

			// 1 byte: is_ninety
			byte isNinety = reader.ReadByte();

			// 1 byte: is_fifty
			byte isFifty = reader.ReadByte();

			// 1 byte: is_moving
			byte isMoving = reader.ReadByte();

			// 1 byte: is_statechange
			StateChange isStateChange = GetStateChangeFromByte(reader.ReadByte());

			// 1 byte: is_flanking
			byte isFlanking = reader.ReadByte();

			// 1 byte: is_shields
			byte isShields = reader.ReadByte();

			// 1 byte: is_offcycle
			byte isOffcycle = reader.ReadByte();

			// 4 bytes: padding
			reader.Skip(4);

			return new ParsedCombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId,
				srcInstid, dstInstid, srcMasterInstid, dstMasterInstid, iff, buff, result, isActivation, isBuffRemove,
				isNinety, isFifty, isMoving, isStateChange, isFlanking, isShields, isOffcycle);
		}

		private static FriendOrFoe GetFriendOrFoeFromByte(byte b)
		{
			return b < (byte) FriendOrFoe.Unknown ? (FriendOrFoe) b : FriendOrFoe.Unknown;
		}

		private static Result GetResultFromByte(byte b)
		{
			// Does not perform checking as that would change the result value which is used in buff remove events to indicate remaining stacks
			return (Result) b;
		}

		private static BuffRemove GetBuffRemoveFromByte(byte b)
		{
			return b <= 3 ? (BuffRemove) b : BuffRemove.None;
		}

		private static Activation GetActivationFromByte(byte b)
		{
			return b < (byte) Activation.Unknown ? (Activation) b : Activation.Unknown;
		}

		private static StateChange GetStateChangeFromByte(byte b)
		{
			return b < (byte) StateChange.Unknown ? (StateChange) b : StateChange.Unknown;
		}
	}
}