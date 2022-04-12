using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr11List : ObservableCollection<PropertyStr11>
	{
		public static PropertyStr11List FromEnumerable(IEnumerable<PropertyStr11> list)
		{
			var collection = new PropertyStr11List();
			return Fill(collection, list);
		}

		private static PropertyStr11List Fill(PropertyStr11List collection, IEnumerable<PropertyStr11> list)
		{
			foreach (PropertyStr11 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr11 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

