using System.Collections.Generic;
using Avalonia.Media.Imaging;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// Display row for one squad-composition required-count filter (a core profession or an
	/// elite specialization), pairing the pre-resolved icon and label with the underlying Core
	/// <see cref="PlayerCountFilter"/> so the advanced-filters window can bind directly to
	/// <c>Filter.FilterType</c> / <c>Filter.PlayerCount</c> (both already raise
	/// <see cref="System.ComponentModel.INotifyPropertyChanged"/>, so no proxy properties are needed).
	/// </summary>
	public sealed class CompositionFilterRow
	{
		public Bitmap Icon { get; }
		public string Label { get; }
		public PlayerCountFilter Filter { get; }

		/// <summary>The three comparison modes, in display order, for the row's dropdown.</summary>
		public IReadOnlyList<PlayerCountFilterType> FilterTypeOptions { get; } = new[]
		{
			PlayerCountFilterType.GreaterOrEqual,
			PlayerCountFilterType.Equal,
			PlayerCountFilterType.LessOrEqual,
		};

		public CompositionFilterRow(Bitmap icon, string label, PlayerCountFilter filter)
		{
			Icon = icon;
			Label = label;
			Filter = filter;
		}
	}
}
