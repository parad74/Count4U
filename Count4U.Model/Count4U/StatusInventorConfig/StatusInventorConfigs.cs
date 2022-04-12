using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
    public class StatusInventorConfigs : ObservableCollection<StatusInventorConfig>
    {

        public static StatusInventorConfigs FromEnumerable(System.Collections.Generic.IEnumerable<StatusInventorConfig> List)
        {
            StatusInventorConfigs statuses = new StatusInventorConfigs();
            foreach (StatusInventorConfig item in List)
            {
				statuses.Add(item);
            }
			return statuses;
        }

        public StatusInventorConfig CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }
    }
}
