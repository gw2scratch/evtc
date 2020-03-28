namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	public class OriginalAgentData
	{
		public ulong Address { get; }
		public int Id { get; }

		public OriginalAgentData(ulong address, int id)
		{
			Address = address;
			Id = id;
		}
	}
}