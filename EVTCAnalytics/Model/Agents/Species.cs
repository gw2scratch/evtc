using System;
using System.Text;

namespace GW2Scratch.EVTCAnalytics.Model.Agents;

/// <summary>
/// Represents the species of a NPC
/// </summary>
public class Species(uint id)
{
	/// <summary>
	/// The ID of the species.
	/// </summary>
	public uint Id { get; } = id;

	/// <summary>
	/// The content GUID of this species stored as 16 bytes.
	/// </summary>
	public byte[] ContentGuid { get; internal set; }

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
