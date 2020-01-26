using System.Collections.Generic;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.GameData.Encounters;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Names;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Phases;
using GW2Scratch.EVTCAnalytics.Processing.Encounters.Results;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters
{
	public class WorldVersusWorldEncounterData : BaseEncounterData
	{
		public IReadOnlyList<Player> Enemies { get; }

		public WorldVersusWorldEncounterData(
			IEnumerable<Player> enemies)
			: base(
				Encounter.WorldVersusWorld,
				Enumerable.Empty<Agent>(),
				new PhaseSplitter(new StartTrigger(new PhaseDefinition("Combat"))),
				new ConstantResultDeterminer(EncounterResult.Unknown),
				new ConstantModeDeterminer(EncounterMode.Normal)
			)
		{
			Enemies = enemies.ToArray();
		}
	}
}