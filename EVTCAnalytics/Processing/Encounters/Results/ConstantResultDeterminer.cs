using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
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

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			return new ResultDeterminerResult(result, null);
		}
	}
}