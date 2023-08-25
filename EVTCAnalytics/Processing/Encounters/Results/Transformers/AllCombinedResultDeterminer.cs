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
		public enum TimeSelection
		{
			Max,
			Min
		}

		private readonly IResultDeterminer[] determiners;
		private readonly TimeSelection timeSelection;

		public AllCombinedResultDeterminer(TimeSelection timeSelection, params IResultDeterminer[] determiners)
		{
			if (determiners.Length == 0)
			{
				throw new ArgumentException("At least one determiner has to be provided", nameof(determiners));
			}

			this.timeSelection = timeSelection;
			this.determiners = determiners;
		}
		
		/// <summary>
		/// Time selection is defaulted to <see cref="TimeSelection.Max"/>.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown when <paramref name="determiners"/> is empty.</exception>
		public AllCombinedResultDeterminer(params IResultDeterminer[] determiners)
		{
			if (determiners.Length == 0)
			{
				throw new ArgumentException("At least one determiner has to be provided", nameof(determiners));
			}

			timeSelection = TimeSelection.Max;
			this.determiners = determiners;
		}

		public IReadOnlyList<Type> RequiredEventTypes => determiners.SelectMany(x => x.RequiredEventTypes).Distinct().ToList();
		public IReadOnlyList<uint> RequiredBuffSkillIds => determiners.SelectMany(x => x.RequiredBuffSkillIds).Distinct().ToList();

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
					time = timeSelection switch
					{
						TimeSelection.Max => results.Where(x => x.Time.HasValue).Select(x => x.Time).Max(),
						TimeSelection.Min => results.Where(x => x.Time.HasValue).Select(x => x.Time).Min(),
						_ => throw new ArgumentOutOfRangeException()
					};
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