using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class UnitPlans : ObservableCollection<UnitPlan>
    {
		public static UnitPlans FromEnumerable(IEnumerable<UnitPlan> list)
        {
			var collection = new UnitPlans();
            return Fill(collection, list);
        }

		private static UnitPlans Fill(UnitPlans collection, IEnumerable<UnitPlan> list)
        {
			foreach (UnitPlan item in list)
                collection.Add(item);
            return collection;
        }

		public UnitPlan CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
