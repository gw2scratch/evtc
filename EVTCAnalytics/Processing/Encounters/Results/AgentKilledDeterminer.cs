using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// Returns success if an agent is killed - has a death event or a killing blow towards them.
	/// </summary>
	public class AgentKilledDeterminer : AnyCombinedResultDeterminer
	{
		public AgentKilledDeterminer(Agent agent) : base(new AgentDeadDeterminer(agent), new AgentKillingBlowDeterminer(agent))
		{
		}
	}
}