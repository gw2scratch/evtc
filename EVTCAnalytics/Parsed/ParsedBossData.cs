namespace GW2Scratch.EVTCAnalytics.Parsed
{
	/// <summary>
	/// Contains data about the main target of the fight.
	/// </summary>
	public class ParsedBossData
	{
		/// <summary>
		/// Gets the ID used to trigger the encounter logging.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This may be a species id of an NPC, an arcdps id of a gadget, or a magic value
		/// for a special type of log, such as World versus World logs.
		/// </para>
		/// <para>
		/// It is possible that future versions of arcdps may introduce new magic values here.
		/// </para>
		/// </remarks>
		public ushort ID { get; }

		public ParsedBossData(ushort id)
		{
			ID = id;
		}
	}
}