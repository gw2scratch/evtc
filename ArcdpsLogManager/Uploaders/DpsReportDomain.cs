namespace GW2Scratch.ArcdpsLogManager.Uploaders
{
	public class DpsReportDomain
	{
		public string Domain { get; }
		public string Description { get; }

		public DpsReportDomain(string domain, string description)
		{
			Domain = domain;
			Description = description;
		}
	}
}