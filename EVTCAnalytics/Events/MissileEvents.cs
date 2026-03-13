using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Model.Skills;

namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// An event representing a <see cref="Skill"/> cast by an <see cref="Agent"/> that launches one or more missiles.
	/// </summary>
	public abstract class MissileEvent(long time, Agent agent, Skill skill)
		: AgentEvent(time, agent), ISkillEvent
	{
		public Skill Skill { get; } = skill;
	}

	/// <summary>
	/// An event representing the creation of a missile.
	/// </summary>
	public class MissileCreateEvent(
		long time,
		Agent agent,
		float[] position,
		uint skinId,
		Skill skill,
		uint trackableId) 
		: MissileEvent(time, agent, skill), ISkillEvent
	{
		/// <summary>
		/// Position of the missile at creation.
		/// </summary>
		public float[] Position { get; } = position;
		/// <summary>
		/// Skin id of the missile. Used to distinguish player-specific missile skins (e.g. legendary weapon effects).
		/// </summary>
		public uint SkinId { get; } = skinId;
		/// <summary>
		/// Trackable id of this missile. Used for pairing with a corresponding <see cref="MissileLaunchEvent"/> and <see cref="MissileRemoveEvent"/>.
		/// </summary>
		public uint TrackableId { get; } = trackableId;
	}

	/// <summary>
	/// An event representing the launch of a missile.
	/// </summary>
	public class MissileLaunchEvent(
		long time,
		Agent src,
		Agent dst,
		float[] targetPosition,
		float[] launchPosition,
		Skill skill,
		uint launchMotionType,
		short motionRadius,
		uint launchFlags,
		bool isFirstLaunch,
		float missileSpeed,
		uint trackableId) 
		: MissileEvent(time, src, skill), ISkillEvent
	{
		/// <summary>
		/// Target destination.
		/// </summary>
		public Agent Dst = dst;
		/// <summary>
		/// Position of the target.
		/// </summary>
		public float[] TargetPosition { get; } = targetPosition;
		/// <summary>
		/// Position of the missile at launch.
		/// </summary>
		public float[] LaunchPosition { get; } = launchPosition;
		/// <summary>
		/// Launch motion type from client.
		/// </summary>
		public uint LaunchMotionType { get; } = launchMotionType;
		/// <summary>
		/// Motion radius.
		/// </summary>
		public short MotionRadius { get; } = motionRadius;
		/// <summary>
		/// Launch flags from client, unknown usage.
		/// </summary>
		public uint LaunchFlags { get; } = launchFlags;
		/// <summary>
		/// Is the first launch of the missile. <see langword="true"/> if it's the first, <see langword="false"/> otherwise.
		/// </summary>
		public bool IsFirstLaunch { get; } = isFirstLaunch;
		/// <summary>
		/// Missile speed in inch/ms.
		/// </summary>
		public float MissileSpeed { get; } = missileSpeed;
		/// <summary>
		/// Trackable id of this missile. Used for pairing with a corresponding <see cref="MissileCreateEvent"/> and <see cref="MissileRemoveEvent"/>.
		/// </summary>
		public uint TrackableId { get; } = trackableId;
	}

	/// <summary>
	/// An event representing the removal of a missile.
	/// </summary>
	public class MissileRemoveEvent(
		long time,
		Agent agent,
		int friendlyFireTotalDamage,
		Skill skill,
		byte hasHitEnemy,
		uint trackableId) 
		: MissileEvent(time, agent, skill), ISkillEvent
	{
		/// <summary>
		/// Done damage to a friendly Agent (reflected by enemy).
		/// </summary>
		public int FriendlyFireTotalDamage { get; } = friendlyFireTotalDamage;
		/// <summary>
		/// If the missile has hit an enemy Agent. <see langword="true"/> if it has, <see langword="false"/> otherwise.
		/// </summary>
		public bool HasHitEnemy { get; } = hasHitEnemy != 0;
		/// <summary>
		/// Trackable id of this missile. Used for pairing with a corresponding <see cref="MissileCreateEvent"/> and <see cref="MissileLaunchEvent"/>.
		/// </summary>
		public uint TrackableId { get; } = trackableId;
	}
}
