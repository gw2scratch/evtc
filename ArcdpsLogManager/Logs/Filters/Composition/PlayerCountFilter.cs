using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition
{
	public abstract class PlayerCountFilter : ILogFilter, INotifyPropertyChanged
	{
		private int playerCount = 0;
		private PlayerCountFilterType filterType = PlayerCountFilterType.GreaterOrEqual;

		public PlayerCountFilterType FilterType
		{
			get => filterType;
			set {
				if (filterType == value) return;
				filterType = value;
				OnPropertyChanged();
			}

		}

		public int PlayerCount
		{
			get => playerCount;
			set {
				if (playerCount == value) return;
				playerCount = value;
				OnPropertyChanged();
			}
		}

		protected abstract int GetPlayerCount(LogData log);

		public bool FilterLog(LogData log)
		{
			int count = GetPlayerCount(log);
			return FilterType switch {
				PlayerCountFilterType.GreaterOrEqual => count >= PlayerCount,
				PlayerCountFilterType.Equal => count == PlayerCount,
				PlayerCountFilterType.LessOrEqual => count <= PlayerCount,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}