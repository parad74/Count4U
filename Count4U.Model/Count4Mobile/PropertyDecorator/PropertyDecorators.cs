using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class PropertyDecorators : ObservableCollection<PropertyDecorator>
	{
		public static PropertyDecorators FromEnumerable(IEnumerable<PropertyDecorator> list)
		{
			var collection = new PropertyDecorators();
			return Fill(collection, list);
		}

		private static PropertyDecorators Fill(PropertyDecorators collection, IEnumerable<PropertyDecorator> list)
		{
			foreach (PropertyDecorator item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public PropertyDecorator CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

