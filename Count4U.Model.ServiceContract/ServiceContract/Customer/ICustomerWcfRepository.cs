using Count4U.Model.Main;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Collections.Generic;


namespace Count4U.Model.ServiceContract
{
	[ServiceContract]
	public interface ICustomerWcfRepository
	{		
		[OperationContract]
		Customer GetCustomer(string customerCode)	;

		[OperationContract]
		List<Customer> GetCustomers();

		[OperationContract]
		bool Delete(string customerCode) ;

		[OperationContract]
		bool Insert(Customer customer)	;

		[OperationContract]
		bool Update(Customer customer);

		
	}
}

