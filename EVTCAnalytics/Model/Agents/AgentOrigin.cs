using System.Collections.Generic;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	/// <summary>
	/// Contains metadata of the origins of an <see cref="Agent"/> from raw log data.
	/// Some <see cref="Agent"/>s might be multiple merged raw agents and this class allows identifying these cases.
	/// </summary>
	public class AgentOrigin
	{
		/// <summary>
		/// A value indicating whether this agent is a result of merging multiple raw <see cref="Parsed.ParsedAgent"/> into one.
		/// </summary>
		public bool Merged => OriginalAgentData.Count > 1;
		
		/// <summary>
		/// Provides <see cref="OriginalAgentData"/> for all raw <see cref="Parsed.ParsedAgent"/> this agent was created from.
		/// </summary>
		public IReadOnlyList<OriginalAgentData> OriginalAgentData { get; internal set; }

		/// <summary>
		/// Creates a new <see cref="AgentOrigin"/> for an agent created from a single raw <see cref="Parsed.ParsedAgent"/>.
		/// </summary>
		public AgentOrigin(OriginalAgentData originalAgentData)
		{
			OriginalAgentData = new List<OriginalAgentData> {originalAgentData};
		}

		/// <summary>
		/// Creates a new <see cref="AgentOrigin"/> for an agent created from a multiple raw <see cref="Parsed.ParsedAgent"/>.
		/// </summary>
		public AgentOrigin(IEnumerable<OriginalAgentData> originalAgentData)
		{
			OriginalAgentData = originalAgentData.ToList();
		}
	}
}