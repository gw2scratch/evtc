namespace GW2Scratch.ArcdpsLogManager.Configuration
{
	/// <summary>
	/// Persisted main-window placement. Used by the Avalonia UI to restore window size/position/
	/// maximized state across launches; ignored by the legacy Eto UI.
	/// </summary>
	public class WindowStateInfo
	{
		public double? Width { get; set; }
		public double? Height { get; set; }
		public int? X { get; set; }
		public int? Y { get; set; }
		public bool Maximized { get; set; }
	}
}
