namespace ScratchEVTCParser.Statistics.Encounters
{
	public class Phase
	{
		public Phase(long startingTime, long endingTime, int phaseOrder, string name)
		{
			StartingTime = startingTime;
			EndingTime = endingTime;
			PhaseOrder = phaseOrder;
			Name = name;
		}

		public long StartingTime { get; }
		public long EndingTime { get; }
		public int PhaseOrder { get; }
		public string Name { get; }
	}
}