using GW2Scratch.EVTCAnalytics.Model.Agents;

namespace GW2Scratch.EVTCAnalytics.Model.Effects;

/// <summary>
/// Represents a game visual effect.
/// </summary>
public class Effect(uint id) : ContentLocal(id, null)
{	
	/// <summary>
	/// The last duration from the duration list.
	/// </summary>
	/// <remarks>
	/// Available since 20241030
	/// </remarks>
	public float DefaultDuration { get; internal set; }
}