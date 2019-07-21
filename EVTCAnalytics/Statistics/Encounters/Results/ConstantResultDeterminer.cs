using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results
{
	/// <summary>
	/// Always returns the same provided result.
	/// </summary>
	public class ConstantResultDeterminer : IResultDeterminer
	{
		private readonly EncounterResult result;

		public ConstantResultDeterminer(EncounterResult result)
		{
			this.result = result;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			return result;
		}
	}
}