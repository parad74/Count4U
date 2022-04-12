using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr4List : ObservableCollection<PropertyStr4>
	{
		public static PropertyStr4List FromEnumerable(IEnumerable<PropertyStr4> list)
		{
			var collection = new PropertyStr4List();
			return Fill(collection, list);
		}

		private static PropertyStr4List Fill(PropertyStr4List collection, IEnumerable<PropertyStr4> list)
		{
			foreach (PropertyStr4 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr4 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

