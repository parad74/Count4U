using System;
using System.Collections.ObjectModel;

namespace Count4U.Model.Audit
{
   public class Inventors : ObservableCollection<Inventor>
    {

       public static Inventors FromEnumerable(System.Collections.Generic.IEnumerable<Inventor> List)
        {
            Inventors inventors = new Inventors();
            foreach (Inventor item in List)
            {
                inventors.Add(item);
            }
            return inventors;
        }

       public Inventor CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get;  set; }
    }
}
