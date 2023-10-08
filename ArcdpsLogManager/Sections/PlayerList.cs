using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GW2Scratch.ArcdpsLogManager.Controls;
using GW2Scratch.ArcdpsLogManager.Gw2Api;
using GW2Scratch.ArcdpsLogManager.Logs;
using GW2Scratch.ArcdpsLogManager.Logs.Naming;
using GW2Scratch.ArcdpsLogManager.Processing;
using GW2Scratch.ArcdpsLogManager.Sections.Players;

namespace GW2Scratch.ArcdpsLogManager.Sections
{
	public class PlayerList : DynamicLayout
	{
		private LogCache LogCache { get; }
		private ApiData ApiData { get; }
		private LogDataProcessor LogProcessor { get; }
		private UploadProcessor UploadProcessor { get; }
		private ImageProvider ImageProvider { get; }
		private ILogNameProvider LogNameProvider { get; }

		private ObservableCollection<PlayerData> playerData;
		private SelectableFilterCollection<PlayerData> filtered;
		private Task debounceFilterTask;
		private CancellationTokenSource debounceFilterCancellationTokenSource;

		private readonly GridView<PlayerData> playerGridView;
		private readonly GridViewSorter<PlayerData> sorter;
		private readonly Label accountCountLabel = new Label {VerticalAlignment = VerticalAlignment.Center};
		private readonly Label characterCountLabel = new Label {VerticalAlignment = VerticalAlignment.Center};

		public ObservableCollection<PlayerData> DataStore
		{
			get => playerData;
			set {
				if (value == null)
				{
					value = new ObservableCollection<PlayerData>();
				}

				playerData = value;
				filtered = new SelectableFilterCollection<PlayerData>(playerGridView, playerData)
					{Filter = FilterPlayerData};
				if (playerGridView != null)
				{
					playerGridView.DataStore = filtered;
					sorter.UpdateDataStore();
				}

				StartBackgroundCountLabelUpdate();
			}
		}
		
		private bool ShowLogListButtons { get; }
		private string PlayerFilter { get; set; } = "";
		
		public PlayerData SelectedPlayer => playerGridView.SelectedItem;

		public event EventHandler<GridCellMouseEventArgs> PlayerDoubleClicked;

		public PlayerList(LogCache logCache, ApiData apiData, LogDataProcessor logProcessor,
			UploadProcessor uploadProcessor, ImageProvider imageProvider, ILogNameProvider logNameProvider,
			bool showLogListButtons = true)
		{
			LogCache = logCache;
			ApiData = apiData;
			LogProcessor = logProcessor;
			ImageProvider = imageProvider;
			LogNameProvider = logNameProvider;
			UploadProcessor = uploadProcessor;
			ShowLogListButtons = showLogListButtons;

			var playerDetailPanel = ConstructPlayerDetailPanel();
			playerGridView = ConstructPlayerGridView(playerDetailPanel);
			sorter = new GridViewSorter<PlayerData>(playerGridView);
			sorter.EnableSorting();

			DataStore = new ObservableCollection<PlayerData>();

			var playerFilterBox = new TextBox();
			playerFilterBox.TextBinding.Bind(this, x => x.PlayerFilter);
			playerFilterBox.TextChanged += (sender, args) => {

				if (this.debounceFilterCancellationTokenSource != null)
				{
					this.debounceFilterCancellationTokenSource.Cancel();
					try
					{
						this.debounceFilterTask.Wait();
					}
					catch (AggregateException ex)
					{
						if (ex.InnerException is TaskCanceledException)
						{
							// NOP
						}
						else
						{
							throw;
						}
					}
					this.debounceFilterTask.Dispose();
					this.debounceFilterCancellationTokenSource.Dispose();
					this.debounceFilterTask = null;
					this.debounceFilterCancellationTokenSource = null;
				}

				this.debounceFilterCancellationTokenSource = new CancellationTokenSource();
				this.debounceFilterTask = Task.Run(async () =>
				{
					await Task.Delay(200, this.debounceFilterCancellationTokenSource.Token);
					this.debounceFilterCancellationTokenSource.Token.ThrowIfCancellationRequested();

					await Application.Instance.InvokeAsync(() =>
					{
						playerGridView.UnselectAll();
						Refresh();
					});
					_ = Task.Run(() =>
					{
						this.debounceFilterCancellationTokenSource.Cancel();
						this.debounceFilterTask.Wait();
						this.debounceFilterTask.Dispose();
						this.debounceFilterCancellationTokenSource.Dispose();
						this.debounceFilterTask = null;
						this.debounceFilterCancellationTokenSource = null;
					});
				});
			};

			BeginVertical(spacing: new Size(5, 5), padding: new Padding(5));
			{
				BeginHorizontal();
				{
					Add(new Label
						{Text = "Filter by character or account name", VerticalAlignment = VerticalAlignment.Center});
					Add(playerFilterBox);
					Add(null, true);
					Add(accountCountLabel, true, true);
					Add(characterCountLabel, true, true);
					EndVertical();
				}
				EndHorizontal();
			}
			EndVertical();
			BeginVertical(yscale: true);
			{
				BeginHorizontal();
				{
					Add(playerGridView, true);
					Add(playerDetailPanel);
				}
				EndHorizontal();
			}
			EndVertical();
		}

