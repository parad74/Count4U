using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class PropertyStrToObjects : ObservableCollection<PropertyStrToObject>
    {
		public static PropertyStrToObjects FromEnumerable(IEnumerable<PropertyStrToObject> list)
        {
			var collection = new PropertyStrToObjects();
            return Fill(collection, list);
        }

		private static PropertyStrToObjects Fill(PropertyStrToObjects collection, IEnumerable<PropertyStrToObject> list)
        {
			foreach (PropertyStrToObject item in list)
			{
				collection.Add(item);
			}
            return collection;
        }

		public PropertyStrToObject CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
