using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Exceptions;
using GW2Scratch.EVTCAnalytics.GameData;
using GW2Scratch.EVTCAnalytics.IO;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Effects;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;
using System.Text;

namespace GW2Scratch.EVTCAnalytics.Processing
{
	/// <summary>
	/// The log processor responsible for turning raw log data into
	/// easy-to-use objects and providing encounter-specific data.
	/// </summary>
	public class LogProcessor
	{
		// Professions in order of arcdps ids; id = index + 1
		private static readonly Profession[] Professions =
		{
			Profession.Guardian,
			Profession.Warrior,
			Profession.Engineer,
			Profession.Ranger,
			Profession.Thief,
			Profession.Elementalist,
			Profession.Mesmer,
			Profession.Necromancer,
			Profession.Revenant,
		};

		/// <summary>
		/// Gets or sets the encounter identifier.
		/// </summary>
		/// <remarks>
		/// This may be used to add support for more encounters by extending the default encounter identifier
		/// or it could be used to override the default logic.
		/// </remarks>
		public IEncounterIdentifier EncounterIdentifier { get; set; } = new EncounterIdentifier();
		
		/// <summary>
		/// Gets or sets the encounter-specific data provider.
		/// </summary>
		/// <remarks>
		/// This may be used to add support for more encounters by extending the default encounter data provider
		/// or it could be used to override the default logic.
		/// </remarks>
		public IEncounterDataProvider EncounterDataProvider { get; set; } = new EncounterDataProvider();

		/// <summary>
		/// Gets or sets the handling method for unknown events. The default value is <see langword="true"/>.
		/// </summary>
		public bool IgnoreUnknownEvents { get; set; } = true;

		/// <summary>
		/// Turn raw log data into easy-to-use objects.
		/// </summary>
		/// <remarks>
		/// This method is less efficient than using <see cref="ProcessLog(string, EVTCParser)"/>
		/// and <see cref="ProcessLog(byte[], EVTCParser)"/> if you do not use the <see cref="ParsedLog"/>
		/// in any other way.
		/// </remarks>
		/// <seealso cref="ProcessLog(string, EVTCParser)"/>
		/// <seealso cref="ProcessLog(byte[], EVTCParser)"/>
		public Log ProcessLog(ParsedLog log)
		{
			var state = new LogProcessorState();
			state.EvtcVersion = log.LogVersion.BuildVersion;

			state.Agents = GetAgents(log).ToList();
			state.AgentsByAddress = new Dictionary<ulong, Agent>();
			state.AgentsById = new Dictionary<int, List<Agent>>();
			state.EffectsById = new Dictionary<uint, Effect>();
			state.MarkersById = new Dictionary<uint, Marker>();
			state.Errors = new List<LogError>();
			foreach (var agent in state.Agents)
			{
				foreach (var origin in agent.AgentOrigin.OriginalAgentData)
				{
					state.AgentsByAddress[origin.Address] = agent;

					int id = origin.Id;
					if (!state.AgentsById.TryGetValue(id, out var agentsWithId))
					{
						agentsWithId = new List<Agent>();
						state.AgentsById[id] = agentsWithId;
					}

					agentsWithId.Add(agent);
				}
			}

			state.Skills = GetSkills(log).ToList();
			GetDataFromCombatItems(log, state);

			SetLogTypeAndTarget(state, log.ParsedBossData);

			SetAgentAwareTimes(state);
			AssignAgentMasters(log, state); // Has to be done after setting aware times

			SetEncounterData(state);

			foreach (var step in state.EncounterData.ProcessingSteps)
			{
				step.Process(state);
			}

			return new Log(state);
		}

		/// <summary>
		/// Turn raw log data into easy-to-use objects. This overload is more efficient if you do not need a <see cref="ParsedLog"/>
		/// </summary>
		public Log ProcessLog(string evtcFilename, EVTCParser parser)
		{
			return ProcessLog(parser.GetSinglePassReader(evtcFilename));
		}

		public Log ProcessLog(byte[] bytes, EVTCParser parser)
		{
			return ProcessLog(parser.GetSinglePassReader(bytes));
		}

