using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr12List : ObservableCollection<PropertyStr12>
	{
		public static PropertyStr12List FromEnumerable(IEnumerable<PropertyStr12> list)
		{
			var collection = new PropertyStr12List();
			return Fill(collection, list);
		}

		private static PropertyStr12List Fill(PropertyStr12List collection, IEnumerable<PropertyStr12> list)
		{
			foreach (PropertyStr12 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr12 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

