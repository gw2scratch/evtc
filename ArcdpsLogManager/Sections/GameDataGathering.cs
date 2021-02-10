using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.EVTCAnalytics;
using GW2Scratch.EVTCAnalytics.Model;
using GW2Scratch.EVTCAnalytics.Model.Agents;
using GW2Scratch.EVTCAnalytics.Processing;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class GameDataCollecting : DynamicLayout
	{
		private class SpeciesData : IEquatable<SpeciesData>
		{
			public int SpeciesId { get; }
			public string Name { get; }
			public int Count { get; set; } = 1;

			public SpeciesData(int speciesId, string name)
			{
				SpeciesId = speciesId;
				Name = name;
			}

			public bool Equals(SpeciesData other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return SpeciesId == other.SpeciesId && string.Equals(Name, other.Name);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((SpeciesData) obj);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(SpeciesId, Name);
			}

			public static bool operator ==(SpeciesData left, SpeciesData right)
			{
				return Equals(left, right);
			}

			public static bool operator !=(SpeciesData left, SpeciesData right)
			{
				return !Equals(left, right);
			}
		}

		private class SkillData : IEquatable<SkillData>
		{
			public uint SkillId { get; }
			public string Name { get; }
			public int Count { get; set; } = 1;

			public SkillData(uint skillId, string name)
			{
				SkillId = skillId;
				Name = name;
			}

			public bool Equals(SkillData other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return SkillId == other.SkillId && string.Equals(Name, other.Name);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((SkillData) obj);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(SkillId, Name);
			}

			public static bool operator ==(SkillData left, SkillData right)
			{
				return Equals(left, right);
			}

			public static bool operator !=(SkillData left, SkillData right)
			{
				return !Equals(left, right);
			}
		}

		private EVTCParser Parser { get; } = new EVTCParser();
		private LogProcessor Processor { get; } = new LogProcessor();
		private CancellationTokenSource cancellationTokenSource;

		public GameDataCollecting(LogList logList)
		{
			var gatherButton = new Button {Text = "Collect data"};
			var cancelButton = new Button {Text = "Cancel"};
			var exportSpeciesButton = new Button {Text = "Export species data to csv"};
			var exportSkillsButton = new Button {Text = "Export skill data to csv"};
			var progressBar = new ProgressBar();
			var progressLabel = new Label {Text = ""};
			var speciesGridView = new GridView<SpeciesData>();
			var skillGridView = new GridView<SkillData>();

			var dataTabs = new TabControl();
			dataTabs.Pages.Add(new TabPage {Text = "Species", Content = speciesGridView});
			dataTabs.Pages.Add(new TabPage {Text = "Skills", Content = skillGridView});

			BeginVertical(new Padding(5), new Size(5, 5));
			{
				AddCentered(
					"Collects a list of all different agent species and skills found in logs (uses current filters).");
				AddCentered("Requires all logs to be processed again as this data is not cached.");
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
					Binding = new DelegateBinding<SpeciesData, string>(x => x.SpeciesId.ToString())
				}
			});
			speciesGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SpeciesData, string>(x => x.Name)
				}
			});
			speciesGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Times seen",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SpeciesData, string>(x => x.Count.ToString())
				}
			});

			skillGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Skill ID",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SkillData, string>(x => x.SkillId.ToString())
				}
			});
			skillGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Name",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SkillData, string>(x => x.Name)
				}
			});
			skillGridView.Columns.Add(new GridColumn
			{
				HeaderText = "Times seen",
				DataCell = new TextBoxCell()
				{
					Binding = new DelegateBinding<SkillData, string>(x => x.Count.ToString())
				}
			});

			var speciesSorter = new GridViewSorter<SpeciesData>(speciesGridView);
			var skillSorter = new GridViewSorter<SkillData>(skillGridView);

			speciesSorter.EnableSorting();
			skillSorter.EnableSorting();

			cancelButton.Click += (sender, args) => cancellationTokenSource?.Cancel();
			gatherButton.Click += (sender, args) =>
				GatherData(logList, progressBar, progressLabel, speciesGridView, skillGridView, speciesSorter,
					skillSorter);
			exportSkillsButton.Click += (sender, args) =>
				SaveToCsv(skillGridView.DataStore ?? Enumerable.Empty<SkillData>());
			exportSpeciesButton.Click += (sender, args) =>
				SaveToCsv(speciesGridView.DataStore ?? Enumerable.Empty<SpeciesData>());
		}

		private void SaveToCsv(IEnumerable<SkillData> skillData)
		{
			var dialog = new SaveFileDialog();
			if (dialog.ShowDialog(this) == DialogResult.Ok)
			{
				using (var writer = new StreamWriter(dialog.FileName))
				{
					writer.WriteLine("ID,Name,Times seen");
					foreach (var data in skillData)
					{
						writer.WriteLine($"{data.SkillId},{data.Name},{data.Count}");
					}
				}
			}
		}

		private void SaveToCsv(IEnumerable<SpeciesData> speciesData)
		{
			var dialog = new SaveFileDialog();
			if (dialog.ShowDialog(this) == DialogResult.Ok)
			{
				using (var writer = new StreamWriter(dialog.FileName))
				{
					writer.WriteLine("ID,Name,Times seen");
					foreach (var data in speciesData)
					{
						writer.WriteLine($"{data.SpeciesId},{data.Name},{data.Count}");
					}
				}
			}
		}

		private void GatherData(LogList logList, ProgressBar progressBar, Label progressLabel,
			GridView<SpeciesData> speciesGridView, GridView<SkillData> skillGridView,
			GridViewSorter<SpeciesData> speciesSorter, GridViewSorter<SkillData> skillSorter)
		{
			cancellationTokenSource?.Cancel();
			cancellationTokenSource = new CancellationTokenSource();

			var logs = logList.DataStore.ToArray();
			ParseLogs(
				logs,
				cancellationTokenSource.Token,
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
						speciesGridView.DataStore = new FilterCollection<SpeciesData>(species);
						skillGridView.DataStore = new FilterCollection<SkillData>(skills);

						speciesSorter.UpdateDataStore();
						skillSorter.UpdateDataStore();
					});
				}
			});
		}

		private Task<(IEnumerable<SpeciesData>, IEnumerable<SkillData>)> ParseLogs(IReadOnlyCollection<LogData> logs,
			CancellationToken cancellationToken, IProgress<(int done, int totalLogs, int failed)> progress = null)
		{
			return Task.Run(() =>
			{
				var species = new Dictionary<int, Dictionary<SpeciesData, int>>();
				var skills = new Dictionary<uint, Dictionary<SkillData, int>>();

				int done = 0;
				int failed = 0;
				foreach (var log in logs)
				{
					cancellationToken.ThrowIfCancellationRequested();

					Log processedLog;
					try
					{
						var parsedLog = Parser.ParseLog(log.FileInfo.FullName);
						processedLog = Processor.ProcessLog(parsedLog);
					}
					catch
					{
						failed++;
						continue;
					}

					foreach (var agent in processedLog.Agents.OfType<NPC>())
					{
						int id = agent.SpeciesId;
						string name = agent.Name;

						// Ignore missing data. If the species id, in most cases all other data is meaningless.
						if (id == 0) continue;

						var speciesData = new SpeciesData(id, name);
						if (!species.ContainsKey(id))
						{
							species[id] = new Dictionary<SpeciesData, int>();
						}

						if (!species[id].ContainsKey(speciesData))
						{
							species[id][speciesData] = 0;
						}

						species[id][speciesData]++;
					}

					foreach (var skill in processedLog.Skills)
					{
						uint id = skill.Id;
						string name = skill.Name;

						// Ignore missing data. If the species id, in most cases all other data is meaningless.
						if (id == 0) continue;

						var skillData = new SkillData(id, name);
						if (!skills.ContainsKey(id))
						{
							skills[id] = new Dictionary<SkillData, int>();
						}

						if (!skills[id].ContainsKey(skillData))
						{
							skills[id][skillData] = 0;
						}

						skills[id][skillData]++;
					}

					done++;
					progress?.Report((done, logs.Count, failed));
				}

				var speciesEnumerable = (IEnumerable<SpeciesData>) species.Values.SelectMany(x => x).Select(x =>
					{
						var key = x.Key;
						key.Count = x.Value;
						return key;
					}).OrderBy(x => x.SpeciesId)
					.ThenBy(x => x.Name);
				var skillEnumerable = (IEnumerable<SkillData>) skills.Values.SelectMany(x => x).Select(x =>
					{
						var key = x.Key;
						key.Count = x.Value;
						return key;
					}).OrderBy(x => x.SkillId)
					.ThenBy(x => x.Name);

				return (speciesEnumerable, skillEnumerable);
			}, cancellationToken);
		}
	}
}