using GW2Scratch.ArcdpsLogManager.Logs.Filters.Instability;
using GW2Scratch.EVTCAnalytics.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters;

public class InstabilityFilters : ILogFilter, INotifyPropertyChanged, IDefaultable
{
	public enum FilterType
	{
		All,
		Any,
	}

	private readonly Dictionary<MistlockInstability, bool> instabilities = new Dictionary<MistlockInstability, bool>();
	private const bool DefaultInstabilitySelection = false;
	private const FilterType DefaultFilterType = FilterType.All;
	private FilterType type = DefaultFilterType;

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


	public InstabilityFilters()
	{
		foreach (MistlockInstability instability in Enum.GetValues(typeof(MistlockInstability)))
		{
			instabilities[instability] = DefaultInstabilitySelection;
		}
	}

	public bool this[MistlockInstability index]
	{
		get => instabilities[index];
		set
		{
			if (instabilities[index] == value) return;
			instabilities[index] = value;
			OnPropertyChanged();
		}
	}

	public bool FilterLog(LogData log)
	{
		var currentInstabilities = this.instabilities.Where(x => x.Value).Select(x => x.Key);

		ILogFilter filter = Type switch
		{
			FilterType.All => new AllInstabilitiesFilter(currentInstabilities),
			FilterType.Any => new AnyInstabilityFilter(currentInstabilities),
			_ => throw new ArgumentOutOfRangeException()
		};
		return filter.FilterLog(log);
	}

	public event PropertyChangedEventHandler PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public bool IsDefault => Type == DefaultFilterType && instabilities.All(x => x.Value == DefaultInstabilitySelection);

	public void ResetToDefault()
	{
		Type = DefaultFilterType;
		foreach (var instability in instabilities)
		{
			instabilities[instability.Key] = DefaultInstabilitySelection;
		}
	}
}