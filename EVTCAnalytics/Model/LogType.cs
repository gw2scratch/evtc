namespace GW2Scratch.EVTCAnalytics.Model
{
	/// <summary>
	/// The type of the log, distinguished by the restrictions on data available.
	/// </summary>
	public enum LogType
	{
		/// <summary>
		/// PvE logs are recorded in PvE instances and have no significant restrictions.
		/// </summary>
		PvE,
		/// <summary>
		/// World vs World logs, with restrictions on the information available about enemy players.
		/// </summary>
		WorldVersusWorld
	}
}