		public void UpdateDataFromLogs(IEnumerable<LogData> logs)
		{
			// There is potential for a race condition here.
			// TODO: Ensure that the task gets cancelled if its still running from a previous UpdateDataFromLogs() call
			Task.Run(() => {
				var logsByAccountName = new Dictionary<string, List<LogData>>();
				foreach (var log in logs)
				{
					if (log.ParsingStatus != ParsingStatus.Parsed) continue;

					foreach (var player in log.Players)
					{
						if (!logsByAccountName.ContainsKey(player.AccountName))
						{
							logsByAccountName[player.AccountName] = new List<LogData>();
						}

						logsByAccountName[player.AccountName].Add(log);
					}
				}

				var collection = new ObservableCollection<PlayerData>(logsByAccountName
					.Select(x => new PlayerData(x.Key, x.Value)).OrderByDescending(x => x.Logs.Count));

				Application.Instance.Invoke(() => { DataStore = collection; });
			});
		}

		private void Refresh()
		{
			filtered.Refresh();
			StartBackgroundCountLabelUpdate();
		}

		private void StartBackgroundCountLabelUpdate()
		{
			// TODO: There is potential for race conditions here if multiple updates are started
			// and earlier ones are significantly slower than later ones.
			Task.Run(() => {
				int accountCount = filtered.Count;
				int characterCount = filtered
					.SelectMany(x =>
						x.Logs.SelectMany(log => log.Players).Where(player => player.AccountName == x.AccountName))
					.Select(x => x.Name)
					.Distinct()
					.Count();
				Application.Instance.AsyncInvoke(() => UpdateCountLabels(accountCount, characterCount));
			});
		}

		private void UpdateCountLabels(int accountCount, int characterCount)
		{
			accountCountLabel.Text = $"{accountCount} accounts";
			characterCountLabel.Text = $"{characterCount} characters";
		}

		private bool FilterPlayerData(PlayerData playerData)
		{
			if (string.IsNullOrWhiteSpace(PlayerFilter))
			{
				return true;
			}

			return playerData.AccountName.IndexOf(PlayerFilter, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
			       playerData.Logs.Any(log =>
				       log.Players.Any(player =>
					       player.AccountName == playerData.AccountName &&
					       player.Name.IndexOf(PlayerFilter, StringComparison.CurrentCultureIgnoreCase) >= 0
				       )
			       );
		}

		private PlayerDetailPanel ConstructPlayerDetailPanel()
		{
			return new PlayerDetailPanel(LogCache, ApiData, LogProcessor, UploadProcessor, ImageProvider,
				LogNameProvider, ShowLogListButtons);
		}

		private GridView<PlayerData> ConstructPlayerGridView(PlayerDetailPanel playerDetailPanel)
		{
			var gridView = new GridView<PlayerData>();
			gridView.Columns.Add(new GridColumn {
				HeaderText = "Account name",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<PlayerData, string>(x => x.AccountName.Substring(1))}
			});
			gridView.Columns.Add(new GridColumn {
				HeaderText = "Log count",
				DataCell = new TextBoxCell
					{Binding = new DelegateBinding<PlayerData, string>(x => x.Logs.Count.ToString())}
			});

			gridView.SelectionChanged += (sender, args) => {
				if (gridView.SelectedItem != null)
				{
					playerDetailPanel.PlayerData = gridView.SelectedItem;
				}
			};
			
			gridView.CellDoubleClick += (sender, args) =>
			{
				PlayerDoubleClicked?.Invoke(sender, args);
			};

			return gridView;
		}
	}
}