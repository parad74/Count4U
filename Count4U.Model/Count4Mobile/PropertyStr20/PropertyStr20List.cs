using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr20List : ObservableCollection<PropertyStr20>
	{
		public static PropertyStr20List FromEnumerable(IEnumerable<PropertyStr20> list)
		{
			var collection = new PropertyStr20List();
			return Fill(collection, list);
		}

		private static PropertyStr20List Fill(PropertyStr20List collection, IEnumerable<PropertyStr20> list)
		{
			foreach (PropertyStr20 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr20 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

