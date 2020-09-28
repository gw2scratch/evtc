using System.Collections.Generic;
using System.Linq;
using static GW2Scratch.ArcdpsLogManager.Logs.LogData;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	public class FavoritesLogGroup : TagLogGroup
	{
		public FavoritesLogGroup() : base(TagInfo.Favorites) {}
	}

	/// <summary>
	/// A log group for logs with a given <see cref="TagInfo">tag</see> filters by tag name and type.
	/// </summary>
	public class TagLogGroup : LogGroup
	{
		public TagInfo Tag { get; }

		public override string Name => Tag.Name;
		public override IEnumerable<LogGroup> Subgroups => Enumerable.Empty<LogGroup>();

		public TagLogGroup(string tagType, string tagName)
		{
			Tag = new TagInfo(tagType, tagName);
		}

		public TagLogGroup(TagInfo tag)
		{
			Tag = tag;
		}

		public override bool IsInGroup(LogData log)
		{
            // we could possibly do prefix matching on name here for extra usability w/user-defined tags
			return log.Tags.Contains(Tag);
		}
	}
}