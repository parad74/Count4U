using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
    public class UnitTypes : ObservableCollection<UnitType>
    {

        public static UnitTypes FromEnumerable(System.Collections.Generic.IEnumerable<UnitType> List)
        {
            UnitTypes unitTypes = new UnitTypes();
            foreach (UnitType item in List)
            {
                unitTypes.Add(item);
            }
            return unitTypes;
        }

        public UnitType CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