		private Log ProcessLog(EVTCParser.SinglePassEVTCReader reader)
		{
			// For now, we mostly duplicate ProcessLog(ParsedLog log) above.
			// It might be worth it to unify the interface somehow.
			
			// Note that we are trying to reduce allocations of Parsed structs here
			
			var state = new LogProcessorState();

			var logVersion = reader.ReadLogVersion();
			state.EvtcVersion = logVersion.BuildVersion;
			var bossData = reader.ReadBossData();

			// Get data from agents.
			var agentsByAddress = new Dictionary<ulong, Agent>();
			var agentReader = reader.GetAgentReader();
			var playerAddresses = new HashSet<ulong>();
			ParsedAgent parsedAgent;
			state.Agents = new List<Agent>();
			while (agentReader.GetNext(out parsedAgent))
			{
				var agent = ProcessAgent(parsedAgent, playerAddresses);
				if (agent != null)
				{
					state.Agents.Add(agent);
					agentsByAddress[parsedAgent.Address] = agent;
				}
			}
			state.AgentsByAddress = agentsByAddress;
			
			SetLogTypeAndTarget(state, bossData);
			
			// Get data from skills.
			var skillsById = new Dictionary<uint, Skill>();
			var skillReader = reader.GetSkillReader();
			ParsedSkill parsedSkill;
			state.Skills = new List<Skill>();
			while (skillReader.GetNext(out parsedSkill))
			{
				var skill = ProcessSkill(parsedSkill);
				// Rarely, in old logs a skill may be duplicated (typically a skill with id 0),
				// so we only use the first definition
				if (!skillsById.ContainsKey(skill.Id))
				{
					skillsById.Add(skill.Id, skill);
				}
				state.Skills.Add(skill);
			}

			state.SkillsById = skillsById;

			// Get data from combat items.
			var masterRelations = new Dictionary<(ulong, ushort), long>();
			var idsByAddress = new Dictionary<ulong, int>();
			
			state.GameLanguage = GameLanguage.Other;
			state.Events = new List<Event>();
			state.Errors = new List<LogError>();
			state.EffectsById = new Dictionary<uint, Effect>();
			state.MarkersById = new Dictionary<uint, Marker>();

			var combatItemReader = reader.GetCombatItemReader(bossData, state.MainTarget, state.Agents, state.GameBuild, state.LogType, EncounterIdentifier, EncounterDataProvider);
			ParsedCombatItem combatItem;
			state.Events = new List<Event>();
			while (combatItemReader.GetNext(out combatItem))
			{
				if (combatItem.IsStateChange == StateChange.Normal)
				{
					idsByAddress[combatItem.SrcAgent] = combatItem.SrcAgentId;
					
					if (combatItem.SrcMasterId != 0)
					{
						masterRelations[(combatItem.SrcAgent, combatItem.SrcMasterId)] = combatItem.Time;
					}

					if (combatItem.DstMasterId != 0)
					{
						masterRelations[(combatItem.DstAgent, combatItem.DstMasterId)] = combatItem.Time;
					}
				}
				else if (combatItem.IsStateChange == StateChange.AttackTarget)
				{
					ulong attackTargetAddress = combatItem.SrcAgent;
					ulong masterGadgetAddress = combatItem.DstAgent;

					AttackTarget target = null;
					Gadget master = null;
					foreach (var agent in state.Agents)
					{
						switch (agent)
						{
							case Gadget gadget when gadget.AgentOrigin.OriginalAgentData.Any(x=> x.Address == masterGadgetAddress):
								master = gadget;
								break;
							case AttackTarget attackTarget when attackTarget.AgentOrigin.OriginalAgentData.Any(x => x.Address == attackTargetAddress):
								target = attackTarget;
								break;
						}
					}

					if (master != null && target != null)
					{
						master.AddAttackTarget(target);
						target.Gadget = master;
					}
				}
				
				ProcessCombatItem(state, combatItem);
			}

			// Set agent origins now that we have read combat items and have ids.
			foreach ((ulong address, Agent agent) in agentsByAddress)
			{
				if (!idsByAddress.TryGetValue(address, out int id))
				{
					id = -1;
				}
				agent.AgentOrigin = new AgentOrigin(new OriginalAgentData(address, id));
			}
			
			// Build map of id->agents.
			state.AgentsById = new Dictionary<int, List<Agent>>();
			foreach (var agent in state.Agents)
			{
				foreach (var origin in agent.AgentOrigin.OriginalAgentData)
				{
					int id = origin.Id;
					if (!state.AgentsById.TryGetValue(id, out var agentsWithId))
					{
						agentsWithId = new List<Agent>();
						state.AgentsById[id] = agentsWithId;
					}

					agentsWithId.Add(agent);
				}
			}
			
			SetAgentAwareTimes(state);
			
			// Resolve masters, requires aware times.
			foreach (((ulong address, ushort masterId), long time) in masterRelations)
			{
				if (!state.AgentsByAddress.TryGetValue(address, out Agent minion))
				{
					continue;
				}

				if (minion.Master == null)
				{
					AssignMaster(state, minion, masterId, time);
				}
			}
			state.MastersAssigned = true;

			SetEncounterData(state);

			foreach (var step in state.EncounterData.ProcessingSteps)
			{
				step.Process(state);
			}

			return new Log(state);
		}

		private static void AssignMaster(LogProcessorState state, Agent minion, ushort masterId, long time)
		{
			Agent master = null;
			if (state.AgentsById.TryGetValue(masterId, out var potentialMasters))
			{
				foreach (var agent in potentialMasters)
				{
					if (!(agent is Gadget) && agent.IsWithinAwareTime(time))
					{
						master = agent;
						break;
					}
				}
			}

			if (master != null)
			{
				bool inCycle = false;
				var masterParent = master;
				while (masterParent != null)
				{
					if (masterParent == minion)
					{
						// A cycle is present in the minion hierarchy, this minion would end up as
						// a transitive minion of itself, which could cause infinite looping.
						// This is common in very old logs where minion data is somewhat weird.
						inCycle = true;
						break;
					}

					masterParent = masterParent.Master;
				}

				if (!inCycle)
				{
					minion.Master = master;
					master.MinionList.Add(minion);
				}
			}
		}

		private static void SetLogTypeAndTarget(LogProcessorState state, ParsedBossData bossData)
		{
			Debug.Assert(state.Agents != null);
			
			state.LogType = LogType.PvE;
			if (bossData.ID == ArcdpsBossIds.WorldVersusWorld)
			{
				state.LogType = LogType.WorldVersusWorld;
			}
			else if (bossData.ID == ArcdpsBossIds.Map)
			{
				state.LogType = LogType.Map;
			}
			else
			{
				// The boss id may be either an NPC species id or a Gadget id.
				// Conflicts may happen, in that case the first found agent is chosen,
				// as they are more likely to be the trigger. It is also possible to have
				// multiple agents with the same id if they are not unique, in that case
				// we once again choose the first one.
				foreach (var agent in state.Agents)
				{
					if (agent is NPC npc && npc.SpeciesId == bossData.ID ||
					    agent is Gadget gadget && gadget.VolatileId == bossData.ID)
					{
						state.MainTarget = agent;
						break;
					}
				}
			}
		}

		private void SetEncounterData(LogProcessorState state)
		{
			var encounter = EncounterIdentifier.IdentifyEncounter(state.MainTarget, state.Agents, state.Events, state.Skills);
			state.EncounterData = EncounterDataProvider.GetEncounterData(encounter, state.MainTarget, state.Agents, state.GameBuild, state.LogType);
		}

		private IEnumerable<Skill> GetSkills(ParsedLog log)
		{
			foreach (var skill in log.ParsedSkills)
			{
				yield return ProcessSkill(skill);
			}
		}

