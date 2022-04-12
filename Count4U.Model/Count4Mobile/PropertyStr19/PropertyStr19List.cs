using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr19List : ObservableCollection<PropertyStr19>
	{
		public static PropertyStr19List FromEnumerable(IEnumerable<PropertyStr19> list)
		{
			var collection = new PropertyStr19List();
			return Fill(collection, list);
		}

		private static PropertyStr19List Fill(PropertyStr19List collection, IEnumerable<PropertyStr19> list)
		{
			foreach (PropertyStr19 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr19 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

