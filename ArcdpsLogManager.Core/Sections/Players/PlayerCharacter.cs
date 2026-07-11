using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System.Collections.Generic;

namespace GW2Scratch.ArcdpsLogManager.Sections.Players
{
	/// <summary>
	/// Represents a character belonging to a player, along with logs it is featured in.
	/// </summary>
	public class PlayerCharacter
	{
		public string Name { get; }
		public Profession Profession { get; }
		public IReadOnlyList<LogData> Logs { get; }

		public PlayerCharacter(string name, Profession profession, IReadOnlyList<LogData> logs)
		{
			Name = name;
			Profession = profession;
			Logs = logs;
		}
	}
}