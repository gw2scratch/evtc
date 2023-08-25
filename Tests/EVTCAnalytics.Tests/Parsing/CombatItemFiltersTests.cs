using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Parsed;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;
using GW2Scratch.EVTCAnalytics.Parsing;
using GW2Scratch.EVTCAnalytics.Processing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GW2Scratch.EVTCAnalytics.Tests.Parsing;

public class CombatItemFiltersTests
{
	[Test]
	[TestCaseSource(nameof(GetEventTypes))]
	public void EventIsHandled(Type type)
	{
		Assert.DoesNotThrow(() =>
			{
				var changes = CombatItemFilters.GetStateChangesForEventType(type);
				Assert.IsNotNull(changes);
			}, $"Failed to get state changes for event type {type}"
		);
	}
	
	[Test]
	[TestCaseSource(nameof(GetStateChanges))]
	public void StateChangeIsAlwaysKeptIsImplemented(StateChange stateChange)
	{
		Assert.DoesNotThrow(() =>
			{
				CombatItemFilters.IsAlwaysKept(stateChange);
			}, $"Failed to get CombatItemFilters.IsAlwaysKept() for state change {stateChange}"
		);
	}

	public static IEnumerable<Type> GetEventTypes()
	{
		var assembly = Assembly.GetAssembly(typeof(Event))!;
		var types = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Event)) || type == typeof(Event));
		return types;
	}
	
	public static IEnumerable<StateChange> GetStateChanges()
	{
		return Enum.GetValues<StateChange>();
	}
}