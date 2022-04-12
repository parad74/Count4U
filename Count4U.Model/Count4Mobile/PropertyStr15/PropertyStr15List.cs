using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr15List : ObservableCollection<PropertyStr15>
	{
		public static PropertyStr15List FromEnumerable(IEnumerable<PropertyStr15> list)
		{
			var collection = new PropertyStr15List();
			return Fill(collection, list);
		}

		private static PropertyStr15List Fill(PropertyStr15List collection, IEnumerable<PropertyStr15> list)
		{
			foreach (PropertyStr15 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr15 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

