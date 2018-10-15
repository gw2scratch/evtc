using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using ScratchEVTCParser.Model.Skills;
using ScratchEVTCParser.Statistics.Buffs;

namespace ScratchEVTCParser.Tests.Statistics.Buffs
{
	[TestFixture]
	public class IntensityBuffStackCollectionTests
	{
		private readonly Skill skill = new Skill(1, "Test Skill");

		[Test]
		public void AddingOneStack_CreatesOneSegmentWithOneStack()
		{
			const int timeStart = 10;
			var collection = new IntensityBuffStackCollection(skill, int.MaxValue);

			collection.AddStack(timeStart, 25, null);
			collection.FinalizeCollection(15);

			var segments = collection.BuffSegments.ToArray();

			Assert.AreEqual(1, segments.Length);
			Assert.AreEqual(timeStart, segments[0].TimeStart);
			Assert.AreEqual(1, segments[0].StackCount);
		}

		[Test]
		public void Finalize_UsesTimeOfFinalizeForLastSegmentTimeEnd()
		{
			const int finalizeTime = 15;
			const int timeEnd = 25;
			Debug.Assert(finalizeTime < timeEnd);

			var collection = new IntensityBuffStackCollection(skill, int.MaxValue);

			collection.AddStack(10, timeEnd, null);
			collection.FinalizeCollection(finalizeTime);

			var segments = collection.BuffSegments.ToArray();

			Assert.AreEqual(1, segments.Length);
			Assert.AreEqual(finalizeTime, segments[0].TimeEnd);
		}

		[Test]
		public void AddingTwoStacksAtTheSameTime_CreatesOneSegmentWithTwoStacks()
		{
			var collection = new IntensityBuffStackCollection(skill, int.MaxValue);

			collection.AddStack(10, 25, null);
			collection.AddStack(10, 25, null);
			collection.FinalizeCollection(15);

			var segments = collection.BuffSegments.ToArray();

			Assert.AreEqual(1, segments.Length);
			Assert.AreEqual(10, segments[0].TimeStart);
			Assert.AreEqual(2, segments[0].StackCount);
		}

		[Test]
		public void AddStack_ProcessesRunningOutStacks()
		{
			var collection = new IntensityBuffStackCollection(skill, int.MaxValue);

			collection.AddStack(10, 25, null);
			collection.AddStack(10, 20, null);

			collection.AddStack(30, 35, null);
			collection.FinalizeCollection(35);

			var segments = collection.BuffSegments.ToArray();

			// 10 - 20 2 stacks
			// 20 - 25 1 stacks, second added stack ran out
			// 25 - 30 0 stacks, first added stack ran out
			// 30 - 35 1 stack, third stack added
			Assert.AreEqual(4, segments.Length);
			Assert.AreEqual(10, segments[0].TimeStart);
			Assert.AreEqual(2, segments[0].StackCount);
			Assert.AreEqual(20, segments[0].TimeEnd);
			Assert.AreEqual(20, segments[1].TimeStart);
			Assert.AreEqual(1, segments[1].StackCount);
			Assert.AreEqual(25, segments[1].TimeEnd);
			Assert.AreEqual(25, segments[2].TimeStart);
			Assert.AreEqual(0, segments[2].StackCount);
			Assert.AreEqual(30, segments[2].TimeEnd);
			Assert.AreEqual(30, segments[3].TimeStart);
			Assert.AreEqual(1, segments[3].StackCount);
		}

		[Test]
		public void Finalize_ProcessesRunningOutStacks()
		{
			var collection = new IntensityBuffStackCollection(skill, int.MaxValue);

			collection.AddStack(10, 25, null);
			collection.AddStack(10, 20, null);
			collection.FinalizeCollection(30);

			var segments = collection.BuffSegments.ToArray();

			// 10 - 20 2 stacks
			// 20 - 25 1 stacks, second added stack ran out
			// 25 - 30 0 stacks, first added stack ran out
			Assert.AreEqual(3, segments.Length);
			Assert.AreEqual(10, segments[0].TimeStart);
			Assert.AreEqual(2, segments[0].StackCount);
			Assert.AreEqual(20, segments[0].TimeEnd);
			Assert.AreEqual(20, segments[1].TimeStart);
			Assert.AreEqual(1, segments[1].StackCount);
			Assert.AreEqual(25, segments[1].TimeEnd);
			Assert.AreEqual(25, segments[2].TimeStart);
			Assert.AreEqual(0, segments[2].StackCount);
		}

		[Test]
		public void RemoveAllStacks_ProcessesRunningOutStacks()
		{
			var collection = new IntensityBuffStackCollection(skill, int.MaxValue);

			collection.AddStack(10, 30, null);
			collection.AddStack(10, 20, null);
			collection.RemoveAllStacks(25);
			collection.FinalizeCollection(30);

			var segments = collection.BuffSegments.ToArray();

			// 10 - 20 2 stacks
			// 20 - 25 1 stacks, second added stack ran out
			// 25 - 30 0 stacks, first added stack was removed
			Assert.AreEqual(3, segments.Length);
			Assert.AreEqual(10, segments[0].TimeStart);
			Assert.AreEqual(2, segments[0].StackCount);
			Assert.AreEqual(20, segments[0].TimeEnd);
			Assert.AreEqual(20, segments[1].TimeStart);
			Assert.AreEqual(1, segments[1].StackCount);
			Assert.AreEqual(25, segments[1].TimeEnd);
			Assert.AreEqual(25, segments[2].TimeStart);
			Assert.AreEqual(0, segments[2].StackCount);
		}

