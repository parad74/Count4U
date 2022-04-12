using System.Collections.ObjectModel;
using Count4U.Model.Main;

namespace Count4U.Model.Main
{
    /// <summary>
    /// Collection class for Branches entitites.
    /// </summary>
    public class Branches : ObservableCollection<Branch>
    {
   
        public static Branches FromEnumerable(System.Collections.Generic.IEnumerable<Branch> List)
        {
            Branches branches = new Branches();
            foreach (Branch item in List)
            {
                branches.Add(item);
            }
            return branches;
        }

        public Branch CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get;  set; }
    }
}
