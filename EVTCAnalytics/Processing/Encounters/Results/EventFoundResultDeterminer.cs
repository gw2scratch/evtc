using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	public abstract class EventFoundResultDeterminer : IResultDeterminer
	{
		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			var foundEvent = GetEvent(events);

			var result = foundEvent != null ? EventFound : EventNotFound;
			var time = foundEvent?.Time;

			return new ResultDeterminerResult(result, time);
		}

		protected virtual EncounterResult EventFound { get; } = EncounterResult.Success;
		protected virtual EncounterResult EventNotFound { get; } = EncounterResult.Failure;

		protected abstract Event GetEvent(IEnumerable<Event> events);
	}
}