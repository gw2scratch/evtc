namespace ScratchEVTCParser.Parsed
{
	public class LogVersion
	{
		public string BuildVersion { get; }
		public byte Revision { get; }

		public LogVersion(string buildVersion, byte revision)
		{
			BuildVersion = buildVersion;
			Revision = revision;
		}
	}
}