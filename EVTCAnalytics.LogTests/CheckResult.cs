using System;
using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace EVTCAnalytics.LogTests
{
	public class CheckResult
	{
		public bool Correct { get; set; }
		public bool ProcessingFailed { get; set; }
		public Exception ProcessingException { get; set; }
		public Result<Encounter> Encounter { get; set; }
		public Result<EncounterResult> Result { get; set; }
		public Result<EncounterMode> Mode { get; set; }
		public Result<List<LogPlayer>> Players { get; set; }
	}
}