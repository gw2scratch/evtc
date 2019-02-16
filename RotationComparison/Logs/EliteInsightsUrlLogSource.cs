using System;
using System.Collections.Generic;
using RotationComparison.Rotations;

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

		public IEnumerable<Rotation> GetRotations()
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