using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr18List : ObservableCollection<PropertyStr18>
	{
		public static PropertyStr18List FromEnumerable(IEnumerable<PropertyStr18> list)
		{
			var collection = new PropertyStr18List();
			return Fill(collection, list);
		}

		private static PropertyStr18List Fill(PropertyStr18List collection, IEnumerable<PropertyStr18> list)
		{
			foreach (PropertyStr18 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr18 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

