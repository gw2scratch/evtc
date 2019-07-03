using System;
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

		public sbyte ReadSByte()
		{
			return (sbyte) ReadByte();
		}

		public short ReadInt16()
		{
			short value = (short) (bytes[Position] | bytes[Position + 1] << 8);
			Position += 2;
			return value;
		}

		public ushort ReadUInt16()
		{
			return (ushort) ReadInt16();
		}

		public int ReadInt32()
		{
			int value = bytes[Position] |
			            bytes[Position + 1] << 8 |
			            bytes[Position + 2] << 16 |
			            bytes[Position + 3] << 24;
			Position += 4;
			return value;
		}

		public uint ReadUInt32()
		{
			return (uint) ReadInt32();
		}

		public long ReadInt64()
		{
			uint lower = (uint) (bytes[Position] |
			            bytes[Position + 1] << 8 |
			            bytes[Position + 2] << 16 |
			            bytes[Position + 3] << 24);

			uint upper = (uint) (bytes[Position + 4] |
			            bytes[Position + 5] << 8 |
			            bytes[Position + 6] << 16 |
			            bytes[Position + 7] << 24);

			Position += 8;
			return (long) (lower | (ulong) upper << 32);
		}

		public ulong ReadUInt64()
		{
			return (ulong) ReadInt64();
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