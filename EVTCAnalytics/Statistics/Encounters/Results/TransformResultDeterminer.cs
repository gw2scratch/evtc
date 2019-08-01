using System;
using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results
{
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

		public EncounterResult GetResult(IEnumerable<Event> events)
		{
			return transformation(resultDeterminer.GetResult(events));
		}
	}
}