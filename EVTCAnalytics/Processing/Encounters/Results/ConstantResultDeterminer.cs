using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using System;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that always returns the same provided result.
	/// </summary>
	public class ConstantResultDeterminer : IResultDeterminer
	{
		private readonly EncounterResult result;

		public ConstantResultDeterminer(EncounterResult result)
		{
			this.result = result;
		}
		
		public IReadOnlyList<Type> RequiredEventTypes { get; } = new List<Type>();
		public IReadOnlyList<uint> RequiredBuffSkillIds { get; } = new List<uint>();
		public IReadOnlyList<PhysicalDamageEvent.Result> RequiredPhysicalDamageEventResults { get; } = new List<PhysicalDamageEvent.Result>();

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			return new ResultDeterminerResult(result, null);
		}
	}
}