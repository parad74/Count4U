using System;
using Count4U.Model.Interface.Audit;
using System.Collections.ObjectModel;

namespace Count4U.Model.Audit
{
	public class StatusInventors : ObservableCollection<StatusInventor>
    {

		public static StatusInventors FromEnumerable(System.Collections.Generic.IEnumerable<StatusInventor> List)
        {
			StatusInventors statusAudits = new StatusInventors();
			foreach (StatusInventor item in List)
            {
				statusAudits.Add(item);
            }
			return statusAudits;
        }

		public StatusInventor CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
