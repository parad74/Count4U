using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr7List : ObservableCollection<PropertyStr7>
	{
		public static PropertyStr7List FromEnumerable(IEnumerable<PropertyStr7> list)
		{
			var collection = new PropertyStr7List();
			return Fill(collection, list);
		}

		private static PropertyStr7List Fill(PropertyStr7List collection, IEnumerable<PropertyStr7> list)
		{
			foreach (PropertyStr7 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr7 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

