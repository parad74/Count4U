using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr2List : ObservableCollection<PropertyStr2>
	{
		public static PropertyStr2List FromEnumerable(IEnumerable<PropertyStr2> list)
		{
			var collection = new PropertyStr2List();
			return Fill(collection, list);
		}

		private static PropertyStr2List Fill(PropertyStr2List collection, IEnumerable<PropertyStr2> list)
		{
			foreach (PropertyStr2 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr2 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

