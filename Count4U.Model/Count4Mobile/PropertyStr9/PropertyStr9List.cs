using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr9List : ObservableCollection<PropertyStr9>
	{
		public static PropertyStr9List FromEnumerable(IEnumerable<PropertyStr9> list)
		{
			var collection = new PropertyStr9List();
			return Fill(collection, list);
		}

		private static PropertyStr9List Fill(PropertyStr9List collection, IEnumerable<PropertyStr9> list)
		{
			foreach (PropertyStr9 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr9 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

