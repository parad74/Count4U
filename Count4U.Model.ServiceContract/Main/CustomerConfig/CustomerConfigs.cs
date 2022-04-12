using System.Collections.ObjectModel;
using Count4U.Model.Main;
using System.Collections.Generic;

namespace Count4U.Model.Main
{
    /// <summary>
    /// Collection class for CustomerConfigs entitites.
    /// </summary>
    public class CustomerConfigs : ObservableCollection<CustomerConfig>
    {
		public static CustomerConfigs FromEnumerable(IEnumerable<CustomerConfig> list)
		{
			var collection = new CustomerConfigs();
			return Fill(collection, list);
		}

		private static CustomerConfigs Fill(CustomerConfigs collection, IEnumerable<CustomerConfig> list)
		{
			foreach (CustomerConfig item in list)
				collection.Add(item);
			return collection;
		}

        public CustomerConfig CurrentItem { get; set; }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; set; }
    }
}


