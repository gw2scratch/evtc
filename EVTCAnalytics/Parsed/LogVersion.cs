namespace GW2Scratch.EVTCAnalytics.Parsed
{
	public class LogVersion
	{
		/// <summary>
		/// The version of arcdps. The typical format is EVTCYYYYMMDD, where YYYY-MM-DD
		/// is the release date of this arcdps build.
		/// <remarks>
		/// The author of arcdps tries to avoid releasing two builds on the same day,
		/// so the versions should be unique.
		/// </remarks>
		/// </summary>
		public string BuildVersion { get; }

		/// <summary>
		/// The revision of the binary log storage format.
		/// </summary>
		public byte Revision { get; }

		public LogVersion(string buildVersion, byte revision)
		{
			BuildVersion = buildVersion;
			Revision = revision;
		}
	}
}