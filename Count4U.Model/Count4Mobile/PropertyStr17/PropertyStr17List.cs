using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr17List : ObservableCollection<PropertyStr17>
	{
		public static PropertyStr17List FromEnumerable(IEnumerable<PropertyStr17> list)
		{
			var collection = new PropertyStr17List();
			return Fill(collection, list);
		}

		private static PropertyStr17List Fill(PropertyStr17List collection, IEnumerable<PropertyStr17> list)
		{
			foreach (PropertyStr17 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr17 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

