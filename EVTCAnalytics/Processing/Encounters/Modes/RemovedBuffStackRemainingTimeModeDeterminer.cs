using System;
using System.Linq;
using GW2Scratch.EVTCAnalytics.Events;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Processing.Encounters.Modes
{
	/// <summary>
	/// Determines encounter mode according to whether a buff was removed with a specific remaining time.
	/// </summary>
	public class RemovedBuffStackRemainingTimeModeDeterminer : IModeDeterminer
	{
		public const int RemainingTimePrecision = 100;
		private readonly int buffId;
		private readonly EncounterMode mode1;
		private readonly int remaining1;
		private readonly EncounterMode mode2;
		private readonly int remaining2;

		/// <summary>
		/// Creates a determiner that identifies the encounter mode according to whether a buff was removed with a specific remaining time.
		/// </summary>
		/// <param name="buffId">The skill id of the tracked buff.</param>
		/// <param name="mode1">First encounter mode option.</param>
		/// <param name="remaining1">The remaining time of the buff in order to identify the encounter as <paramref name="mode1"/>.</param>
		/// <param name="mode2">Second encounter mode option.</param>
		/// <param name="remaining2">The remaining time of the buff in order to identify the encounter as <paramref name="mode2"/>.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="remaining1"/> and <paramref name="remaining2"/> are within
		/// 2*<see cref="RemainingTimePrecision"/>, which would result in ambiguous mode detection as a remaining buff duration would be too close to both.</exception>
		public RemovedBuffStackRemainingTimeModeDeterminer(
			int buffId,
			EncounterMode mode1,
			int remaining1,
			EncounterMode mode2,
			int remaining2
		)
		{
			this.buffId = buffId;
			this.mode1 = mode1;
			this.remaining1 = remaining1;
			this.mode2 = mode2;
			this.remaining2 = remaining2;
			if (Math.Abs(this.remaining1 - this.remaining2) < RemainingTimePrecision * 2)
			{
				throw new ArgumentException("Ambiguous buff remaining times." +
				                            "For some values both modes would be a possible result." +
				                            $"Make sure {nameof(remaining1)} and {nameof(remaining2)} are not " +
				                            $"within {2*RemainingTimePrecision} of each other",
					nameof(this.remaining1));
			}
		}

		public EncounterMode? GetMode(Log log)
		{
			// First we try to determine which of the two possible modes this is.
			EncounterMode? mode = null;
			bool buffAppliedAfterModeIdentified = false;

			foreach (var buffEvent in log.Events.OfType<BuffEvent>().Where(x => x.Buff.Id == buffId))
			{
				if (buffEvent is ManualStackRemovedBuffEvent remove)
				{
					if (mode != null)
					{
						// We have already identified a mode, now we are only looking for a buff application.
						continue;
					}

					if (Math.Abs(remove.RemainingDuration - remaining1) < RemainingTimePrecision)
					{
						mode = mode1;
					}
					else if (Math.Abs(remove.RemainingDuration - remaining2) < RemainingTimePrecision)
					{
						mode = mode2;
					}
				}
				else if (buffEvent is BuffApplyEvent)
				{
					if (mode != null)
					{
						buffAppliedAfterModeIdentified = true;
						break;
					}
				}
			}

			// After finding out the potential mode, we make sure the buff was applied again.
			// If the buff was not reapplied again, we only found the potential mode according
			// to the last buff removal of the fight and thus the remaining time may be anything,
			// so we fall back to the default mode.

			if (buffAppliedAfterModeIdentified)
			{
				return mode.Value;
			}
			else
			{
				return null;
			}
		}
	}
}