using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr3List : ObservableCollection<PropertyStr3>
	{
		public static PropertyStr3List FromEnumerable(IEnumerable<PropertyStr3> list)
		{
			var collection = new PropertyStr3List();
			return Fill(collection, list);
		}

		private static PropertyStr3List Fill(PropertyStr3List collection, IEnumerable<PropertyStr3> list)
		{
			foreach (PropertyStr3 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr3 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

