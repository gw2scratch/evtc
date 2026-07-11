using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using GW2Scratch.ArcdpsLogManager.Avalonia.Models;
using GW2Scratch.ArcdpsLogManager.Avalonia.ViewModels;

namespace GW2Scratch.ArcdpsLogManager.Avalonia.Views
{
	/// <summary>
	/// Code-behind for the Game data debug section. Handles the two things the XAML view can't
	/// express as bindings: opening a native save-file dialog for CSV export (Avalonia counterpart
	/// of the Eto <c>SaveFileDialog</c> usage) and opening a standalone log-list window for a
	/// clicked species/skill row (Avalonia counterpart of the Eto <c>GridView.CellClick</c> handler
	/// that opens a <c>Form</c> with a <c>LogList</c>).
	/// </summary>
	public partial class GameDataSectionView : UserControl
	{
		public GameDataSectionView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}

		private void OnOpenSpeciesLogsClick(object? sender, RoutedEventArgs e)
		{
			if (sender is Control { DataContext: SpeciesDataRow row } && DataContext is GameDataSectionViewModel vm)
			{
				OpenLogListWindow($"arcdps Log Manager: logs containing species {row.Name} (ID {row.SpeciesId})",
					row.Logs, vm);
			}
		}

		private void OnOpenSkillLogsClick(object? sender, RoutedEventArgs e)
		{
			if (sender is Control { DataContext: SkillDataRow row } && DataContext is GameDataSectionViewModel vm)
			{
				OpenLogListWindow($"arcdps Log Manager: logs containing skill {row.Name} (ID {row.SkillId})",
					row.Logs, vm);
			}
		}

		private void OpenLogListWindow(string title, IReadOnlyList<GW2Scratch.ArcdpsLogManager.Logs.LogData> logs,
			GameDataSectionViewModel vm)
		{
			var window = new LogListWindow
			{
				DataContext = new LogListWindowViewModel(title, logs, vm.Images, vm.NameProvider, vm.CacheService, vm.ApiData),
			};
			window.Show();
		}

		private async void OnExportSpeciesClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is not GameDataSectionViewModel vm)
			{
				return;
			}

			var file = await PickCsvFileAsync("species.csv");
			if (file == null)
			{
				return;
			}

			await using var stream = await file.OpenWriteAsync();
			using var writer = new StreamWriter(stream);
			writer.WriteLine("ID,Name,Times seen");
			foreach (var row in vm.SpeciesRows)
			{
				writer.WriteLine($"{row.SpeciesId},{row.Name},{row.TimesSeen}");
			}
		}

		private async void OnExportSkillsClick(object? sender, RoutedEventArgs e)
		{
			if (DataContext is not GameDataSectionViewModel vm)
			{
				return;
			}

			var file = await PickCsvFileAsync("skills.csv");
			if (file == null)
			{
				return;
			}

			await using var stream = await file.OpenWriteAsync();
			using var writer = new StreamWriter(stream);
			writer.WriteLine("ID,Name,Times seen,Type");
			foreach (var row in vm.SkillRows)
			{
				writer.WriteLine($"{row.SkillId},{row.Name},{row.TimesSeen},{row.TypeText}");
			}
		}

		private async System.Threading.Tasks.Task<IStorageFile?> PickCsvFileAsync(string suggestedName)
		{
			var topLevel = TopLevel.GetTopLevel(this);
			if (topLevel?.StorageProvider is not { } storage)
			{
				return null;
			}

			return await storage.SaveFilePickerAsync(new FilePickerSaveOptions
			{
				Title = "Export to CSV",
				SuggestedFileName = suggestedName,
				DefaultExtension = "csv",
				FileTypeChoices = new[]
				{
					new FilePickerFileType("CSV file") { Patterns = new[] { "*.csv" } },
				},
			});
		}
	}
}
