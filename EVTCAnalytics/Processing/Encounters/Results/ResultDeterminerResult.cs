namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A container for the output of a <see cref="IResultDeterminer"/>.
	/// Contains the result of the encounter and the time it was identified.
	/// </summary>
	public class ResultDeterminerResult
	{
		/// <summary>
		/// Gets the result of the encounter.
		/// </summary>
		public EncounterResult EncounterResult { get; }

		/// <summary>
		/// Gets the earliest time the result was identified or null in case it happens
		/// at the end of a log (usually because an event is not present).
		/// </summary>
		public long? Time { get; }

		public ResultDeterminerResult(EncounterResult encounterResult, long? time)
		{
			EncounterResult = encounterResult;
			Time = time;
		}
	}
}