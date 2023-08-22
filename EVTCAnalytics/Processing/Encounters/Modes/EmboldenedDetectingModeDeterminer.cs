using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using System;
using System.Linq;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	/// <summary>
	/// An encounter mode determiner that detects Emboldened mode.
	/// </summary>
	public class EmboldenedDetectingModeDeterminer : IModeDeterminer
	{
		public EncounterMode? GetMode(Log log)
		{
			// In case Emboldened does not appear in the log at all, we can fail quickly without
			// having to go through events.
			if (log.Skills.All(x => x.Id != SkillIds.Emboldened))
			{
				return EncounterMode.Normal;
			}

			var emboldenedCounts = new Dictionary<Agent, int>();
			foreach (var buffEvent in log.Events.OfType<InitialBuffEvent>())
			{
				if (buffEvent.Buff?.Id == SkillIds.Emboldened)
				{
					if (!emboldenedCounts.TryGetValue(buffEvent.Agent, out _))
					{
						emboldenedCounts[buffEvent.Agent] = 0;
					}
					emboldenedCounts[buffEvent.Agent] += 1;
				}
			}

			// In case Emboldened appears only due to buff applications, we currently default to 0.
			// Note that this can be both correct and wrong, depending on when this happens:
			// - at the end of the fight, the log can capture the emboldened buff being applied (if coming from CM fight) / reset, we want to ignore this
			// - in the middle of a fight, this can be legitimate when the emboldened wing changes to the one the fight is happening in, we want to count this
			// TODO: fix this ^
			
			var maxEmboldenedCount = emboldenedCounts.Values.DefaultIfEmpty(0).Max();
			var mode = maxEmboldenedCount switch {
				<= 0 => EncounterMode.Normal,
				1 => EncounterMode.Emboldened1,
				2 => EncounterMode.Emboldened2,
				3 => EncounterMode.Emboldened3,
				4 => EncounterMode.Emboldened4,
				5 => EncounterMode.Emboldened5,
				> 5 => EncounterMode.Emboldened5,
			};

			return mode;
		}
	}
}