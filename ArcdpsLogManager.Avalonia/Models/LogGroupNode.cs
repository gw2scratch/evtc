using System.Collections.Generic;
using System.Linq;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// A tree node wrapping a <see cref="LogGroup"/> for the encounter-group filter tree
	/// (Avalonia counterpart of the Eto <c>LogEncounterFilterTree</c>). Selecting a node filters
	/// the logs to that group's subtree.
	/// </summary>
	public sealed class LogGroupNode
	{
		public LogGroup Group { get; }
		public string Name => Group.Name;
		public IReadOnlyList<LogGroupNode> Children { get; }

		public LogGroupNode(LogGroup group)
		{
			Group = group;
			Children = group.Subgroups.Select(g => new LogGroupNode(g)).ToList();
		}
	}
}
