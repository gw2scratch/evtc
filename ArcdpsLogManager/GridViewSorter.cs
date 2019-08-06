using System;
using System.Collections.Generic;
using Eto.Forms;

namespace GW2Scratch.ArcdpsLogManager
{
	public class GridViewSorter<T>
	{
		private readonly GridView gridView;
		private readonly IReadOnlyDictionary<GridColumn, Comparison<T>> customSorts;
		private bool sortedAscending = false;
		private GridColumn sortColumn = null;

		/// <summary>
		/// Create a new <see cref="GridView"/> sorter.
		/// </summary>
		/// <remarks>If the <see cref="GridView.DataStore"/> is changed, <see cref="UpdateDataStore"/> has to be called
		/// to apply the sort.</remarks>
		/// <param name="gridView">A <see cref="GridView"/> that will be sorted.</param>
		/// <param name="customSorts">Optional custom sorting logic for specified columns.</param>
		public GridViewSorter(GridView gridView, IReadOnlyDictionary<GridColumn, Comparison<T>> customSorts = null)
		{
			this.gridView = gridView;
			this.customSorts = customSorts;
		}

		/// <summary>
		/// Has to be called if the <see cref="GridView.DataStore"/> changes to apply the sort again.
		/// </summary>
		public void UpdateDataStore()
		{
			ApplySort();
		}

		/// <summary>
		/// Enable sorting on the <see cref="GridView"/>. Registers handlers for clicks on the column headers.
		/// </summary>
		public void EnableSorting()
		{
			foreach (var gridColumn in gridView.Columns)
			{
				gridColumn.Sortable = true;
			}

			gridView.ColumnHeaderClick += (sender, args) => { ClickColumn(args.Column); };
		}

		/// <summary>
		/// Sort the <see cref="GridView"/> by the provided column in ascending order.
		/// </summary>
		/// <param name="column">A column in the <see cref="GridView"/>.</param>
		public void SortByAscending(GridColumn column) => SortBy(column, true);

		/// <summary>
		/// Sort the <see cref="GridView"/> by the provided column in descending order.
		/// </summary>
		/// <param name="column">A column in the <see cref="GridView"/>.</param>
		public void SortByDescending(GridColumn column) => SortBy(column, false);

		private void SortBy(GridColumn column, bool ascending)
		{
			sortColumn = column;
			sortedAscending = ascending;
			ApplySort();
		}

		private void ClickColumn(GridColumn column)
		{
			if (sortColumn == column)
			{
				sortedAscending = !sortedAscending;
			}
			else
			{
				sortedAscending = true;
				sortColumn = column;
			}

			ApplySort();
		}

		private void ApplySort()
		{
			if (sortColumn == null)
			{
				return;
			}

			if (gridView.DataStore is FilterCollection<T> collection)
			{
				Comparison<T> sort = null;
				customSorts?.TryGetValue(sortColumn, out sort);

				if (sort == null && sortColumn.DataCell is TextBoxCell textBoxCell)
				{
					sort = GetTextComparison(textBoxCell);
				}

				if (sort == null)
				{
					// If we don't know how to sort out the current column, we keep the old sort.
					return;
				}

				if (!sortedAscending)
				{
					var sortToReverse = sort; // This copy is required
					sort = (x, y) => -sortToReverse(x, y);
				}

				collection.Sort = sort;
			}
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