using GW2Scratch.EVTCAnalytics.Parsed.Enums;

namespace GW2Scratch.EVTCAnalytics.Parsed
{
	/// <summary>
	/// Gets the raw data from a <c>cbtevent</c> struct as defined by arcdps.
	/// </summary>
	/// <remarks>
	/// The values of this struct encode values for different types of events
	/// in various ways, see the arcdps EVTC readme for the main documentation.
	/// </remarks>
	public struct ParsedCombatItem
	{
		public long Time { get; internal set; }
		public ulong SrcAgent { get; internal set; }
		public ulong DstAgent { get; internal set; }
		public int Value { get; internal set; }
		public int BuffDmg { get; internal set; }
		public uint OverstackValue { get; internal set; }
		public uint SkillId { get; internal set; }
		public ushort SrcAgentId { get; internal set; }
		public ushort DstAgentId { get; internal set; }
		public ushort SrcMasterId { get; internal set; }
		public ushort DstMasterId { get; internal set; }
		public FriendOrFoe Iff { get; internal set; }
		public byte Buff { get; internal set; }
		public Result Result { get; internal set; }
		public Activation IsActivation { get; internal set; }
		public BuffRemove IsBuffRemove { get; internal set; }
		public byte IsNinety { get; internal set; }
		public byte IsFifty { get; internal set; }
		public byte IsMoving { get; internal set; }
		public StateChange IsStateChange { get; internal set; }
		public byte IsFlanking { get; internal set; }
		public byte IsShields { get; internal set; }
		public byte IsOffCycle { get; internal set; }
		public uint Padding { get; internal set; }

		public ParsedCombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
			uint skillId, ushort srcAgentId, ushort dstAgentId, ushort srcMasterId, ushort dstMasterId, FriendOrFoe iff, byte buff,
			Result result, Activation isActivation, BuffRemove isBuffRemove, byte isNinety, byte isFifty,
			byte isMoving, StateChange isStateChange, byte isFlanking, byte isShields, byte isOffCycle, uint padding)
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
			Padding = padding;
		}
	}
}