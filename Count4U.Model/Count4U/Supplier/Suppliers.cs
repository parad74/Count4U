using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
   
    public class Suppliers : ObservableCollection<Supplier>
    {

        public static Suppliers FromEnumerable(System.Collections.Generic.IEnumerable<Supplier> List)
        {
            Suppliers suppliers = new Suppliers();
            foreach (Supplier item in List)
            {
                suppliers.Add(item);
            }
            return suppliers;
        }

        public Supplier CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
