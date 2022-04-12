using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class TemporaryInventorys : ObservableCollection<TemporaryInventory>
	{
		public static TemporaryInventorys FromEnumerable(IEnumerable<TemporaryInventory> list)
		{
			var collection = new TemporaryInventorys();
			return Fill(collection, list);
		}

		private static TemporaryInventorys Fill(TemporaryInventorys collection, IEnumerable<TemporaryInventory> list)
		{
			foreach (TemporaryInventory item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public TemporaryInventory CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

