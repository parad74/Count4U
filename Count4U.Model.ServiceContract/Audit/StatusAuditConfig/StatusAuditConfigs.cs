using System;
using Count4U.Model.Interface.Audit;
using System.Collections.ObjectModel;

namespace Count4U.Model.Audit
{
	public class StatusAuditConfigs : ObservableCollection<StatusAuditConfig>
    {

		public static StatusAuditConfigs FromEnumerable(System.Collections.Generic.IEnumerable<StatusAuditConfig> List)
        {
			StatusAuditConfigs statuses = new StatusAuditConfigs();
			foreach (StatusAuditConfig item in List)
            {
				statuses.Add(item);
            }
			return statuses;
        }

		public StatusAuditConfig CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
