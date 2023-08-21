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
		private readonly long minTimeSinceStart;

		/// <summary>
		/// Creates a new <see cref="GroupedEventDeterminer{T}"/>.
		/// </summary>
		/// <param name="eventCounted">A predicate checking whether an event is counted or not.</param>
		/// <param name="count">The required amount of counted events to result in success.</param>
		/// <param name="timeSpan">The time span in which <paramref name="count"/> events have to occur.</param>
		/// <param name="minTimeSinceStart">The time that has to have occured since the first event of any kind for this to be eligible.</param>
		public GroupedEventDeterminer(Func<T, bool> eventCounted, int count, long timeSpan, long minTimeSinceStart = 0)
		{
			this.eventCounted = eventCounted ?? throw new ArgumentNullException(nameof(eventCounted));
			this.count = count;
			this.timeSpan = timeSpan;
			this.minTimeSinceStart = minTimeSinceStart;
		}

		public IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type> { typeof(T) };

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			var countedEvents = new LinkedList<T>();

			long? startTime = null;
			
			foreach (var e in events)
			{
				if (startTime == null && e ! is UnknownEvent)
				{
					startTime = e.Time;
				}
				
				var isEligibleTime = startTime != null && e.Time - startTime >= minTimeSinceStart;

				if (e is T current && eventCounted(current) && isEligibleTime)
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
			}

			return new ResultDeterminerResult(EncounterResult.Failure, null);
		}
	}
}
