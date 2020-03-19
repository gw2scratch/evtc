using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	public class LogProcessorContext
	{
		public List<Agent> Agents { get; set; }
		public List<Skill> Skills { get; set; }
		public List<Event> Events { get; set; }
		public LogTime LogStartTime { get; set; }
		public LogTime LogEndTime { get; set; }
		public Player PointOfView { get; set; }
		public string EvtcVersion { get; set; }
		public int? GameBuild { get; set; }
		public int? GameShardId { get; set; }
		public int? GameLanguageId { get; set; }
		public int? MapId { get; set; }
		public LogType LogType { get; set; }
		public GameLanguage GameLanguage { get; set; }
		public IEncounterData EncounterData { get; set; }
		public bool AwareTimesSet { get; set; }
		public bool MastersAssigned { get; set; }
	}
}