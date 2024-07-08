using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.EVTCAnalytics.Events;

public class SquadGroundMarkerEvent(long time, SquadMarkerType markerType) : Event(time)
{
	public SquadMarkerType MarkerType { get; } = markerType;
}

/// <summary>
/// An event representing a squad marker being placed on the ground.
/// </summary>
public class SquadGroundMarkerPlaceEvent(long time, SquadMarkerType markerType, float[] position) : SquadGroundMarkerEvent(time, markerType)
{
	public float[] Position { get; } = position;
}

/// <summary>
/// An event representing a squad marker being removed from the ground.
/// </summary>
public class SquadGroundMarkerRemoveEvent(long time, SquadMarkerType markerType) : SquadGroundMarkerEvent(time, markerType);