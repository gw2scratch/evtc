using System;

namespace GW2Scratch.ArcdpsLogManager.Timing
{
	/// <summary>
	/// A cooldown measures whether a time period has has passed from last trigger.
	/// </summary>
	public class Cooldown
	{
		private DateTime LastTrigger { get; set; }
		private TimeSpan CooldownPeriod { get; set; }

		/// <summary>
		/// Creates a new Cooldown instance
		/// </summary>
		/// <param name="cooldownPeriod">Time that has to pass between "uses"</param>
		public Cooldown(TimeSpan cooldownPeriod)
		{
			LastTrigger = DateTime.Now - cooldownPeriod;
			CooldownPeriod = cooldownPeriod;
		}

		/// <summary>
		/// Returns true if the cooldown has cooled down and starts the cooldown again.
		/// If it has not cooled down yet, the timer is NOT reset and returns false.
		/// </summary>
		/// <example>
		/// <code>
		/// Cooldown cd = new Cooldown(new TimeSpan(0, 0, 3)); // 3 seconds
		/// while (true) {
		///     if (cd.TryUse())
		///     {
		///        ... an action that can only happen every 3 seconds ...
		///     }
		/// }
		/// </code>
		/// </example>
		/// <seealso cref="CheckUse"/>
		/// <param name="currentTime">Current time</param>
		/// <returns>True if the cooldown is cooled down yet, false otherwise</returns>
		public bool TryUse(DateTime currentTime)
		{
			// Allow triggering on perfect time
			if (currentTime - LastTrigger < CooldownPeriod) // Still cooling down
				return false;

			// Cooled down and using
			Reset(currentTime);
			return true;
		}

		/// <summary>
		/// Checks whether the cooldown has cooled down yet. Does not change reset the cooldown even if it has ran out.
		/// </summary>
		/// <seealso cref="TryUse"/>
		/// <param name="currentTime">Current time</param>
		/// <returns>True if the cooldown is cooled down yet, false otherwise</returns>
		public bool CheckUse(DateTime currentTime)
		{
			// Allow triggering on perfect time
			return currentTime - LastTrigger >= CooldownPeriod;
		}

		/// <summary>
		/// Resets the cooldown, starting the cooldown from the beginning.
		/// </summary>
		/// <param name="currentTime">Current time</param>
		public void Reset(DateTime currentTime)
		{
			LastTrigger = currentTime;
		}
	}
}