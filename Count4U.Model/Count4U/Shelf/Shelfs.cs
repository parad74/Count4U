using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{

	public class Shelfs : ObservableCollection<Shelf>
    {

		public static Shelfs FromEnumerable(System.Collections.Generic.IEnumerable<Shelf> List)
        {
			Shelfs shelfs = new Shelfs();
			foreach (Shelf item in List)
            {
				shelfs.Add(item);
            }
			return shelfs;
        }

		public Shelf CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
