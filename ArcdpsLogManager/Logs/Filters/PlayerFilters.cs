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

	private const int DefaultMinPlayerCount = 0;
	private const int DefaultMaxPlayerCount = 100;
	private const FilterType DefaultFilterType = FilterType.All;
	private FilterType type = DefaultFilterType;
	private int minPlayerCount = DefaultMinPlayerCount;
	private int maxPlayerCount = DefaultMaxPlayerCount;
	public ObservableCollection<RequiredPlayerFilter> RequiredPlayers { get; }

	public int MinPlayerCount
	{
		get => minPlayerCount;
		set
		{
			if (value == minPlayerCount) return;
			minPlayerCount = value;
			OnPropertyChanged();
		}
	}

	public int MaxPlayerCount
	{
		get => maxPlayerCount;
		set
		{
			if (value == maxPlayerCount) return;
			maxPlayerCount = value;
			OnPropertyChanged();
		}
	}

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
		bool requiredPlayers = Type switch
		{
			FilterType.All => RequiredPlayers.All(req => req.FilterLog(log)),
			FilterType.Any => RequiredPlayers.Any(req => req.FilterLog(log)),
			FilterType.None => RequiredPlayers.All(req => !req.FilterLog(log)),
			_ => throw new ArgumentOutOfRangeException()
		};

		int playerCount = log?.Players?.Count() ?? 0;
		bool playerCountFilter = MinPlayerCount <= playerCount && playerCount <= MaxPlayerCount;

		return requiredPlayers && playerCountFilter;
	}

	public bool IsDefault => Type == DefaultFilterType && RequiredPlayers.Count == 0 &&
	                         MinPlayerCount == DefaultMinPlayerCount && MaxPlayerCount == DefaultMaxPlayerCount;
	
	public void ResetToDefault()
	{
		Type = DefaultFilterType;
		MinPlayerCount = DefaultMinPlayerCount;
		MaxPlayerCount = DefaultMaxPlayerCount;
		RequiredPlayers.Clear();
	}
}