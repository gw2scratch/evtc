using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace GW2Scratch.EVTCAnalytics.Parsing;

public class CombatItemFilters : ICombatItemFilters
{
	private IReadOnlyList<Type> RequiredEventTypes { get; }
	private bool[] StateChangeFilter { get; }
	private bool[] PhysicalDamageResultFilter { get; }
	private IIdFilter BuffIdFilter { get; }
	private bool BuffDamageRequired { get; }
	
	public CombatItemFilters(IReadOnlyList<Type> requiredEventTypes, IReadOnlyList<uint> requiredBuffIds, IReadOnlyList<PhysicalDamageEvent.Result> requiredResults)
	{
		RequiredEventTypes = requiredEventTypes;
		StateChangeFilter = BuildStateChangeFilter(RequiredEventTypes);
		PhysicalDamageResultFilter = BuildPhysicalDamageResultFilter(requiredResults);
		BuffIdFilter = BuildBuffIdFilter(requiredBuffIds);
		BuffDamageRequired = requiredEventTypes.Any(IsBuffDamage);
	}

	public bool IsBuffEventRequired(uint skillId)
	{
		return BuffIdFilter.IsKept(skillId);
	}

	public bool IsPhysicalDamageResultRequired(byte result)
	{
		return PhysicalDamageResultFilter[result];
	}

	public bool IsBuffDamageRequired() => BuffDamageRequired;

	public bool IsStateChangeRequired(StateChange stateChange)
	{
		return StateChangeFilter[(int) stateChange];
	}
	
	public bool IsStateChangeRequired(byte stateChange)
	{
		return StateChangeFilter[stateChange];
	}

	private static IIdFilter BuildBuffIdFilter(IReadOnlyList<uint> requiredBuffIds)
	{
		var max = requiredBuffIds.DefaultIfEmpty().Max();
		// If the skills are in a reasonably range, we use an array as the filter (as of 2023, highest skill IDs are ~70000).
		// In case many new skills are introduced, or more importantly, in case arcdps or an issue in arcdps introduces high ids,
		// we use a hash set which will not take all available memory.
		if (max < 262144)
		{
			return new UnboundedArrayFilter(requiredBuffIds);
		}
		return new HashSetFilter(requiredBuffIds);
	}

	private static bool[] BuildStateChangeFilter(IEnumerable<Type> eventTypes)
	{
		var stateChanges = new bool[256];
		foreach (var type in eventTypes)
		{
			foreach (var stateChange in GetStateChangesForEventType(type))
			{
				stateChanges[(int) stateChange] = true;
			}
		}

		foreach (StateChange stateChange in Enum.GetValues(typeof(StateChange)))
		{
			if (IsAlwaysKept(stateChange))
			{
				stateChanges[(int) stateChange] = true;
			}
		}

		return stateChanges;
	}
	
	private static bool[] BuildPhysicalDamageResultFilter(IEnumerable<PhysicalDamageEvent.Result> requiredResults)
	{
		var results = new bool[256];
		foreach (var result in requiredResults)
		{
			foreach (var rawResult in GetRawResultsForResult(result))
			{
				results[(int) rawResult] = true;
			}
		}

		return results;
	}

	public static IEnumerable<Result> GetRawResultsForResult(PhysicalDamageEvent.Result result)
	{
		return result switch
		{
			PhysicalDamageEvent.Result.Normal => new[] { Result.Normal },
			PhysicalDamageEvent.Result.Critical => new[] { Result.Critical },
			PhysicalDamageEvent.Result.Glance => new[] { Result.Glance },
			PhysicalDamageEvent.Result.Interrupt => new[] { Result.Interrupt },
			PhysicalDamageEvent.Result.DowningBlow => new[] { Result.Downed },
			PhysicalDamageEvent.Result.KillingBlow => new[] { Result.KillingBlow },
			PhysicalDamageEvent.Result.Ignored => new[] { Result.Block, Result.Evade, Result.Absorb, Result.Blind },
			_ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
		};
	}


