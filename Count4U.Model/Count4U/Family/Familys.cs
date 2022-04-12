using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{

	public class Familys : ObservableCollection<Family>
    {

		public static Familys FromEnumerable(System.Collections.Generic.IEnumerable<Family> List)
        {
			Familys familys = new Familys();
			foreach (Family item in List)
            {
                familys.Add(item);
            }
            return familys;
        }

		public Family CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
