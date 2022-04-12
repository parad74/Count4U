using System.Collections.ObjectModel;

namespace Count4U.Model
{
    /// <summary>
    /// Collection class for Branches entitites.
    /// </summary>
	public class MaskTemplates : ObservableCollection<MaskTemplate>
    {

		public static MaskTemplates FromEnumerable(System.Collections.Generic.IEnumerable<MaskTemplate> List)
        {
			MaskTemplates branches = new MaskTemplates();
			foreach (MaskTemplate item in List)
            {
                branches.Add(item);
            }
            return branches;
        }

		public MaskTemplate CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
