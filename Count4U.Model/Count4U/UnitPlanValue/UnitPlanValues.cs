using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class UnitPlanValues : ObservableCollection<UnitPlanValue>
    {
		public static UnitPlanValues FromEnumerable(IEnumerable<UnitPlanValue> list)
        {
			var collection = new UnitPlanValues();
            return Fill(collection, list);
        }

		private static UnitPlanValues Fill(UnitPlanValues collection, IEnumerable<UnitPlanValue> list)
        {
			foreach (UnitPlanValue item in list)
                collection.Add(item);
            return collection;
        }

		public UnitPlanValue CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
