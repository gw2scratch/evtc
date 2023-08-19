using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GW2Scratch.EVTCAnalytics.Parsing;

public class CombatItemFilters
{
	private IReadOnlyList<Type> RequiredEventTypes { get; }
	private bool[] StateChangeFilter { get; }
	
	public CombatItemFilters(IReadOnlyList<Type> requiredEventTypes)
	{
		RequiredEventTypes = requiredEventTypes;
		StateChangeFilter = BuildStateChangeFilter(RequiredEventTypes);
	}

	public bool IsStateChangeRequired(StateChange stateChange)
	{
		return StateChangeFilter[(int) stateChange];
	}
	
	public bool IsStateChangeRequired(byte stateChange)
	{
		return StateChangeFilter[stateChange];
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

		return stateChanges;
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
}