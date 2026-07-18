using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Sections.GameData;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class GameDataCollecting : DynamicLayout
	{
		// The gathering/parsing logic (SpeciesGatherResult/SkillGatherResult/GameDataGatherer)
		// lives in ArcdpsLogManager.Core (GW2Scratch.ArcdpsLogManager.Sections.GameData) so it has
		// a single source of truth shared with the Avalonia port instead of a diverging copy here.
		private readonly GameDataGatherer gatherer = new GameDataGatherer();
		private CancellationTokenSource cancellationTokenSource;

		public GameDataCollecting(LogList logList, LogCache logCache, ApiData apiData, LogDataProcessor logProcessor,
			UploadProcessor uploadProcessor, ImageProvider imageProvider, ILogNameProvider nameProvider)
		{
			var threadCountStepper = new NumericStepper
			{
				// 4 seems to be a good default value, there doesn't seem to be any scaling beyond this,
				// at least on a Ryzen 5900X with an SSD.
				Value = Math.Min(Environment.ProcessorCount, 4),
				MinValue = 1, Increment = 1.0
			};
			var gatherButton = new Button {Text = "Collect data"};
			var cancelButton = new Button {Text = "Cancel"};
			var exportSpeciesButton = new Button {Text = "Export species data to csv"};
			var exportSkillsButton = new Button {Text = "Export skill data to csv"};
			var progressBar = new ProgressBar();
			var progressLabel = new Label {Text = ""};
			var speciesGridView = new GridView<SpeciesGatherResult>();
			var skillGridView = new GridView<SkillGatherResult>();

			var dataTabs = new TabControl();
			dataTabs.Pages.Add(new TabPage {Text = "Species", Content = speciesGridView});
			dataTabs.Pages.Add(new TabPage {Text = "Skills", Content = skillGridView});

			BeginVertical(new Padding(5), new Size(5, 5));
			{
				AddCentered(
					"Collects a list of all different agent species and skills found in logs (uses current filters).");
				AddCentered("Requires all logs to be processed again as this data is not cached.");
				AddSpace(yscale: false);
				BeginCentered(spacing: new Size(5, 5));
				{
					var label = new Label { Text = "Threads to use for parallel processing", VerticalAlignment = VerticalAlignment.Center };
					AddRow(label, threadCountStepper);
				}
				EndCentered();
				BeginCentered(spacing: new Size(5, 5));
				{
					AddRow(gatherButton, cancelButton);
				}
				EndCentered();
				BeginCentered(spacing: new Size(5, 5));
				{
					AddRow(progressBar);
					AddRow(progressLabel);
				}
				EndCentered();

				BeginHorizontal(true);
				Add(dataTabs);
				EndHorizontal();
				BeginCentered(spacing: new Size(5, 5));
				{
					AddRow(exportSpeciesButton, exportSkillsButton);
				}
				EndCentered();
			}
			EndVertical();

			speciesGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Species ID",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SpeciesGatherResult, string>(x => x.SpeciesId.ToString())
				}
			});
			speciesGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SpeciesGatherResult, string>(x => x.Name)
				}
			});
			speciesGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Times seen",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SpeciesGatherResult, string>(x => x.Logs.Count.ToString())
				}
			});

			var speciesLogsColumn = new GridColumn
			{
				HeaderText = "Logs",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SpeciesGatherResult, string>(x => $"{x.Logs.Count}, click me to open log list"),
				}
			};
			speciesGridView.Columns.Add(speciesLogsColumn);

			speciesGridView.CellClick += (sender, args) =>
			{
				if (args.GridColumn == speciesLogsColumn)
				{
					if (args.Item is SpeciesGatherResult speciesData)
					{
						var form = new Form
						{
							Content = new LogList(logCache, apiData, logProcessor, uploadProcessor, imageProvider,
								nameProvider)
							{
								DataStore = new FilterCollection<LogData>(speciesData.Logs)
							},
							Width = 900,
							Height = 700,
							Title = $"arcdps Log Manager: logs containing species {speciesData.Name} (ID {speciesData.SpeciesId})"
						};
						form.Show();
					}
				}
			};
			
			skillGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Skill ID",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SkillGatherResult, string>(x => x.SkillId.ToString())
				}
			});
			skillGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SkillGatherResult, string>(x => x.Name)
				}
			});
			skillGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Type",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SkillGatherResult, string>(x => x.Type.ToString())
				}
			});
			skillGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Times seen",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SkillGatherResult, string>(x => x.Logs.Count.ToString())
				}
			});
			var skillLogsColumn = new GridColumn
			{
				HeaderText = "Logs",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SkillGatherResult, string>(x => $"{x.Logs.Count}, click me to open log list"),
				}
			};
			skillGridView.Columns.Add(skillLogsColumn);

			skillGridView.CellClick += (sender, args) =>
			{
				if (args.GridColumn == skillLogsColumn)
				{
					if (args.Item is SkillGatherResult skillData)
					{
						var form = new Form
						{
							Content = new LogList(logCache, apiData, logProcessor, uploadProcessor, imageProvider,
								nameProvider)
							{
								DataStore = new FilterCollection<LogData>(skillData.Logs)
							},
							Width = 900,
							Height = 700,
							Title = $"arcdps Log Manager: logs containing skill {skillData.Name} (ID {skillData.SkillId})"
						};
						form.Show();
					}
				}
			};

			var speciesSorter = new GridViewSorter<SpeciesGatherResult>(speciesGridView);
			var skillSorter = new GridViewSorter<SkillGatherResult>(skillGridView);

			speciesSorter.EnableSorting();
			skillSorter.EnableSorting();

			cancelButton.Click += (sender, args) => cancellationTokenSource?.Cancel();
			gatherButton.Click += (sender, args) =>
				GatherData(logList, progressBar, progressLabel, speciesGridView, skillGridView, speciesSorter, skillSorter, (int) threadCountStepper.Value);
			exportSkillsButton.Click += (sender, args) =>
				SaveToCsv(skillGridView.DataStore ?? Enumerable.Empty<SkillGatherResult>());
			exportSpeciesButton.Click += (sender, args) =>
				SaveToCsv(speciesGridView.DataStore ?? Enumerable.Empty<SpeciesGatherResult>());
		}

		private void SaveToCsv(IEnumerable<SkillGatherResult> skillData)
		{
			var dialog = new SaveFileDialog();
			if (dialog.ShowDialog(this) == DialogResult.Ok)
			{
				using (var writer = new StreamWriter(dialog.FileName))
				{
					writer.WriteLine("ID,Name,Times seen,Type");
					foreach (var data in skillData)
					{
						writer.WriteLine($"{data.SkillId},{data.Name},{data.Logs.Count},{data.Type}");
					}
				}
			}
		}

		private void SaveToCsv(IEnumerable<SpeciesGatherResult> speciesData)
		{
			var dialog = new SaveFileDialog();
			if (dialog.ShowDialog(this) == DialogResult.Ok)
			{
				using (var writer = new StreamWriter(dialog.FileName))
				{
					writer.WriteLine("ID,Name,Times seen");
					foreach (var data in speciesData)
					{
						writer.WriteLine($"{data.SpeciesId},{data.Name},{data.Logs.Count}");
					}
				}
			}
		}

		private void GatherData(LogList logList, ProgressBar progressBar, Label progressLabel,
			GridView<SpeciesGatherResult> speciesGridView, GridView<SkillGatherResult> skillGridView,
			GridViewSorter<SpeciesGatherResult> speciesSorter, GridViewSorter<SkillGatherResult> skillSorter, int maxDegreeOfParallelism)
		{
			cancellationTokenSource?.Cancel();
			cancellationTokenSource = new CancellationTokenSource();

			var logs = logList.DataStore.ToArray();
			gatherer.GatherAsync(
				logs,
				cancellationTokenSource.Token,
				maxDegreeOfParallelism,
				new Progress<(int done, int totalLogs, int failed)>(progress =>
				{
					(int done, int totalLogs, int failed) = progress;
					Application.Instance.Invoke(() =>
					{
						progressBar.MaxValue = totalLogs;
						progressBar.Value = done;
						progressLabel.Text = $"{done}/{totalLogs} ({failed} failed)";
					});
				})
			).ContinueWith(task =>
			{
				if (task.Status == TaskStatus.RanToCompletion)
				{
					Application.Instance.Invoke(() =>
					{
						var (species, skills) = task.Result;
						speciesGridView.DataStore = new FilterCollection<SpeciesGatherResult>(species);
						skillGridView.DataStore = new FilterCollection<SkillGatherResult>(skills);

						speciesSorter.UpdateDataStore();
						skillSorter.UpdateDataStore();
					});
				}
			});
		}
	}
}