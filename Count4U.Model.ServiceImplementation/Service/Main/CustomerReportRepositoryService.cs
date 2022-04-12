using System;
using System.Collections.Generic;
using Count4U.Model.Main;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Main;
using Count4U.Model;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.UnityExtensions;
using Count4U.Model.SelectionParams;
using Count4U.Model.ServiceContract;
using Common.Utility.Constant;

namespace Count4U.Model.Service
{
    public class CustomerReportRepositoryService : ICustomerReportWcfRepository
    {
	
		public List<CustomerReport> GetCustomerReportList()
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerReportRepository customerReportRepository = serviceLocator.GetInstance<ICustomerReportRepository>();
			CustomerReports customerReports = customerReportRepository.GetCustomerReports();
			List<CustomerReport> customersList = new List<CustomerReport>();
			foreach (CustomerReport item in customerReports)
			{
				customersList.Add(item);
			}
			return customersList;
		}

		public CustomerReports GetCustomerReports()
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerReportRepository customerReportRepository = serviceLocator.GetInstance<ICustomerReportRepository>();
			CustomerReports customerReports = customerReportRepository.GetCustomerReports();

			return customerReports;
		}

		//public CustomerReports GetCustomerReports(SelectParams selectParams)
		//{
		//	IServiceLocator serviceLocator = Init.Common.ServiceLocatorResolve;
		//	ICustomerReportRepository customerReportRepository = serviceLocator.GetInstance<ICustomerReportRepository>();
		//	CustomerReports customerReports = customerReportRepository.GetCustomerReports(selectParams);

		//	return customerReports;
		//}

		public CustomerReport GetCustomerReport(long id)
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerReportRepository customerReportRepository = serviceLocator.GetInstance<ICustomerReportRepository>();
			CustomerReport customerReport = customerReportRepository.GetCustomerReport(id);
			return customerReport;
		}

		public bool Insert(CustomerReport customerReport)
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerReportRepository customerReportRepository = serviceLocator.GetInstance<ICustomerReportRepository>();
			customerReportRepository.Insert(customerReport);
			return true;
	
		}

		public bool Update(CustomerReport customerReport)
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerReportRepository customerReportRepository = serviceLocator.GetInstance<ICustomerReportRepository>();
			customerReportRepository.Update(customerReport);
			return true;
	
		}

		public bool Delete(long id)
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerReportRepository customerReportRepository = serviceLocator.GetInstance<ICustomerReportRepository>();
			customerReportRepository.Delete(id);
			return true;
		}


	}
}
