using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
    public class Types : ObservableCollection<Type>
    {

        public static Types FromEnumerable(System.Collections.Generic.IEnumerable<Type> List)
        {
            Types types = new Types();
            foreach (Type item in List)
            {
                types.Add(item);
            }
            return types;
        }

        public Type CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
    }
}
