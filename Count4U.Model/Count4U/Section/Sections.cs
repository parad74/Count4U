using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
    public class Sections : ObservableCollection<Section>
    {

        public static Sections FromEnumerable(System.Collections.Generic.IEnumerable<Section> List)
        {
            Sections sections = new Sections();
            foreach (Section item in List)
            {
                sections.Add(item);
            }
            return sections;
        }

        public Section CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