	public static IEnumerable<StateChange> GetStateChangesForEventType(Type eventType)
	{
		if (!(eventType.IsSubclassOf(typeof(Event)) || eventType == typeof(Event)))
		{
			throw new ArgumentException($"Type {eventType} is not an event type.");
		}
		
		// If a type has children types, we also need to include their state changes.
		var subclasses = Assembly.GetAssembly(eventType)!.GetTypes().Where(type => type.IsSubclassOf(eventType));
		var stateChanges = new HashSet<StateChange>();
		foreach (var subclass in subclasses)
		{
			stateChanges.UnionWith(GetStateChangesForEventType(subclass));
		}
		
		stateChanges.UnionWith(GetDirectStateChangesForEventType(eventType));

		return stateChanges;
	}
	
	public static bool IsBuffDamage(Type eventType)
	{
		if (!(eventType.IsSubclassOf(typeof(Event)) || eventType == typeof(Event)))
		{
			throw new ArgumentException($"Type {eventType} is not an event type.");
		}
		
		// If a type has children types, we also need to include their state changes.
		var subclasses = Assembly.GetAssembly(eventType)!.GetTypes().Where(type => type.IsSubclassOf(eventType));
		bool isBuffDamage = false;
		foreach (var subclass in subclasses)
		{
			isBuffDamage = isBuffDamage || IsBuffDamage(subclass);
		}

		isBuffDamage = isBuffDamage || IsDirectBuffDamage(eventType);

		return isBuffDamage;
	}

