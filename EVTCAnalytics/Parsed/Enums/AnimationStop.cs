namespace GW2Scratch.EVTCAnalytics.Parsed.Enums
{
	public enum AnimationStop : byte
	{
		None = 0,
		Instant = 1,
		MultiDefunc = 2,
		Transition = 3,
		Partial = 4,
		Ended = 5,
		Cancel = 6,
		StowDraw = 7,
		Interrupt = 8,
		Death = 9,
		Downed = 10,
		CrowdControl = 11,
		Command = 12,
		MotionSkill = 13,
		MoveDodge = 14,
		MotionSkillViaReset = 15,
		MoveSkill = 16,
		MovePos = 17,
		Any = 18,
		GadgetViaReset = 19,
		ManualExpiry = 20,
		Despawn = 21,
		ReturnControl = 22,
		Ready = 23,

		Unknown
	}
}