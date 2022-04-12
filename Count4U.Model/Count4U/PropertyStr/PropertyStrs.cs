using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class PropertyStrs : ObservableCollection<PropertyStr>
    {
		public static PropertyStrs FromEnumerable(IEnumerable<PropertyStr> list)
        {
			var collection = new PropertyStrs();
            return Fill(collection, list);
        }

		private static PropertyStrs Fill(PropertyStrs collection, IEnumerable<PropertyStr> list)
        {
			foreach (PropertyStr item in list)
			{
				collection.Add(item);
			}
            return collection;
        }

		public PropertyStr CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
