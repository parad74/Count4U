using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr14List : ObservableCollection<PropertyStr14>
	{
		public static PropertyStr14List FromEnumerable(IEnumerable<PropertyStr14> list)
		{
			var collection = new PropertyStr14List();
			return Fill(collection, list);
		}

		private static PropertyStr14List Fill(PropertyStr14List collection, IEnumerable<PropertyStr14> list)
		{
			foreach (PropertyStr14 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr14 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

