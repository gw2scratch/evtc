using System;
using System.Text;

namespace GW2Scratch.EVTCAnalytics.Model.Effects;

/// <summary>
/// Represents a game visual effect.
/// </summary>
public class Effect
{
	/// <summary>
	/// The ID number of the effect.
	/// </summary>
	public uint Id { get; }

	/// <summary>
	/// The content GUID of this effect stored as 16 bytes.
	/// </summary>
	/// <remarks>
	/// Values from arcdps versions before 20220709 are wrong.
	/// </remarks>
	public byte[] ContentGuid { get; internal set; }

	public Effect(uint id)
	{
		Id = id;
	}

	public override string ToString()
	{
		return $"{Id} ({(ContentGuid != null ? GuidToString(ContentGuid) : "No GUID")})";
	}

	private static string GuidToString(byte[] guidBytes)
	{
		string GetPart(byte[] bytes, int from, int to)
		{
			var builder = new StringBuilder();
			for (int i = from; i < to; i++)
			{
				builder.Append($"{bytes[i]:x2}");
			}

			return builder.ToString();
		}

		if (guidBytes == null) return null;
		if (guidBytes.Length != 16)
		{
			throw new ArgumentException("The GUID has to consist of 16 bytes", nameof(guidBytes));
		}

		return $"{GetPart(guidBytes, 0, 4)}-{GetPart(guidBytes, 4, 6)}-{GetPart(guidBytes, 6, 8)}" +
		       $"-{GetPart(guidBytes, 8, 10)}-{GetPart(guidBytes, 10, 16)}";
	}
}