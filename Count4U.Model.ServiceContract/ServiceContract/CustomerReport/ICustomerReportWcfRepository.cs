using Count4U.Model.Main;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Collections.Generic;


namespace Count4U.Model.ServiceContract
{
	[ServiceContract]
	public interface ICustomerReportWcfRepository
	{
		[OperationContract]
		List<CustomerReport> GetCustomerReportList();

		[OperationContract]
		CustomerReports GetCustomerReports();

		//[OperationContract]
		//CustomerReports GetCustomerReports(SelectParams selectParams);

		[OperationContract]
		CustomerReport GetCustomerReport(long id);

		[OperationContract]
		bool Insert(CustomerReport customerReport);

		[OperationContract]
		bool Update(CustomerReport customerReport);

		[OperationContract]
		bool Delete(long id);
	}
}

