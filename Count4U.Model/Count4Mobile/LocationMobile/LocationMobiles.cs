using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4Mobile
{
	public class LocationMobiles : ObservableCollection<LocationMobile>
	{
		public static LocationMobiles FromEnumerable(IEnumerable<LocationMobile> list)
		{
			var collection = new LocationMobiles();
			return Fill(collection, list);
		}

		private static LocationMobiles Fill(LocationMobiles collection, IEnumerable<LocationMobile> list)
		{
			foreach (LocationMobile item in list)
			{
				collection.Add(item);
			}
			return collection;
		}

		public LocationMobile CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}

