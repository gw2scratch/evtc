using ScratchEVTCParser.Parsed.Enums;

namespace ScratchEVTCParser.Parsed
{
	public class ParsedCombatItem
	{
		public long Time { get; }
		public ulong SrcAgent { get; }
		public ulong DstAgent { get; }
		public int Value { get; }
		public int BuffDmg { get; }
		public ushort OverstackValue { get; }
		public ushort SkillId { get; }
		public ushort SrcAgentId { get; }
		public ushort DstAgentId { get; }
		public ushort SrcMasterId { get; }
		public FriendOrFoe Iff { get; }
		public ushort Buff { get; }
		public Result Result { get; }
		public Activation IsActivation { get; }
		public BuffRemove IsBuffRemove { get; }
		public ushort IsNinety { get; }
		public ushort IsFifty { get; }
		public ushort IsMoving { get; }
		public StateChange IsStateChange { get; }
		public ushort IsFlanking { get; }
		public ushort IsShields { get; }

		public ParsedCombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, ushort overstackValue,
			ushort skillId, ushort srcAgentId, ushort dstAgentId, ushort srcMasterId, FriendOrFoe iff, ushort buff,
			Result result, Activation isActivation, BuffRemove isBuffRemove, ushort isNinety, ushort isFifty,
			ushort isMoving, StateChange isStateChange, ushort isFlanking, ushort isShields)
		{
			Time = time;
			SrcAgent = srcAgent;
			DstAgent = dstAgent;
			Value = value;
			BuffDmg = buffDmg;
			OverstackValue = overstackValue;
			SkillId = skillId;
			SrcAgentId = srcAgentId;
			DstAgentId = dstAgentId;
			SrcMasterId = srcMasterId;
			Iff = iff;
			Buff = buff;
			Result = result;
			IsActivation = isActivation;
			IsBuffRemove = isBuffRemove;
			IsNinety = isNinety;
			IsFifty = isFifty;
			IsMoving = isMoving;
			IsStateChange = isStateChange;
			IsFlanking = isFlanking;
			IsShields = isShields;
		}
	}
}