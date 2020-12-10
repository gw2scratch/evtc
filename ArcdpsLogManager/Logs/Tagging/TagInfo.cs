using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Logs.Tagging
{
	[DebuggerDisplay(nameof(Name))]
	public class TagInfo : IEquatable<TagInfo>
	{
		/// <summary>
		/// The name of this tag.
		/// </summary>
		[JsonProperty]
		public string Name { get; }

		[JsonConstructor]
		public TagInfo(string name)
		{
			Name = name;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TagInfo);
		}

		public bool Equals(TagInfo other)
		{
			return other != null &&
			       Name == other.Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}