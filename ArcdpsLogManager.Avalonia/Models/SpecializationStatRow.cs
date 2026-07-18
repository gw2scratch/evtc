using Avalonia.Media.Imaging;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>A profession row for the Statistics → Specializations tab (icon, name, count, average).</summary>
	public sealed class ProfessionStatRow
	{
		public Bitmap? Icon { get; }
		public string Name { get; }
		public int Count { get; }
		public string Average { get; }

		public ProfessionStatRow(Bitmap? icon, string name, int count, int logCount)
		{
			Icon = icon;
			Name = name;
			Count = count;
			Average = logCount > 0 ? $"{count / (float) logCount:0.00}" : "0";
		}
	}

	/// <summary>An elite-specialization row for the Statistics → Specializations tab (icon, name, count).</summary>
	public sealed class SpecStatRow
	{
		public Bitmap? Icon { get; }
		public string Name { get; }
		public int Count { get; }

		public SpecStatRow(Bitmap? icon, string name, int count)
		{
			Icon = icon;
			Name = name;
			Count = count;
		}
	}
}
