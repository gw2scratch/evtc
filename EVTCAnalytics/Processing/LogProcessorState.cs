using System.Collections.Generic;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Effects;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Processing.Encounters;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	/// <summary>
	/// Contains the state of the log processor.
	/// </summary>
	public class LogProcessorState
	{
		public List<Agent> Agents { get; set; }
		public Dictionary<ulong, Agent> AgentsByAddress { get; set; }
		public Dictionary<int, List<Agent>> AgentsById { get; set; }
		public Dictionary<uint, Skill> SkillsById { get; set; }
		public Dictionary<uint, Effect> EffectsById { get; set; }
		public Dictionary<uint, Marker> MarkersById { get; set; }
		public Dictionary<uint, Species> SpeciesById { get; set; }
		public Dictionary<uint, Team> TeamsById { get; set; }
		public List<Skill> Skills { get; set; }
		public List<Event> Events { get; set; }
		public List<LogError> Errors { get; set; }
		public LogTime LogStartTime { get; set; }
		public LogTime LogEndTime { get; set; }
		public Player PointOfView { get; set; }
		public string EvtcVersion { get; set; }
		public InstanceStart InstanceStart { get; set; }
		public int? GameBuild { get; set; }
		public int? GameShardId { get; set; }
		public int? GameLanguageId { get; set; }
		public int? MapId { get; set; }
		public int? FractalScale { get; set; }
		public LogType LogType { get; set; }
		public GameLanguage GameLanguage { get; set; }
		public IEncounterData EncounterData { get; set; }
		public bool AwareTimesSet { get; set; }
		public bool MastersAssigned { get; set; }
		public Agent MainTarget { get; set; }
		public string ArcdpsBuild { get; set; }
		public Dictionary<uint, EffectStartEvent> OngoingEffects { get; set; }
	}
}