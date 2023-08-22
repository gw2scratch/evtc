using System;
using System.Buffers.Binary;
using System.Text;

namespace GW2Scratch.EVTCAnalytics.IO
{
	/// <summary>
	/// A little-endian Binary Reader for reading from a Byte array.
	/// No safety checks are being done.
	/// </summary>
	public class ByteArrayBinaryReader
	{
		private readonly byte[] bytes;
		private readonly Encoding encoding;

		public int Position { get; private set; }
		public int Length => bytes.Length;
		public bool BytesLeft => Position < Length;

		public ByteArrayBinaryReader(byte[] bytes, Encoding encoding)
		{
			Position = 0;
			this.bytes = bytes;
			this.encoding = encoding;
		}

		public byte ReadByte()
		{
			byte value = bytes[Position];
			Position++;
			return value;
		}
		
		public byte PeekByte(int offset)
		{
			return bytes[Position + offset];
		}
		
		public Span<byte> PeekRange(int offset, int length)
		{
			return bytes.AsSpan()[(Position + offset)..(Position + offset + length)];
		}

		public sbyte ReadSByte()
		{
			return (sbyte) ReadByte();
		}

		public short ReadInt16()
		{
			short value = BinaryPrimitives.ReadInt16LittleEndian(bytes.AsSpan()[Position..]);
			Position += 2;
			return value;
		}

		public ushort ReadUInt16()
		{
			ushort value = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan()[Position..]);
			Position += 2;
			return value;
		}

		public int ReadInt32()
		{
			int value = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan()[Position..]);
			Position += 4;
			return value;
		}

		public uint ReadUInt32()
		{
			return (uint) ReadInt32();
		}

		public long ReadInt64()
		{
			long value = BinaryPrimitives.ReadInt64LittleEndian(bytes.AsSpan()[Position..]);
			Position += 8;
			return value;
		}

		public ulong ReadUInt64()
		{
			ulong value = BinaryPrimitives.ReadUInt64LittleEndian(bytes.AsSpan()[Position..]);
			Position += 8;
			return value;
		}

		public float ReadSingle()
		{
			float value = BitConverter.ToSingle(bytes, Position);
			Position += 4;
			return value;
		}

		public double ReadDouble()
		{
			double value = BitConverter.ToDouble(bytes, Position);
			Position += 8;
			return value;
		}

		public string ReadString(int length)
		{
			string value = encoding.GetString(bytes, Position, length);
			Position += length;
			return value;
		}

		public void Skip(int byteCount)
		{
			Position += byteCount;
		}
	}
}