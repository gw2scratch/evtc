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
	public class EVTCParser
	{
		public ParsedLog ParseLog(string evtcFilename)
		{
			using (var fileStream = new FileStream(evtcFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				if (evtcFilename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					using (var arch = new ZipArchive(fileStream, ZipArchiveMode.Read))
					{
						using (var data = arch.Entries[0].Open())
						{
							return ParseLog(data);
						}
					}
				}

				return ParseLog(fileStream);
			}
		}


		private ParsedLog ParseLog(Stream stream)
		{
            LogVersion logVersion;
            ParsedBossData bossData;
            ParsedAgent[] agents;
            ParsedSkill[] skills;
			ParsedCombatItem[] combatItems;

			using (var reader = new BinaryReader(stream, Encoding.UTF8))
			{
				logVersion = ParseLogData(reader);
				bossData = ParseBossData(reader);
				agents = ParseAgents(reader).ToArray();
				skills = ParseSkills(reader).ToArray();
				combatItems = ParseCombatItems(stream).ToArray();
			}

			return new ParsedLog(logVersion, bossData, agents, skills, combatItems);
		}

		private static bool TryRead(Stream stream, byte[] data)
		{
			int offset = 0;
			int count = data.Length;
			while (count > 0)
			{
				var bytesRead = stream.Read(data, offset, count);
				if (bytesRead == 0)
				{
					return false;
				}

				offset += bytesRead;
				count -= bytesRead;
			}

			return true;
		}

		private static string ReadUTF8String(BinaryReader reader, int length)
		{
			return reader.BaseStream.ReadString(length, Encoding.UTF8, false);
		}

		private LogVersion ParseLogData(BinaryReader reader)
		{
			// 12 bytes: arc build version
			string buildVersion = ReadUTF8String(reader, 12);

			// 1 byte: revision
			byte revision = reader.ReadByte();

			return new LogVersion(buildVersion, revision);
		}

		/// <summary>
		/// Parses boss related data
		/// </summary>
		private ParsedBossData ParseBossData(BinaryReader reader)
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
		private IEnumerable<ParsedAgent> ParseAgents(BinaryReader reader)
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
				String name = ReadUTF8String(reader, 68);

				ParsedAgent parsedAgent = new ParsedAgent(address, name, prof, isElite, toughness, concentration, healing, condition,
					hitboxWidth, hitboxHeight);

				yield return parsedAgent;
			}
		}

		/// <summary>
		/// Parses skill related data
		/// </summary>
		private IEnumerable<ParsedSkill> ParseSkills(BinaryReader reader)
		{
			// 4 bytes: player count
			int skillCount = reader.ReadInt32();

			// 68 bytes: each skill
			for (int i = 0; i < skillCount; i++)
			{
				// 4 bytes: skill ID
				int skillId = reader.ReadInt32();

				// 64 bytes: name
				var name = ReadUTF8String(reader, 64);

				var skill = new ParsedSkill(skillId, name);
				yield return skill;
			}
		}

		private static FriendOrFoe GetFriendOrFoeFromByte(byte b)
		{
			return b < (byte) FriendOrFoe.Unknown ? (FriendOrFoe) b : FriendOrFoe.Unknown;
		}

		private static Result GetResultFromByte(byte b)
		{
			return b < (byte) Result.Unknown ? (Result) b : Result.Unknown;
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
            return b < (byte)StateChange.Unknown ? (StateChange)b : StateChange.Unknown;
        }

		private static ParsedCombatItem ReadCombatItem(BinaryReader reader)
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
			reader.BaseStream.SafeSkip(9);

			// 1 byte: iff
			FriendOrFoe iff = GetFriendOrFoeFromByte(reader.ReadByte());

			// 1 byte: buff
			ushort buff = reader.ReadByte();

			// 1 byte: result
			Result result = GetResultFromByte(reader.ReadByte());

			// 1 byte: is_activation
			Activation isActivation = GetActivationFromByte(reader.ReadByte());

			// 1 byte: is_buffremove
			BuffRemove isBuffRemove = GetBuffRemoveFromByte(reader.ReadByte());

			// 1 byte: is_ninety
			ushort isNinety = reader.ReadByte();

			// 1 byte: is_fifty
			ushort isFifty = reader.ReadByte();

			// 1 byte: is_moving
			ushort isMoving = reader.ReadByte();

			// 1 byte: is_statechange
			StateChange isStateChange = GetStateChangeFromByte(reader.ReadByte());

			// 1 byte: is_flanking
			ushort isFlanking = reader.ReadByte();

			// 1 byte: is_flanking
			ushort isShields = reader.ReadByte();
			// 2 bytes: garbage
			reader.BaseStream.SafeSkip(2);

			//save
			// Add combat
			return new ParsedCombatItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId,
				srcInstid, dstInstid, srcMasterInstid, iff, buff, result, isActivation, isBuffRemove,
				isNinety, isFifty, isMoving, isStateChange, isFlanking, isShields);
		}

		/// <summary>
		/// Parses combat related data
		/// </summary>
		private IEnumerable<ParsedCombatItem> ParseCombatItems(Stream stream)
		{
			// 64 bytes: each combat
			var data = new byte[64];
			using (var ms = new MemoryStream(data, writable: false))
			using (var reader = new BinaryReader(ms))
			{
				while (true)
				{
					if (!TryRead(stream, data)) break;
					ms.Seek(0, SeekOrigin.Begin);
					ParsedCombatItem combatItem = ReadCombatItem(reader);
					yield return combatItem;
				}
			}
		}
	}
}