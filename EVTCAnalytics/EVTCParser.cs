using GW2Scratch.EVTCAnalytics.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using GW2Scratch.EVTCAnalytics.IO;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;
using GW2Scratch.EVTCAnalytics.Processing;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GW2Scratch.EVTCAnalytics
{
	/// <summary>
	/// Reads raw data from an EVTC log and builds a <see cref="ParsedLog"/> with the raw data.
	/// </summary>
	public class EVTCParser
	{
		internal interface IAgentReader
		{
			bool Exhausted { get; }
			bool GetNext(out ParsedAgent agent);
		}

		internal class AgentReader : IAgentReader
		{
			public bool Exhausted { get; private set; }
			
			private readonly ByteArrayBinaryReader reader;
			private readonly int agentCount;
			private int position;

			public AgentReader(ByteArrayBinaryReader reader, int agentCount)
			{
				this.reader = reader;
				this.agentCount = agentCount;
				this.position = 0;
			}

			public bool GetNext(out ParsedAgent agent)
			{
				if (position < agentCount)
				{
					ParseAgent(reader, out agent);
					position += 1;
					return true;
				}

				agent = default;
				Exhausted = true;
				return false;
			}
		}
		
		internal class AgentReaderStream : IAgentReader
		{
			public bool Exhausted { get; private set; }
			
			private readonly Stream stream;
			private readonly int agentCount;
			private int position;

			public AgentReaderStream(Stream stream, int agentCount)
			{
				this.stream = stream;
				this.agentCount = agentCount;
				this.position = 0;
			}

			public bool GetNext(out ParsedAgent agent)
			{
				if (position < agentCount)
				{
					ParseAgent(stream, out agent);
					position += 1;
					return true;
				}

				agent = default;
				Exhausted = true;
				return false;
			}
		}

		internal interface ISkillReader
		{
			bool Exhausted { get; }
			bool GetNext(out ParsedSkill skill);
		}

		internal class SkillReader : ISkillReader
		{
			public bool Exhausted { get; private set; }
			
			private readonly ByteArrayBinaryReader reader;
			private readonly int skillCount;
			private int position;

			public SkillReader(ByteArrayBinaryReader reader, int skillCount)
			{
				this.reader = reader;
				this.skillCount = skillCount;
				this.position = 0;
			}

			public bool GetNext(out ParsedSkill skill)
			{
				if (position < skillCount)
				{
					ParseSkill(reader, out skill);
					position += 1;
					return true;
				}

				skill = default;
				Exhausted = true;
				return false;
			}
		}
		
		internal class SkillReaderStream : ISkillReader
		{
			public bool Exhausted { get; private set; }
			
			private readonly Stream stream;
			private readonly int skillCount;
			private int position;

			public SkillReaderStream(Stream stream, int skillCount)
			{
				this.stream = stream;
				this.skillCount = skillCount;
				this.position = 0;
			}

			public bool GetNext(out ParsedSkill skill)
			{
				if (position < skillCount)
				{
					ParseSkill(stream, out skill);
					position += 1;
					return true;
				}

				skill = default;
				Exhausted = true;
				return false;
			}
		}

		internal interface ICombatItemReader
		{
			public bool GetNext(out ParsedCombatItem combatItem);
			public bool Exhausted { get; }
		}

		private class CombatItemReaderRevision0 : ICombatItemReader
		{
			public bool Exhausted { get; private set; }
			
			private readonly ByteArrayBinaryReader reader;

			public CombatItemReaderRevision0(ByteArrayBinaryReader reader)
			{
				this.reader = reader;
			}

			public bool GetNext(out ParsedCombatItem combatItem)
			{
				if (reader.Length - reader.Position >= 64)
				{
					ReadCombatItemRevision0(reader, out combatItem);
					return true;
				}

				combatItem = default;
				Exhausted = true;
				return false;
			}
		}

		private class CombatItemReaderRevision1 : ICombatItemReader
		{
			public bool Exhausted { get; private set; }
			
			private readonly ByteArrayBinaryReader reader;

			public CombatItemReaderRevision1(ByteArrayBinaryReader reader)
			{
				this.reader = reader;
			}

			public bool GetNext(out ParsedCombatItem combatItem)
			{
				if (reader.Length - reader.Position >= 64)
				{
					// 64 bytes: combat item
					ReadCombatItemRevision1(reader, out combatItem);

					return true;
				}

				combatItem = default;
				Exhausted = true;
				return false;
			}
		}
		
		private class CombatItemReaderRevision1Stream : ICombatItemReader
		{
			public bool Exhausted { get; private set; }
			
			private readonly Stream stream;

			public CombatItemReaderRevision1Stream(Stream stream)
			{
				this.stream = stream;
			}

			public bool GetNext(out ParsedCombatItem combatItem)
			{
				// 64 bytes: combat item
				if (ReadCombatItemRevision1(stream, out combatItem))
				{
					return true;
				}

				combatItem = default;
				Exhausted = true;
				return false;
			}
		}

		internal interface ISinglePassEVTCReader : IDisposable
		{
			LogVersion ReadLogVersion();
			ParsedBossData ReadBossData();
			IAgentReader GetAgentReader();
			ISkillReader GetSkillReader();
			ICombatItemReader GetCombatItemReader();
		}

		/// <summary>
		/// Allows reading the EVTC file in a single pass effectively.
		/// The methods have to be called in the correct order (see remarks).
		/// </summary>
		/// <remarks>
		/// The correct order of calling methods is as follows:
		/// <list type="number">
		/// <item><see cref="ReadLogVersion" /></item>
		/// <item><see cref="ReadBossData" /></item>
		/// <item><see cref="GetAgentReader" /></item>
		/// <item><see cref="GetSkillReader" /></item>
		/// <item><see cref="GetCombatItemReader" /></item>
		/// </list>
		/// <para>
		/// Note that all readers have to be fully exhausted before calling the next method.
		/// </para>
		/// </remarks>
		internal class SinglePassEVTCReader : ISinglePassEVTCReader
		{
			enum ReaderPosition
			{
				BeforeVersion,
				BeforeBossData,
				BeforeAgents,
				InAgents,
				InSkills,
				InCombatItems,
			}

			private readonly EVTCParser parser;
			private readonly ByteArrayBinaryReader reader;
			private ReaderPosition position;
			private AgentReader agentReader;
			private SkillReader skillReader;
			private ICombatItemReader combatItemReader;
			private int revision;

			public SinglePassEVTCReader(EVTCParser parser, ByteArrayBinaryReader reader)
			{
				if (reader.Position != 0)
				{
					throw new InvalidOperationException("The reader must be at the very start.");
				}

				this.parser = parser;
				this.reader = reader;
				position = ReaderPosition.BeforeVersion;
			}

			public LogVersion ReadLogVersion()
			{
				if (position != ReaderPosition.BeforeVersion)
				{
					throw new InvalidOperationException("ReadLogVersion must be called when the reader is positioned before the log version.");
				}

				var version = parser.ParseLogVersion(reader);
				position = ReaderPosition.BeforeBossData;
				revision = version.Revision;
				return version;
			}

			public ParsedBossData ReadBossData()
			{
				if (position != ReaderPosition.BeforeBossData)
				{
					throw new InvalidOperationException("ReadBossData must be called when the reader is positioned just before the boss data.");
				}

				var bossData = parser.ParseBossData(reader);
				position = ReaderPosition.BeforeAgents;
				return bossData;
			}
			
			public IAgentReader GetAgentReader()
			{
				if (position != ReaderPosition.BeforeAgents)
				{
					throw new InvalidOperationException("GetAgentReader must be called when the reader is positioned just before the agent data.");
				}

				if (agentReader != null)
				{
					throw new InvalidOperationException("GetAgentReader must only be called once.");
				}
				
				int agentCount = reader.ReadInt32();
				position = ReaderPosition.InAgents;
				agentReader = new AgentReader(reader, agentCount);
				return agentReader;
			}
			
			public ISkillReader GetSkillReader()
			{
				if (position != ReaderPosition.InAgents)
				{
					throw new InvalidOperationException("GetSkillReader must be called directly after reading agent data.");
				}
				
				if (skillReader != null)
				{
					throw new InvalidOperationException("GetSkillReader must only be called once.");
				}
				
				if (!agentReader.Exhausted)
				{
					throw new InvalidOperationException("The agent reader obtained previously must be exhausted fully.");
				}

				int skillCount = reader.ReadInt32();
				position = ReaderPosition.InSkills;
				skillReader = new SkillReader(reader, skillCount);
				return skillReader;
			}
			
			public ICombatItemReader GetCombatItemReader()
			{
				if (position != ReaderPosition.InSkills)
				{
					throw new InvalidOperationException("GetCombatItemReader must be called directly after reading skill data.");
				}
				
				if (combatItemReader != null)
				{
					throw new InvalidOperationException("GetCombatItemReader must only be called once.");
				}
				
				if (!skillReader.Exhausted)
				{
					throw new InvalidOperationException("The skill reader obtained previously must be exhausted fully.");
				}

				position = ReaderPosition.InCombatItems;
				combatItemReader = revision switch
				{
					0 => new CombatItemReaderRevision0(reader),
					1 => new CombatItemReaderRevision1(reader),
					_ => throw new NotSupportedException("Only EVTC revisions 0 and 1 are supported.")
				};

				return combatItemReader;
			}

			public void Dispose()
			{
			}
		}
		
		/// <summary>
		/// Allows reading the EVTC file in a single pass effectively.
		/// The methods have to be called in the correct order (see remarks).
		/// </summary>
		/// <remarks>
		/// The correct order of calling methods is as follows:
		/// <list type="number">
		/// <item><see cref="ReadLogVersion" /></item>
		/// <item><see cref="ReadBossData" /></item>
		/// <item><see cref="GetAgentReader" /></item>
		/// <item><see cref="GetSkillReader" /></item>
		/// <item><see cref="GetCombatItemReader" /></item>
		/// </list>
		/// <para>
		/// Note that all readers have to be fully exhausted before calling the next method.
		/// </para>
		/// </remarks>
		internal class SinglePassEVTCReaderStream : ISinglePassEVTCReader
		{
			enum ReaderPosition
			{
				BeforeVersion,
				BeforeBossData,
				BeforeAgents,
				InAgents,
				InSkills,
				InCombatItems,
			}

			private readonly EVTCParser parser;
			private readonly Stream stream;
			private readonly IDisposable[] disposables;
			private ReaderPosition position;
			private AgentReaderStream agentReader;
			private SkillReaderStream skillReader;
			private ICombatItemReader combatItemReader;
			private int revision;

			public SinglePassEVTCReaderStream(EVTCParser parser, Stream stream, params IDisposable[] disposables)
			{
				this.parser = parser;
				this.stream = stream;
				this.disposables = disposables;
				position = ReaderPosition.BeforeVersion;
			}

			public LogVersion ReadLogVersion()
			{
				if (position != ReaderPosition.BeforeVersion)
				{
					throw new InvalidOperationException("ReadLogVersion must be called when the reader is positioned before the log version.");
				}

				var version = parser.ParseLogVersion(stream);
				position = ReaderPosition.BeforeBossData;
				revision = version.Revision;
				return version;
			}

			public ParsedBossData ReadBossData()
			{
				if (position != ReaderPosition.BeforeBossData)
				{
					throw new InvalidOperationException("ReadBossData must be called when the reader is positioned just before the boss data.");
				}

				var bossData = parser.ParseBossData(stream);
				position = ReaderPosition.BeforeAgents;
				return bossData;
			}
			
			public IAgentReader GetAgentReader()
			{
				if (position != ReaderPosition.BeforeAgents)
				{
					throw new InvalidOperationException("GetAgentReader must be called when the reader is positioned just before the agent data.");
				}

				if (agentReader != null)
				{
					throw new InvalidOperationException("GetAgentReader must only be called once.");
				}
				
				Span<byte> bytes = stackalloc byte[4];
				ReadBytesExact(stream, bytes);
				int agentCount = BinaryPrimitives.ReadInt32LittleEndian(bytes);
				position = ReaderPosition.InAgents;
				agentReader = new AgentReaderStream(stream, agentCount);
				return agentReader;
			}
			
			public ISkillReader GetSkillReader()
			{
				if (position != ReaderPosition.InAgents)
				{
					throw new InvalidOperationException("GetSkillReader must be called directly after reading agent data.");
				}
				
				if (skillReader != null)
				{
					throw new InvalidOperationException("GetSkillReader must only be called once.");
				}
				
				if (!agentReader.Exhausted)
				{
					throw new InvalidOperationException("The agent reader obtained previously must be exhausted fully.");
				}

				Span<byte> bytes = stackalloc byte[4];
				ReadBytesExact(stream, bytes);
				int skillCount = BinaryPrimitives.ReadInt32LittleEndian(bytes);
				position = ReaderPosition.InSkills;
				skillReader = new SkillReaderStream(stream, skillCount);
				return skillReader;
			}
			
			public ICombatItemReader GetCombatItemReader()
			{
				if (position != ReaderPosition.InSkills)
				{
					throw new InvalidOperationException("GetCombatItemReader must be called directly after reading skill data.");
				}
				
				if (combatItemReader != null)
				{
					throw new InvalidOperationException("GetCombatItemReader must only be called once.");
				}
				
				if (!skillReader.Exhausted)
				{
					throw new InvalidOperationException("The skill reader obtained previously must be exhausted fully.");
				}

				position = ReaderPosition.InCombatItems;
				combatItemReader = revision switch
				{
					0 => throw new NotImplementedException("Not implemented yet."),
					1 => new CombatItemReaderRevision1Stream(stream),
					_ => throw new NotSupportedException("Only EVTC revisions 0 and 1 are supported.")
				};

				return combatItemReader;
			}

			public void Dispose()
			{
				stream?.Dispose();
				foreach (var disposable in disposables)
				{
					disposable?.Dispose();
				}
			}
		}

		/// <summary>
		/// Reads raw data from an EVTC log file.
		/// </summary>
		/// <param name="evtcFilename">The filename of the log.</param>
		/// <exception cref="NotSupportedException">Thrown for unsupported log revisions.</exception>
		/// <exception cref="LogParsingException">Thrown when parsing fails due to a malformed log or if an empty ZIP archive is supplied.</exception>
		/// <remarks>
		/// <para>
		/// Note that the extension is used to identify whether this is a zipped EVTC log.
		/// If the filename ends with .zip or .zevtc, it is opened as a zip file.
		/// </para>
		/// <para>
		/// If you do not need access to the <see cref="ParsedLog"/>,
		/// using the <see cref="LogProcessor.ProcessLog(string, EVTCParser)" /> method is more efficient.
		/// </para>
		/// </remarks>
		/// <returns>The raw data from the log.</returns>
		/// <seealso cref="LogProcessor.ProcessLog(string, EVTCParser)" />
		public ParsedLog ParseLog(string evtcFilename)
		{
			if (evtcFilename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ||
			    evtcFilename.EndsWith(".zevtc", StringComparison.OrdinalIgnoreCase))
			{
				using var fileStream = new FileStream(evtcFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
				using var arch = new ZipArchive(fileStream, ZipArchiveMode.Read);
				if (arch.Entries.Count == 0)
				{
					throw new LogParsingException("No EVTC file in ZIP archive.");
				}

				using var data = arch.Entries[0].Open();

				var bytes = new byte[arch.Entries[0].Length];
				int read;
				int offset = 0;
				while ((read = data.Read(bytes, offset, bytes.Length - offset)) > 0)
				{
					offset += read;
				}
				
				return ParseLog(bytes);
			}

			var fileBytes = File.ReadAllBytes(evtcFilename);
			return ParseLog(fileBytes);
		}

		/// <summary>
		/// Reads raw data from the bytes of an EVTC log.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If you do not need access to the <see cref="ParsedLog"/>,
		/// using the <see cref="LogProcessor.ProcessLog(byte[], EVTCParser)" /> method is more efficient.
		/// </para>
		/// </remarks>
		/// <param name="bytes">The contents of an uncompressed EVTC log, as bytes.</param>
		/// <exception cref="LogParsingException">Thrown when parsing fails due to a malformed log.</exception>
		/// <returns>The raw data from the log.</returns>
		/// <seealso cref="LogProcessor.ProcessLog(byte[], EVTCParser)" />
		public ParsedLog ParseLog(byte[] bytes)
		{
			var reader = new ByteArrayBinaryReader(bytes, Encoding.UTF8);

			LogVersion logVersion;
			ParsedBossData bossData;
			List<ParsedAgent> agents;
			List<ParsedSkill> skills;
			List<ParsedCombatItem> combatItems;
			try
			{
				logVersion = ParseLogVersion(reader);
			}
			catch (Exception e)
			{
				throw new LogParsingException("Failed to parse log metadata.", e);
			}

			try
			{
				bossData = ParseBossData(reader);
			}
			catch (Exception e)
			{
				throw new LogParsingException("Failed to parse boss data.", e);
			}

			try
			{
				agents = ParseAgents(reader).ToList();
			}
			catch (Exception e)
			{
				throw new LogParsingException("Failed to parse agents.", e);
			}

			try
			{
				skills = ParseSkills(reader).ToList();
			}
			catch (Exception e)
			{
				throw new LogParsingException("Failed to parse skills.", e);
			}

			try
			{
				combatItems = ParseCombatItems(logVersion.Revision, reader).ToList();
			}
			catch (Exception e)
			{
				throw new LogParsingException("Failed to parse combat items.", e);
			}

			return new ParsedLog(logVersion, bossData, agents, skills, combatItems);
		}
		
		/*
		internal ISinglePassEVTCReader GetSinglePassReader(string evtcFilename)
		{
			if (evtcFilename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ||
			    evtcFilename.EndsWith(".zevtc", StringComparison.OrdinalIgnoreCase))
			{
				using var fileStream = new FileStream(evtcFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
				using var arch = new ZipArchive(fileStream, ZipArchiveMode.Read);
				if (arch.Entries.Count == 0)
				{
					throw new LogParsingException("No EVTC file in ZIP archive.");
				}

				using var data = arch.Entries[0].Open();

				var bytes = new byte[arch.Entries[0].Length];
				int read;
				int offset = 0;
				while ((read = data.Read(bytes, offset, bytes.Length - offset)) > 0)
				{
					offset += read;
				}
				return GetSinglePassReader(bytes);
			}

			var fileBytes = File.ReadAllBytes(evtcFilename);
			return GetSinglePassReader(fileBytes);
		}
		*/

		internal ISinglePassEVTCReader GetSinglePassReader(string evtcFilename)
		{
			if (evtcFilename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ||
			    evtcFilename.EndsWith(".zevtc", StringComparison.OrdinalIgnoreCase))
			{
				// Disposed by the single pass reader when it is disposed
				var fileStream = new FileStream(evtcFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
				var arch = new ZipArchive(fileStream, ZipArchiveMode.Read);
				if (arch.Entries.Count == 0)
				{
					throw new LogParsingException("No EVTC file in ZIP archive.");
				}

				return GetSinglePassReader(arch.Entries[0].Open(), fileStream, arch);
			}

			// TODO: stream here as well
			var fileBytes = File.ReadAllBytes(evtcFilename);
			return GetSinglePassReader(fileBytes);
		}

		internal ISinglePassEVTCReader GetSinglePassReader(byte[] bytes)
		{
			return new SinglePassEVTCReader(this, new ByteArrayBinaryReader(bytes, Encoding.UTF8));
		}
		
		internal ISinglePassEVTCReader GetSinglePassReader(Stream stream, params IDisposable[] disposables)
		{
			return new SinglePassEVTCReaderStream(this, stream, disposables);
		}

		/// <summary>
		/// Parses log metadata.
		/// </summary>
		private LogVersion ParseLogVersion(ByteArrayBinaryReader reader)
		{
			// 12 bytes: arc build version
			string buildVersion = reader.ReadString(12);

			// 1 byte: revision
			byte revision = reader.ReadByte();

			return new LogVersion(buildVersion, revision);
		}
		
		/// <summary>
		/// Parses log metadata.
		/// </summary>
		private LogVersion ParseLogVersion(Stream stream)
		{
			// 12 bytes: arc build version
			Span<byte> bytes = stackalloc byte[13];
			ReadBytesExact(stream, bytes);
			
			string buildVersion = Encoding.UTF8.GetString(bytes[..12]);

			// 1 byte: revision
			byte revision = bytes[12];

			return new LogVersion(buildVersion, revision);
		}

		/// <summary>
		/// Parses boss related data.
		/// </summary>
		private ParsedBossData ParseBossData(ByteArrayBinaryReader reader)
		{
			// 2 bytes: boss species ID
			ushort id = reader.ReadUInt16();
			// 1 byte: unused
			reader.Skip(1);

			return new ParsedBossData(id);
		}
		
		/// <summary>
		/// Parses boss related data.
		/// </summary>
		private ParsedBossData ParseBossData(Stream stream)
		{
			Span<byte> bytes = stackalloc byte[3];
			ReadBytesExact(stream, bytes);

			// 2 bytes: boss species ID
			ushort id = BinaryPrimitives.ReadUInt16LittleEndian(bytes[..2]);
			// 1 byte: unused

			return new ParsedBossData(id);
		}

		/// <summary>
		/// Parses a single agent from the reader.
		/// </summary>
		/// <remarks>
		/// Consumes 96 bytes from the reader.
		/// </remarks>
		private static void ParseAgent(ByteArrayBinaryReader reader, out ParsedAgent agent)
		{
			// 8 bytes: agent address
			ulong address = reader.ReadUInt64();

			// 4 bytes: profession
			uint prof = reader.ReadUInt32();

			// 4 bytes: is_elite
			uint isElite = reader.ReadUInt32();

			// 2 bytes: toughness
			short toughness = reader.ReadInt16();
			// 2 bytes: concentration
			short concentration = reader.ReadInt16();
			// 2 bytes: healing
			short healing = reader.ReadInt16();
			// 2 bytes: hb_width
			short hitboxWidth = reader.ReadInt16();
			// 2 bytes: condition
			short condition = reader.ReadInt16();
			// 2 bytes: hb_height
			short hitboxHeight = reader.ReadInt16();
			// 68 bytes: name
			string name = reader.ReadString(68);

			agent = new ParsedAgent(address, name, prof, isElite, toughness, concentration,
				healing, condition, hitboxWidth, hitboxHeight);
		}
	
		/// <summary>
		/// Parses a single agent from a stream.
		/// </summary>
		/// <remarks>
		/// Consumes 96 bytes from the stream.
		/// </remarks>
		private static void ParseAgent(Stream stream, out ParsedAgent agent)
		{
			// TODO: try heap-allocating this one (reusing one instance)
			Span<byte> bytes = stackalloc byte[96];
			ReadBytesExact(stream, bytes);
			
			// 8 bytes: agent address
			ulong address = BinaryPrimitives.ReadUInt64LittleEndian(bytes[..8]);

			// 4 bytes: profession
			uint prof = BinaryPrimitives.ReadUInt32LittleEndian(bytes[8..12]);

			// 4 bytes: is_elite
			uint isElite = BinaryPrimitives.ReadUInt32LittleEndian(bytes[12..16]);

			// 2 bytes: toughness
			short toughness = BinaryPrimitives.ReadInt16LittleEndian(bytes[16..18]);
			// 2 bytes: concentration
			short concentration = BinaryPrimitives.ReadInt16LittleEndian(bytes[18..20]);
			// 2 bytes: healing
			short healing = BinaryPrimitives.ReadInt16LittleEndian(bytes[20..22]);
			// 2 bytes: hb_width
			short hitboxWidth = BinaryPrimitives.ReadInt16LittleEndian(bytes[22..24]);
			// 2 bytes: condition
			short condition = BinaryPrimitives.ReadInt16LittleEndian(bytes[24..26]);
			// 2 bytes: hb_height
			short hitboxHeight = BinaryPrimitives.ReadInt16LittleEndian(bytes[26..28]);
			// 68 bytes: name
			string name = Encoding.UTF8.GetString(bytes[28..]);

			agent = new ParsedAgent(address, name, prof, isElite, toughness, concentration,
				healing, condition, hitboxWidth, hitboxHeight);
		}

		/// <summary>
		/// Parses agent related data.
		/// </summary>
		private IEnumerable<ParsedAgent> ParseAgents(ByteArrayBinaryReader reader)
		{
			// 4 bytes: agent count
			int agentCount = reader.ReadInt32();

			var agentReader = new AgentReader(reader, agentCount);

			ParsedAgent agent;
			while (agentReader.GetNext(out agent))
			{
				yield return agent;
			}
		}
		
		/// <summary>
		/// Parses agent related data.
		/// </summary>
		private IEnumerable<ParsedAgent> ParseAgents(Stream stream)
		{
			Span<byte> bytes = stackalloc byte[4];
			ReadBytesExact(stream, bytes);
			
			// 4 bytes: agent count
			int agentCount = BinaryPrimitives.ReadInt32LittleEndian(bytes);

			var agentReader = new AgentReaderStream(stream, agentCount);

			ParsedAgent agent;
			while (agentReader.GetNext(out agent))
			{
				yield return agent;
			}
		}

		/// <summary>
		/// Parses skill related data.
		/// </summary>
		private IEnumerable<ParsedSkill> ParseSkills(ByteArrayBinaryReader reader)
		{
			// 4 bytes: skill count
			int skillCount = reader.ReadInt32();

			// 68 bytes: each skill
			var skillReader = new SkillReader(reader, skillCount);
			
			ParsedSkill skill;
			while (skillReader.GetNext(out skill))
			{
				yield return skill;
			}
		}
		
		/// <summary>
		/// Parses skill related data.
		/// </summary>
		private IEnumerable<ParsedSkill> ParseSkills(Stream stream)
		{
			Span<byte> bytes = stackalloc byte[4];
			ReadBytesExact(stream, bytes);
			
			// 4 bytes: skill count
			int skillCount = BinaryPrimitives.ReadInt32LittleEndian(bytes);

			// 68 bytes: each skill
			var skillReader = new SkillReaderStream(stream, skillCount);
			
			ParsedSkill skill;
			while (skillReader.GetNext(out skill))
			{
				yield return skill;
			}
		}

		/// <summary>
		/// Parses a single skill.
		/// </summary>
		private static void ParseSkill(ByteArrayBinaryReader reader, out ParsedSkill skill)
		{
			// 4 bytes: skill ID
			int skillId = reader.ReadInt32();

			// 64 bytes: name
			var name = reader.ReadString(64);

			skill = new ParsedSkill(skillId, name);
		}
		
		/// <summary>
		/// Parses a single skill.
		/// </summary>
		private static void ParseSkill(Stream stream, out ParsedSkill skill)
		{
			// TODO: try heap-allocating this one (reusing one instance)
			Span<byte> bytes = stackalloc byte[68];
			ReadBytesExact(stream, bytes);

			// 4 bytes: skill ID
			int skillId = BinaryPrimitives.ReadInt32LittleEndian(bytes[0..4]);

			// 64 bytes: name
			var name = Encoding.UTF8.GetString(bytes[4..]);

			skill = new ParsedSkill(skillId, name);
		}

		/// <summary>
		/// Parses combat related data.
		/// </summary>
		private IEnumerable<ParsedCombatItem> ParseCombatItems(int revision, ByteArrayBinaryReader reader)
		{
			ICombatItemReader combatItemReader = revision switch
			{
				0 => new CombatItemReaderRevision0(reader),
				1 => new CombatItemReaderRevision1(reader),
				_ => throw new NotSupportedException("Only EVTC revisions 0 and 1 are supported.")
			};
			
			ParsedCombatItem combatItem;
			while (combatItemReader.GetNext(out combatItem))
			{
				yield return combatItem;
			}
		}
		
		/// <summary>
		/// Parses combat related data.
		/// </summary>
		private IEnumerable<ParsedCombatItem> ParseCombatItems(int revision, Stream stream)
		{
			ICombatItemReader combatItemReader = revision switch
			{
				0 => throw new NotImplementedException("This is not implemented yet."),
				1 => new CombatItemReaderRevision1Stream(stream),
				_ => throw new NotSupportedException("Only EVTC revisions 0 and 1 are supported.")
			};
			
			ParsedCombatItem combatItem;
			while (combatItemReader.GetNext(out combatItem))
			{
				yield return combatItem;
			}
		}

		private static void ReadCombatItemRevision0(ByteArrayBinaryReader reader, out ParsedCombatItem item)
		{
			// See ReadCombatItemRevision1 for notes about performance and why it's done this way.
			
			// Padding and DstMasterId are both left as 0, as they are not included in revision 0 logs.
			item = default;
			
			// 8 bytes: time
			item.Time = reader.ReadInt64();

			// 8 bytes: src_agent
			item.SrcAgent = reader.ReadUInt64();

			// 8 bytes: dst_agent
			item.DstAgent = reader.ReadUInt64();

			// 4 bytes: value
			item.Value = reader.ReadInt32();

			// 4 bytes: buff_dmg
			item.BuffDmg = reader.ReadInt32();

			// 2 bytes: overstack_value
			item.OverstackValue = reader.ReadUInt16();

			// 2 bytes: skill_id
			item.SkillId = reader.ReadUInt16();

			// 2 bytes: src_instid
			item.SrcAgentId = reader.ReadUInt16();

			// 2 bytes: dst_instid
			item.DstAgentId = reader.ReadUInt16();

			// 2 bytes: src_master_instid
			item.SrcMasterId = reader.ReadUInt16();

			// 9 bytes: garbage
			reader.Skip(9);

			// 1 byte: iff
			item.Iff = GetFriendOrFoeFromByte(reader.ReadByte());

			// 1 byte: buff
			item.Buff = reader.ReadByte();

			// 1 byte: result
			item.Result = GetResultFromByte(reader.ReadByte());

			// 1 byte: is_activation
			item.IsActivation = GetActivationFromByte(reader.ReadByte());

			// 1 byte: is_buffremove
			item.IsBuffRemove = GetBuffRemoveFromByte(reader.ReadByte());

			// 1 byte: is_ninety
			item.IsNinety = reader.ReadByte();

			// 1 byte: is_fifty
			item.IsFifty = reader.ReadByte();

			// 1 byte: is_moving
			item.IsMoving = reader.ReadByte();

			// 1 byte: is_statechange
			item.IsStateChange = GetStateChangeFromByte(reader.ReadByte());

			// 1 byte: is_flanking
			item.IsFlanking = reader.ReadByte();

			// 1 byte: is_shields
			item.IsShields = reader.ReadByte();

			// 1 byte: is_offcycle
			item.IsOffCycle = reader.ReadByte();

			// 1 byte: garbage
			reader.Skip(1);
		}

		private static void ReadCombatItemRevision1(ByteArrayBinaryReader reader, out ParsedCombatItem item)
		{
			// This performs better than calling the constructor at the end to set the item (measured with .NET 6).
			// Marshalling here doesn't seem to be worth it (it's slightly slower)
			item = default;
			
			// 8 bytes: time
			item.Time = reader.ReadInt64();

			// 8 bytes: src_agent
			item.SrcAgent = reader.ReadUInt64();

			// 8 bytes: dst_agent
			item.DstAgent = reader.ReadUInt64();

			// 4 bytes: value
			item.Value = reader.ReadInt32();

			// 4 bytes: buff_dmg
			item.BuffDmg = reader.ReadInt32();

			// 4 bytes: overstack_value
			item.OverstackValue = reader.ReadUInt32();

			// 4 bytes: skill_id
			item.SkillId = reader.ReadUInt32();

			// 2 bytes: src_instid
			item.SrcAgentId = reader.ReadUInt16();

			// 2 bytes: dst_instid
			item.DstAgentId = reader.ReadUInt16();

			// 2 bytes: src_master_instid
			item.SrcMasterId = reader.ReadUInt16();

			// 2 bytes: dst_master_instid
			item.DstMasterId = reader.ReadUInt16();

			// 1 byte: iff
			item.Iff = GetFriendOrFoeFromByte(reader.ReadByte());

			// 1 byte: buff
			item.Buff = reader.ReadByte();

			// 1 byte: result
			item.Result = GetResultFromByte(reader.ReadByte());

			// 1 byte: is_activation
			item.IsActivation = GetActivationFromByte(reader.ReadByte());

			// 1 byte: is_buffremove
			item.IsBuffRemove = GetBuffRemoveFromByte(reader.ReadByte());

			// 1 byte: is_ninety
			item.IsNinety = reader.ReadByte();

			// 1 byte: is_fifty
			item.IsFifty = reader.ReadByte();

			// 1 byte: is_moving
			item.IsMoving = reader.ReadByte();

			// 1 byte: is_statechange
			item.IsStateChange = GetStateChangeFromByte(reader.ReadByte());

			// 1 byte: is_flanking
			item.IsFlanking = reader.ReadByte();

			// 1 byte: is_shields
			item.IsShields = reader.ReadByte();

			// 1 byte: is_offcycle
			item.IsOffCycle = reader.ReadByte();

			// 4 bytes: "padding"
			item.Padding = reader.ReadUInt32();
		}
		
		private static bool ReadCombatItemRevision1(Stream stream, out ParsedCombatItem item)
		{
			Unsafe.SkipInit(out item);
			Span<byte> buffer = MemoryMarshal.Cast<ParsedCombatItem, byte>(MemoryMarshal.CreateSpan(ref item, 1));

			int read = 0;
			while (read != buffer.Length)
			{
				int bytesRead = stream.Read(buffer[read..]);
				if (bytesRead == 0)
				{
					item = default;
					return false;
				}

				read += bytesRead;
			}

			return true;
		}

		private static void ReadBytesExact(Stream stream, Span<byte> buffer)
		{
			int read = 0;
			while (read != buffer.Length)
			{
				int bytesRead = stream.Read(buffer[read..]);
				if (bytesRead == 0) throw new EndOfStreamException();
				read += bytesRead;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static FriendOrFoe GetFriendOrFoeFromByte(byte b)
		{
			return (FriendOrFoe) b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Result GetResultFromByte(byte b)
		{
			return (Result) b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static BuffRemove GetBuffRemoveFromByte(byte b)
		{
			return (BuffRemove) b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Activation GetActivationFromByte(byte b)
		{
			return (Activation) b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static StateChange GetStateChangeFromByte(byte b)
		{
			return (StateChange) b;
		}
	}
}