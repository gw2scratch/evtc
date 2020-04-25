using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	public abstract class LogGroup : ILogFilter
	{
		/// <summary>
		/// The name of the group.
		/// </summary>
		///
		public abstract string Name { get; }

		/// <summary>
		/// Determines whether a log belongs to this group.
		/// <br />
		/// Returns <see langword="true"/> if this group is part of any subgroup.
		/// </summary>
		/// <param name="log">The log.</param>
		/// <returns>A value indicating whether the log belongs to this group.</returns>
		public abstract bool IsInGroup(LogData log);

		/// <summary>
		/// Gets subgroups of this group. If a log belongs to a subgroup, it also has to belong into this group.
		/// </summary>
		/// <returns>Subgroups of this group.</returns>
		public abstract IEnumerable<LogGroup> Subgroups { get; }

		public bool FilterLog(LogData log)
		{
			return IsInGroup(log);
		}
	}
}