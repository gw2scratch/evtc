using CommunityToolkit.Mvvm.ComponentModel;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>A selectable tag for the sidebar's "required tags" filter.</summary>
	public partial class TagFilterItem : ObservableObject
	{
		public string Name { get; }

		[ObservableProperty] private bool isSelected;

		/// <summary>Whether this tag matches the sidebar's tag search box (shown when true).</summary>
		[ObservableProperty] private bool isVisible = true;

		public TagFilterItem(string name)
		{
			Name = name;
		}
	}
}
