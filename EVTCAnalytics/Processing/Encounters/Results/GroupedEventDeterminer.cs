using System;
using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if an event fulfilling a condition
	/// occurred at least a given amount of times in a timespan.
	/// </summary>
	/// <typeparam name="T">The type of the event to be considered</typeparam>
	public class GroupedEventDeterminer<T> : IResultDeterminer where T : Event
	{
		private readonly Func<T, bool> eventCounted;
		private readonly int count;
		private readonly long timeSpan;

		/// <summary>
		/// Creates a new <see cref="GroupedEventDeterminer{T}"/>.
		/// </summary>
		/// <param name="eventCounted">A predicate checking whether an event is counted or not.</param>
		/// <param name="count">The required amount of counted events to result in success.</param>
		/// <param name="timeSpan">The time span in which <paramref name="count"/> events have to occur.</param>
		public GroupedEventDeterminer(Func<T, bool> eventCounted, int count, long timeSpan)
		{
			this.eventCounted = eventCounted ?? throw new ArgumentNullException(nameof(eventCounted));
			this.count = count;
			this.timeSpan = timeSpan;
		}

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			var countedEvents = new LinkedList<T>();

			foreach (var current in events.OfType<T>().Where(x => eventCounted(x)))
			{
				while (countedEvents.Count > 0 && countedEvents.First.Value.Time < current.Time - timeSpan)
				{
					countedEvents.RemoveFirst();
				}

				countedEvents.AddLast(current);

				if (countedEvents.Count >= count)
				{
					return new ResultDeterminerResult(EncounterResult.Success, current.Time);
				}
			}

			return new ResultDeterminerResult(EncounterResult.Failure, null);
		}
	}
}
