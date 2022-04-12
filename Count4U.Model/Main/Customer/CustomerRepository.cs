using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Main.Mapping;
using Count4U.Model.Audit;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface;

namespace Count4U.Model.Main
{
    public class CustomerRepository : ICustomerRepository
    {
		private IServiceLocator _serviceLocator;
        private Customers _customerList;

        #region ICustomerRepository Members



        public Customers GetCustomers(CBIContext context)
        {
			if (this._customerList == null)
            {
				this._customerList = new Customers {
		                   new Customer {Code = "CustomerCode1", Name = "CustomerName1", Phone="127605", Description = "Customer description"},
		                   new Customer {Code = "CustomerCode2", Name = "CustomerName2", Phone="127606", Description = "Customer description"},
		                   new Customer {Code = "CustomerCode3", Name = "CustomerName3", Phone="127607", Description = "Customer description"}
		               };
            }
			return this._customerList;
        }


        public Customers GetCustomers(SelectParams selectParams)
        {
            // заглушка
			return this._customerList;
        }


        public Customer GetCustomer(string Code)
        {
			Customers customers = GetCustomers(CBIContext.Main);
            var customer1 = Customers.FromEnumerable(from c in customers
                                                     where c.Code == Code
                                                     select
                                                     new Customer
                                                     {
                                                         Code = c.Code,
                                                         Name = c.Name,
                                                         Phone = c.Phone
                                                     }).First();
            return customer1;
        }

        public Customers GetCustomersDetails()
        {
            throw new NotImplementedException();
        }

        public void SetCustomers(Customers customers)
        {
			this._customerList = customers;
        }

		//public void SetCurrent(Customer currentCustomer)
		//{
		//    this._customerList.CurrentItem = currentCustomer;
		//}

		//public Customer GetCurrent()
		//{
		//    return this._customerList.CurrentItem;
		//}

		public void SetCurrent(Customer currentCustomer, AuditConfig auditConfig)
		{
			this._customerList.CurrentItem = currentCustomer;
		}

		public Customer GetCurrent(AuditConfig auditConfig)
		{
			return this._customerList.CurrentItem;
		}

		//public Customer Clone(Customer customer)
		//{
		//    var domainObject = customer.Clone();
		//    domainObject.ID = 0;
		//    return domainObject;
		//}

        public void Delete(Customer customer)
        {
			var entity = this.GetEntityByCode(customer.Code);
			if (entity == null) return;
			this.GetCustomers(CBIContext.Main).Remove(entity);
        }

        public void Delete(string customerCode)
        {
			var entity = this.GetEntityByCode(customerCode);
			if (entity == null) return;
			this.GetCustomers(CBIContext.Main).Remove(entity);
        }

        public void Insert(Customer customer)
        {
			if (customer == null) return;
            var entity = customer.ToEntity();
			this.GetCustomers(CBIContext.Main).Add(entity);
        }

        public void Update(Customer customer)
        {
            //Customer this._customer = this._customerList.Where(c => c.Code == customer.Code).FirstOrDefault();
            //if (this._customer != null)
            //{
            //    this._customer.Name = customer.Name;
            //    this._customer.Phone = customer.Phone;
            //}
			if (customer == null) return;
            var entity = this.GetEntityByCode(customer.Code);
			if (entity == null) return;
            entity.ApplyChanges(customer);
        }

        public Customer GetCustomerByCode(string customerCode)
        {
            var entity = this.GetEntityByCode(customerCode);
			if (entity == null) return null;
            return entity.ToDomainObject();
        }

        #endregion

        #region private

        private Customer GetEntityByCode(string customerCode)
        {
			var entity = this.GetCustomers(CBIContext.Main).First(e => e.Code.CompareTo(customerCode) == 0);
            return entity;
        }

        #endregion



		#region ICustomerRepository Members


		public string GetCurrentCode(AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members


		public void Delete(Customer customer, bool full = false)
		{
			throw new NotImplementedException();
		}

		public void Delete(string customerCode, bool full = false)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members


		public IConnectionDB Connection
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}


		#endregion

		#region ICustomerRepository Members


		public List<string> GetCodeList()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members


		public void RefillInventorConfigs(Customer customer)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members


		public void InsertDomainСustomerFromInventorConfig(Customers customers)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members


		public void RefillInventorConfigsAllCustomersInMainDB()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members


		public Dictionary<string, Customer> FillCustomerDictionary()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members


		public void UpdateDomainСustomerByInventorConfig(Customers customers)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members


		public void Delete(List<string> customerCodeList)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members

		public string BuildRelativeDbPath(Customer customer)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ICustomerRepository Members


		public void DeleteDomainObjectOnly(List<string> customerCodeList)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
