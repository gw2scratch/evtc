namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A class containing the output of a <see cref="IResultDeterminer"/>.
	/// Contains the result of the encounter and the time it was identified.
	/// </summary>
	public class ResultDeterminerResult
	{
		/// <summary>
		/// The result of the encounter.
		/// </summary>
		public EncounterResult EncounterResult { get; }

		/// <summary>
		/// Earliest time the result was identified or null in case it happens
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