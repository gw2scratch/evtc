using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Transformers
{
	/// <summary>
	/// Removes the time information from a wrapped <see cref="IResultDeterminer"/>.
	/// </summary>
	/// <remarks>
	/// This is useful in cases where an event does not occur at the end of a log,
	/// but there is nothing reliable that can actually be checked at the end of the log.
	/// </remarks>
	public class IgnoreTimeResultDeterminerWrapper : IResultDeterminer
	{
		private readonly IResultDeterminer resultDeterminer;

		public IgnoreTimeResultDeterminerWrapper(IResultDeterminer resultDeterminer)
		{
			this.resultDeterminer = resultDeterminer;
		}

		public IReadOnlyList<Type> RequiredEventTypes => resultDeterminer.RequiredEventTypes;
		public IReadOnlyList<uint> RequiredBuffSkillIds => resultDeterminer.RequiredBuffSkillIds;
		public IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults => resultDeterminer.RequiredPhysicalDamageEventResults;

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			var result = resultDeterminer.GetResult(events);
			return new ResultDeterminerResult(result.EncounterResult, null);
		}
	}
}