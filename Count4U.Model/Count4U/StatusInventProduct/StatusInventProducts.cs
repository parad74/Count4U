using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
   public class StatusInventProducts : ObservableCollection<StatusInventProduct>
    {

       public static StatusInventProducts FromEnumerable(System.Collections.Generic.IEnumerable<StatusInventProduct> List)
        {
            StatusInventProducts statusInventors = new StatusInventProducts();
            foreach (StatusInventProduct item in List)
            {
                statusInventors.Add(item);
            }
            return statusInventors;
        }

       public StatusInventProduct CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
