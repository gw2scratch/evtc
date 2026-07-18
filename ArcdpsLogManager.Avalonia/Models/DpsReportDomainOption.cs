using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Uploads;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Models
{
	/// <summary>
	/// Wraps a <see cref="DpsReportDomain"/> as a bindable radio-button option for the Settings
	/// window's upload domain list (Avalonia counterpart of the Eto <c>RadioButtonList</c>, which
	/// binds selection via a plain <c>SelectedValue</c> that Avalonia's <c>RadioButton</c> has no
	/// direct equivalent for).
	/// </summary>
	public sealed partial class DpsReportDomainOption : ObservableObject
	{
		public DpsReportDomain Domain { get; }
		public string DisplayText => Domain.Domain;
		public string Description => Domain.Description;

		[ObservableProperty] private bool isSelected;

		public DpsReportDomainOption(DpsReportDomain domain, bool isSelected)
		{
			Domain = domain;
			this.isSelected = isSelected;
		}
	}
}
