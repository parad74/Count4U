using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr13List : ObservableCollection<PropertyStr13>
	{
		public static PropertyStr13List FromEnumerable(IEnumerable<PropertyStr13> list)
		{
			var collection = new PropertyStr13List();
			return Fill(collection, list);
		}

		private static PropertyStr13List Fill(PropertyStr13List collection, IEnumerable<PropertyStr13> list)
		{
			foreach (PropertyStr13 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr13 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

