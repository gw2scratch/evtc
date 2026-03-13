using System;

namespace GW2Scratch.EVTCAnalytics.Model.Skills;

public class SkillData
{
	public float Recharge { get; internal set; }
	public float Range0 { get; internal set; }
	public float Range1 { get; internal set; }
	public float TooltipTime { get; internal set; }
	public byte ActionByte { get; internal set; }
	public SkillAction Action { get; internal set; }
	public ulong AtMillisecond { get; internal set; }
	public enum SkillAction : byte
	{
		EffectHappened = 4,
		AnimationCompleted = 5,
		Unknown = byte.MaxValue,
	}
	public SkillAction GetSkillAction(byte bt)
	{
		return Enum.IsDefined(typeof(SkillAction), bt) ? (SkillAction) bt : SkillAction.Unknown;
	}
}