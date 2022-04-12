using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class Locations : ObservableCollection<Location>
    {
        public static Locations FromEnumerable(IEnumerable<Location> list)
        {
            var collection = new Locations();
            return Fill(collection, list);
        }

        private static Locations Fill(Locations collection, IEnumerable<Location> list)
        {
			foreach (Location item in list)
			{
				collection.Add(item);
			}
            return collection;
        }

        public Location CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
