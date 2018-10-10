using System;
using Eto.Forms;

namespace ScratchLogBrowser
{
	public class GridViewSorter<T>
	{
		private readonly GridView gridView;
		private readonly FilterCollection<T> dataStore;
		private bool sortedAscending = false;
		private GridColumn sortColumn = null;

		public GridViewSorter(GridView gridView, FilterCollection<T> dataStore)
		{
			this.gridView = gridView;
			this.dataStore = dataStore;
		}

		public void EnableSorting()
		{
			foreach (var gridColumn in gridView.Columns)
			{
				gridColumn.Sortable = true;
			}
			gridView.ColumnHeaderClick += (sender, args) =>
			{
				if (args.Column.DataCell is TextBoxCell textBoxCell)
				{
					if (sortColumn == args.Column)
					{
						sortedAscending = !sortedAscending;
					}
					else
					{
						sortedAscending = true;
					}

					sortColumn = args.Column;

					dataStore.Sort = (agent1, agent2) =>
					{
						string propertyAgent1 = textBoxCell.Binding.GetValue(agent1);
						string propertyAgent2 = textBoxCell.Binding.GetValue(agent2);

						// Try to compare as numbers if possible. Many columns will only contain numbers.
						if (long.TryParse(propertyAgent1, out long value1) &&
						    long.TryParse(propertyAgent2, out long value2))
						{
							if (sortedAscending)
							{
								return value1.CompareTo(value2);
							}
							else
							{
								return value2.CompareTo(value1);
							}
						}
						else
						{
							if (sortedAscending)
							{
								return String.Compare(propertyAgent1, propertyAgent2, StringComparison.Ordinal);
							}
							else
							{
								return String.Compare(propertyAgent2, propertyAgent1, StringComparison.Ordinal);
							}
						}
					};
				}
			};
		}
	}
}