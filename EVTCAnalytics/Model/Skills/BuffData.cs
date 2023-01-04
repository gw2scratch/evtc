namespace GW2Scratch.EVTCAnalytics.Model.Skills;

public class BuffData
{
	/// <summary>
	/// Is probably Invulnerability.
	/// </summary>
	public bool IsInvulnerability { get; internal set; }
	
	/// <summary>
	/// Is probably damage inversion.
	/// </summary>
	public bool IsInversion { get; internal set; }
	
	/// <summary>
	/// Is probably Resistance.
	/// </summary>
	/// <remarks>
	/// Added with EVTC20200428.
	/// </remarks>
	public bool? IsResistance { get; internal set; }
	
	public BuffCategory Category { get; internal set; }
	
	public byte StackingType { get; internal set; }
	public uint MaxStacks { get; internal set; }
	public uint DurationCap { get; internal set; }
}