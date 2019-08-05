using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Statistics.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.Statistics.Encounters
{
	public class WorldVersusWorldEncounter : BaseEncounter
	{
		public IReadOnlyList<Player> Enemies { get; }

		public WorldVersusWorldEncounter(IEnumerable<Player> enemies, IEnumerable<Event> events) :
			base(Enumerable.Empty<Agent>(), events,
				new PhaseSplitter(new StartTrigger(new PhaseDefinition("Combat"))),
				new ConstantResultDeterminer(EncounterResult.Unknown),
				new ConstantEncounterNameProvider("World versus World"))
		{
			Enemies = enemies.ToArray();
		}
	}
}