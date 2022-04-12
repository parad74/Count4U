using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Count4U.Model.Main;

namespace Count4U.Model.Main
{
	public class CustomerReports : ObservableCollection<CustomerReport>
	{
		public static CustomerReports FromEnumerable(IEnumerable<CustomerReport> list)
		{
			var collection = new CustomerReports();
			return Fill(collection, list);
		}

		public static CustomerReports Fill(CustomerReports collection, IEnumerable<CustomerReport> list)
		{
			foreach (CustomerReport item in list)
				collection.Add(item);
			return collection;
		}

		[DataMember(Name = "CurrentItem", Order = 1, IsRequired = true)]
		public CustomerReport CurrentItem { get; set; }

		[DataMember(Name = "CurrentChanged", Order = 1, IsRequired = true)]
		public System.EventHandler CurrentChanged { get; set; }

		[DataMember(Name = "TotalCount", Order = 1, IsRequired = true)]
		public long TotalCount { get; set; }
	}
}

