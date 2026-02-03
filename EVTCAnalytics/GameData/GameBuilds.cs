namespace GW2Scratch.EVTCAnalytics.GameData
{
	/// <summary>
	/// Provides significant game build (version) numbers.
	/// </summary>
	public static class GameBuilds
	{
		/// <summary>
		/// The release of The Key of Ahdashim, which included a significant rework to raid rewards.
		/// </summary>
		public static int AhdashimRelease = 97235;

		/// <summary>
		/// The release of Cosmic Observatory Challenge Mode, which revamped the health of the normal mode as well.<br></br>
		/// https://wiki.guildwars2.com/wiki/Game_updates/2023-11-07
		/// </summary>
		public static int CosmicObservatoryCMRelease = 153978;
		
		/// <summary>
		/// The Temple of Febe Challenge Mode health fix unifying the health in first phase and in later phases.
		/// </summary>
		/// <remarks>
		/// Note that there were more health changes to this encounter, make sure this is what you mean.
		/// </remarks>
		public static int TempleOfFebeHealthFix = 158968;

		/// <summary>
		/// Lonely Tower release, no challenge mode.
		/// <br />
		/// https://wiki.guildwars2.com/wiki/Game_updates/2024-05-21
		/// </summary>
		public static int LonelyTowerRelease = 163141;

		/// <summary>
		/// Lonely Tower Challenge Mode release with HP nerf.
		/// <br />
		/// https://wiki.guildwars2.com/wiki/Game_updates/2024-06-04
		/// </summary>
		public static int LonelyTowerCMRelease = 163807;

		/// <summary>
		/// Eparch further HP nerfs for all modes.
		/// <br />
		/// https://wiki.guildwars2.com/wiki/Game_updates/2024-06-25
		/// </summary>
		public static int LonelyTowerHPNerf2 = 164824;

		/// <summary>
		/// Wing 8 Release
		/// <br/>
		/// https://wiki.guildwars2.com/wiki/Game_updates/2024-11-19
		/// </summary>
		public static int MountBalriorRelease = 171452;
		
		/// <summary>
		/// Wing 8 Challenge Mode Release
		/// <br/>
		/// https://wiki.guildwars2.com/wiki/Game_updates/2025-03-11
		/// </summary>
		public static int MountBalriorCMRelease = 176750;

		/// <summary>
		/// Kinfall Fractal Release<br/>
		/// https://wiki.guildwars2.com/wiki/Game_updates/2025-06-03
		/// </summary>
		public static int KinfallRelease = 181657;

		/// <summary>
		/// Kinfall Fractal CM Release<br/>
		/// https://wiki.guildwars2.com/wiki/Game_updates/2025-06-24
		/// </summary>
		public static int KinfallCMRelease = 182824;

		/// <summary>
		/// Guardian's Glade Release & Raid Quickplay<br/>
		/// https://wiki.guildwars2.com/wiki/Game_updates/2026-02-03
		/// </summary>
		public static int GuardiansGladeRaidQuickplayRelease = 194969;
	}
}