	/// <summary>
	/// Returns a list of state changes that will directly produce this type,
	/// state changes that result in subclasses of this type are not included.
	/// </summary>
	private static IEnumerable<StateChange> GetDirectStateChangesForEventType(Type eventType)
	{
		if (eventType == typeof(Event)) return Array.Empty<StateChange>();

		if (eventType == typeof(AgentEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(AgentEnterCombatEvent)) return new[] { StateChange.EnterCombat };
		if (eventType == typeof(AgentExitCombatEvent)) return new[] { StateChange.ExitCombat };
		if (eventType == typeof(AgentRevivedEvent)) return new[] { StateChange.ChangeUp };
		if (eventType == typeof(AgentDownedEvent)) return new[] { StateChange.ChangeDown };
		if (eventType == typeof(AgentDeadEvent)) return new[] { StateChange.ChangeDead };
		if (eventType == typeof(AgentSpawnEvent)) return new[] { StateChange.Spawn };
		if (eventType == typeof(AgentDespawnEvent)) return new[] { StateChange.Despawn };
		if (eventType == typeof(AgentHealthUpdateEvent)) return new[] { StateChange.HealthUpdate };
		if (eventType == typeof(AgentWeaponSwapEvent)) return new[] { StateChange.WeaponSwap };
		if (eventType == typeof(AgentMaxHealthUpdateEvent)) return new[] { StateChange.MaxHealthUpdate };
		if (eventType == typeof(AgentTagEvent)) return new[] { StateChange.Tag };
		if (eventType == typeof(InitialBuffEvent)) return new[] { StateChange.BuffInitial };
		if (eventType == typeof(PositionChangeEvent)) return new[] { StateChange.Position };
		if (eventType == typeof(VelocityChangeEvent)) return new[] { StateChange.Velocity };
		if (eventType == typeof(FacingChangeEvent)) return new[] { StateChange.Rotation };
		if (eventType == typeof(TeamChangeEvent)) return new[] { StateChange.TeamChange };
		if (eventType == typeof(TargetableChangeEvent)) return new[] { StateChange.Targetable };
		if (eventType == typeof(DefianceBarHealthUpdateEvent)) return new[] { StateChange.BreakbarPercent };
		if (eventType == typeof(BarrierUpdateEvent)) return new[] { StateChange.BarrierUpdate };
		if (eventType == typeof(DefianceBarStateUpdateEvent)) return new[] { StateChange.BreakbarState };
		if (eventType == typeof(EffectEvent)) return new[] { StateChange.Effect };
		if (eventType == typeof(EffectStartEvent)) return new[] { StateChange.Effect2 };
		if (eventType == typeof(EffectEndEvent)) return new[] { StateChange.Effect2 };

		if (eventType == typeof(BuffEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(BuffRemoveEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(AllStacksRemovedBuffEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(SingleStackRemovedBuffEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(ManualStackRemovedBuffEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(BuffApplyEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(ActiveBuffStackEvent)) return new[] { StateChange.StackActive };
		if (eventType == typeof(ResetBuffStackEvent)) return new[] { StateChange.StackReset };
		if (eventType == typeof(BuffExtensionEvent)) return Array.Empty<StateChange>();
		
		if (eventType == typeof(DamageEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(PhysicalDamageEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(IgnoredPhysicalDamageEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(IgnoredBuffDamageEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(BuffDamageEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(OffCycleBuffDamageEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(DefianceBarDamageEvent)) return Array.Empty<StateChange>();
		
		if (eventType == typeof(RewardEvent)) return new[] { StateChange.Reward };
		
		if (eventType == typeof(SkillCastEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(EndSkillCastEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(StartSkillCastEvent)) return Array.Empty<StateChange>();
		if (eventType == typeof(ResetSkillCastEvent)) return Array.Empty<StateChange>();

		// The unknown event can come from any state change, including not yet implemented ones,
		// so we need to return all of them.
		Debug.Assert(Enum.GetUnderlyingType(typeof(StateChange)) == typeof(byte));
		if (eventType == typeof(UnknownEvent)) return Enumerable.Range(0, 256).Select(x => (StateChange) x);

		throw new ArgumentException($"Event type {eventType} is not supported.");
	}
	
	/// <summary>
	/// Returns whether this type requires a buff damage combat item.
	/// Requirements of subclasses of this type are not included.
	/// </summary>
	private static bool IsDirectBuffDamage(Type eventType)
	{
		if (eventType == typeof(Event)) return false;

		if (eventType == typeof(AgentEvent)) return false;
		if (eventType == typeof(AgentEnterCombatEvent)) return false;
		if (eventType == typeof(AgentExitCombatEvent)) return false;
		if (eventType == typeof(AgentRevivedEvent)) return false;
		if (eventType == typeof(AgentDownedEvent)) return false;
		if (eventType == typeof(AgentDeadEvent)) return false;
		if (eventType == typeof(AgentSpawnEvent)) return false;
		if (eventType == typeof(AgentDespawnEvent)) return false;
		if (eventType == typeof(AgentHealthUpdateEvent)) return false;
		if (eventType == typeof(AgentWeaponSwapEvent)) return false;
		if (eventType == typeof(AgentMaxHealthUpdateEvent)) return false;
		if (eventType == typeof(AgentTagEvent)) return false;
		if (eventType == typeof(InitialBuffEvent)) return false;
		if (eventType == typeof(PositionChangeEvent)) return false;
		if (eventType == typeof(VelocityChangeEvent)) return false;
		if (eventType == typeof(FacingChangeEvent)) return false;
		if (eventType == typeof(TeamChangeEvent)) return false;
		if (eventType == typeof(TargetableChangeEvent)) return false;
		if (eventType == typeof(DefianceBarHealthUpdateEvent)) return false;
		if (eventType == typeof(BarrierUpdateEvent)) return false;
		if (eventType == typeof(DefianceBarStateUpdateEvent)) return false;
		if (eventType == typeof(EffectEvent)) return false;
		if (eventType == typeof(EffectStartEvent)) return false;
		if (eventType == typeof(EffectEndEvent)) return false;

		if (eventType == typeof(BuffEvent)) return false;
		if (eventType == typeof(BuffRemoveEvent)) return false;
		if (eventType == typeof(AllStacksRemovedBuffEvent)) return false;
		if (eventType == typeof(SingleStackRemovedBuffEvent)) return false;
		if (eventType == typeof(ManualStackRemovedBuffEvent)) return false;
		if (eventType == typeof(BuffApplyEvent)) return false;
		if (eventType == typeof(ActiveBuffStackEvent)) return false;
		if (eventType == typeof(ResetBuffStackEvent)) return false;
		if (eventType == typeof(BuffExtensionEvent)) return false;
		
		if (eventType == typeof(DamageEvent)) return false;
		if (eventType == typeof(PhysicalDamageEvent)) return false;
		if (eventType == typeof(IgnoredPhysicalDamageEvent)) return false;
		if (eventType == typeof(IgnoredBuffDamageEvent)) return true;
		if (eventType == typeof(BuffDamageEvent)) return true;
		if (eventType == typeof(OffCycleBuffDamageEvent)) return true;
		if (eventType == typeof(DefianceBarDamageEvent)) return false; // This is a physical event, even "soft CC".
		
		if (eventType == typeof(RewardEvent)) return false;

		if (eventType == typeof(SkillCastEvent)) return false;
		if (eventType == typeof(EndSkillCastEvent)) return false;
		if (eventType == typeof(StartSkillCastEvent)) return false;
		if (eventType == typeof(ResetSkillCastEvent)) return false;

		// The unknown event can come from anything
		if (eventType == typeof(UnknownEvent)) return true;

		throw new ArgumentException($"Event type {eventType} is not supported.");
	}

	/// <summary>
	/// Returns whether a state change is always kept in the log.
	///
	/// Important: Do not use for state changes that are not defined enum values of <see cref="StateChange"/>.
	/// The expected use is to check only for <c>Enum.GetValues(typeof(StateChange))</c>.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if a state change is not a built-in handled one.</exception>
	public static bool IsAlwaysKept(StateChange stateChange)
	{
		// This is implemented this way to make any newly introduced StateChange
		// fail a test in EVTCAnalytics.Tests unless explicitly implemented here.

		return stateChange switch
		{
			StateChange.Normal => true,
			StateChange.EnterCombat => false,
			StateChange.ExitCombat => false,
			StateChange.ChangeUp => false,
			StateChange.ChangeDead => false,
			StateChange.ChangeDown => false,
			StateChange.Spawn => false,
			StateChange.Despawn => false,
			StateChange.HealthUpdate => false,
			StateChange.LogStart => true,
			StateChange.LogEnd => true,
			StateChange.WeaponSwap => false,
			StateChange.MaxHealthUpdate => false,
			StateChange.PointOfView => true,
			StateChange.Language => true,
			StateChange.GWBuild => true,
			StateChange.ShardId => true,
			StateChange.Reward => false,
			StateChange.BuffInitial => false,
			StateChange.Position => false,
			StateChange.Velocity => false,
			StateChange.Rotation => false,
			StateChange.TeamChange => false,
			StateChange.AttackTarget => true,
			StateChange.Targetable => false,
			StateChange.MapId => true,
			StateChange.ReplInfo => false,
			StateChange.StackActive => false,
			StateChange.StackReset => false,
			StateChange.Guild => true,
			StateChange.BuffInfo => true,
			StateChange.BuffFormula => true,
			StateChange.SkillInfo => true,
			StateChange.SkillTiming => true,
			StateChange.BreakbarState => false,
			StateChange.BreakbarPercent => false,
			StateChange.Error => true,
			StateChange.Tag => false,
			StateChange.BarrierUpdate => false,
			StateChange.StatReset => true,
			StateChange.Extension => true,
			StateChange.ApiDelayed => true,
			StateChange.InstanceStart => true,
			StateChange.TickRate => true,
			StateChange.Last90BeforeDown => false,
			StateChange.Effect => false,
			StateChange.IdToGuid => true,
			StateChange.LogNPCUpdate => true,
			StateChange.IdleEvent => false,
			StateChange.ExtensionCombat => true,
			StateChange.FractalScale => true,
			StateChange.Effect2 => false,
			_ => throw new ArgumentOutOfRangeException(nameof(stateChange), stateChange, null)
		};
	}
}