namespace ScratchEVTCParser.Model.Agents
{
	public class Gadget : Agent
	{
		public int VolatileId { get; }

		public Gadget(ulong address, int id, string name, int hitboxWidth, int hitboxHeight)
			: base(address, id, name, hitboxWidth, hitboxHeight)
		{
			VolatileId = id;
		}
	}
}