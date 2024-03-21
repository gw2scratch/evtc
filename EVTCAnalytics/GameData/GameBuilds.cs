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
		/// The release of Cosmic Observatory Challenge Mode, which revamped the health of the normal mode as well.
		/// </summary>
		public static int CosmicObservatoryCMRelease = 153978;
		
		/// <summary>
		/// The Temple of Febe Challenge Mode health fix unifying the health in first phase and in later phases.
		/// </summary>
		/// <remarks>
		/// Note that there were more health changes to this encounter, make sure this is what you mean.
		/// </remarks>
		public static int TempleOfFebeHealthFix = 158968;
	}
}