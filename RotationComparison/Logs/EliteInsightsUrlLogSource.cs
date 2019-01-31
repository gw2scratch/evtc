using System;
using System.Collections.Generic;
using ScratchEVTCParser.Statistics.PlayerDataParts;

namespace RotationComparison.Logs
{
	public class EliteInsightsUrlLogSource : ILogSource
	{
		private string dpsReportUrl;
		private string[] characterNames;

		public EliteInsightsUrlLogSource(string dpsReportUrl)
		{
			this.dpsReportUrl = dpsReportUrl;
		}

		public void SetCharacterNameFilter(string[] names)
		{
			characterNames = names;
		}

		public IEnumerable<PlayerRotation> GetRotations()
		{
			throw new NotImplementedException();
		}

		public string GetEncounterName()
		{
			throw new NotImplementedException();
		}

		public string GetLogName()
		{
			throw new NotImplementedException();
		}
	}
}