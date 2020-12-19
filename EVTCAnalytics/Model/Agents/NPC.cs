namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	/// <summary>
	/// An NPC is a non-player <see cref="Agent"/>. An NPC may be hostile or friendly depending on its team,
	/// they are capable of movement, casting skills and even of entering downed state in rare cases.
	/// </summary>
	public class NPC : Agent
	{
		/// <summary>
		/// The species ID of this NPC. The species is a template that specifies a default
		/// name, model, size and likely also skills. Some of these defaults may be overriden.
		/// </summary>
		public int SpeciesId { get; }
		
		/// <summary>
		/// The amount of the Toughness stat this NPC had at the time it first appeared in the log.
		/// </summary>
		/// <remarks>
		/// In case the NPC is affected by dynamic effects increasing stat amounts while it first appears,
		/// this value may be affected as well.
		/// </remarks>
		public int Toughness { get; }
		
		/// <summary>
		/// The amount of the Concentration stat this NPC had at the time it first appeared in the log.
		/// </summary>
		/// <remarks>
		/// In case the NPC is affected by dynamic effects increasing stat amounts while it first appears,
		/// this value may be affected as well.
		/// </remarks>
		public int Concentration { get; }
		
		/// <summary>
		/// The amount of the Healing stat this NPC had at the time it first appeared in the log.
		/// </summary>
		/// <remarks>
		/// In case the NPC is affected by dynamic effects increasing stat amounts while it first appears,
		/// this value may be affected as well.
		/// </remarks>
		public int Healing { get; }
		
		/// <summary>
		/// The amount of the Condition stat this NPC had at the time it first appeared in the log.
		/// </summary>
		/// <remarks>
		/// In case the NPC is affected by dynamic effects increasing stat amounts while it first appears,
		/// this value may be affected as well.
		/// </remarks>
		public int Condition { get; }

		/// <summary>
		/// Creates a new instance of an <see cref="NPC"/>.
		/// </summary>
		public NPC(AgentOrigin agentOrigin, string name, int speciesId, int toughness, int concentration, int healing,
			int condition, int hitboxWidth, int hitboxHeight) : base(agentOrigin, name, hitboxWidth, hitboxHeight)
		{
			SpeciesId = speciesId;
			Toughness = toughness;
			Concentration = concentration;
			Healing = healing;
			Condition = condition;
		}
	}
}