		private Skill ProcessSkill(ParsedSkill skill)
		{
			var name = skill.Name.Trim('\0');
			uint skillId = checked((uint) skill.SkillId);
			return new Skill(skillId, name);
		}

		private IEnumerable<Agent> GetAgents(ParsedLog log)
		{
			var idsByAddress = new Dictionary<ulong, int>();
			foreach (var combatItem in log.ParsedCombatItems)
			{
				if (combatItem.IsStateChange == StateChange.Normal)
				{
					idsByAddress[combatItem.SrcAgent] = combatItem.SrcAgentId;
				}
			}

			var playerAddresses = new HashSet<ulong>();

			foreach (var agent in log.ParsedAgents)
			{
				var processedAgent = ProcessAgent(agent, playerAddresses);
				if (processedAgent != null)
				{
					if (!idsByAddress.TryGetValue(agent.Address, out int id))
					{
						id = -1;
					}
					processedAgent.AgentOrigin = new AgentOrigin(new OriginalAgentData(agent.Address, id));
					yield return processedAgent;
				}
			}
		}
		
		private Agent ProcessAgent(in ParsedAgent agent, HashSet<ulong> playerAddresses)
		{
			// We do not set agent origins, instead we rely on the caller to set those. This allows us to process
			// agents without having to enumerate all combat items first.
			
			if (agent.IsElite != 0xFFFFFFFF)
			{
				// Player
				uint professionIndex = agent.Prof - 1;
				Profession profession;

				// We need to check if the profession is valid.
				// The 2021-05-25 game update caused old versions of arcdps to report invalid profession values.
				// Future game versions might also introduce new professions.
				if (professionIndex < Professions.Length)
				{
					profession = Professions[professionIndex];
				}
				else
				{
					profession = Profession.None;
				}

				EliteSpecialization specialization;
				if (agent.IsElite == 0)
				{
					specialization = EliteSpecialization.None;
				}
				else if (agent.IsElite == 1)
				{
					specialization = Characters.GetHeartOfThornsEliteSpecialization(profession);
				}
				else
				{
					specialization = Characters.GetEliteSpecializationFromId(agent.IsElite);
				}

				// All parts of the name of the player might not be available, most commonly in WvW logs.
				var nameParts = agent.Name.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
				string characterName = nameParts[0];
				string accountName = nameParts.Length > 1 ? nameParts[1] : ":Unknown";
				string subgroupLiteral = nameParts.Length > 2 ? nameParts[2] : "";
				bool identified = nameParts.Length >= 3;

				if (!int.TryParse(subgroupLiteral, out int subgroup))
				{
					subgroup = -1;
				}

				if (playerAddresses.Contains(agent.Address))
				{
					// If there is already an agent with this address, this is the same player after changing
					// characters. For now we simply merge everything into the first instance, even though that
					// may result in a player using skills of a different profession, a better solution will be
					// needed for the future.
					return null;
				}

				playerAddresses.Add(agent.Address);

				return new Player(null, characterName, agent.Toughness, agent.Concentration,
					agent.Healing, agent.Condition, agent.HitboxWidth, agent.HitboxHeight, accountName, profession,
					specialization, subgroup, identified);
			}
			else
			{
				if (agent.Prof >> 16 == 0xFFFF)
				{
					// Gadget or Attack Target
					int volatileId = (int)(agent.Prof & 0xFFFF);
					string name = agent.Name.Trim('\0');

					var origin = new AgentOrigin(new OriginalAgentData(agent.Address, volatileId));
					if (name.StartsWith("at"))
					{
						// The attack target name is structured as "at[MasterAddress]-[GadgetId]-[MasterId]"
						// Preferably, the AttackTarget statechange would be used to detect if this is an
						// attack target to not rely on the name which could change in the future
						return new AttackTarget(origin, volatileId, name, agent.HitboxWidth,
							agent.HitboxHeight);
					}
					else
					{
						return new Gadget(origin, volatileId, name, agent.HitboxWidth,
							agent.HitboxHeight);
					}
				}
				else
				{
					// NPC
					string name = agent.Name.Trim('\0');
					int speciesId = (int)(agent.Prof & 0xFFFF);

					return new NPC(null, name, speciesId, agent.Toughness, agent.Concentration,
						agent.Healing, agent.Condition, agent.HitboxWidth, agent.HitboxHeight);
				}
			}
		}

		private void SetAgentAwareTimes(LogProcessorState state)
		{
			foreach (var ev in state.Events)
			{
				if (ev is AgentEvent agentEvent)
				{
					if (agentEvent.Agent == null) continue;

					if (agentEvent.Agent.FirstAwareTime == 0 || ev.Time < agentEvent.Agent.FirstAwareTime)
					{
						agentEvent.Agent.FirstAwareTime = ev.Time;
					}

					if (agentEvent.Agent.LastAwareTime == long.MaxValue || ev.Time > agentEvent.Agent.LastAwareTime)
					{
						agentEvent.Agent.LastAwareTime = ev.Time;
					}
				}
				else if (ev is DamageEvent damageEvent)
				{
					if (damageEvent.Attacker != null)
					{
						if (damageEvent.Attacker.FirstAwareTime == 0 || ev.Time < damageEvent.Attacker.FirstAwareTime)
						{
							damageEvent.Attacker.FirstAwareTime = ev.Time;
						}

						if (damageEvent.Attacker.LastAwareTime == long.MaxValue ||
						    ev.Time > damageEvent.Attacker.LastAwareTime)
						{
							damageEvent.Attacker.LastAwareTime = ev.Time;
						}
					}

					if (damageEvent.Defender != null)
					{
						if (damageEvent.Defender.FirstAwareTime == 0 || ev.Time < damageEvent.Defender.FirstAwareTime)
						{
							damageEvent.Defender.FirstAwareTime = ev.Time;
						}

						if (damageEvent.Defender.LastAwareTime == long.MaxValue ||
						    ev.Time > damageEvent.Defender.LastAwareTime)
						{
							damageEvent.Defender.LastAwareTime = ev.Time;
						}
					}
				}
			}

			state.AwareTimesSet = true;
		}

