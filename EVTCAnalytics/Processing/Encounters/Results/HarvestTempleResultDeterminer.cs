using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Results
{
	/// <summary>
	/// A result determiner that returns success if an agent is despawned.
	/// </summary>
	public class HarvestTempleResultDeterminer : IResultDeterminer
	{
		private readonly Gadget soowon;
		private readonly IReadOnlyList<Player> players;

		public HarvestTempleResultDeterminer(Gadget soowon, IReadOnlyList<Player> players)
		{
			this.soowon = soowon;
			this.players = players;
		}

		public ResultDeterminerResult GetResult(IEnumerable<Event> events)
		{
			var attackTarget = soowon.AttackTargets.First();

			var targetOff = events.OfType<TargetableChangeEvent>().Where(x => x.AttackTarget == attackTarget && x.Time > soowon.FirstAwareTime + 1000 && !x.IsTargetable).ToList();

			if (targetOff.Count < 2)
			{
				return new ResultDeterminerResult(EncounterResult.Failure, null);
			}
			else if (targetOff.Count > 2)
			{
				return new ResultDeterminerResult(EncounterResult.Failure, null);
			}

			var lastDmg = events.OfType<DamageEvent>().Where(x => x.Defender == soowon).LastOrDefault();
			if (null == lastDmg)
			{
				return new ResultDeterminerResult(EncounterResult.Failure, null);
			}

			// check if at least one player is alive
			foreach (var p in players)
			{
				var death = events.OfType<AgentDeadEvent>().FirstOrDefault(x => x.Agent == p && x.Time < targetOff[1].Time + 200);
				var despawn = events.OfType<AgentDespawnEvent>().FirstOrDefault(x => x.Agent == p && x.Time < targetOff[1].Time + 200);

				if (death == null && despawn == null)
				{
					// alive
					return new ResultDeterminerResult(EncounterResult.Success, lastDmg.Time);
				}

			}
			return new ResultDeterminerResult(EncounterResult.Failure, lastDmg.Time);
		}
	}
}