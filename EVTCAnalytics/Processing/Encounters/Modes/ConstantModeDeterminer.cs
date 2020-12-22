using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	/// <summary>
	/// An encounter mode determiner that always returns the same value.
	/// </summary>
	public class ConstantModeDeterminer : IModeDeterminer
	{
		private readonly EncounterMode mode;

		public ConstantModeDeterminer(EncounterMode mode)
		{
			this.mode = mode;
		}

		public EncounterMode GetMode(Log log)
		{
			return mode;
		}
	}
}