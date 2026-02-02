namespace GW2Scratch.EVTCAnalytics.GameData
{
	/// <summary>
	/// Provides common arcdps ids for gadgets.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Note that these ids do not come from the game, they are defined by arcdps instead.
	/// </para>
	/// <para>
	/// As the gadget ids is effectively a hash of parameters of the gadget, this is prone to collisions
	/// and some dynamically placed gadgets may not always have the same id.
	/// </para>
	/// </remarks>
	public static class GadgetIds
	{
		// Wing 1
		public const int EtherealBarrier = 47188;
		public const int EtherealBarrierChina = 48133; // ID for the Chinese version of GW2

		// Wing 4
		// Deimos
		public const int ShackledPrisoner = 53040;
		public const int DeimosLastPhase = 24660;

		// Wing 6
		// Conjured Amalgamate
		public const int ConjuredAmalgamate = 43974;
		
		// Strike Missions - End of Dragons
		public const int TheDragonvoid = 43488;
		public const int TheDragonvoidFinal = 1378;
	}
}