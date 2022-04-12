using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr1List : ObservableCollection<PropertyStr1>
	{
		public static PropertyStr1List FromEnumerable(IEnumerable<PropertyStr1> list)
		{
			var collection = new PropertyStr1List();
			return Fill(collection, list);
		}

		private static PropertyStr1List Fill(PropertyStr1List collection, IEnumerable<PropertyStr1> list)
		{
			foreach (PropertyStr1 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr1 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

