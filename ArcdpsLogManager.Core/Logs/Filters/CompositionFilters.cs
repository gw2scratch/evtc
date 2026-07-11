using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2Scratch.ArcdpsLogManager.GameData;
using GW2Scratch.ArcdpsLogManager.Logs.Filters.Composition;

namespace GW2Scratch.ArcdpsLogManager.Logs.Filters
{
	public class CompositionFilters : ILogFilter, INotifyPropertyChanged, IDefaultable
	{
		public IReadOnlyList<CoreProfessionPlayerCountFilter> CoreProfessionFilters { get; }
		public IReadOnlyList<EliteSpecializationPlayerCountFilter> HeartOfThornsSpecializationFilters { get; }
		public IReadOnlyList<EliteSpecializationPlayerCountFilter> PathOfFireSpecializationFilters { get; }
		public IReadOnlyList<EliteSpecializationPlayerCountFilter> EndOfDragonsSpecializationFilters { get; }
		public IReadOnlyList<EliteSpecializationPlayerCountFilter> VisionsOfEternitySpecializationFilters { get; }

		public CompositionFilters() : this(
			ProfessionData.Professions.Select(x => x.Profession)
				.Select(profession => new CoreProfessionPlayerCountFilter(profession)),
			ProfessionData.Professions.Select(x => x.HoT)
				.Select(specialization => new EliteSpecializationPlayerCountFilter(specialization)),
			ProfessionData.Professions.Select(x => x.PoF)
				.Select(specialization => new EliteSpecializationPlayerCountFilter(specialization)),
			ProfessionData.Professions.Select(x => x.EoD)
				.Select(specialization => new EliteSpecializationPlayerCountFilter(specialization)),
			ProfessionData.Professions.Select(x => x.VoE)
				.Select(specialization => new EliteSpecializationPlayerCountFilter(specialization))
		)
		{
		}

		private CompositionFilters(IEnumerable<CoreProfessionPlayerCountFilter> coreFilters,
			IEnumerable<EliteSpecializationPlayerCountFilter> hotFilters,
			IEnumerable<EliteSpecializationPlayerCountFilter> pofFilters,
			IEnumerable<EliteSpecializationPlayerCountFilter> eodFilters,
			IEnumerable<EliteSpecializationPlayerCountFilter> voeFilters
		)
		{
			CoreProfessionFilters = coreFilters.ToList();
			HeartOfThornsSpecializationFilters = hotFilters.ToList();
			PathOfFireSpecializationFilters = pofFilters.ToList();
			EndOfDragonsSpecializationFilters = eodFilters.ToList();
			VisionsOfEternitySpecializationFilters = voeFilters.ToList();

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

			foreach (var filter in EndOfDragonsSpecializationFilters)
			{
				filter.PropertyChanged += (_, _) => OnPropertyChanged(nameof(EndOfDragonsSpecializationFilters));
			}

			foreach (var filter in VisionsOfEternitySpecializationFilters)
			{
				filter.PropertyChanged += (_, _) => OnPropertyChanged(nameof(VisionsOfEternitySpecializationFilters));
			}
		}

		public bool FilterLog(LogData log)
		{
			return CoreProfessionFilters.All(filter => filter.FilterLog(log)) &&
				   HeartOfThornsSpecializationFilters.All(filter => filter.FilterLog(log)) &&
				   PathOfFireSpecializationFilters.All(filter => filter.FilterLog(log)) &&
				   EndOfDragonsSpecializationFilters.All(filter => filter.FilterLog(log)) &&
				   VisionsOfEternitySpecializationFilters.All(filter => filter.FilterLog(log));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public CompositionFilters DeepClone()
		{
			return new CompositionFilters(
				CoreProfessionFilters.Select(x => x.DeepClone()),
				HeartOfThornsSpecializationFilters.Select(x => x.DeepClone()),
				PathOfFireSpecializationFilters.Select(x => x.DeepClone()),
				EndOfDragonsSpecializationFilters.Select(x => x.DeepClone()),
				VisionsOfEternitySpecializationFilters.Select(x => x.DeepClone())
			);
		}

		protected bool Equals(CompositionFilters other)
		{
			return CoreProfessionFilters.SequenceEqual(other.CoreProfessionFilters)
			       && HeartOfThornsSpecializationFilters.SequenceEqual(other.HeartOfThornsSpecializationFilters)
			       && PathOfFireSpecializationFilters.SequenceEqual(other.PathOfFireSpecializationFilters)
			       && EndOfDragonsSpecializationFilters.SequenceEqual(other.EndOfDragonsSpecializationFilters)
				   && VisionsOfEternitySpecializationFilters.SequenceEqual(other.VisionsOfEternitySpecializationFilters);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CompositionFilters) obj);
		}

		public override int GetHashCode()
		{
			var code = new HashCode();
			foreach (var filter in CoreProfessionFilters)
			{
				code.Add(filter.GetHashCode());
			}

			foreach (var filter in HeartOfThornsSpecializationFilters)
			{
				code.Add(filter.GetHashCode());
			}

			foreach (var filter in PathOfFireSpecializationFilters)
			{
				code.Add(filter.GetHashCode());
			}

			foreach (var filter in EndOfDragonsSpecializationFilters)
			{
				code.Add(filter.GetHashCode());
			}

			foreach (var filter in VisionsOfEternitySpecializationFilters)
			{
				code.Add(filter.GetHashCode());
			}

			return code.ToHashCode();
		}

		public bool IsDefault => Equals(new CompositionFilters());

		public void ResetToDefault()
		{
			var filterGroups = new IReadOnlyList<PlayerCountFilter>[]
			{
				CoreProfessionFilters,
				HeartOfThornsSpecializationFilters,
				PathOfFireSpecializationFilters,
				EndOfDragonsSpecializationFilters,
				VisionsOfEternitySpecializationFilters,
			};
			
			foreach (var group in filterGroups)
			{
				foreach (var filter in group)
				{
					filter.ResetToDefault();
				}
			}
		}
	}
}