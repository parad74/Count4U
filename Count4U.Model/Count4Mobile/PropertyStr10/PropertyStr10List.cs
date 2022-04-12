using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr10List : ObservableCollection<PropertyStr10>
	{
		public static PropertyStr10List FromEnumerable(IEnumerable<PropertyStr10> list)
		{
			var collection = new PropertyStr10List();
			return Fill(collection, list);
		}

		private static PropertyStr10List Fill(PropertyStr10List collection, IEnumerable<PropertyStr10> list)
		{
			foreach (PropertyStr10 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr10 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

