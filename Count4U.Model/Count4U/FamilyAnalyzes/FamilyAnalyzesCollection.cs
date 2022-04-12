using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Count4U.Model.Count4U
{

	public class FamilyAnalyzesCollection : ObservableCollection<FamilyAnalyzes>
    {

		public static FamilyAnalyzesCollection FromEnumerable(IEnumerable<FamilyAnalyzes> List)
        {
			FamilyAnalyzesCollection familys = new FamilyAnalyzesCollection();
			foreach (FamilyAnalyzes item in List)
            {
                familys.Add(item);
            }
            return familys;
        }

		private static FamilyAnalyzesCollection Fill(FamilyAnalyzesCollection collection, IEnumerable<FamilyAnalyzes> list)
		{
			foreach (FamilyAnalyzes item in list)
				collection.Add(item);
			return collection;
		}

		public FamilyAnalyzes CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
