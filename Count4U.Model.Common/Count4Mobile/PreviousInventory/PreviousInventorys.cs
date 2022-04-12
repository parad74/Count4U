using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PreviousInventorys : ObservableCollection<PreviousInventory>
	{
		public static PreviousInventorys FromEnumerable(IEnumerable<PreviousInventory> list)
		{
			var collection = new PreviousInventorys();
			return Fill(collection, list);
		}

		private static PreviousInventorys Fill(PreviousInventorys collection, IEnumerable<PreviousInventory> list)
		{
			foreach (PreviousInventory item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PreviousInventory CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

