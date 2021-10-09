namespace GW2Scratch.EVTCAnalytics.Parsed
{
	/// <summary>
	/// Log version data.
	/// </summary>
	public readonly struct LogVersion
	{
		/// <summary>
		/// Gets the version of arcdps. The typical format is EVTCYYYYMMDD, where YYYY-MM-DD
		/// is the release date of this arcdps build.
		/// <remarks>
		/// The author of arcdps tries to avoid releasing two builds on the same day,
		/// so the versions should be unique.
		/// </remarks>
		/// </summary>
		public string BuildVersion { get; }

		/// <summary>
		/// Gets the revision of the binary log storage format.
		/// </summary>
		public byte Revision { get; }

		public LogVersion(string buildVersion, byte revision)
		{
			BuildVersion = buildVersion;
			Revision = revision;
		}
	}
}