namespace GW2Scratch.EVTCAnalytics.GameData;

public static class MapIds
{
	public const int EternalBattlegrounds = 38;
	public const int RedDesertBorderlands = 1099;
	public const int BlueAlpineBorderlands = 96;
	public const int GreenAlpineBorderlands = 95;
	public const int ObsidianSanctum = 899;
	public const int EdgeOfTheMists = 968;
	public const int ArmisticeBastion = 1315;
	public const int RaidWing1 = 1062;
	public const int RaidWing2 = 1149;
	public const int RaidWing3 = 1156;
	public const int RaidWing4 = 1188;
	public const int RaidWing5 = 1264;
	public const int RaidWing6 = 1303;
	public const int RaidWing7 = 1323;
	public const int RaidWing8 = 1564;

	public const int XunlaiJadeJunkyard = 1451;
	public const int OldLionsCourt = 1485;
	public const int HarvestTemple = 1437;
	public const int SilentSurf = 1500;
	public const int CosmicObservatory = 1515;
	public const int TempleOfFebe = 1520;
	public const int LonelyTower = 1538;

	public static bool IsRaidMap(int id)
	{
		return id is RaidWing1 or RaidWing2 or RaidWing3 or RaidWing4 or RaidWing5 or RaidWing6 or RaidWing7 or RaidWing8;
	}
}