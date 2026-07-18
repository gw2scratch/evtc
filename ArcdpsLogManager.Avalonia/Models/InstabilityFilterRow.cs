using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Logs.Filters;
using GW2Scratch.EVTCAnalytics.Model;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// Display row for one selectable Mistlock Instability in the advanced-filters window,
	/// proxying <see cref="Logs.Filters.InstabilityFilters"/>'s indexer (which cannot be bound
	/// directly from XAML) as a plain <see cref="IsChecked"/> property.
	/// </summary>
	public sealed class InstabilityFilterRow : ObservableObject
	{
		private readonly InstabilityFilters filters;

		public MistlockInstability Instability { get; }
		public Bitmap Icon { get; }
		public string Name { get; }

		public InstabilityFilterRow(InstabilityFilters filters, MistlockInstability instability, Bitmap icon, string name)
		{
			this.filters = filters;
			Instability = instability;
			Icon = icon;
			Name = name;

			// The indexer setter reports the change under the compiler-generated "Item" name, not
			// per-instability, so any change (including a Reset) may affect this row; just re-check.
			filters.PropertyChanged += (_, _) => OnPropertyChanged(nameof(IsChecked));
		}

		public bool IsChecked
		{
			get => filters[Instability];
			set
			{
				if (filters[Instability] == value) return;
				filters[Instability] = value;
				OnPropertyChanged();
			}
		}
	}
}
