﻿namespace GW2Scratch.EVTCAnalytics.Parsed.Enums
{
	/// <summary>
	/// The type of a state change as defined by the <c>cbtstatechange</c> arcdps enum.
	/// </summary>
	public enum StateChange : byte
	{
		Normal = 0,
		EnterCombat = 1,
		ExitCombat = 2,
		ChangeUp = 3,
		ChangeDead = 4,
		ChangeDown = 5,
		Spawn = 6,
		Despawn = 7,
		HealthUpdate = 8,
		LogStart = 9,
		LogEnd = 10,
		WeaponSwap = 11,
		MaxHealthUpdate = 12,
		PointOfView = 13,
		Language = 14,
		GWBuild = 15,
		ShardId = 16,
		Reward = 17,
		BuffInitial = 18,
		Position = 19,
		Velocity = 20,
		Rotation = 21,
		TeamChange = 22,
		AttackTarget = 23,
		Targetable = 24,
		MapId = 25,
		ReplInfo = 26, // Internal in arcdps, shouldn't appear
		StackActive = 27,
		StackReset = 28,
		Guild = 29,
		BuffInfo = 30,
		BuffFormula = 31,
		SkillInfo = 32,
		SkillTiming = 33,
		BreakbarState = 34,
		BreakbarPercent = 35,
		Error = 36,
		Tag = 37,
		BarrierUpdate = 38,
		StatReset = 39, // Only in realtime API; should not appear in logs
		Extension = 40,
		ApiDelayed = 41, // Only in realtime API; should not appear in logs
		InstanceStart = 42,
		TickRate = 43,
		Last90BeforeDown = 44,
		Effect = 45,
		IdToGuid = 46,
	};
}