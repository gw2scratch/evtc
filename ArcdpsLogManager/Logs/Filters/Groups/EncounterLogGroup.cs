using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Groups
{
	/// <summary>
	/// A log group that a logs with a given <see cref="Encounter"/> belong to.
	/// </summary>
	public class EncounterLogGroup : LogGroup
	{
		public Encounter Encounter { get; }

		public override string Name { get; }
		public override IEnumerable<LogGroup> Subgroups => Enumerable.Empty<LogGroup>();

		public EncounterLogGroup(Encounter encounter)
		{
			Encounter = encounter;

			// TODO: A way to specify names from the outside to allow for future localization
			if (!EncounterNames.TryGetEncounterNameForLanguage(GameLanguage.English, encounter, out string name))
			{
				name = encounter.ToString();
			}

			Name = name;
		}

		public override bool IsInGroup(LogData log)
		{
			return log.Encounter == Encounter;
		}
	}
}