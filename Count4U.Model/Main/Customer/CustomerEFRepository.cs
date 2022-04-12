using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.App_Data;
using Count4U.Model.Interface.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Main.MappingEF;
using Count4U.Model.Audit.MappingEF;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Audit;
using System.IO;
using Count4U.Model.Count4U;
using NLog;
using AuditConfig = Count4U.Model.Audit.AuditConfig;
using InventorConfig = Count4U.Model.Count4U.InventorConfig;
using Microsoft.Practices.ServiceLocation;
using Count4U.Localization;

namespace Count4U.Model.Main
{
   public class CustomerEFRepository :  BaseEFRepository, ICustomerRepository
    {
	   //private Customer this._current;
	   //private Customers this._customers;
	   private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
	   private IServiceLocator _serviceLocator;

	   public CustomerEFRepository(IConnectionDB connection, IServiceLocator serviceLocator)
            : base(connection)
        {
			this._serviceLocator = serviceLocator;
        }

	   public IConnectionDB Connection
	   {
		   get { return this._connection; }
		   set { this._connection = value; }
	   }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

        #endregion	
		
		
    	#region ICustomerRepository Members

        public Customers GetCustomers(CBIContext contextCBI)
        {
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
				case CBIContext.Main:
					using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
					{
						var domainObjects = dc.Customer.ToList().Select(e => e.ToDomainObject());
						return Customers.FromEnumerable(domainObjects);
					}
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig.ToList().Select(e => e.ToDomainObject());
						return AuditConfigs.CustomersFromEnumerable(domainObjects, this._serviceLocator);
					}
				default:
					using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
					{
						var domainObjects = dc.Customer.ToList().Select(e => e.ToDomainObject());
						return Customers.FromEnumerable(domainObjects);
					}
			}
        }

		public Customers GetCustomers(SelectParams selectParams)	
		{
			if (selectParams == null)
			{
				return this.GetCustomers(CBIContext.Main);
			}

			long totalCount = 0;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{                               
			    // Получение сущностей и общего количества из БД.
				// Getting entities and total count from database.
				var entities = GetEntities(dc, AsQueryable(dc.Customer), dc.Customer.AsQueryable(), selectParams, out totalCount);

				// Преобразование сущностей в объекты предметной области.
				// Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToDomainObject());

				// Возврат результата.
				// Returning result.
				Customers result = Customers.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public Customer GetCustomer(string code)
		{
			if (string.IsNullOrWhiteSpace(code) == true) return null;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, code);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public List<string> GetCodeList()
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = dc.Customer.Select(e => e.Code).Distinct().ToList();
				return entity;
			}
		}

		public Customers GetCustomersDetails()
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = dc.Customer.ToList().Select(e => e.ToDomainObject());
				return Customers.FromEnumerable(domainObjects);
			}
   		}

		//public void SetCurrent(Customer currentCustomer)
		//{
		//    this._current = currentCustomer;
		//}

		//public Customer GetCurrent()
		//{
		//    return this._current;
		//}

		public void SetCurrent(Customer currentCustomer, AuditConfig auditConfig)
		{
			if (auditConfig == null) return;
			if (currentCustomer == null)
			{
				auditConfig.Clear();
			}
			else
			{
				if (auditConfig.CustomerCode != currentCustomer.Code)
				{
					auditConfig.Clear();
					auditConfig.CustomerCode = currentCustomer.Code;
					auditConfig.CustomerName = currentCustomer.Name;
				}
			}
		}

		public Customer GetCurrent(AuditConfig auditConfig)
		{
			if (auditConfig == null) return null;
			if (string.IsNullOrWhiteSpace(auditConfig.CustomerCode) == true) return null;
			return this.GetCustomerByCode(auditConfig.CustomerCode);
		}

		public string GetCurrentCode(AuditConfig auditConfig)
		{
			if (auditConfig == null) return "";
			if (string.IsNullOrWhiteSpace(auditConfig.CustomerCode) == true) return "";
			return auditConfig.CustomerCode;
		}

		//public Customer Clone(Customer customer)
		//{
		//    if (customer == null) return null;
		//    var domainObject = customer.Clone();
		//    domainObject.ID = 0;
		//    domainObject.Code = "";
		//    return domainObject;
		//}

		public void Delete(Customer customer)
		{
			if (customer == null) return;
			if (string.IsNullOrWhiteSpace(customer.Code) == true) return;
			//if (full == true)
			//{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entity = this.GetEntityByCode(dc, customer.Code);
					if (entity == null) return;
					string folder = PropertiesSettings.FolderCustomer.Trim('\\') + @"\";
					this.Connection.RemoveDB(entity.DBPath, folder, false);
					dc.Customer.DeleteObject(entity);
					dc.SaveChanges();
				}
			//}
		}

		public void Delete(string customerCode)
		{
			if (string.IsNullOrWhiteSpace(customerCode) == true) return;
			//if (full == true)
			//{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, customerCode);
				if (entity == null) return;
				string folder = PropertiesSettings.FolderCustomer.Trim('\\') + @"\";
				this.Connection.RemoveDB(entity.DBPath, folder, false);
				dc.Customer.DeleteObject(entity);
				dc.SaveChanges();
			}
		//}
		}

		public void Delete(List<string> customerCodeList)
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (var customerCode in customerCodeList)
				{
					if (string.IsNullOrWhiteSpace(customerCode) == true) return;

					var entities = dc.Branch.Where(e => e.CustomerCode == customerCode).ToList();
					if (entities != null)
					{
						if (entities.Count == 0) entities = null;
					}

					if (entities == null)
					{
						var entity = this.GetEntityByCode(dc, customerCode);
						if (entity == null) return;
						string folder = PropertiesSettings.FolderCustomer.Trim('\\') + @"\";
						this.Connection.RemoveDB(entity.DBPath, folder, false);
						dc.Customer.DeleteObject(entity);
					}
				}
				dc.SaveChanges();
			}	   // dc
		}


		public void DeleteDomainObjectOnly(List<string> customerCodeList)
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (var customerCode in customerCodeList)
				{
					if (string.IsNullOrWhiteSpace(customerCode) == true) return;
					var entity = this.GetEntityByCode(dc, customerCode);
					if (entity == null) return;
					dc.Customer.DeleteObject(entity);

				}
				dc.SaveChanges();
			}	   // dc
		}

		public void Insert(Customer customer)
		{
			if (customer == null)
			{
				_logger.Trace("Insert", "customer is null");
				return;
			}
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = customer.ToEntity();
				string folder = PropertiesSettings.FolderCustomer.Trim('\\') + @"\";
				string pathDB = this.Connection.CopyEmptyCount4UAndAnaliticDB(customer.DBPath, folder);
				entity.CreateDate = DateTime.Now;
				entity.ModifyDate = DateTime.Now;
				entity.LastUpdatedCatalog = DateTime.Now;
				dc.Customer.AddObject(entity);
				dc.SaveChanges();
			}

			InventorConfigs inventorConfigs = FillInventorProperty(customer);
			string customerDBPath = PropertiesSettings.FolderCustomer.Trim('\\') + @"\" + customer.DBPath;
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(customerDBPath)))
			{
				var list = db.InventorConfigs.ToList();
				if (list.Count > 0)
				{
					list.ForEach(e => db.InventorConfigs.DeleteObject(e));
					db.SaveChanges();
				}

				var entity = customer.ToInventorConfigEntity();
				db.InventorConfigs.AddObject(entity);

				foreach (var inventorConfig in inventorConfigs)
				{
					db.InventorConfigs.AddObject(inventorConfig.ToEntity());
				}
				db.SaveChanges();
			}
		}

		public void InsertDomainСustomerFromInventorConfig(Customers customers)
		{
			if (customers == null) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (Customer customer in customers)
				{
					if (customer == null) { continue; }
					App_Data.Customer entity = GetEntityByCode(dc, customer.Code);
					if (entity == null)
					{
						entity = customer.ToEntity();
						entity.ModifyDate = DateTime.Now;
						if (entity.CreateDate == DateTime.MinValue) 	{ entity.CreateDate = DateTime.Now; }
						entity.LastUpdatedCatalog = DateTime.Now;
						entity.Restore = DateTime.Now.ToString();
						entity.RestoreBit = true;
						dc.Customer.AddObject(entity);
					}
				}
				dc.SaveChanges();
			}
		}


		public string BuildRelativeDbPath(Customer customer)
		{
			return PropertiesSettings.FolderCustomer.Trim('\\') + @"\" + customer.DBPath;
		}

		public void Update(Customer customer)
		{
			if (customer == null) return;
			//customer.CreateDate = DateTime.Now;
			//customer.ModifyDate = DateTime.Now;

			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, customer.Code);
				if (entity == null) return;
				entity.ApplyChanges(customer);
				dc.SaveChanges();
			}

			//string relativePath = BuildRelativeDbPath(customer);
			//this.Connection.CopyEmptyAnaliticDB(relativePath);

			this.RefillInventorConfigs(customer);
		}

	
		// Update Customer в БД MainDB по InventorConfig  из БД Count4U, по заданному пути  pathDB
		public void UpdateDomainСustomerByInventorConfig(Customers customers)
		{
			if (customers == null) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (Customer customer in customers)
				{
					if (customer == null) { continue; }
					//customer.CreateDate = DateTime.Now;
					//customer.ModifyDate = DateTime.Now;
					var entity = this.GetEntityByCode(dc, customer.Code);
					if (entity == null) continue;
					entity.ApplyChanges(customer);
					//this.RefillInventorConfigs(customer);
				}
				dc.SaveChanges();
			}
		}

		public void RefillInventorConfigs(Customer customer)
		{
			if (customer == null) return;
			string customerDBPath = PropertiesSettings.FolderCustomer.Trim('\\') + @"\" + customer.DBPath;
             string fullPath = this.Connection.BuildCount4UDBFilePath(customerDBPath);
			 if (String.IsNullOrWhiteSpace(fullPath) == true || File.Exists(fullPath) == false) return;

			InventorConfigs inventorConfigs = FillInventorProperty(customer);
			
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(customerDBPath)))
			{
				db.InventorConfigs.ToList().ForEach(e => db.InventorConfigs.DeleteObject(e));
				db.SaveChanges();

				var entity = customer.ToInventorConfigEntity();
				db.InventorConfigs.AddObject(entity);

				foreach (var inventorConfig in inventorConfigs)
				{
					db.InventorConfigs.AddObject(inventorConfig.ToEntity());
				}
				db.SaveChanges();
			}


       //     CreateInfoFile(customer, customerDBPath);
		}


        private void CreateInfoFile(Customer customer, string customerDBPath, InventorConfigs inventorConfigs = null)
        {
            string customerFullPath = this.Connection.BuildCount4UDBFolderPath(customerDBPath);

            if (Directory.Exists(customerFullPath) == true)
            {
                string[] files = Directory.GetFiles(customerFullPath, "*.c4u", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    string path = Path.GetFullPath(files[i]);
                    string fileNameExtension = Path.GetExtension(files[i]);
                    if (fileNameExtension == ".c4u")
                    {
                        try
                        {
                            File.Delete(path);
                        }
                        catch { }
                    }
                }

                //DDMMYYYY_[CustomerCode]_[BranchCodeERP].
                string newFileInfo = DateTime.Now.ToString("ddMMyyyy") + "_Customer_" + customer.Code + ".c4u";
                string newFileInfoPath = customerFullPath + newFileInfo;
                if (File.Exists(newFileInfoPath) == false)
                {
                    Encoding writerEncoding = Encoding.GetEncoding("windows-1255");
                    try
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            StreamWriter sw = new StreamWriter(ms, writerEncoding);
                            sw.WriteLine(newFileInfoPath);
                            if (inventorConfigs != null)
                            {
                                foreach (InventorConfig inventorConfig in inventorConfigs)
                                {

                                    string igInfo = inventorConfig.TypeObject + "=" + inventorConfig.Description;
                                    sw.WriteLine(igInfo);

                                }
                            }
                            sw.Flush();
                            File.WriteAllText(newFileInfoPath, writerEncoding.GetString(ms.ToArray()));

                        }
                    }
                    catch { }

                }
            }

        }

		public void RefillInventorConfigsAllCustomersInMainDB()
		{
			Customers customers = this.GetCustomers(CBIContext.Main);
			foreach (var customer in customers)
			{
				this.RefillInventorConfigs(customer);
			}
		}

        public void SetCustomers(Customers customers)
        {
            throw new NotImplementedException();
        }

   		public Customer GetCustomerByCode(string customerCode)
		{
			if (string.IsNullOrEmpty(customerCode) == true) return null;

			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, customerCode);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		//public string GetMaxCode()
		//{
		//    using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString))
		//    {
		//        if (dc.Customer.Count() == 0) return "";
		//        return dc.Customer.Max(e => e.Code);
		//    }
		//}


		public Dictionary<string, Customer> FillCustomerDictionary()
		{
			Dictionary<string, Customer> customerDictionary = new Dictionary<string, Customer>();

			Customers customers = this.GetCustomers(CBIContext.Main);

			foreach (Customer customer in customers)
			{
				customerDictionary[customer.Code] = customer;
			}
			return customerDictionary;
		}

		#endregion
		#region private

		private InventorConfigs FillInventorProperty(Customer customer)
		{
			InventorConfigs inventorConfigs = new InventorConfigs();
            if (customer != null)
            {
                inventorConfigs.Add(new InventorConfig { TypeObject = "DomainType", Description = DomainTypeEnum.Customer.ToString(), InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Code", Description = customer.Code, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "CustomerCode", Description = customer.Code, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Name", Description = customer.Name, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Phone", Description = customer.Phone, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "DBPath", Description = customer.DBPath, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Description", Description = customer.Description, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Address", Description = customer.Address, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ContactPerson", Description = customer.ContactPerson, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportCatalogAdapterCode", Description = customer.ExportCatalogAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportIturAdapterCode", Description = customer.ExportIturAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Mail", Description = customer.Mail, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Fax", Description = customer.Fax, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportCatalogAdapterParms", Description = customer.ImportCatalogAdapterParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportCatalogProviderCode", Description = customer.ImportCatalogProviderCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportIturAdapterParms", Description = customer.ImportIturAdapterParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportIturProviderCode", Description = customer.ImportIturProviderCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportLocationAdapterParms", Description = customer.ImportLocationAdapterParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportLocationProviderCode", Description = customer.ImportLocationProviderCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportPDAAdapterParms", Description = customer.ImportPDAAdapterParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportPDAProviderCode", Description = customer.ImportPDAProviderCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "LogoPath", Description = customer.LogoPath, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportSectionAdapterCode", Description = customer.ImportSectionAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportSectionAdapterCode", Description = customer.ExportSectionAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "UpdateCatalogAdapterCode", Description = customer.UpdateCatalogAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportERPAdapterCode", Description = customer.ExportERPAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportSupplierAdapterCode", Description = customer.ImportSupplierAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportBranchAdapterCode", Description = customer.ImportBranchAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportBranchAdapterCode", Description = customer.ExportBranchAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
            }

            try
            {
                string customerDBPath = PropertiesSettings.FolderCustomer.Trim('\\') + @"\" + customer.DBPath;
                CreateInfoFile(customer, customerDBPath, inventorConfigs);
            }
            catch { }

			return inventorConfigs;
		}

		private App_Data.Customer GetEntityByCode(App_Data.MainDB dc, string code)
		{
			var entity = dc.Customer.FirstOrDefault(e => e.Code.CompareTo(code) == 0);
			return entity;
		}
		#endregion

	

	}
}
