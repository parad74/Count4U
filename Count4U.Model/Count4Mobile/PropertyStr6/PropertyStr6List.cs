using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr6List : ObservableCollection<PropertyStr6>
	{
		public static PropertyStr6List FromEnumerable(IEnumerable<PropertyStr6> list)
		{
			var collection = new PropertyStr6List();
			return Fill(collection, list);
		}

		private static PropertyStr6List Fill(PropertyStr6List collection, IEnumerable<PropertyStr6> list)
		{
			foreach (PropertyStr6 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr6 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

