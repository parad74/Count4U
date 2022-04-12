using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class StatusIturGroups : ObservableCollection<StatusIturGroup>
    {
		public static StatusIturGroups FromEnumerable(IEnumerable<StatusIturGroup> list)
        {
			var collection = new StatusIturGroups();
            return Fill(collection, list);
        }

		private static StatusIturGroups Fill(StatusIturGroups collection, IEnumerable<StatusIturGroup> list)
        {
			foreach (StatusIturGroup item in list)
                collection.Add(item);
            return collection;
        }

		public StatusIturGroup CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
