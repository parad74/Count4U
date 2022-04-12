using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyStr8List : ObservableCollection<PropertyStr8>
	{
		public static PropertyStr8List FromEnumerable(IEnumerable<PropertyStr8> list)
		{
			var collection = new PropertyStr8List();
			return Fill(collection, list);
		}

		private static PropertyStr8List Fill(PropertyStr8List collection, IEnumerable<PropertyStr8> list)
		{
			foreach (PropertyStr8 item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyStr8 CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

