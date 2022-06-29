namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	/// <summary>
	/// An encounter mode or difficulty.
	/// </summary>
	/// <remarks>
	/// Challenge Modes are available for many Guild Wars 2 encounters, Emboldened is available in raids, and more
	/// values may be introduced in the future if new modes are added to the game.
	/// </remarks>
	public enum EncounterMode
	{
		Unknown,
		/// <summary>
		/// The standard version of an encounter.
		/// </summary>
		Normal,
		/// <summary>
		/// A harder version of an encounter.
		/// </summary>
		Challenge,
		/// <summary>
		/// 1 stack of Emboldened (easy mode; extra stats).
		/// </summary>
		Emboldened1,
		/// <summary>
		/// 2 stacks of Emboldened (easy mode; extra stats).
		/// </summary>
		Emboldened2,
		/// <summary>
		/// 3 stacks of Emboldened (easy mode; extra stats).
		/// </summary>
		Emboldened3,
		/// <summary>
		/// 4 stacks of Emboldened (easy mode; extra stats).
		/// </summary>
		Emboldened4,
		/// <summary>
		/// 5 stacks of Emboldened (easy mode; extra stats).
		/// </summary>
		Emboldened5,
	}
	
	public static class EncounterModeExtensions
	{
		/// <summary>
		/// Checks this encounter mode is Emboldened regardless of stack count.
		/// </summary>
		public static bool IsEmboldened(this EncounterMode type)
		{
			return type switch
			{
				EncounterMode.Emboldened1 => true,
				EncounterMode.Emboldened2 => true,
				EncounterMode.Emboldened3 => true,
				EncounterMode.Emboldened4 => true,
				EncounterMode.Emboldened5 => true,
				_ => false
			};
		}
	}
}