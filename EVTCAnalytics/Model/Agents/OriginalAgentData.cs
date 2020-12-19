namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	/// <summary>
	/// Represents the original identification values of a <see cref="Parsed.ParsedAgent"/> used in
	/// construction of an <see cref="Agent"/>.
	/// </summary>
	public class OriginalAgentData
	{
		/// <summary>
		/// An address of a <see cref="Parsed.ParsedAgent"/>.
		/// </summary>
		public ulong Address { get; }
		
		/// <summary>
		/// An ID of a <see cref="Parsed.ParsedAgent"/>.
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// Creates a new instance of <see cref="OriginalAgentData"/>.
		/// </summary>
		public OriginalAgentData(ulong address, int id)
		{
			Address = address;
			Id = id;
		}
	}
}