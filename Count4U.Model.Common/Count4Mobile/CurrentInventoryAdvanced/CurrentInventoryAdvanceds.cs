using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class CurrentInventoryAdvanceds : ObservableCollection<CurrentInventoryAdvanced>
	{
		public static CurrentInventoryAdvanceds FromEnumerable(IEnumerable<CurrentInventoryAdvanced> list)
		{
			var collection = new CurrentInventoryAdvanceds();
			return Fill(collection, list);
		}

		private static CurrentInventoryAdvanceds Fill(CurrentInventoryAdvanceds collection, IEnumerable<CurrentInventoryAdvanced> list)
		{
			foreach (CurrentInventoryAdvanced item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public CurrentInventoryAdvanced CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