		private void AssignAgentMasters(ParsedLog log, LogProcessorState state)
		{
			// Requires aware times to be set first
			Debug.Assert(state.AwareTimesSet);
			Debug.Assert(!state.MastersAssigned);
			Debug.Assert(state.AgentsByAddress != null);
			Debug.Assert(state.AgentsById != null);

			foreach (var combatItem in log.ParsedCombatItems)
			{
				if (combatItem.IsStateChange == StateChange.Normal)
				{
					if (combatItem.SrcMasterId != 0)
					{
						if (!state.AgentsByAddress.TryGetValue(combatItem.SrcAgent, out Agent minion))
						{
							continue;
						}
						
						if (minion != null && minion.Master == null)
						{
							AssignMaster(state, minion, combatItem.SrcMasterId, combatItem.Time);
						}
					}
					
					if (combatItem.DstMasterId != 0)
					{
						if (!state.AgentsByAddress.TryGetValue(combatItem.DstAgent, out Agent minion))
						{
							continue;
						}
						
						if (minion != null && minion.Master == null)
						{
							AssignMaster(state, minion, combatItem.DstMasterId, combatItem.Time);
						}
					}
				}

				if (combatItem.IsStateChange == StateChange.AttackTarget)
				{
					ulong attackTargetAddress = combatItem.SrcAgent;
					ulong masterGadgetAddress = combatItem.DstAgent;

					AttackTarget target = null;
					Gadget master = null;
					foreach (var agent in state.Agents)
					{
						switch (agent)
						{
							case Gadget gadget when gadget.AgentOrigin.OriginalAgentData.Any(x=> x.Address == masterGadgetAddress):
								master = gadget;
								break;
							case AttackTarget attackTarget when attackTarget.AgentOrigin.OriginalAgentData.Any(x => x.Address == attackTargetAddress):
								target = attackTarget;
								break;
						}
					}

					if (master != null && target != null)
					{
						master.AddAttackTarget(target);
						target.Gadget = master;
					}
				}
			}

			state.MastersAssigned = true;
		}
		
		private void GetDataFromCombatItems(ParsedLog log, LogProcessorState state)
		{
			Debug.Assert(state.Agents != null);
			Debug.Assert(state.Skills != null);
			Debug.Assert(state.AgentsByAddress != null);

			var skillsById = new Dictionary<uint, Skill>();
			foreach (var skill in state.Skills)
			{
				// Rarely, in old logs a skill may be duplicated (typically a skill with id 0),
				// so we only use the first definition
				if (!skillsById.ContainsKey(skill.Id))
				{
					skillsById.Add(skill.Id, skill);
				}
			}

			state.GameLanguage = GameLanguage.Other;
			state.Events = new List<Event>();
			state.SkillsById = skillsById;
			foreach (var item in log.ParsedCombatItems)
			{
				ProcessCombatItem(state, item);
			}
		}

