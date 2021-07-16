using System;
using System.Runtime.CompilerServices;

namespace GW2Scratch.EVTCAnalytics.IO
{
	/// <summary>
	/// Converts between value types.
	/// Assumes that the endianness of the caller is the same as that of the source of the values.
	/// </summary>
	/// <remarks>
	/// This only exists because <see cref="System.BitConverter"/> requires to create byte arrays,
	/// to do these conversions, this is slightly more performant. In the future, it might not be required thanks
	/// to better BitConverter optimizations, but we should make sure to profile it again.
	/// </remarks>
	internal static class BitConversions
	{
		/// <summary>
		/// Converts a float encoded in the bytes of a <see cref="System.Int32"/> to a <see cref="System.Single"/>.
		/// </summary>
		/// <param name="value"><see cref="Int32"/> that will be reinterpreted as a float.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToSingle(int value)
		{
			unsafe
			{
				return *((float*) &value);
			}
		}

		/// <summary>
		/// Converts a float encoded in the bytes of a <see cref="System.UInt32"/> to a <see cref="System.Single"/>.
		/// </summary>
		/// <param name="value"><see cref="UInt32"/> that will be reinterpreted as a float.</param>
		public static float ToSingle(uint value)
		{
			unsafe
			{
				return *((float*) &value);
			}
		}
	}
}