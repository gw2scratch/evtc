using System;

namespace GW2Scratch.EVTCAnalytics.IO
{
	public static class BitConversions
	{
		/// <summary>
		/// Converts a float encoded in the bytes of a <see cref="Int32"/> to a <see cref="Single"/>.
		/// </summary>
		/// <param name="value"><see cref="Int32"/> that will be reinterpreted as a float.</param>
		public static float ToSingle(int value)
		{
			unsafe
			{
				return *((float*) &value);
			}
		}

		/// <summary>
		/// Converts a float encoded in the bytes of a <see cref="UInt32"/> to a <see cref="Single"/>.
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