		private void ProcessCombatItem(LogProcessorState state, in ParsedCombatItem item)
		{
			switch (item.IsStateChange)
			{
				case StateChange.LogStart when state.LogStartTime != null:
					throw new LogProcessingException("Multiple log start combat items found");
				case StateChange.LogStart:
				{
					var serverTime = DateTimeOffset.FromUnixTimeSeconds(item.Value);
					var localTime = DateTimeOffset.FromUnixTimeSeconds(item.BuffDmg);

					state.LogStartTime = new LogTime(localTime, serverTime, item.Time);
					return;
				}
				case StateChange.LogEnd when item.Value == 0 && item.BuffDmg == 0:
					// This is an erroneous extra log end without any data,
					// we ignore it. Happened in every log with EVTC20200506.
					return;
				case StateChange.LogEnd when state.LogEndTime != null:
					throw new LogProcessingException("Multiple log end combat items found");
				case StateChange.LogEnd:
				{
					var serverTime = DateTimeOffset.FromUnixTimeSeconds(item.Value);
					var localTime = DateTimeOffset.FromUnixTimeSeconds(item.BuffDmg);

					state.LogEndTime = new LogTime(localTime, serverTime, item.Time);
					return;
				}
				case StateChange.PointOfView:
				{
					if (state.AgentsByAddress.TryGetValue(item.SrcAgent, out var agent))
					{
						state.PointOfView = agent as Player ??
						                    throw new LogProcessingException("The point of view agent is not a player");
					}

					return;
				}
				case StateChange.Language:
				{
					int languageId = (int)item.SrcAgent;
					state.GameLanguageId = languageId;
					state.GameLanguage = GameLanguageIds.GetLanguageById(languageId);
					return;
				}
				case StateChange.GWBuild:
					state.GameBuild = (int)item.SrcAgent;
					return;
				case StateChange.ShardId:
					state.GameShardId = (int)item.SrcAgent;
					return;
				case StateChange.MapId:
					state.MapId = (int)item.SrcAgent;
					return;
				case StateChange.Guild:
				{
					if (state.AgentsByAddress.TryGetValue(item.SrcAgent, out Agent agent))
					{
						var player = (Player)agent;
						var guid = new byte[16];

						// It is unclear how the arcdps values would be stored on a big-endian platform
						Debug.Assert(BitConverter.IsLittleEndian);

						var dstBytes = BitConverter.GetBytes(item.DstAgent);
						var valueBytes = BitConverter.GetBytes(item.Value);
						var buffDamageBytes = BitConverter.GetBytes(item.BuffDmg);

						guid[0] = dstBytes[3];
						guid[1] = dstBytes[2];
						guid[2] = dstBytes[1];
						guid[3] = dstBytes[0];
						guid[4] = dstBytes[5];
						guid[5] = dstBytes[4];
						guid[6] = dstBytes[7];
						guid[7] = dstBytes[6];
						guid[8] = valueBytes[0];
						guid[9] = valueBytes[1];
						guid[10] = valueBytes[2];
						guid[11] = valueBytes[3];
						guid[12] = buffDamageBytes[0];
						guid[13] = buffDamageBytes[1];
						guid[14] = buffDamageBytes[2];
						guid[15] = buffDamageBytes[3];

						player.GuildGuid = guid;
					}

					return;
				}
				case StateChange.AttackTarget:
					// Only used for master assignment
					// Contains if the attack target is targetable as the value.
					return;
				case StateChange.BuffInfo:
				{
					if (state.SkillsById.TryGetValue(item.SkillId, out var skill))
					{
						bool isResistanceAvailable = string.Compare(state.EvtcVersion, "EVTC20200428", StringComparison.OrdinalIgnoreCase) >= 0;
						
						Span<byte> padding = stackalloc byte[4];
						BitConverter.TryWriteBytes(padding, item.Padding);
						
						skill.BuffData = new BuffData
						{
							IsInversion = item.IsShields != 0,
							IsInvulnerability = item.IsFlanking != 0,
							IsResistance = isResistanceAvailable ? padding[1] != 0 : null,
							Category = (BuffCategory) item.IsOffCycle,
							StackingType = padding[0],
							MaxStacks = item.SrcMasterId,
							DurationCap = item.OverstackValue
						};
						
					}

					return;
				}
				case StateChange.SkillInfo:
				{
					// (float*)&time[4]
					// recharge range0 range1 tooltiptime
					Span<byte> bytes = stackalloc byte[16];
					BitConverter.TryWriteBytes(bytes[..8], item.Time);
					BitConverter.TryWriteBytes(bytes[8..16], item.SrcAgent);
					if (state.SkillsById.TryGetValue(item.SkillId, out var skill))
					{
						skill.SkillData = new SkillData
						{
							Recharge = BitConverter.ToSingle(bytes[..4]),
							Range0 = BitConverter.ToSingle(bytes[4..8]),
							Range1 = BitConverter.ToSingle(bytes[8..12]),
							TooltipTime = BitConverter.ToSingle(bytes[12..16]),
						};
					}

					return;
				}
				case StateChange.InstanceStart:
					state.InstanceStart = new InstanceStart(item.SrcAgent);
					return;
				case StateChange.IdToGuid:
					if (item.OverstackValue == 0)
					{
						// Effect
						if (state.EffectsById.TryGetValue(item.SkillId, out var effect))
						{
							var guid = new byte[16];
							effect.ContentGuid = guid;
							BitConverter.GetBytes(item.SrcAgent).CopyTo(guid, 0);
							BitConverter.GetBytes(item.DstAgent).CopyTo(guid, 8);
						}
					}
					else if (item.OverstackValue == 1)
					{
						// Marker
						if (state.MarkersById.TryGetValue(item.SkillId, out var marker))
						{
							var guid = new byte[16];
							marker.ContentGuid = guid;
							BitConverter.GetBytes(item.SrcAgent).CopyTo(guid, 0);
							BitConverter.GetBytes(item.DstAgent).CopyTo(guid, 8);
						}
					}
					return;
				case StateChange.Error:
					Span<byte> error = stackalloc byte[32];
					BitConverter.GetBytes(item.Time).CopyTo(error[..8]);
					BitConverter.GetBytes(item.SrcAgent).CopyTo(error[8..16]);
					BitConverter.GetBytes(item.DstAgent).CopyTo(error[16..24]);
					BitConverter.GetBytes(item.Value).CopyTo(error[24..28]);
					BitConverter.GetBytes(item.BuffDmg).CopyTo(error[28..32]);
					var errorString = Encoding.UTF8.GetString(error);
					
					state.Errors.Add(new LogError(errorString));
					return;
				case StateChange.FractalScale:
					state.FractalScale = (int) item.SrcAgent;
					return;
				default:
				{
					var processedEvent = GetEvent(state, item);
					if (processedEvent is not UnknownEvent || !IgnoreUnknownEvents)
					{
						state.Events.Add(processedEvent);
					}

					return;
				}
			}
		}

