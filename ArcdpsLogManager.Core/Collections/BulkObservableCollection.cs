using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GW2Scratch.ArcdpsLogManager.Collections
{
	/// <summary>
	/// A version of ObservableCollection with support for efficiently adding multiple items through <see cref="AddRange"/>
	/// (triggering only one collection update event).
	/// </summary>
	public class BulkObservableCollection<T> : ObservableCollection<T>
	{
		public BulkObservableCollection()
		{
		}

		public BulkObservableCollection(IEnumerable<T> collection) : base(collection)
		{
		}

		public BulkObservableCollection(List<T> list) : base(list)
		{
		}

		public void AddRange(IEnumerable<T> range) {
			foreach (var item in range) {
				Items.Add(item);
			}

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void Reset(IEnumerable<T> range) {
			Items.Clear();

			AddRange(range);
		}
	}
}