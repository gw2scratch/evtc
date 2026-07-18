using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2Scratch.ArcdpsLogManager.Avalonia.Services;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Compressing;
using GW2Scratch.ArcdpsLogManager.Processing;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels
{
	/// <summary>
	/// View model for the log-compression dialog (Avalonia counterpart of the Eto
	/// <c>CompressDialog</c>): compresses uncompressed logs to .zevtc using the shared
	/// <see cref="LogCompressionProcessor"/>, showing live progress.
	/// </summary>
	public partial class CompressWindowViewModel : ObservableObject, IDisposable
	{
		private readonly Window owner;
		private readonly LogCompressionProcessor processor;
		private readonly List<LogData> compressibleLogs;

		[ObservableProperty] private string compressibleCountText;
		[ObservableProperty] private string compressedCountText;
		[ObservableProperty] private string savedSpaceText = "0.00 MB";
		[ObservableProperty] private bool canCompress;
		[ObservableProperty] private bool canCancel;
		[ObservableProperty] private double progressValue;
		[ObservableProperty] private double progressMax;

		public CompressWindowViewModel(Window owner, IReadOnlyList<LogData> logs, LogCompressionProcessor processor)
		{
			this.owner = owner;
			this.processor = processor;

			compressibleLogs = logs
				.Where(x => !CompressionUtils.HasZipExtension(x.FileName))
				.Where(x => !processor.IsScheduledOrBeingProcessed(x))
				.ToList();

			compressibleCountText = compressibleLogs.Count.ToString();
			compressedCountText = processor.ProcessedItemCount.ToString();
			canCompress = processor.GetScheduledItemCount() == 0 && compressibleLogs.Count > 0;
			canCancel = processor.GetScheduledItemCount() > 0;
			progressMax = compressibleLogs.Count;

			processor.Stopping += OnStopping;
			processor.StoppingWithError += OnStoppingWithError;
			processor.Processed += OnProcessed;
		}

		public void Compress()
		{
			CanCompress = false;
			CanCancel = true;
			foreach (var log in compressibleLogs)
			{
				processor.Schedule(log);
			}
		}

		public void CancelCompression()
		{
			processor.StopBackgroundTask();
		}

		private void OnStopping(object? sender, EventArgs e)
		{
			Dispatcher.UIThread.Post(() =>
			{
				CanCompress = true;
				CanCancel = false;
			});
		}

		private void OnStoppingWithError(object? sender, BackgroundProcessorErrorEventArgs args)
		{
			Dispatcher.UIThread.Post(() =>
			{
				CanCompress = true;
				CanCancel = false;
				_ = Dialogs.ShowInfoAsync(owner, "Compression error", $"Failed to compress a log.\n\n{args.Exception}");
			});
		}

		private void OnProcessed(object? sender, BackgroundProcessorEventArgs<LogData> args)
		{
			Dispatcher.UIThread.Post(() =>
			{
				bool last = args.CurrentScheduledItems == 0;
				if (last)
				{
					CanCompress = true;
				}

				CanCancel = !last;

				ProgressValue = args.TotalProcessedItems;
				ProgressMax = args.TotalScheduledItems;

				double megabytes = processor.SavedBytes / 1000.0 / 1000.0; // Yes, 1000-based MB.
				CompressibleCountText = args.CurrentScheduledItems.ToString();
				CompressedCountText = args.TotalProcessedItems.ToString();
				SavedSpaceText = $"{megabytes:0.00} MB";
			});
		}

		public void Dispose()
		{
			processor.Stopping -= OnStopping;
			processor.StoppingWithError -= OnStoppingWithError;
			processor.Processed -= OnProcessed;
		}
	}
}