		private Event GetEvent(LogProcessorState state, in ParsedCombatItem item)
		{
			Debug.Assert(state.AgentsByAddress != null);
			Debug.Assert(state.SkillsById != null);

			Agent GetAgentByAddress(ulong address)
			{
				if (state.AgentsByAddress.TryGetValue(address, out Agent agent))
				{
					return agent;
				}

				return null;
			}

			Skill GetSkillById(uint id)
			{
				if (state.SkillsById.TryGetValue(id, out Skill skill))
				{
					return skill;
				}

				return null;
			}
			
			Skill GetSkillByIdOrAdd(uint id)
			{
				if (state.SkillsById.TryGetValue(id, out Skill skill))
				{
					return skill;
				}

				var newSkill = new Skill(id, $"{id} (not in skill list)");
				state.Skills.Add(newSkill);
				state.SkillsById.Add(id, newSkill);
				return newSkill;
			}

			if (item.IsStateChange != StateChange.Normal)
			{
				switch (item.IsStateChange)
				{
					case StateChange.EnterCombat:
						return new AgentEnterCombatEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							(int) item.DstAgent);
					case StateChange.ExitCombat:
						return new AgentExitCombatEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.ChangeUp:
						return new AgentRevivedEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.ChangeDead:
						return new AgentDeadEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.ChangeDown:
						return new AgentDownedEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.Spawn:
						return new AgentSpawnEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.Despawn:
						return new AgentDespawnEvent(item.Time, GetAgentByAddress(item.SrcAgent));
					case StateChange.HealthUpdate:
						var healthFraction = item.DstAgent / 10000f;
						return new AgentHealthUpdateEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							healthFraction);
					case StateChange.WeaponSwap:
						WeaponSet newWeaponSet;
						switch (item.DstAgent)
						{
							case 0:
								newWeaponSet = WeaponSet.Water1;
								break;
							case 1:
								newWeaponSet = WeaponSet.Water2;
								break;
							case 4:
								newWeaponSet = WeaponSet.Land1;
								break;
							case 5:
								newWeaponSet = WeaponSet.Land2;
								break;
							default:
								newWeaponSet = WeaponSet.Unknown;
								break;
						}

						return new AgentWeaponSwapEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							newWeaponSet);
					case StateChange.MaxHealthUpdate:
						return new AgentMaxHealthUpdateEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							item.DstAgent);
					case StateChange.Reward:
						return new RewardEvent(item.Time, item.DstAgent, item.Value);
					case StateChange.BuffInitial:
					{
						// arcdps up to 20220628 missed skills referenced only from buff initials in the skill list.
						// To deal with that, we might need to add the skill if it does not exist.
						Skill buff = GetSkillByIdOrAdd(item.SkillId);
						
						int durationApplied = item.Value;
						uint durationOfRemovedStack = item.OverstackValue;
						var agent = GetAgentByAddress(item.DstAgent);
						var sourceAgent = GetAgentByAddress(item.SrcAgent);
						return new InitialBuffEvent(item.Time, agent, buff, sourceAgent, durationApplied,
							durationOfRemovedStack);
					}
					case StateChange.Position:
					{
						float x = BitConversions.ToSingle((uint) (item.DstAgent & 0xFFFFFFFF));
						float y = BitConversions.ToSingle((uint) (item.DstAgent >> 32 & 0xFFFFFFFF));
						float z = BitConversions.ToSingle(item.Value);

						return new PositionChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y, z);
					}
					case StateChange.Velocity:
					{
						float x = BitConversions.ToSingle((uint) (item.DstAgent & 0xFFFFFFFF));
						float y = BitConversions.ToSingle((uint) (item.DstAgent >> 32 & 0xFFFFFFFF));
						float z = BitConversions.ToSingle(item.Value);

						return new VelocityChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y, z);
					}
					case StateChange.Rotation:
					{
						float x = BitConversions.ToSingle((uint) (item.DstAgent & 0xFFFFFFFF));
						float y = BitConversions.ToSingle((uint) (item.DstAgent >> 32 & 0xFFFFFFFF));

						return new FacingChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent), x, y);
					}
					case StateChange.TeamChange:
						return new TeamChangeEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							item.DstAgent);
					case StateChange.Targetable:
					{
						var agent = GetAgentByAddress(item.SrcAgent);
						if (agent is AttackTarget target)
						{
							return new TargetableChangeEvent(item.Time, target, item.DstAgent != 0);
						}
						else
						{
							return new UnknownEvent(item.Time, item);
						}
					}
					case StateChange.ReplInfo:
						return new UnknownEvent(item.Time, item);
					case StateChange.StackActive:
						return new ActiveBuffStackEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							(uint) item.DstAgent);
					case StateChange.StackReset:
						return new ResetBuffStackEvent(item.Time, GetAgentByAddress(item.SrcAgent), item.Padding,
							item.Value);
					case StateChange.BreakbarState:
						var breakbarState = item.Value switch
						{
							0 => DefianceBarStateUpdateEvent.DefianceBarState.Active,
							1 => DefianceBarStateUpdateEvent.DefianceBarState.Recovering,
							2 => DefianceBarStateUpdateEvent.DefianceBarState.Immune,
							3 => DefianceBarStateUpdateEvent.DefianceBarState.None,
							_ => DefianceBarStateUpdateEvent.DefianceBarState.Unknown
						};
						return new DefianceBarStateUpdateEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							breakbarState);
					case StateChange.BreakbarPercent:
						// This encoding is inconsistent with the health update.
						float breakbarHealthFraction = BitConversions.ToSingle(item.Value);
						return new DefianceBarHealthUpdateEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							breakbarHealthFraction);
					case StateChange.BuffFormula:
					// TODO: Figure out what the contents are
					case StateChange.SkillTiming:
						// TODO: Figure out what the contents are
						return new UnknownEvent(item.Time, item);
					case StateChange.Tag:
						uint markerId = (uint) item.Value;
						if (!state.MarkersById.TryGetValue(markerId, out Marker marker))
						{
							marker = new Marker(markerId);
							state.MarkersById[markerId] = marker;
						}

						// Added in aug.23.2022
						bool isCommanderAvailable = string.Compare(state.EvtcVersion, "EVTC20220823", StringComparison.OrdinalIgnoreCase) >= 0;
						bool? isCommander = isCommanderAvailable switch
						{
							true => item.Buff != 0,
							false => null,
						};
						
						return new AgentTagEvent(item.Time, GetAgentByAddress(item.SrcAgent), marker, isCommander);
					case StateChange.BarrierUpdate:
						var barrierFraction = item.DstAgent / 10000f;
						return new BarrierUpdateEvent(item.Time, GetAgentByAddress(item.SrcAgent), barrierFraction);
					case StateChange.StatReset:
						// Should not appear in logs
						return new UnknownEvent(item.Time, item);
					case StateChange.Extension:
						// TODO: Implement
						return new UnknownEvent(item.Time, item);
					case StateChange.ApiDelayed:
						// Should not appear in logs
						return new UnknownEvent(item.Time, item);
					case StateChange.TickRate:
						return new UnknownEvent(item.Time, item);
					case StateChange.Last90BeforeDown:
						return new UnknownEvent(item.Time, item);
					case StateChange.Effect:
					{
						// Note that the meaning of fields silently changed at some point (notably, is_flanking).
						
						// src_agent effect master.
						Agent master = GetAgentByAddress(item.SrcAgent);

						// skillid = effectid,
						uint effectId = item.SkillId;
						if (!state.EffectsById.TryGetValue(effectId, out Effect effect))
						{
							effect = new Effect(effectId);
							state.EffectsById[effectId] = effect;
						}

						// dst_agent if around dst,
						// else value/buffdmg/overstack = float[3] xyz,
						//      &iff = float[2] xy orient,
						//      &pad61 = float[1] z orient,
						Agent aroundAgent = GetAgentByAddress(item.DstAgent);

						float[] position = null;
						float[] orientation = new float[3];

						if (item.DstAgent == 0)
						{
							position = new float[3];
							position[0] = BitConversions.ToSingle(item.Value); // x
							position[1] = BitConversions.ToSingle(item.BuffDmg); // y
							position[2] = BitConversions.ToSingle(item.OverstackValue); // z
						}

						// iff + buff + result + is_activation = x orientation
						Span<byte> xOrientationBytes = stackalloc byte[4];
						xOrientationBytes[0] = (byte) item.Iff;
						xOrientationBytes[1] = item.Buff;
						xOrientationBytes[2] = (byte) item.Result;
						xOrientationBytes[3] = (byte) item.IsActivation;

						// is_buffremove + is_ninety + is_fifty + is_moving = y orientation
						Span<byte> yOrientationBytes = stackalloc byte[4];
						yOrientationBytes[0] = (byte) item.IsBuffRemove;
						yOrientationBytes[1] = item.IsNinety;
						yOrientationBytes[2] = item.IsFifty;
						yOrientationBytes[3] = item.IsMoving;

						// &is_shields = uint16 duration,
						Span<byte> durationBytes = stackalloc byte[2];
						durationBytes[0] = item.IsShields;
						durationBytes[1] = item.IsOffCycle;

						orientation[0] = BitConverter.ToSingle(xOrientationBytes);
						orientation[1] = BitConverter.ToSingle(yOrientationBytes);
						orientation[2] = BitConversions.ToSingle(item.Padding);
						ushort duration = BitConverter.ToUInt16(durationBytes);

						// is_flanking = only z orient
						bool zOrientationOnly = item.IsFlanking > 0;

						return new EffectEvent(item.Time, master, effect, aroundAgent, position, orientation, zOrientationOnly, duration);
					}
					case StateChange.LogNPCUpdate:
						// TODO: implement
						return new UnknownEvent(item.Time, item);
					case StateChange.IdleEvent:
						return new UnknownEvent(item.Time, item);
					case StateChange.ExtensionCombat:
						// Not sure if this ever appears in logs.
						return new UnknownEvent(item.Time, item);
					case StateChange.Effect2:
					{
						// Official docs as of 2023-07-18:
						// src_agent is owner. dst_agent if at agent, else &value = float[3] xyz.
						// &iff = uint32 duration.
						// &buffremove = uint32 trackable id.
						// &is_shields = int16[3] orientation, value ranges from -31415 (pi) to +31415 or int16 MIN/MAX if out of those bounds
						
						// Official docs as of 2023-07-20:
						// src_agent is owner. dst_agent if at agent, else &value = float[3] xyz.
						// &iff = uint32 duraation.
						// &buffremove = uint32 trackable id.
						// &is_shields = int16[3] orientation, values are original*1000 clamped to int16 (not in realtime api)
						
						// src_agent is owner of the effect.
						Agent master = GetAgentByAddress(item.SrcAgent);

						// is_buffremove + is_ninety + is_fifty + is_moving = trackable id
						Span<byte> trackableIdBytes = stackalloc byte[4];
						trackableIdBytes[0] = (byte) item.IsBuffRemove;
						trackableIdBytes[1] = item.IsNinety;
						trackableIdBytes[2] = item.IsFifty;
						trackableIdBytes[3] = item.IsMoving;
						uint trackableId = BitConverter.ToUInt32(trackableIdBytes);
						
						// skillid = effectid (not documented)
						uint effectId = item.SkillId;
						Effect effect;
						if (effectId != 0)
						{
							if (!state.EffectsById.TryGetValue(effectId, out effect))
							{
								effect = new Effect(effectId);
								state.EffectsById[effectId] = effect;
							}
						}
						else
						{
							// TODO: retrieve effect from a list of ongoing effects
							return new EffectEndEvent(item.Time, trackableId);
						}

						// dst_agent if around dst,
						Agent aroundAgent = GetAgentByAddress(item.DstAgent);

						float[] position = null;
						ushort[] orientation = new ushort[3];

						// else &value = float[3] xyz
						if (item.DstAgent == 0)
						{
							position = new float[3];
							position[0] = BitConversions.ToSingle(item.Value); // x
							position[1] = BitConversions.ToSingle(item.BuffDmg); // y
							position[2] = BitConversions.ToSingle(item.OverstackValue); // z
						}

						// iff + buff + result + is_activation = duration
						Span<byte> durationBytes = stackalloc byte[4];
						durationBytes[0] = (byte) item.Iff;
						durationBytes[1] = item.Buff;
						durationBytes[2] = (byte) item.Result;
						durationBytes[3] = (byte) item.IsActivation;
						

						// is shields + is_offcycle + pad61 + pad62 + pad63 + pad64 = int16[3] orientation
						Span<byte> orientationBytes = stackalloc byte[6];
						durationBytes[0] = item.IsShields;
						durationBytes[1] = item.IsOffCycle;
						BitConverter.TryWriteBytes(orientationBytes[2..6], item.Padding);

						ushort duration = BitConverter.ToUInt16(durationBytes);

						return new EffectStartEvent(item.Time, master, effect, aroundAgent, position, orientation, duration, trackableId);
					}
					default:
						return new UnknownEvent(item.Time, item);
				}
			}
			else if (item.IsActivation != Activation.None)
			{
				switch (item.IsActivation)
				{
					case Activation.CancelCancel:
						return new EndSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value, EndSkillCastEvent.SkillEndType.Cancel);
					case Activation.CancelFire:
						return new EndSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value, EndSkillCastEvent.SkillEndType.Fire);
					case Activation.Normal:
						return new StartSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value, StartSkillCastEvent.SkillCastType.Normal);
					case Activation.Quickness:
						return new StartSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value,
							StartSkillCastEvent.SkillCastType.WithQuickness);
					case Activation.Reset:
						return new ResetSkillCastEvent(item.Time, GetAgentByAddress(item.SrcAgent),
							GetSkillById(item.SkillId), item.Value);
					default:
						return new UnknownEvent(item.Time, item);
				}
			}
			else if (item.Buff != 0 && item.IsBuffRemove != BuffRemove.None)
			{
				Skill buff = GetSkillById(item.SkillId);
				int remainingDuration = item.Value;
				int remainingIntensity = item.BuffDmg;
				int stacksRemoved = (int) item.Result;
				var cleansingAgent = GetAgentByAddress(item.DstAgent);
				var agent = GetAgentByAddress(item.SrcAgent);
				switch (item.IsBuffRemove)
				{
					case BuffRemove.All:
						return new AllStacksRemovedBuffEvent(item.Time, agent, buff, cleansingAgent,
							stacksRemoved);
					case BuffRemove.Single:
						uint stackId = item.Padding;
						return new SingleStackRemovedBuffEvent(item.Time, agent, buff, cleansingAgent,
							remainingDuration, remainingIntensity, stackId);
					case BuffRemove.Manual:
						return new ManualStackRemovedBuffEvent(item.Time, agent, buff, cleansingAgent,
							remainingDuration, remainingIntensity);
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else if (item.Buff > 0 && item.BuffDmg == 0)
			{
				Skill buff = GetSkillById(item.SkillId);
				int durationApplied = item.Value;
				uint durationOfRemovedStack = item.OverstackValue;
				var agent = GetAgentByAddress(item.DstAgent);
				var sourceAgent = GetAgentByAddress(item.SrcAgent);
				return new BuffApplyEvent(item.Time, agent, buff, sourceAgent, durationApplied,
					durationOfRemovedStack);
			}
			else if (item.Buff > 0 && item.Value == 0)
			{
				Skill buff = GetSkillById(item.SkillId);
				int buffDamage = item.BuffDmg;
				bool isOffCycle = item.IsOffCycle > 0;
				Agent attacker = GetAgentByAddress(item.SrcAgent);
				Agent defender = GetAgentByAddress(item.DstAgent);
				bool isMoving = item.IsMoving > 0;
				bool isNinety = item.IsNinety > 0;
				bool isFlanking = item.IsFlanking > 0;
				bool isIgnored = item.Result != 0;

				if (isIgnored)
				{
					var reason = item.Result == (Result) 1
						? IgnoredBuffDamageEvent.Reason.InvulnerableBuff
						: IgnoredBuffDamageEvent.Reason.InvulnerableSkill;

					return new IgnoredBuffDamageEvent(item.Time, attacker, defender, buff, buffDamage,
						isMoving, isNinety, isFlanking, reason);
				}
				else
				{
					if (isOffCycle)
					{
						return new OffCycleBuffDamageEvent(item.Time, attacker, defender, buff, buffDamage,
							isMoving, isNinety, isFlanking);
					}
					else
					{
						return new BuffDamageEvent(item.Time, attacker, defender, buff, buffDamage, isMoving,
							isNinety, isFlanking);
					}
				}
			}
			else if (item.Buff == 0)
			{
				int damage = item.Value;
				uint shieldDamage = item.OverstackValue;
				Agent attacker = GetAgentByAddress(item.SrcAgent);
				Agent defender = GetAgentByAddress(item.DstAgent);
				Skill skill = GetSkillById(item.SkillId);
				bool isMoving = item.IsMoving > 0;
				bool isNinety = item.IsNinety > 0;
				bool isFlanking = item.IsFlanking > 0;

				// TODO: Rewrite
				bool ignored = false;
				bool defianceBar = false;
				var hitResult = PhysicalDamageEvent.Result.Normal;
				var ignoreReason = IgnoredPhysicalDamageEvent.Reason.Absorbed;
				switch (item.Result)
				{
					case Result.Normal:
						hitResult = PhysicalDamageEvent.Result.Normal;
						break;
					case Result.Critical:
						hitResult = PhysicalDamageEvent.Result.Critical;
						break;
					case Result.Glance:
						hitResult = PhysicalDamageEvent.Result.Glance;
						break;
					case Result.Block:
						ignored = true;
						ignoreReason = IgnoredPhysicalDamageEvent.Reason.Blocked;
						break;
					case Result.Evade:
						ignored = true;
						ignoreReason = IgnoredPhysicalDamageEvent.Reason.Evaded;
						break;
					case Result.Interrupt:
						hitResult = PhysicalDamageEvent.Result.Interrupt;
						break;
					case Result.Absorb:
						ignored = true;
						ignoreReason = IgnoredPhysicalDamageEvent.Reason.Absorbed;
						break;
					case Result.Blind:
						ignored = true;
						ignoreReason = IgnoredPhysicalDamageEvent.Reason.Missed;
						break;
					case Result.KillingBlow:
						hitResult = PhysicalDamageEvent.Result.KillingBlow;
						break;
					case Result.Downed:
						hitResult = PhysicalDamageEvent.Result.DowningBlow;
						break;
					case Result.DefianceBar:
						defianceBar = true;
						break;
					default:
						return new UnknownEvent(item.Time, item);
				}

				if (!defianceBar)
				{
					if (!ignored)
					{
						return new PhysicalDamageEvent(item.Time, attacker, defender, skill, damage, isMoving,
							isNinety, isFlanking, shieldDamage, hitResult);
					}
					else
					{
						return new IgnoredPhysicalDamageEvent(item.Time, attacker, defender, skill, damage,
							isMoving, isNinety, isFlanking, shieldDamage, ignoreReason);
					}
				}
				else
				{
					return new DefianceBarDamageEvent(item.Time, attacker, defender, skill, damage, isMoving,
						isNinety, isFlanking);
				}
			}

			return new UnknownEvent(item.Time, item);
		}
	}
}