namespace GW2Scratch.EVTCAnalytics.Parsed.Enums
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
		Tag = 37, // Renamed from CBTS_TAG to CBTS_MARKER within arcdps at some point.
		BarrierUpdate = 38,
		StatReset = 39, // Only in realtime API; should not appear in logs
		Extension = 40,
		ApiDelayed = 41, // Only in realtime API; should not appear in logs
		InstanceStart = 42,
		TickRate = 43,
		Last90BeforeDown = 44, // Removed with 20240612; "redundant as healthpercent is part of the limited set"
		Effect = 45, // Not used since 20230716
		IdToGuid = 46,
		LogNPCUpdate = 47,
		IdleEvent = 48, // Internal in arcdps; shouldn't appear
		ExtensionCombat = 49, // Intended for extensions to indicate used skills for the skill table
		FractalScale = 50,
		Effect2 = 51, // Replaces Effect since 20230716
		Ruleset = 52, // Added 20240328
		SquadMarker = 53, // Added 20240328
		ArcBuild = 54, // Added 20240614
		Glider = 55, // Added 20240627
	};
}