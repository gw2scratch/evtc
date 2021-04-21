using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters
{
	public class CompositionFilters : ILogFilter, INotifyPropertyChanged
	{
		public IReadOnlyList<CoreProfessionPlayerCountFilter> CoreProfessionFilters { get; }
		public IReadOnlyList<EliteSpecializationPlayerCountFilter> HeartOfThornsSpecializationFilters { get; }
		public IReadOnlyList<EliteSpecializationPlayerCountFilter> PathOfFireSpecializationFilters { get; }

		public CompositionFilters()
		{
			var coreFilters = GameData.Professions.Select(x => x.Profession)
				.Select(profession => new CoreProfessionPlayerCountFilter(profession))
				.ToList();
			var hotFilters = GameData.Professions.Select(x => x.HoT)
				.Select(specialization => new EliteSpecializationPlayerCountFilter(specialization))
				.ToList();
			var pofFilters = GameData.Professions.Select(x => x.PoF)
				.Select(specialization => new EliteSpecializationPlayerCountFilter(specialization))
				.ToList();

			CoreProfessionFilters = coreFilters;
			HeartOfThornsSpecializationFilters = hotFilters;
			PathOfFireSpecializationFilters = pofFilters;

			foreach (var filter in CoreProfessionFilters)
			{
				filter.PropertyChanged += (_, _) => OnPropertyChanged(nameof(CoreProfessionFilters));
			}

			foreach (var filter in HeartOfThornsSpecializationFilters)
			{
				filter.PropertyChanged += (_, _) => OnPropertyChanged(nameof(HeartOfThornsSpecializationFilters));
			}

			foreach (var filter in PathOfFireSpecializationFilters)
			{
				filter.PropertyChanged += (_, _) => OnPropertyChanged(nameof(PathOfFireSpecializationFilters));
			}
		}

		public bool FilterLog(LogData log)
		{
			return CoreProfessionFilters.All(filter => filter.FilterLog(log)) &&
			       HeartOfThornsSpecializationFilters.All(filter => filter.FilterLog(log)) &&
			       PathOfFireSpecializationFilters.All(filter => filter.FilterLog(log));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}