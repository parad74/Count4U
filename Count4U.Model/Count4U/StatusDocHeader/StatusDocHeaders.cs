using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
   public class StatusDocHeaders : ObservableCollection<StatusDocHeader>
    {

       public static StatusDocHeaders FromEnumerable(System.Collections.Generic.IEnumerable<StatusDocHeader> List)
        {
            StatusDocHeaders statusDocHeaders = new StatusDocHeaders();
            foreach (StatusDocHeader item in List)
            {
                statusDocHeaders.Add(item);
            }
            return statusDocHeaders;
        }

       public StatusDocHeader CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
