using CommunityToolkit.Mvvm.ComponentModel;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>A selectable tag for the sidebar's "required tags" filter.</summary>
	public partial class TagFilterItem : ObservableObject
	{
		public string Name { get; }

		[ObservableProperty] private bool isSelected;

		public TagFilterItem(string name)
		{
			Name = name;
		}
	}
}
