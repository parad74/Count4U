using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.GenerationReport
{
	public class ContextReports : ObservableCollection<ContextReport>
    {
		public static ContextReports FromEnumerable(IEnumerable<ContextReport> list)
        {
			var collection = new ContextReports();
            return Fill(collection, list);
        }

		private static ContextReports Fill(ContextReports collection, IEnumerable<ContextReport> list)
        {
			foreach (ContextReport item in list)
                collection.Add(item);
            return collection;
        }

		public ContextReports CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
