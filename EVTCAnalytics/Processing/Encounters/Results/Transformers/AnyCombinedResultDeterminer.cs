using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Transformers
{
	/// <summary>
	/// Combines the result of multiple determiners, returning success if any succeeded, then unknown if any
	/// is unknown and failure otherwise.
	/// </summary>
	public class AnyCombinedResultDeterminer : IResultDeterminer
	{
		private readonly IResultDeterminer[] determiners;

		public AnyCombinedResultDeterminer(params IResultDeterminer[] determiners)
		{
			if (determiners.Length == 0)
			{
				throw new ArgumentException("At least one determiner has to be provided", nameof(determiners));
			}

			this.determiners = determiners;
		}

		public IReadOnlyList<Type> RequiredEventTypes => determiners.SelectMany(x => x.RequiredEventTypes).Distinct().ToList();
		public IReadOnlyList<uint> RequiredBuffSkillIds => determiners.SelectMany(x => x.RequiredBuffSkillIds).Distinct().ToList();
		public IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults => determiners.SelectMany(x => x.RequiredPhysicalDamageEventResults).Distinct().ToList();

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			var e = events as Event[] ?? events.ToArray();
			var results = determiners.Select(x => x.GetResult(e)).ToArray();

			var firstSuccess = results.OrderBy(x => x.Time).FirstOrDefault(x => x.EncounterResult == EncounterResult.Success);
			if (firstSuccess != null)
			{
				return firstSuccess;
			}

			if (results.Any(x => x.EncounterResult == EncounterResult.Unknown))
			{
				return new ResultDeterminerResult(EncounterResult.Unknown, null);
			}

			return new ResultDeterminerResult(EncounterResult.Failure, null);
		}
	}
}