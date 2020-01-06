using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// Combines the result of multiple determiners, returning success if all succeed, unknown if any
	/// is unknown and failure otherwise.
	/// </summary>
	public class AllCombinedResultDeterminer : IResultDeterminer
	{
		private readonly IResultDeterminer[] determiners;

		public AllCombinedResultDeterminer(params IResultDeterminer[] determiners)
		{
			if (determiners.Length == 0)
			{
				throw new ArgumentException("At least one determiner has to be provided", nameof(determiners));
			}

			this.determiners = determiners;
		}

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			var e = events as Event[] ?? events.ToArray();
			var results = determiners.Select(x => x.GetResult(e)).ToArray();

			if (results.All(x => x == EncounterResult.Success)) return EncounterResult.Success;
			if (results.Any(x => x == EncounterResult.Unknown)) return EncounterResult.Unknown;
			return EncounterResult.Failure;
		}
	}
}