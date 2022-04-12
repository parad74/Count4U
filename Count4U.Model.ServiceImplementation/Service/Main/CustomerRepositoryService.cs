using System;
using System.Collections.Generic;
using Count4U.Model.Main;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Main;
using Count4U.Model;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.UnityExtensions;
using Common.Utility.Constant;
using Count4U.Model.ServiceContract;

namespace Count4U.WPF.Service
{
	public class CustomerRepositoryService : ICustomerWcfRepository
    {
		//private IServiceLocator _serviceLocator;
		//private ICustomerRepository _customerRepository;

		////public CustomerRepositoryService(ICustomerRepository customerRepository, IServiceLocator serviceLocator)
		////{
		////	_serviceLocator = serviceLocator;
		////	_customerRepository = customerRepository;
		////}

        public Customer GetCustomer(string customerCode)
        {
			//CustomerEFRepository customerRepository = Count4U.WindowsHost.Program.ContainerStatic.Resolve<ICustomerRepository>();
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerRepository customerRepository	=serviceLocator.GetInstance<ICustomerRepository>();
			Customer customer = customerRepository.GetCustomerByCode(customerCode);
			return customer;

           // return _customerRepository.GetCustomerByCode(customerCode);
			//return new Customer() { Code = "111", Name = "FistCustomer111" };
        }

        public List<Customer> GetCustomers()						   
        {
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerRepository customerRepository = serviceLocator.GetInstance<ICustomerRepository>();
			Customers customers = customerRepository.GetCustomers(CBIContext.Main);
			List<Customer> customersList = new List<Customer>();
			foreach (Customer item in customers)
			{
				customersList.Add(item);
			}
			return customersList;
			//List<Customer> list = new List<Customer>();
			//Customer customer1= new Customer() { Code = "111", Name = "FistCustomer111" };
			//Customer customer2 = new Customer() { Code = "111", Name = "FistCustomer111" };
			//Customer customer3 = new Customer() { Code = "111", Name = "FistCustomer111" };
			//list.Add(customer1);
			//list.Add(customer2);
			//list.Add(customer3);
            //Customers customers =  _customerRepository.GetCustomers(CBIContext.Main);
			//return list;
        }

		public bool Delete(string customerCode)
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerRepository customerRepository = serviceLocator.GetInstance<ICustomerRepository>();
			customerRepository.Delete(customerCode);
			return true;
		}

		public bool Insert(Customer customer)
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerRepository customerRepository = serviceLocator.GetInstance<ICustomerRepository>();
			customerRepository.Insert(customer);
			return true;
		}


		public bool Update(Customer customer)
		{
			IServiceLocator serviceLocator = GlogalConstantStatic.ServiceLocatorStatic;
			ICustomerRepository customerRepository = serviceLocator.GetInstance<ICustomerRepository>();
			customerRepository.Update(customer);
			return true;
		}

    }
}
