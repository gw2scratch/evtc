using System;
using System.Collections.Generic;
using GW2Scratch.RotationComparison.Rotations;

namespace GW2Scratch.RotationComparison.Logs
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
			return dpsReportUrl;
		}
	}
}