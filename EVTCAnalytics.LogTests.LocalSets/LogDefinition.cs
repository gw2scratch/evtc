using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.LogTests.LocalSets
{
	public class LogDefinition
	{
		public string Filename { get; set; }
		public Encounter? Encounter { get; set; }
		public EncounterResult? Result { get; set; }
		public EncounterMode? Mode { get; set; }
		public List<LogPlayer> Players { get; set; }
		public string Comment { get; set; }
	}
}