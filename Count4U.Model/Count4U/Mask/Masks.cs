using System.Collections.ObjectModel;

namespace Count4U.Model
{
    /// <summary>
    /// Collection class for Branches entitites.
    /// </summary>
	public class Masks : ObservableCollection<Mask>
    {

		public static Masks FromEnumerable(System.Collections.Generic.IEnumerable<Mask> List)
        {
			Masks branches = new Masks();
			foreach (Mask item in List)
            {
                branches.Add(item);
            }
            return branches;
        }

		public Mask CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
