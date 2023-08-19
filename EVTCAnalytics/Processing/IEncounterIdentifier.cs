using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	/// <summary>
	/// An interface for identifying encounters from log data.
	/// </summary>
	public interface IEncounterIdentifier
	{
		/// <summary>
		/// Identify the encounter within the log.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is intended to be used before a <see cref="Log"/> is created,
		/// which is why passing in individual parts of the log is required.
		/// </para>
		/// </remarks>
		/// <param name="mainTarget">The main target of the log.</param>
		/// <param name="events">The events of the log.</param>
		/// <param name="agents">The agents of the log</param>
		/// <param name="skills">The skills of the log</param>
		/// <returns>The encounter within this log.</returns>
		Encounter IdentifyEncounter(Agent mainTarget, IReadOnlyList<Agent> agents, IReadOnlyList<Event> events, IReadOnlyList<Skill> skills);

		/// <summary>
		/// Identifies potential encounters early during parsing.
		/// </summary>
		IEnumerable<Encounter> IdentifyPotentialEncounters(ParsedBossData bossData);
	}
}