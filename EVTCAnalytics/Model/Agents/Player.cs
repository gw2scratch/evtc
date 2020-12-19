namespace GW2Scratch.EVTCAnalytics.Model.Agents
{
	/// <summary>
	/// Represents a character of a player.
	/// </summary>
	public class Player : Agent
	{
		/// <summary>
		/// A value indicating whether all values are available. If false, <see cref="Agent.Name"/>,
		/// <see cref="AccountName"/> and <see cref="Subgroup"/> will likely be random values. This
		/// is most common in WvW on enemies as they are anonymized.
		/// </summary>
		public bool Identified { get; }

		/// <summary>
		/// The name of the account of this player.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This name uses the internal representation used by the game which always includes a colon ":"
		/// as a prefix, which is hidden in most UI elements of the game.
		/// </para>
		/// <para>
		/// The name may be a full-length GUID in rare cases and almost always ends with .xxxx, where xxxx is a
		/// discriminator number that is made of unique digits and never starts with a 0.
		/// There is at least one known instance of an account name without a discriminator (or with a value of 0000).
		/// </para>
		/// </remarks>
		public string AccountName { get; }
		
		/// <summary>
		/// The number of the subgroup the player was located in when the encounter started.
		/// </summary>
		/// <remarks>
		/// If the subgroup of the player is changed during the encounter, it is not updated here.
		/// </remarks>
		public int Subgroup { get; }
		
		/// <summary>
		/// A value indicating the profession of the player character.
		/// </summary>
		public Profession Profession { get; }
		
		/// <summary>
		/// A value indicating the current elite specialization of the player character.
		/// </summary>
		public EliteSpecialization EliteSpecialization { get; }
		
		/// <summary>
		/// A relative value in a 0-10 range indicating the amount of the Toughness stat of the player character
		/// at the time it first appeared in the log.
		/// The value ranges from 0, being lowest, to 10, being highest of all players in the encounter and scales linearly.
		/// </summary>
		/// <remarks>
		/// In case the player is affected by dynamic effects increasing stat amounts while it first appears,
		/// this value may be affected as well.
		/// </remarks>
		public int Toughness { get; }
		
		/// <summary>
		/// A relative value in a 0-10 range indicating the amount of the Concentration stat of the player character
		/// at the time it first appeared in the log.
		/// The value ranges from 0, being lowest, to 10, being highest of all players in the encounter and scales linearly.
		/// </summary>
		/// <remarks>
		/// In case the player is affected by dynamic effects increasing stat amounts while it first appears,
		/// this value may be affected as well.
		/// </remarks>
		public int Concentration { get; }
		
		/// <summary>
		/// A relative value in a 0-10 range indicating the amount of the Healing stat of the player character
		/// at the time it first appeared in the log.
		/// The value ranges from 0, being lowest, to 10, being highest of all players in the encounter and scales linearly.
		/// </summary>
		/// <remarks>
		/// In case the player is affected by dynamic effects increasing stat amounts while it first appears,
		/// this value may be affected as well.
		/// </remarks>
		public int Healing { get; }
		
		/// <summary>
		/// A relative value in a 0-10 range indicating the amount of the Condition stat of the player character
		/// at the time it first appeared in the log.
		/// The value ranges from 0, being lowest, to 10, being highest of all players in the encounter and scales linearly.
		/// </summary>
		/// <remarks>
		/// In case the player is affected by dynamic effects increasing stat amounts while it first appears,
		/// this value may be affected as well.
		/// </remarks>
		public int Condition { get; }
		
		/// <summary>
		/// The GUID of the guild represented by the player stored as 16 bytes.
		/// An array of 16 zero bytes indicates that no guild was represented.
		/// </summary>
		public byte[] GuildGuid { get; internal set; }

		/// <summary>
		/// Creates a new instance of a <see cref="Player"/> character.
		/// </summary>
		public Player(AgentOrigin agentOrigin, string name, int toughness, int concentration, int healing, int condition,
			int hitboxWidth, int hitboxHeight, string accountName, Profession profession,
			EliteSpecialization eliteSpecialization, int subgroup, bool identified)
			: base(agentOrigin, name, hitboxWidth, hitboxHeight)
		{
			Toughness = toughness;
			Concentration = concentration;
			Healing = healing;
			Condition = condition;

			Identified = identified;
			Subgroup = subgroup;
			AccountName = accountName;
			Profession = profession;
			EliteSpecialization = eliteSpecialization;
		}
	}
}