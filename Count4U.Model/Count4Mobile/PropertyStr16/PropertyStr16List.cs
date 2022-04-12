using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr16List : ObservableCollection<PropertyStr16>
	{
		public static PropertyStr16List FromEnumerable(IEnumerable<PropertyStr16> list)
		{
			var collection = new PropertyStr16List();
			return Fill(collection, list);
		}

		private static PropertyStr16List Fill(PropertyStr16List collection, IEnumerable<PropertyStr16> list)
		{
			foreach (PropertyStr16 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr16 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

