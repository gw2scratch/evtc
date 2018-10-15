using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ScratchEVTCParser.Model.Agents;
using ScratchEVTCParser.Model.Skills;

namespace ScratchEVTCParser.Statistics.Buffs
{
	public class IntensityBuffStackCollection : IBuffStackCollection
	{
		private readonly int stackLimit;
		private readonly List<BuffStack> currentStacks = new List<BuffStack>();

		private bool finalized = false;
		private long lastTime = long.MinValue;
		private int currentStackCount = 0;

		private List<BuffStatusSegment> segments = new List<BuffStatusSegment>();

		public Skill Buff { get; }
		public IEnumerable<BuffStatusSegment> BuffSegments => segments;

		public IntensityBuffStackCollection(Skill buff, int stackLimit)
		{
			Buff = buff;
			this.stackLimit = stackLimit;
		}

		public void AddStack(long timeStart, long timeEnd, Agent source)
		{
			var currentTime = timeStart;
			EnsureConsistency(currentTime);

            StartNewSegment(timeStart);
			currentStacks.Add(new BuffStack(timeStart, timeEnd, Buff, source));
			currentStackCount++;
		}

		public void RemoveAllStacks(long time)
		{
			EnsureConsistency(time);
			StartNewSegment(time);
			currentStacks.Clear();
			currentStackCount = 0;
		}

		public void RemoveStack(long time, long durationRemaining)
		{
			EnsureConsistency(time);

			bool removed = false;
			for (int i = 0; i < currentStacks.Count; i++)
			{
				var stack = currentStacks[i];
				if (stack.TimeEnd - time == durationRemaining)
				{
					// Remove by copying last stack in list to this position and removing the last stack.
					currentStacks[i] = currentStacks[currentStacks.Count - 1];
					currentStacks.RemoveAt(currentStacks.Count - 1);
					removed = true;
					break;
				}
			}

			if (removed)
			{
                StartNewSegment(time);
				currentStackCount--;
			}
			else
			{
				// TODO: Count misses?
			}
		}

		public void FinalizeCollection(long time)
		{
			EnsureConsistency(time);

			// Finish the last segment
            StartNewSegment(time);

			// Remove segments with 0 duration
			segments = segments.Where(x => x.TimeEnd - x.TimeStart > 0).ToList();
			finalized = true;
		}

		private void RemoveExpiredStacks(long currentTime)
		{
			var removeTimes = new List<long>();
			for (int i = currentStacks.Count - 1; i >= 0; i--)
			{
				if (currentStacks[i].TimeEnd < currentTime)
				{
					// Remove by copying last stack in list to this position and removing the last stack.
					removeTimes.Add(currentStacks[i].TimeEnd);
					currentStacks[i] = currentStacks[currentStacks.Count - 1];
					currentStacks.RemoveAt(currentStacks.Count - 1);
				}
			}

			removeTimes.Sort();

			foreach (var removeTime in removeTimes)
			{
				StartNewSegment(removeTime);
				currentStackCount--;
			}
		}

		private void EnsureConsistency(long currentTime)
		{
			if (finalized)
			{
				throw new InvalidOperationException("This collection has been finalized.");
			}

			if (segments.Count == 0)
			{
				// Add an initial segment to avoid handling a special case when there are no segments yet.
				segments.Add(new BuffStatusSegment(currentTime, currentTime, 0));
			}

			if (currentTime < lastTime)
			{
				throw new ArgumentException("Stacks have to be added ordered by time", nameof(currentTime));
			}

			lastTime = currentTime;

			RemoveExpiredStacks(currentTime);
		}

		private void StartNewSegment(long segmentTimeStart)
		{
			int stackCount = Math.Min(stackLimit, currentStackCount);

			var previousSegment = segments[segments.Count - 1];

			if (previousSegment.StackCount == stackCount)
			{
				// Merge consecutive segments with the same stack counts.
				var extendedSegment = new BuffStatusSegment(previousSegment.TimeStart, segmentTimeStart, stackCount);
				segments[segments.Count - 1] = extendedSegment;
				return;
			}

            var segmentStart = previousSegment.TimeEnd;
            segments.Add(new BuffStatusSegment(segmentStart, segmentTimeStart, stackCount));
		}
	}
}