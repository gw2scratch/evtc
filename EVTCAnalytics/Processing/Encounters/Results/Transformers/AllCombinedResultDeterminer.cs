using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Transformers
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

		public IReadOnlyList<Type> RequiredEventTypes => determiners.SelectMany(x => x.RequiredEventTypes).Distinct().ToList();

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			var e = events as Event[] ?? events.ToArray();
			var results = determiners.Select(x => x.GetResult(e)).ToArray();

			EncounterResult result;
			long? time;
			if (results.All(x => x.EncounterResult == EncounterResult.Success))
			{
				result = EncounterResult.Success;
				if (results.Length == 0)
				{
					time = null;
				}
				else
				{
					time = results.Where(x => x.Time.HasValue).Select(x => x.Time).Max();
				}
			}
			else if (results.Any(x => x.EncounterResult == EncounterResult.Unknown))
			{
				result = EncounterResult.Unknown;
				time = null;
			}
			else
			{
				result = EncounterResult.Failure;
				time = null;
			}

			return new ResultDeterminerResult(result, time);
		}
	}
}