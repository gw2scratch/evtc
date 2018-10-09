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
		public uint OverstackValue { get; }
		public uint SkillId { get; }
		public ushort SrcAgentId { get; }
		public ushort DstAgentId { get; }
		public ushort SrcMasterId { get; }
		public ushort DstMasterId { get; }
		public FriendOrFoe Iff { get; }
		public byte Buff { get; }
		public Result Result { get; }
		public Activation IsActivation { get; }
		public BuffRemove IsBuffRemove { get; }
		public byte IsNinety { get; }
		public byte IsFifty { get; }
		public byte IsMoving { get; }
		public StateChange IsStateChange { get; }
		public byte IsFlanking { get; }
		public byte IsShields { get; }
		public byte IsOffCycle { get; }

		public ParsedCombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
			uint skillId, ushort srcAgentId, ushort dstAgentId, ushort srcMasterId, ushort dstMasterId, FriendOrFoe iff, byte buff,
			Result result, Activation isActivation, BuffRemove isBuffRemove, byte isNinety, byte isFifty,
			byte isMoving, StateChange isStateChange, byte isFlanking, byte isShields, byte isOffCycle)
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
			DstMasterId = dstMasterId;
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
			IsOffCycle = isOffCycle;
		}
	}
}