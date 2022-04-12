using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class CurrentInventorys : ObservableCollection<CurrentInventory>
	{
		public static CurrentInventorys FromEnumerable(IEnumerable<CurrentInventory> list)
		{
			var collection = new CurrentInventorys();
			return Fill(collection, list);
		}

		private static CurrentInventorys Fill(CurrentInventorys collection, IEnumerable<CurrentInventory> list)
		{
			foreach (CurrentInventory item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public CurrentInventory CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

