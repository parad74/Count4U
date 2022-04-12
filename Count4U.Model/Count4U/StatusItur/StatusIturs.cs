using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class StatusIturs : ObservableCollection<StatusItur>
    {
        public static StatusIturs FromEnumerable(IEnumerable<StatusItur> list)
        {
            var collection = new StatusIturs();
            return Fill(collection, list);
        }

        private static StatusIturs Fill(StatusIturs collection, IEnumerable<StatusItur> list)
        {
            foreach (StatusItur item in list)
                collection.Add(item);
            return collection;
        }

        public StatusItur CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
