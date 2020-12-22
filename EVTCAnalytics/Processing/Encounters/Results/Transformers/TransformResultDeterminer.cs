using System;
using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results.Transformers
{
	/// <summary>
	/// Transforms a result from a <see cref="IResultDeterminer"/> by using a specified function afterwards.
	/// </summary>
	public class TransformResultDeterminer : IResultDeterminer
	{
		private readonly IResultDeterminer resultDeterminer;
		private readonly Func<EncounterResult, EncounterResult> transformation;

		public TransformResultDeterminer(IResultDeterminer resultDeterminer,
			Func<EncounterResult, EncounterResult> transformation)
		{
			this.resultDeterminer = resultDeterminer;
			this.transformation = transformation;
		}

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			var result = resultDeterminer.GetResult(events);
			return new ResultDeterminerResult(transformation(result.EncounterResult), result.Time);
		}
	}
}