		[Test]
		public void RemoveStack_ProcessesRunningOutStacks()
		{
			var collection = new IntensityBuffStackCollection(skill, int.MaxValue);

			collection.AddStack(10, 30, null);
			collection.AddStack(10, 25, null);
			collection.AddStack(10, 20, null);
			collection.RemoveStack(15, 10); // Second stack has 10 remaining now
			collection.FinalizeCollection(30);

			var segments = collection.BuffSegments.ToArray();

			// 10 - 15 3 stacks
			// 15 - 20 2 stacks, second stack removed
			// 20 - 30 1 stack, third stack ran out
			Assert.AreEqual(3, segments.Length);
			Assert.AreEqual(10, segments[0].TimeStart);
			Assert.AreEqual(3, segments[0].StackCount);
			Assert.AreEqual(15, segments[0].TimeEnd);
			Assert.AreEqual(15, segments[1].TimeStart);
			Assert.AreEqual(2, segments[1].StackCount);
			Assert.AreEqual(20, segments[1].TimeEnd);
			Assert.AreEqual(20, segments[2].TimeStart);
			Assert.AreEqual(1, segments[2].StackCount);
		}

		[Test]
		public void StackCountIsHigherThanStackLimit_SegmentStackAmountIsCapped()
		{
			const int stackLimit = 5;
			const int extraStacks = 3;
			var collection = new IntensityBuffStackCollection(skill, stackLimit);

			for (int i = 0; i < stackLimit + extraStacks; i++)
			{
				collection.AddStack(10, 25, null);
			}

			collection.FinalizeCollection(15);

			var segments = collection.BuffSegments.ToArray();

			Assert.AreEqual(1, segments.Length);
			Assert.AreEqual(10, segments[0].TimeStart);
			Assert.AreEqual(15, segments[0].TimeEnd);
			Assert.AreEqual(stackLimit, segments[0].StackCount);
		}

		[Test]
		public void StackCountIsHigherThanStackLimit_SegmentsWithSameCountAreMerged()
		{
			const int stackLimit = 5;
			const int extraStacks = 3;
			var collection = new IntensityBuffStackCollection(skill, stackLimit);

			for (int i = 0; i < stackLimit + extraStacks; i++)
			{
				collection.AddStack(10 + i , 20 + stackLimit + extraStacks, null);
			}

			collection.FinalizeCollection(10 + stackLimit + extraStacks);

			var segments = collection.BuffSegments.ToArray();

			// 10 -- 11 1 stack
			// 11 -- 12 2 stacks
			// 12 -- 13 3 stacks
			// 13 -- 14 4 stacks
			// 14 -- 15 5 stacks |
			// 15 -- 16 5 stacks | merged into
			// 16 -- 17 5 stacks | 14 -- 18 5 stacks
			// 17 -- 18 5 stacks |

			Assert.AreEqual(stackLimit, segments.Length);
			for (int i = 0; i < stackLimit - 1; i++)
			{
                Assert.AreEqual(10 + i, segments[i].TimeStart);
                Assert.AreEqual(10 + i + 1, segments[i].TimeEnd);
                Assert.AreEqual(i + 1, segments[i].StackCount);
			}

			var lastSegment = segments.Last();
			Assert.AreEqual(10 + stackLimit - 1, lastSegment.TimeStart);
			Assert.AreEqual(10 + stackLimit + extraStacks, lastSegment.TimeEnd);
			Assert.AreEqual(stackLimit, lastSegment.StackCount);
		}

		[Test]
		public void RemoveAllStacks_ReducesStackCountToZero()
		{
			const int stackCount = 3;
			var collection = new IntensityBuffStackCollection(skill, int.MaxValue);

			for (int i = 0; i < stackCount; i++)
			{
				collection.AddStack(10 + i , 20 + stackCount, null);
			}

			collection.RemoveAllStacks(10 + stackCount);
			collection.FinalizeCollection(10 + stackCount + 2);

			var segments = collection.BuffSegments.ToArray();

			// 10 -- 11 1 stack
			// 11 -- 12 2 stacks
			// 12 -- 13 3 stacks
			// 13 remove
			// 13 -- 15 0 stacks

			Assert.AreEqual(stackCount + 1, segments.Length);
			for (int i = 0; i < segments.Length - 1; i++)
			{
                Assert.AreEqual(10 + i, segments[i].TimeStart);
                Assert.AreEqual(10 + i + 1, segments[i].TimeEnd);
                Assert.AreEqual(i + 1, segments[i].StackCount);
			}

			var lastSegment = segments.Last();
			Assert.AreEqual(10 + stackCount, lastSegment.TimeStart);
			Assert.AreEqual(0, lastSegment.StackCount);
		}
	}
}