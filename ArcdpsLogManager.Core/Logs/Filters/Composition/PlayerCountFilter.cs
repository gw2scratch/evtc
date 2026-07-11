using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition
{
	public abstract class PlayerCountFilter : ILogFilter, INotifyPropertyChanged, IDefaultable
	{
		// Important: The filter has to always succeed with default settings.
		// For more details, see the FilterLog implementation.
		private const int DefaultPlayerCount = 0;
		private const PlayerCountFilterType DefaultType = PlayerCountFilterType.GreaterOrEqual;
		
		private int playerCount = DefaultPlayerCount;
		private PlayerCountFilterType filterType = DefaultType;

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

		public bool IsDefault => FilterType == DefaultType && PlayerCount == DefaultPlayerCount;
		
		public void ResetToDefault()
		{
			PlayerCount = DefaultPlayerCount;
			FilterType = DefaultType;
		}

		protected abstract int GetPlayerCount(LogData log);

		public bool FilterLog(LogData log)
		{
			if (IsDefault)
			{
				// This saves a significant amount of time when most filters are left as default
				// as we don't need to check the player counts at all.
				return true;
			}
			
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