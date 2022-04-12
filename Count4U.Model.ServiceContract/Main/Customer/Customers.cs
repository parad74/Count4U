using System.Collections.ObjectModel;
using Count4U.Model.Main;

namespace Count4U.Model.Main
{
    /// <summary>
    /// Collection class for Customers entitites.
    /// </summary>
    public class Customers : ObservableCollection<Customer>
    {
		public static Customers FromEnumerable(System.Collections.Generic.IEnumerable<Customer> List)
		{
			Customers customers = new Customers();
			foreach (Customer item in List)
			{
				customers.Add(item);
			}
			return customers;
		}

	     public Customer CurrentItem 
         {
             get; 
             set; 
         }

        public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get;  set; }
    }
}

