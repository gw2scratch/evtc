using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Processing;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the "Services" debug section (Avalonia counterpart of the Eto
	/// <c>ManagerForm.ConstructMainTabControl</c>'s inline "Services" <c>TabPage</c>, built from
	/// several <c>Controls/BackgroundProcessorDetail.cs</c> instances). Only shown when
	/// <see cref="Configuration.Settings.ShowDebugData"/> is enabled, exactly like the Eto tab.
	/// </summary>
	public partial class ServicesSectionViewModel : ObservableObject
	{
		public BackgroundProcessorStatusViewModel UploadStatus { get; } = new("Uploads");
		public BackgroundProcessorStatusViewModel ApiStatus { get; } = new("Guild Wars 2 API");
		public BackgroundProcessorStatusViewModel LogProcessingStatus { get; } = new("Log processing");
		public BackgroundProcessorStatusViewModel CompressionStatus { get; } = new("Log compression");

		/// <summary>Called once the background log-processing service exists (needs a writable cache
		/// and configured log directories, same precondition as the Eto tab's processors).</summary>
		public void AttachApiProcessor(ApiProcessor processor) => ApiStatus.Attach(processor);

		public void AttachLogDataProcessor(LogDataProcessor processor) => LogProcessingStatus.Attach(processor);

		public void AttachCompressionProcessor(LogCompressionProcessor processor) =>
			CompressionStatus.Attach(processor);

		public void AttachUploadProcessor(UploadProcessor processor) => UploadStatus.Attach(processor);
	}
}
