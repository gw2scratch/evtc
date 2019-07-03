using System;
using System.Collections.Generic;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager
{
	public class GridViewSorter<T>
	{
		private readonly GridView gridView;
		private bool sortedAscending = false;
		private GridColumn sortColumn = null;

		public FilterCollection<T> DataStore { get; set; }

		public GridViewSorter(GridView gridView, FilterCollection<T> dataStore)
		{
			this.gridView = gridView;
			DataStore = dataStore;
		}

		public void EnableSorting(Dictionary<GridColumn, Comparison<T>> customSorts = null)
		{
			foreach (var gridColumn in gridView.Columns)
			{
				gridColumn.Sortable = true;
			}

			gridView.ColumnHeaderClick += (sender, args) =>
			{
				if (DataStore == null) return;

				Comparison<T> sort = null;
				customSorts?.TryGetValue(args.Column, out sort);

				if (sort == null && args.Column.DataCell is TextBoxCell textBoxCell)
				{
					sort = GetTextComparison(textBoxCell);
				}

				if (sort == null) return;

				if (sortColumn == args.Column)
				{
					sortedAscending = !sortedAscending;
				}
				else
				{
					sortedAscending = true;
				}

				sortColumn = args.Column;

				if (!sortedAscending)
				{
					var sortToReverse = sort; // This copy is required
					sort = (x, y) => -sortToReverse(x, y);
				}
				DataStore.Sort = sort;
			};
		}

		private Comparison<T> GetTextComparison(TextBoxCell textBoxCell)
		{
			return (first, second) =>
			{
				string property1 = textBoxCell.Binding.GetValue(first);
				string property2 = textBoxCell.Binding.GetValue(second);

				// Try to compare as numbers if possible. Many columns will only contain numbers.
				if (long.TryParse(property1, out long value1) &&
				    long.TryParse(property2, out long value2))
				{
                    return value1.CompareTo(value2);
				}

				return String.Compare(property1, property2, StringComparison.Ordinal);
			};
		}
	}
}