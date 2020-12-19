namespace GW2Scratch.EVTCAnalytics.Model.Skills
{
	/// <summary>
	/// Represents a game skill.
	/// This may be an ability that may be cast, or a buff effect that may be applied to an <see cref="Agents.Agent" />.
	/// </summary>
	public class Skill
	{
		/// <summary>
		/// The ID number of the skill.
		/// </summary>
		/// <remarks>
		/// The ID space of both abilities and buffs is shared.
		/// These IDs are the same as the IDs exposed by the official Guild Wars 2 API.
		/// </remarks>
		public uint Id { get; }
		
		/// <summary>
		/// Contains the name of the skill.
		/// </summary>
		/// <remarks>
		/// It is very common for skills to not have a name other than the numerical ID.
		/// </remarks>
		public string Name { get; }

		/// <summary>
		/// Creates a new instance of a <see cref="Skill"/>.
		/// </summary>
		/// <param name="id">The ID of the skill.</param>
		/// <param name="name">The name of the skill.</param>
		public Skill(uint id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}