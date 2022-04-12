using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class Devices : ObservableCollection<Device>
    {
		public static Devices FromEnumerable(IEnumerable<Device> list)
        {
			var collection = new Devices();
            return Fill(collection, list);
        }

		private static Devices Fill(Devices collection, IEnumerable<Device> list)
        {
			foreach (Device item in list)
			{
				collection.Add(item);
			}
            return collection;
        }

		public Device CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
