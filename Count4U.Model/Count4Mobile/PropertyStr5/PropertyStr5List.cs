using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr5List : ObservableCollection<PropertyStr5>
	{
		public static PropertyStr5List FromEnumerable(IEnumerable<PropertyStr5> list)
		{
			var collection = new PropertyStr5List();
			return Fill(collection, list);
		}

		private static PropertyStr5List Fill(PropertyStr5List collection, IEnumerable<PropertyStr5> list)
		{
			foreach (PropertyStr5 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr5 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

