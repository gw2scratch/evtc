using GW2Scratch.ArcdpsLogManager.Logs.Filters.Players;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters;

public class PlayerFilters : ILogFilter, INotifyPropertyChanged, IDefaultable
{
	public enum FilterType
	{
		All,
		Any,
		None,
	}
	
	private const FilterType DefaultFilterType = FilterType.All;
	private FilterType type = DefaultFilterType;
	public ObservableCollection<RequiredPlayerFilter> RequiredPlayers { get; }

	public PlayerFilters()
	{
		RequiredPlayers = new ObservableCollection<RequiredPlayerFilter>();
		RequiredPlayers.CollectionChanged += (_, _) => OnPropertyChanged(nameof(RequiredPlayers));
	}
	
	public FilterType Type
	{
		get => type;
		set
		{
			if (Type == value) return;
			type = value;
			OnPropertyChanged();
		}
	}
	
	public event PropertyChangedEventHandler PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public bool FilterLog(LogData log)
	{
		return Type switch
		{
			FilterType.All => RequiredPlayers.All(req => req.FilterLog(log)),
			FilterType.Any => RequiredPlayers.Any(req => req.FilterLog(log)),
			FilterType.None => RequiredPlayers.All(req => !req.FilterLog(log)),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	public bool IsDefault => Type == DefaultFilterType && RequiredPlayers.Count == 0;
	
	public void ResetToDefault()
	{
		Type = DefaultFilterType;
		RequiredPlayers.Clear();
	}
}