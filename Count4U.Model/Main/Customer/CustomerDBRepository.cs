using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Audit;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Main
{
   public class CustomerDBRepository :  BaseEFRepository, ICustomerRepository
   {
       private readonly string _configurationMainDBConnectionString;
	   private IServiceLocator _serviceLocator;

	   public CustomerDBRepository(ConnectionDB connection, IServiceLocator serviceLocator)
            : base(connection)
		{
		    this._configurationMainDBConnectionString = connection.DBSettings.BuildMainDBConnectionString();
		}

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion	
		
		private Customers _customers;
    	#region ICustomerRepository Members

        public Customers GetCustomers(CBIContext context)
        {
			//App_Data.MainDB dc = new App_Data.MainDB(Properties.Settings.Default.MainDBConnectionString);

            App_Data.MainDB dc = new App_Data.MainDB(this._configurationMainDBConnectionString);
            if (this._customers == null)
            {
				
				var custs = from c in dc.Customer
							select new { Code = c.Code };

				this._customers = new Customers();
				foreach (var cust in custs)
				{
					Customer newCustomer = new Customer { Code = cust.Code };
					this._customers.Add(newCustomer);
				}
            }
            return this._customers;
        }

		public Customers GetCustomers(SelectParams selectParams)
		{
			throw new NotImplementedException();
		}

		public Customer GetCustomer(string Code)
		{
		//	App_Data.MainDB dc = new App_Data.MainDB(Properties.Settings.Default.MainDBConnectionString);
            App_Data.MainDB dc = new App_Data.MainDB(this._configurationMainDBConnectionString);
				var cust = (from c in dc.Customer
							where c.Code == Code	 
							select  new Customer { Code = c.Code }).First();
				return cust;
		}

		public Customers GetCustomersDetails()
		{
			throw new NotImplementedException();
		}

		public Customer Clone(Customer customer)
		{
			throw new NotImplementedException();
		}

		public void Delete(Customer customer)
		{
			throw new NotImplementedException();
		}

		public void Delete(string customerCode)
		{
			throw new NotImplementedException();
		}

		public void Insert(Customer customer)
		{
			throw new NotImplementedException();
		}
  
		public void Update(Customer customer)
		{
			throw new NotImplementedException();
		}

        public void SetCustomers(Customers customers)
        {
            throw new NotImplementedException();
        }

   		public Customer GetCustomerByCode(string customerCode)
		{
			throw new NotImplementedException();
		}


		public void SetCurrent(Customer currentCustomer, AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		public Customer GetCurrent(AuditConfig auditConfig)
		{
			throw new NotImplementedException();
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


		public ConnectionDB Connection
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
   }
}
