using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Skills;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
	private bool SkillCastsRequired { get; }

	public CombatItemFilters(IReadOnlyList<Type> requiredEventTypes, IReadOnlyList<uint> requiredBuffIds,
		IReadOnlyList<PhysicalDamageEvent.Result> requiredResults)
	{
		RequiredEventTypes = requiredEventTypes;
		StateChangeFilter = BuildStateChangeFilter(RequiredEventTypes);
		PhysicalDamageResultFilter = BuildPhysicalDamageResultFilter(requiredEventTypes, requiredResults);
		BuffIdFilter = BuildBuffIdFilter(requiredBuffIds);
		BuffDamageRequired = requiredEventTypes.Any(IsBuffDamage);
		SkillCastsRequired = requiredEventTypes.Any(IsSkillCast);
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
	public bool IsSkillCastRequired() => SkillCastsRequired;

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
		// If the skills are in a reasonable range, we use an array as the filter (as of 2023, highest skill IDs are ~70000).
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

	private static bool[] BuildPhysicalDamageResultFilter(IEnumerable<Type> eventTypes, IEnumerable<PhysicalDamageEvent.Result> requiredResults)
	{
		var results = new bool[256];
		foreach (var result in requiredResults)
		{
			foreach (var rawResult in GetRawResultsForResult(result))
			{
				results[(int) rawResult] = true;
			}
		}

		// We are using requiredResults to filter through PhysicalDamageEvent and IgnoredPhysicalDamageEvent.
		// However, some other events may be produced through other raw physical events with other raw results (such as defiance bar events).
		foreach (var type in eventTypes.Where(x => x != typeof(PhysicalDamageEvent) && x != typeof(IgnoredPhysicalDamageEvent)))
		{
			foreach (var rawResult in GetPhysicalResultsForEventType(type))
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
			PhysicalDamageEvent.Result.Normal => [Result.Normal],
			PhysicalDamageEvent.Result.Critical => [Result.Critical],
			PhysicalDamageEvent.Result.Glance => [Result.Glance],
			PhysicalDamageEvent.Result.Interrupt => [Result.Interrupt],
			PhysicalDamageEvent.Result.DowningBlow => [Result.Downed],
			PhysicalDamageEvent.Result.KillingBlow => [Result.KillingBlow],
			PhysicalDamageEvent.Result.Ignored => [Result.Block, Result.Evade, Result.Absorb, Result.Blind],
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

	public static bool IsSkillCast(Type eventType)
	{
		if (!(eventType.IsSubclassOf(typeof(Event)) || eventType == typeof(Event)))
		{
			throw new ArgumentException($"Type {eventType} is not an event type.");
		}

		// If a type has children types, we also need to include their state changes.
		var subclasses = Assembly.GetAssembly(eventType)!.GetTypes().Where(type => type.IsSubclassOf(eventType));
		bool isSkillCast = false;
		foreach (var subclass in subclasses)
		{
			isSkillCast = isSkillCast || IsSkillCast(subclass);
		}

		isSkillCast = isSkillCast || IsDirectSkillCast(eventType);

		return isSkillCast;
	}

	public static IEnumerable<Result> GetPhysicalResultsForEventType(Type eventType)
	{
		if (!(eventType.IsSubclassOf(typeof(Event)) || eventType == typeof(Event)))
		{
			throw new ArgumentException($"Type {eventType} is not an event type.");
		}

		// If a type has children types, we also need to include their state changes.
		var subclasses = Assembly.GetAssembly(eventType)!.GetTypes().Where(type => type.IsSubclassOf(eventType));
		var results = new HashSet<Result>();
		foreach (var subclass in subclasses)
		{
			results.UnionWith(GetPhysicalResultsForEventType(subclass));
		}

		results.UnionWith(GetDirectPhysicalResultsForEventType(eventType));

		return results;
	}

	/// <summary>
	/// Returns a list of state changes that will directly produce this type,
	/// state changes that result in subclasses of this type are not included.
	/// </summary>
	private static IEnumerable<StateChange> GetDirectStateChangesForEventType(Type eventType)
	{
		if (eventType == typeof(Event)) return [];

		if (eventType == typeof(AgentEvent)) return [];
		if (eventType == typeof(AgentEnterCombatEvent)) return [StateChange.EnterCombat];
		if (eventType == typeof(AgentExitCombatEvent)) return [StateChange.ExitCombat];
		if (eventType == typeof(AgentRevivedEvent)) return [StateChange.ChangeUp];
		if (eventType == typeof(AgentDownedEvent)) return [StateChange.ChangeDown];
		if (eventType == typeof(AgentDeadEvent)) return [StateChange.ChangeDead];
		if (eventType == typeof(AgentSpawnEvent)) return [StateChange.Spawn];
		if (eventType == typeof(AgentDespawnEvent)) return [StateChange.Despawn];
		if (eventType == typeof(AgentHealthUpdateEvent)) return [StateChange.HealthUpdate];
		if (eventType == typeof(AgentWeaponSwapEvent)) return [StateChange.WeaponSwap];
		if (eventType == typeof(AgentMaxHealthUpdateEvent)) return [StateChange.MaxHealthUpdate];
		if (eventType == typeof(AgentMarkerEvent)) return [StateChange.Tag];
		if (eventType == typeof(AgentMarkerRemoveAllEvent)) return [StateChange.Tag];
		if (eventType == typeof(InitialBuffEvent)) return [StateChange.BuffInitial];
		if (eventType == typeof(PositionChangeEvent)) return [StateChange.Position];
		if (eventType == typeof(VelocityChangeEvent)) return [StateChange.Velocity];
		if (eventType == typeof(FacingChangeEvent)) return [StateChange.Rotation];
		if (eventType == typeof(TeamChangeEvent)) return [StateChange.TeamChange];
		if (eventType == typeof(TargetableChangeEvent)) return [StateChange.Targetable];
		if (eventType == typeof(DefianceBarHealthUpdateEvent)) return [StateChange.BreakbarPercent];
		if (eventType == typeof(BarrierUpdateEvent)) return [StateChange.BarrierUpdate];
		if (eventType == typeof(DefianceBarStateUpdateEvent)) return [StateChange.BreakbarState];
		if (eventType == typeof(EffectEvent)) return [StateChange.Effect];
		if (eventType == typeof(EffectStartEvent)) return [StateChange.Effect2];
		if (eventType == typeof(EffectEndEvent)) return [StateChange.Effect2];
		if (eventType == typeof(AgentGliderOpenEvent)) return [StateChange.Glider];
		if (eventType == typeof(AgentGliderCloseEvent)) return [StateChange.Glider];

		if (eventType == typeof(BuffEvent)) return [];
		if (eventType == typeof(BuffRemoveEvent)) return [];
		if (eventType == typeof(AllStacksRemovedBuffEvent)) return [];
		if (eventType == typeof(SingleStackRemovedBuffEvent)) return [];
		if (eventType == typeof(ManualStackRemovedBuffEvent)) return [];
		if (eventType == typeof(BuffApplyEvent)) return [];
		if (eventType == typeof(ActiveBuffStackEvent)) return [StateChange.StackActive];
		if (eventType == typeof(ResetBuffStackEvent)) return [StateChange.StackReset];
		if (eventType == typeof(BuffExtensionEvent)) return [];

		if (eventType == typeof(DamageEvent)) return [];
		if (eventType == typeof(PhysicalDamageEvent)) return [];
		if (eventType == typeof(IgnoredPhysicalDamageEvent)) return [];
		if (eventType == typeof(IgnoredBuffDamageEvent)) return [];
		if (eventType == typeof(BuffDamageEvent)) return [];
		if (eventType == typeof(OffCycleBuffDamageEvent)) return [];
		if (eventType == typeof(DefianceBarDamageEvent)) return [];

		if (eventType == typeof(RewardEvent)) return [StateChange.Reward];
		if (eventType == typeof(RateHealthEvent)) return [StateChange.TickRate];
		if (eventType == typeof(StatResetEvent)) return [StateChange.StatReset];
		if (eventType == typeof(LogNPCUpdateEvent)) return [StateChange.LogNPCUpdate];
		if (eventType == typeof(IIDChangeEvent)) return [StateChange.IIDChange];
		if (eventType == typeof(MapChangeEvent)) return [StateChange.MapChange];

		if (eventType == typeof(CrowdControlEvent)) return [];

		if (eventType == typeof(SkillCastEvent)) return [];
		if (eventType == typeof(EndSkillCastEvent)) return [];
		if (eventType == typeof(StartSkillCastEvent)) return [];
		if (eventType == typeof(ResetSkillCastEvent)) return [];

		if (eventType == typeof(SquadGroundMarkerEvent)) return [];
		if (eventType == typeof(SquadGroundMarkerPlaceEvent)) return [StateChange.SquadMarker];
		if (eventType == typeof(SquadGroundMarkerRemoveEvent)) return [StateChange.SquadMarker];

		// The unknown event can come from any state change, including not yet implemented ones,
		// so we need to return all of them.
		Debug.Assert(Enum.GetUnderlyingType(typeof(StateChange)) == typeof(byte));
		if (eventType == typeof(UnknownEvent)) return Enumerable.Range(0, 256).Select(x => (StateChange) x);
		if (eventType == typeof(UnknownExtensionEvent)) return [StateChange.Extension, StateChange.ExtensionCombat];

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
		if (eventType == typeof(AgentMarkerEvent)) return false;
		if (eventType == typeof(AgentMarkerRemoveAllEvent)) return false;
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
		if (eventType == typeof(AgentGliderOpenEvent)) return false;
		if (eventType == typeof(AgentGliderCloseEvent)) return false;

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

		if (eventType == typeof(CrowdControlEvent)) return false;
		
		if (eventType == typeof(RewardEvent)) return false;
		if (eventType == typeof(RateHealthEvent)) return false;
		if (eventType == typeof(StatResetEvent)) return false;
		if (eventType == typeof(LogNPCUpdateEvent)) return false;
		if (eventType == typeof(IIDChangeEvent)) return false;
		if (eventType == typeof(MapChangeEvent)) return false;

		if (eventType == typeof(SkillCastEvent)) return false;
		if (eventType == typeof(EndSkillCastEvent)) return false;
		if (eventType == typeof(StartSkillCastEvent)) return false;
		if (eventType == typeof(ResetSkillCastEvent)) return false;

		if (eventType == typeof(SquadGroundMarkerEvent)) return false;
		if (eventType == typeof(SquadGroundMarkerPlaceEvent)) return false;
		if (eventType == typeof(SquadGroundMarkerRemoveEvent)) return false;

		// The unknown event can come from anything
		if (eventType == typeof(UnknownEvent)) return true;
		if (eventType == typeof(UnknownExtensionEvent)) return false;

		throw new ArgumentException($"Event type {eventType} is not supported.");
	}

	/// <summary>
	/// Returns whether this type requires a skill cast combat item.
	/// Requirements of subclasses of this type are not included.
	/// </summary>
	private static bool IsDirectSkillCast(Type eventType)
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
		if (eventType == typeof(AgentMarkerEvent)) return false;
		if (eventType == typeof(AgentMarkerRemoveAllEvent)) return false;
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
		if (eventType == typeof(AgentGliderOpenEvent)) return false;
		if (eventType == typeof(AgentGliderCloseEvent)) return false;

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
		if (eventType == typeof(IgnoredBuffDamageEvent)) return false;
		if (eventType == typeof(BuffDamageEvent)) return false;
		if (eventType == typeof(OffCycleBuffDamageEvent)) return false;
		if (eventType == typeof(DefianceBarDamageEvent)) return false;
		
		if (eventType == typeof(CrowdControlEvent)) return false;

		if (eventType == typeof(RewardEvent)) return false;
		if (eventType == typeof(RateHealthEvent)) return false;
		if (eventType == typeof(StatResetEvent)) return false;
		if (eventType == typeof(LogNPCUpdateEvent)) return false;
		if (eventType == typeof(IIDChangeEvent)) return false;
		if (eventType == typeof(MapChangeEvent)) return false;

		if (eventType == typeof(SkillCastEvent)) return false;
		if (eventType == typeof(EndSkillCastEvent)) return true;
		if (eventType == typeof(StartSkillCastEvent)) return true;
		if (eventType == typeof(ResetSkillCastEvent)) return true;

		if (eventType == typeof(SquadGroundMarkerEvent)) return false;
		if (eventType == typeof(SquadGroundMarkerPlaceEvent)) return false;
		if (eventType == typeof(SquadGroundMarkerRemoveEvent)) return false;

		// The unknown event can come from anything
		if (eventType == typeof(UnknownEvent)) return true;
		if (eventType == typeof(UnknownExtensionEvent)) return false;

		throw new ArgumentException($"Event type {eventType} is not supported.");
	}

	/// <summary>
	/// Returns a list of results that will directly produce this type,
	/// results that result in subclasses of this type are not included.
	/// </summary>
	private static IEnumerable<Result> GetDirectPhysicalResultsForEventType(Type eventType)
	{
		if (eventType == typeof(Event)) return [];

		if (eventType == typeof(AgentEvent)) return [];
		if (eventType == typeof(AgentEnterCombatEvent)) return [];
		if (eventType == typeof(AgentExitCombatEvent)) return [];
		if (eventType == typeof(AgentRevivedEvent)) return [];
		if (eventType == typeof(AgentDownedEvent)) return [];
		if (eventType == typeof(AgentDeadEvent)) return [];
		if (eventType == typeof(AgentSpawnEvent)) return [];
		if (eventType == typeof(AgentDespawnEvent)) return [];
		if (eventType == typeof(AgentHealthUpdateEvent)) return [];
		if (eventType == typeof(AgentWeaponSwapEvent)) return [];
		if (eventType == typeof(AgentMaxHealthUpdateEvent)) return [];
		if (eventType == typeof(AgentMarkerEvent)) return [];
		if (eventType == typeof(AgentMarkerRemoveAllEvent)) return [];
		if (eventType == typeof(InitialBuffEvent)) return [];
		if (eventType == typeof(PositionChangeEvent)) return [];
		if (eventType == typeof(VelocityChangeEvent)) return [];
		if (eventType == typeof(FacingChangeEvent)) return [];
		if (eventType == typeof(TeamChangeEvent)) return [];
		if (eventType == typeof(TargetableChangeEvent)) return [];
		if (eventType == typeof(DefianceBarHealthUpdateEvent)) return [];
		if (eventType == typeof(BarrierUpdateEvent)) return [];
		if (eventType == typeof(DefianceBarStateUpdateEvent)) return [];
		if (eventType == typeof(EffectEvent)) return [];
		if (eventType == typeof(EffectStartEvent)) return [];
		if (eventType == typeof(EffectEndEvent)) return [];
		if (eventType == typeof(AgentGliderOpenEvent)) return [];
		if (eventType == typeof(AgentGliderCloseEvent)) return [];

		if (eventType == typeof(BuffEvent)) return [];
		if (eventType == typeof(BuffRemoveEvent)) return [];
		if (eventType == typeof(AllStacksRemovedBuffEvent)) return [];
		if (eventType == typeof(SingleStackRemovedBuffEvent)) return [];
		if (eventType == typeof(ManualStackRemovedBuffEvent)) return [];
		if (eventType == typeof(BuffApplyEvent)) return [];
		if (eventType == typeof(ActiveBuffStackEvent)) return [];
		if (eventType == typeof(ResetBuffStackEvent)) return [];
		if (eventType == typeof(BuffExtensionEvent)) return [];

		if (eventType == typeof(DamageEvent)) return [];
		if (eventType == typeof(PhysicalDamageEvent)) return [Result.Normal, Result.Critical, Result.Glance, Result.Interrupt, Result.KillingBlow, Result.Downed];
		if (eventType == typeof(IgnoredPhysicalDamageEvent)) return [Result.Block, Result.Evade, Result.Absorb, Result.Blind];
		if (eventType == typeof(IgnoredBuffDamageEvent)) return [];
		if (eventType == typeof(BuffDamageEvent)) return [];
		if (eventType == typeof(OffCycleBuffDamageEvent)) return [];
		if (eventType == typeof(DefianceBarDamageEvent)) return [Result.DefianceBar];
		
		if (eventType == typeof(CrowdControlEvent)) return [Result.CrowdControl];

		if (eventType == typeof(RewardEvent)) return [];
		if (eventType == typeof(RateHealthEvent)) return [];
		if (eventType == typeof(StatResetEvent)) return [];
		if (eventType == typeof(LogNPCUpdateEvent)) return [];
		if (eventType == typeof(IIDChangeEvent)) return [];
		if (eventType == typeof(MapChangeEvent)) return [];

		if (eventType == typeof(SkillCastEvent)) return [];
		if (eventType == typeof(EndSkillCastEvent)) return [];
		if (eventType == typeof(StartSkillCastEvent)) return [];
		if (eventType == typeof(ResetSkillCastEvent)) return [];

		if (eventType == typeof(SquadGroundMarkerEvent)) return [];
		if (eventType == typeof(SquadGroundMarkerPlaceEvent)) return [];
		if (eventType == typeof(SquadGroundMarkerRemoveEvent)) return [];

		// The unknown event can come from any result, including not yet implemented ones,
		// so we need to return all of them.
		Debug.Assert(Enum.GetUnderlyingType(typeof(Result)) == typeof(byte));
		if (eventType == typeof(UnknownEvent)) return Enumerable.Range(0, 256).Select(x => (Result) x);
		if (eventType == typeof(UnknownExtensionEvent)) return [];

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
			StateChange.Extension => false,
			StateChange.ApiDelayed => true,
			StateChange.InstanceStart => true,
			StateChange.TickRate => false,
			StateChange.Last90BeforeDown => false,
			StateChange.Effect => false,
			StateChange.IdToGuid => true,
			StateChange.LogNPCUpdate => true,
			StateChange.IdleEvent => false,
			StateChange.ExtensionCombat => true,
			StateChange.FractalScale => true,
			StateChange.Effect2 => false,
			StateChange.Ruleset => true,
			StateChange.SquadMarker => false,
			StateChange.ArcBuild => true,
			StateChange.Glider => false,
			StateChange.IIDChange => false,
			StateChange.MapChange => false,
			_ => throw new ArgumentOutOfRangeException(nameof(stateChange), stateChange, null)
		};
	}
}