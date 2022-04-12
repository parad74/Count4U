using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class TemplateInventorys : ObservableCollection<TemplateInventory>
	{
		public static TemplateInventorys FromEnumerable(IEnumerable<TemplateInventory> list)
		{
			var collection = new TemplateInventorys();
			return Fill(collection, list);
		}

		private static TemplateInventorys Fill(TemplateInventorys collection, IEnumerable<TemplateInventory> list)
		{
			foreach (TemplateInventory item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public TemplateInventory CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

