using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing;

namespace GW2Scratch.ArcdpsLogManager.Sections.GameData
{
	/// <summary>
	/// The kind of skill a <see cref="SkillGatherResult"/> represents, based on which of a
	/// <c>Skill</c>'s data fields (if any) were populated by the parser.
	/// </summary>
	public enum GatheredSkillType
	{
		Unknown,
		Buff,
		Ability,
	}

	/// <summary>
	/// A distinct NPC species found while gathering game data, and every log it was seen in.
	/// </summary>
	public sealed class SpeciesGatherResult : IEquatable<SpeciesGatherResult>
	{
		public int SpeciesId { get; }
		public string Name { get; }
		public List<LogData> Logs { get; } = new List<LogData>();

		public SpeciesGatherResult(int speciesId, string name)
		{
			SpeciesId = speciesId;
			Name = name;
		}

		public bool Equals(SpeciesGatherResult other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return SpeciesId == other.SpeciesId && string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SpeciesGatherResult) obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(SpeciesId, Name);
		}

		public static bool operator ==(SpeciesGatherResult left, SpeciesGatherResult right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SpeciesGatherResult left, SpeciesGatherResult right)
		{
			return !Equals(left, right);
		}
	}

	/// <summary>
	/// A distinct skill found while gathering game data, and every log it was seen in.
	/// </summary>
	public sealed class SkillGatherResult : IEquatable<SkillGatherResult>
	{
		public uint SkillId { get; }
		public string Name { get; }
		public List<LogData> Logs { get; } = new List<LogData>();
		public GatheredSkillType Type { get; }

		public SkillGatherResult(uint skillId, string name, GatheredSkillType type)
		{
			SkillId = skillId;
			Name = name;
			Type = type;
		}

		public bool Equals(SkillGatherResult other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return SkillId == other.SkillId && string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SkillGatherResult) obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(SkillId, Name, Type);
		}

		public static bool operator ==(SkillGatherResult left, SkillGatherResult right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SkillGatherResult left, SkillGatherResult right)
		{
			return !Equals(left, right);
		}
	}

	/// <summary>
	/// Parses a set of logs in parallel to gather every distinct NPC species and skill seen,
	/// along with which logs each was seen in. This data is not cached (it is not needed for
	/// everyday use of the manager), so it always re-parses every log with a plain
	/// <see cref="EVTCParser"/>/<see cref="LogProcessor"/> rather than going through the cache.
	/// </summary>
	/// <remarks>
	/// Moved here from the Eto <c>Sections/GameDataGathering.cs</c> (originally
	/// <c>GameDataCollecting.ParseLogs</c>) so the UI-agnostic parsing/aggregation logic has a
	/// single source of truth shared by both the Eto and Avalonia UIs; the Eto class was
	/// repointed at this class rather than kept as a diverging copy.
	/// </remarks>
	public sealed class GameDataGatherer
	{
		private EVTCParser Parser { get; } = new EVTCParser {SinglePassFilteringOptions = {PruneForEncounterData = true}};
		private LogProcessor Processor { get; } = new LogProcessor();

		public Task<(IEnumerable<SpeciesGatherResult> Species, IEnumerable<SkillGatherResult> Skills)> GatherAsync(
			IReadOnlyCollection<LogData> logs, CancellationToken cancellationToken, int maxDegreeOfParallelism,
			IProgress<(int done, int totalLogs, int failed)> progress = null)
		{
			return Task.Run(() =>
			{
				var species = new ConcurrentDictionary<int, ConcurrentDictionary<SpeciesGatherResult, ConcurrentBag<LogData>>>();
				var skills = new ConcurrentDictionary<uint, ConcurrentDictionary<SkillGatherResult, ConcurrentBag<LogData>>>();

				int done = 0;
				int failed = 0;
				var options = new ParallelOptions {MaxDegreeOfParallelism = maxDegreeOfParallelism};
				Parallel.ForEach(logs, options, log =>
				{
					cancellationToken.ThrowIfCancellationRequested();

					GW2Scratch.EVTCAnalytics.Model.Log processedLog;
					try
					{
						processedLog = Processor.ProcessLog(log.FileName, Parser);
					}
					catch
					{
						Interlocked.Increment(ref failed);
						return;
					}

					foreach (var agent in processedLog.Agents.OfType<NPC>())
					{
						int id = agent.SpeciesId;
						string name = agent.Name;

						if (id == 0) continue;

						var speciesData = new SpeciesGatherResult(id, name);
						var dictForSpecies =
							species.GetOrAdd(id, new ConcurrentDictionary<SpeciesGatherResult, ConcurrentBag<LogData>>());

						var listForSpeciesData = dictForSpecies.GetOrAdd(speciesData, new ConcurrentBag<LogData>());
						listForSpeciesData.Add(log);
					}

					foreach (var skill in processedLog.Skills)
					{
						uint id = skill.Id;
						string name = skill.Name;
						var skillType = (skill.SkillData, skill.BuffData) switch
						{
							(null, null) => GatheredSkillType.Unknown,
							(_, null) => GatheredSkillType.Ability,
							(null, _) => GatheredSkillType.Buff,
							_ => GatheredSkillType.Unknown,
						};

						if (id == 0) continue;

						var skillData = new SkillGatherResult(id, name, skillType);
						var dictForSkill =
							skills.GetOrAdd(id, new ConcurrentDictionary<SkillGatherResult, ConcurrentBag<LogData>>());

						var listForSkillData = dictForSkill.GetOrAdd(skillData, new ConcurrentBag<LogData>());
						listForSkillData.Add(log);
					}

					Interlocked.Increment(ref done);
					progress?.Report((done, logs.Count, failed));
				});

				var speciesEnumerable = (IEnumerable<SpeciesGatherResult>) species.Values.SelectMany(x => x).Select(x =>
					{
						var key = x.Key;
						key.Logs.AddRange(x.Value);
						return key;
					}).OrderBy(x => x.SpeciesId)
					.ThenBy(x => x.Name);
				var skillEnumerable = (IEnumerable<SkillGatherResult>) skills.Values.SelectMany(x => x).Select(x =>
					{
						var key = x.Key;
						key.Logs.AddRange(x.Value);
						return key;
					}).OrderBy(x => x.SkillId)
					.ThenBy(x => x.Name);

				return (speciesEnumerable, skillEnumerable);
			}, cancellationToken);
		}
	}
}
