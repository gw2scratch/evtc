using System.IO;
using System.Text;

namespace ScratchEVTCParser
{
	internal static class StreamExtensions
	{
		public static void SafeSkip(this Stream stream, long bytesToSkip)
		{
			if (stream.CanSeek)
			{
				stream.Seek(bytesToSkip, SeekOrigin.Current);
			}
			else
			{
				while (bytesToSkip > 0)
				{
					stream.ReadByte();
					--bytesToSkip;
				}
			}
		}

		public static string ReadString(this Stream stream, int length, Encoding encoding, bool nullTerminated = true)
		{
			var bytes = new byte[length];
			stream.Read(bytes, 0, length);
			if (nullTerminated)
			{
				for (int i = 0; i < length; ++i)
				{
					if (bytes[i] == 0)
					{
						length = i;
						break;
					}
				}
			}

			return encoding.GetString(bytes, 0, length);
		}
	}
}