namespace GW2Scratch.EVTCAnalytics.GameData;

public static class MapIds
{
	public const int RaidWing1 = 1062;
	public const int RaidWing2 = 1149;
	public const int RaidWing3 = 1156;
	public const int RaidWing4 = 1188;
	public const int RaidWing5 = 1264;
	public const int RaidWing6 = 1303;
	public const int RaidWing7 = 1323;

	public const int XunlaiJadeJunkyard = 1451;

	public static bool IsRaidMap(int id)
	{
		return id is RaidWing1 or RaidWing2 or RaidWing3 or RaidWing4 or RaidWing5 or RaidWing6 or RaidWing7;
	}
}