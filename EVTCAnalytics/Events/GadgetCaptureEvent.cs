using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Parsed.Enums;

namespace GW2Scratch.EVTCAnalytics.Events
{
	/// <summary>
	/// Gadget capture event wrapper.
	/// </summary>
	public class GadgetCaptureEvent(long time, Agent agent) : AgentEvent(time, agent)
	{

	}

	/// <summary>
	/// Capture outline is shown.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20260610.
	/// </remarks>
	public class GadgetCaptureOutlineShowEvent(long time, Agent agent, byte color) : GadgetCaptureEvent(time, agent)
	{
		/// <summary>
		/// Color of the circle: white, red, blue, green.
		/// </summary>
		public GadgetCapture Color { get; } = (GadgetCapture)color;
	}

	/// <summary>
	/// Capture percentage update.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20260610.
	/// </remarks>
	public class GadgetCaptureSplitPercentEvent(long time, Agent agent, int percentage, byte cappingFrom, byte cappingBy) : GadgetCaptureEvent(time, agent)
	{
		/// <summary>
		/// Capture percentage.
		/// </summary>
		public int Percentage { get; } = percentage;
		/// <summary>
		/// Being captured from [color id number].
		/// </summary>
		public GadgetCapture CappingFrom { get; } = (GadgetCapture)cappingFrom;
		/// <summary>
		/// Capturing by [color id number].
		/// </summary>
		public GadgetCapture CappingBy { get; } = (GadgetCapture)cappingBy;
	}

	/// <summary>
	/// Capture outline is hidden.
	/// </summary>
	/// <remarks>
	/// Introduced in EVTC20260610.
	/// </remarks>
	public class GadgetCaptureOutlineHideEvent(long time, Agent agent) : GadgetCaptureEvent(time, agent)
	{

	}
}
