namespace ScratchEVTCParser.Parsed.Enums
{
	public enum StateChange : byte
	{
		Normal          =  0,
		EnterCombat     =  1,
		ExitCombat      =  2,
		ChangeUp        =  3,
		ChangeDead      =  4,
		ChangeDown      =  5,
		Spawn           =  6,
		Despawn         =  7,
		HealthUpdate    =  8,
		LogStart        =  9,
		LogEnd          = 10,
		WeaponSwap      = 11,
		MaxHealthUpdate = 12,
		PointOfView     = 13,
		Language    = 14,
		GWBuild         = 15,
		ShardId         = 16,
		Reward          = 17,
		BuffInitial     = 18,
		Position        = 19,
		Velocity        = 20,
		Rotation        = 21,
		TeamChange      = 22,
		Unknown
	};
}