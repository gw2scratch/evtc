using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner base that returns success if an event was found.
	/// </summary>
	public abstract class EventFoundResultDeterminer : IResultDeterminer
	{
		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			var foundEvent = GetEvent(events);

			var result = foundEvent != null ? EventFound : EventNotFound;
			var time = foundEvent?.Time;

			return new ResultDeterminerResult(result, time);
		}

		public abstract IReadOnlyList<Type> RequiredEventTypes { get; }
		public abstract IReadOnlyList<uint> RequiredBuffSkillIds { get; }

		/// <summary>
		/// Gets the result returned if an event is found.
		/// </summary>
		protected virtual EncounterResult EventFound { get; } = EncounterResult.Success;

		/// <summary>
		/// Gets the result returned if no event is found.
		/// </summary>
		protected virtual EncounterResult EventNotFound { get; } = EncounterResult.Failure;

		/// <summary>
		/// Finds an event or returns <see langword="null"/> if no event was found.
		/// </summary>
		protected abstract Event GetEvent(IEnumerable<Event> events);
	}
}