using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface;
using Count4U.Model.Main;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Localization;

namespace Count4U.Model.Count4U
{
	public class InventorConfigEFRepository : BaseEFRepository, IInventorConfigRepository
    {
		public readonly IAuditConfigRepository _auditConfigRepository;
		public readonly IInventorRepository _inventorRepository;
		public readonly IBranchRepository _branchRepository;
		public readonly ICustomerRepository _customerRepository;
		public readonly IContextCBIRepository _contextCBIRepository;
		public readonly ILog _log;
		private IServiceLocator _serviceLocator;
 	
		public InventorConfigEFRepository(IConnectionDB connection,
				IServiceLocator serviceLocator,
				//IAuditConfigRepository auditConfigRepository,
				IInventorRepository inventorRepository ,
				IBranchRepository branchRepository,
				ICustomerRepository customerRepository,
				IContextCBIRepository contextCBIRepository,
				ILog log
            ) : base(connection)
        {
			this._serviceLocator = serviceLocator; 
			this._auditConfigRepository = this._serviceLocator.GetInstance<IAuditConfigRepository>("HistoryCBIConfig");
			this._inventorRepository = inventorRepository;
			this._branchRepository = branchRepository;
			this._customerRepository = customerRepository;
			this._contextCBIRepository = contextCBIRepository;
			this._log = log;
        }

		#region BaseEFRepository Members
		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}
		#endregion
	
		

		public AuditConfigs GetAuditConfigsFromInventorConfigs(string pathDB)
        {
			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = dc.InventorConfigs.ToList().Select(e => e.ToDomainObject());
				return AuditConfigs.InventorConfigs2AuditConfigs(domainObjects);
			}
        }

		public void ClearInventorConfigs(string pathDB)
		{
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				db.InventorConfigs.ToList().ForEach(e => db.InventorConfigs.DeleteObject(e));
				db.SaveChanges();
			}
		}

		public InventorConfig GetCurrentConfig(string pathDB)
		{
			return this.GetInventorConfig(pathDB);
		}

		public void SetCurrentConfig(InventorConfig inventorConfig, string pathDB)
		{
			this.Insert(inventorConfig, pathDB);
		}

		public void SetCurrentConfig(AuditConfig auditConfig, string pathDB)
		{
			this.Insert(auditConfig, pathDB);
		}
	
		//public void Delete(string inventorConfigCode, string pathDB)
		//{
		//    using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
		//    {
		//        var entity = this.GetEntityByCode(db, inventorConfigCode);
		//        db.InventorConfigs.DeleteObject(entity);
		//        db.SaveChanges();
		//    }
		//}

		public void Insert(InventorConfig inventorConfig, string pathDB)
		{
			if (inventorConfig == null) return;
			this.ClearInventorConfigs(pathDB);
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = inventorConfig.ToEntity();
				db.InventorConfigs.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(AuditConfig auditConfig, string pathDB)
		{
			InventorConfig inventorConfig = auditConfig.ToInventorConfig();
			if (inventorConfig != null)
			{
				this.Insert(inventorConfig, pathDB);
			}
		}

		public void Update(InventorConfig inventorConfig, string pathDB)
		{
			if (inventorConfig == null) return;
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, inventorConfig.Code);
				if (entity == null) return;
				entity.ApplyChanges(inventorConfig);
				db.SaveChanges();
			}
		}

		//??
		public void SetInventorStatus(InventorConfig inventorConfig, string statusInventorConfigCode, string pathDB)
		{
			inventorConfig.StatusInventorConfigCode = statusInventorConfigCode;
			this.Update(inventorConfig, pathDB);
		}

		//??
		public string GetInventorStatusCode(string pathDB)
		{
			InventorConfig inventorConfig = this.GetInventorConfig(pathDB);
			if (inventorConfig == null) return "";
			else return inventorConfig.StatusInventorConfigCode;
		}


		// восстановление домайн объектов Customer в БД MainDB, 
		// по файловой системе (файлов Count4UDB.sdf в папке Customer)
		// если есть домайн объект в БД - он не заменяется
		public void RepairDomainCustomer()
		{
			//base.ContextCBIRepository.RefillAllCBIInventorConfigs();
			List<string> count4UDBPathList = new List<string>();
					
			// надо достать релейтив DBPath а потом реботать через репозиторий
			Customers customers = new Customers();
			string folderCustomer = this._connection.DBSettings.FolderCustomer.Trim('\\');
			//count4UDBPathList = contextCBIRepository.FindCount4UDBPathDB(count4UDBPathList, dbSettings.FolderCustomer.Trim('\\'));
			count4UDBPathList = this._contextCBIRepository.FindCount4UDBPathDB(count4UDBPathList, folderCustomer);
			foreach (string pathDB in count4UDBPathList)
			{
				if (pathDB.Contains("removed") == true) continue;
				Customer customer = this.CustomerFromInventorConfigs(pathDB);
				if (customer != null) customers.Add(customer);
			}

			this._customerRepository.InsertDomainСustomerFromInventorConfig(customers);
		}

		// восстановление (добавление или изменение) домайн объектов Customer в БД MainDB, 
		// по файловой системе (файлов Count4UDB.sdf в папке Customer)
		// если есть домайн объект в БД - он не заменяется
		public void RepairDomainCustomer(string pathDB, bool updateByInventorConfig = false)
		{
			if (pathDB.Contains("removed") == true) return;
			// надо достать релейтив DBPath а потом реботать через репозиторий

			Customers customers = new Customers();
			string folderCustomer = this._connection.DBSettings.FolderCustomer.Trim('\\');
			string count4UDBPath = this._contextCBIRepository.FindCount4UDBPathDB(folderCustomer); //путь до папки 	\\App_Data\\Customer
			if (string.IsNullOrWhiteSpace(count4UDBPath) == true) return;

			Customer customer = this.CustomerFromInventorConfigs(pathDB);
			if (customer != null) customers.Add(customer);
			if (updateByInventorConfig == false)
			{
				//добавление домайн объектов Customer в БД MainDB
				this._customerRepository.InsertDomainСustomerFromInventorConfig(customers);
			}
			else
			{
				// Update Customer в БД MainDB по InventorConfig взятому из БД Count4U, по заданному пути  pathDB
				this._customerRepository.UpdateDomainСustomerByInventorConfig(customers);
			}
		}

		// восстановление домайн объектов Branch в БД MainDB, 
		// по файловой системе (файлов Count4UDB.sdf в папке Branch)
		// если есть домайн объект в БД - он не заменяется
		// если нет Customer (в MainDB) с CustomerCode - не добавляем Branch
        public void RepairDomainBranch()
        {
			Dictionary<string, Customer>  customerDictionary = this._customerRepository.FillCustomerDictionary();

			List<string> count4UDBPathList = new List<string>();
				// надо достать релейтив DBPath а потом реботать через репозиторий

			Branches branchs = new Branches();
			string folderBranch = this._connection.DBSettings.FolderBranch.Trim('\\');
			count4UDBPathList = this._contextCBIRepository.FindCount4UDBPathDB(count4UDBPathList, folderBranch);
			foreach (string pathDB in count4UDBPathList)
			{
				if (pathDB.Contains("removed") == true) continue;
				Branch branch = this.BranchFromInventorConfigs(pathDB);
				if (branch != null)
				{
					if (customerDictionary.ContainsKey(branch.CustomerCode) == true)
					{
						branchs.Add(branch);
					}
				}
			}
			//добавляем домайн объекты Branch в БД,  если объекта с таким  branch.Code нет в БД MainDB  
			this._branchRepository.InsertDomainBranchFromInventorConfig(branchs);
		}

		
		/// <summary>
		/// Создает или обновляет  домайн объект Branch в БД MainDB по БД Count4UDB
		/// </summary>
		/// <param name="pathDB">путь до БД Count4UDB</param>
		/// <param name="updateByInventorConfig"></param>
		public void RepairDomainBranch(string pathDB, bool updateByInventorConfig = false)
		{
			if (pathDB.Contains("removed") == true) return;
			// надо достать релейтив DBPath а потом реботать через репозиторий

			Branches branches = new Branches();
			string folderBranch = this._connection.DBSettings.FolderBranch.Trim('\\');
			string count4UDBPath = this._contextCBIRepository.FindCount4UDBPathDB(folderBranch);
			if (string.IsNullOrWhiteSpace(count4UDBPath) == true) return;

			Branch branch = this.BranchFromInventorConfigs(pathDB);
			if (branch != null) branches.Add(branch);
			if (updateByInventorConfig == false)
			{
				//добавление домайн объектов Branch в БД MainDB
				this._branchRepository.InsertDomainBranchFromInventorConfig(branches);
			}
			else
			{
				// Update Branch в БД MainDB по InventorConfig взятому из БД Count4U, по заданному пути  pathDB
				this._branchRepository.UpdateDomainBranchByInventorConfig(branches);
			}
		}

		// восстановление домайн объектов inventor, auditConfig в БД AuditDB, 
		// по файловой системе (файлов Count4UDB.sdf в папке Inventor)
		// если есть домайн объект в БД - он не заменяется
		// если нет Customer или нет Branch (в MainDB) с CustomerCode & BranchCode - не добавляем ни Inventor, ни AditConfig
		public void RepairDomainInventor()
        {
			Dictionary<string, Customer>  customerDictionary = this._customerRepository.FillCustomerDictionary();
			Dictionary<string, Branch>  branchDictionary = this._branchRepository.FillBranchDictionary();
			List<string> count4UDBPathList = new List<string>();
			// надо достать релейтив DBPath а потом реботать через репозиторий

			Inventors inventors = new Inventors();
			AuditConfigs auditConfigs = new AuditConfigs();
			//список всех путей до count4UDB - Inventor
			string folderInventor = this._connection.DBSettings.FolderInventor.Trim('\\');
			count4UDBPathList = this._contextCBIRepository.FindCount4UDBPathDB(count4UDBPathList, folderInventor);
			foreach (string pathDB in count4UDBPathList)
			{
				if (pathDB.Contains("removed") == true) continue;
				Inventor inventor = this.InventorFromInventorConfigs(pathDB);
				AuditConfig auditConfig = this.AuditConfigFromInventorConfigs(pathDB);
				if (customerDictionary.ContainsKey(inventor.CustomerCode) == true
					&& branchDictionary.ContainsKey(inventor.BranchCode) == true)
				{
					if (inventor != null && auditConfig != null)
					{
						inventors.Add(inventor);
						auditConfigs.Add(auditConfig);
					}
				}
			}

			//добавляем домайн объекты Inventor в БД Аudit, если объекта с таким Inventor.Code нет в БД
			this._inventorRepository.InsertDomainInventorFromInventorConfig(inventors);
			//добавляем домайн объекты AuditConfig в БД,  если объекта с таким auditConfig.InventorCode нет в БД Аudit  
			this._auditConfigRepository.InsertDomainAuditConfigFromInventorConfig(auditConfigs);
        
		}

		/// <summary>
		/// Создает или обновляет  домайн объект Inventor в БД AuditDB по БД Count4UDB
		/// </summary>
		/// <param name="pathDB">путь до БД Count4UDB</param>
		/// <param name="updateByInventorConfig"></param>
		public void RepairDomainInventor(string pathDB, bool updateByInventorConfig = false)
		{
			if (pathDB.Contains("removed") == true) return;
			// надо достать релейтив DBPath а потом реботать через репозиторий

			Inventors inventors = new Inventors();
			AuditConfigs auditConfigs = new AuditConfigs();

			string folderInventor = this._connection.DBSettings.FolderInventor.Trim('\\');
			string count4UDBPath = this._contextCBIRepository.FindCount4UDBPathDB(folderInventor);
			if (string.IsNullOrWhiteSpace(count4UDBPath) == true) return;

			Inventor inventor = this.InventorFromInventorConfigs(pathDB);
			if (inventor == null) return;
			inventors.Add(inventor);

			if (updateByInventorConfig == false)
			{
				//добавляем домайн объекты Inventor в БД Аudit, если объекта с таким Inventor.Code нет в БД
				this._inventorRepository.InsertDomainInventorFromInventorConfig(inventors);

				AuditConfig auditConfig = this.AuditConfigFromInventorConfigs(pathDB);
				if (auditConfig != null)
				{
					auditConfigs.Add(auditConfig);
					//добавляем домайн объекты AuditConfig в БД,  если объекта с таким auditConfig.InventorCode нет в БД Аudit  
					this._auditConfigRepository.InsertDomainAuditConfigFromInventorConfig(auditConfigs);
				}
			}
			else
			{
				// Update Inventor, AuditConfig в БД AuditDB по InventorConfig взятому из БД Count4U, 
				//по заданному пути  pathDB
				this._inventorRepository.UpdateDomainInventorByInventorConfig(inventors);
				//this._auditConfigRepository.UpdateDomainAuditConfigByInventorConfig(auditConfigs);
			}
		}

		public void TestRestoreDomainObject_Inventor()
		{
			List<string> count4UDBPathList = new List<string>();
		
			//список всех путей до count4UDB - Inventor
			string folderInventor = this._connection.DBSettings.FolderInventor.Trim('\\');
			count4UDBPathList = this._contextCBIRepository.FindCount4UDBPathDB(count4UDBPathList, folderInventor);
			foreach (string pathDB in count4UDBPathList)
			{
				if (pathDB.Contains("removed") == true) continue;

				this.RestoreDomainObject(pathDB);
			}

		}

		//public void RepairDomainAuditConfig()
		//{
		//    throw new NotImplementedException();
		//}

		//public void RepairDomainAuditConfig(string pathDB, bool updateByInventorConfig = false)
		//{
		//    throw new NotImplementedException();
		//}



		public Branch BranchFromInventorConfigs(string relativePathDB, bool addParentObject = false)
		{
			Branch branch = null;

			branch = new Branch();
			string codeDomainObject = "";
			string subFolderDomainObject = PropertiesSettings.FolderBranch;
			string retPathDB = GetPathDBFromRelativePathDB(relativePathDB, subFolderDomainObject, out codeDomainObject);
			//int len = PropertiesSettings.FolderBranch.Length;
			//string pathDB = PathDB; 
			//pathDB = pathDB.Trim('\\');
			//branch.Code = pathDB.Substring(len).Trim('\\');
			//branch.DBPath = branch.Code;

			branch.Code = codeDomainObject;
			branch.DBPath = retPathDB;
			branch.CustomerCode = "unknown";
			branch.Name = "unknownRestore";
			branch.ModifyDate = DateTime.Now;
			branch.CreateDate = DateTime.Now;
			branch.LastUpdatedCatalog = DateTime.Now;
			branch.Restore = "Restore";
			branch.RestoreBit = true;

			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(relativePathDB)))
			{

				var inventorConfigList = dc.InventorConfigs.Select(e => e);
				Dictionary<string, App_Data.InventorConfig> inventorConfigs = new Dictionary<string, App_Data.InventorConfig>();
				foreach (var inventorConfig in inventorConfigList)
				{
					inventorConfigs[inventorConfig.TypeObject] = inventorConfig;
				}

				if (inventorConfigs.ContainsKey("DomainType") == true)
				{
					if (inventorConfigs["DomainType"].Description == DomainTypeEnum.Branch.ToString())
					{
						if (inventorConfigs.ContainsKey("Code") == true) branch.Restore = branch.Restore + " Code old: " + inventorConfigs["Code"].Description;
						if (inventorConfigs.ContainsKey("Name") == true) branch.Name = inventorConfigs["Name"].Description; 
						if (inventorConfigs.ContainsKey("CustomerCode") == true) branch.CustomerCode = inventorConfigs["CustomerCode"].Description;
						if (inventorConfigs.ContainsKey("Phone") == true) branch.Phone = inventorConfigs["Phone"].Description; else branch.Phone = "";
						if (inventorConfigs.ContainsKey("DBPath") == true) branch.Restore = branch.Restore + " DBPath old: " + inventorConfigs["DBPath"].Description;
						if (inventorConfigs.ContainsKey("Description") == true) branch.Description = inventorConfigs["Description"].Description; else branch.Description = "";
						if (inventorConfigs.ContainsKey("Address") == true) branch.Address = inventorConfigs["Address"].Description; else branch.Address = "";
						if (inventorConfigs.ContainsKey("BranchCodeERP") == true) branch.BranchCodeERP = inventorConfigs["BranchCodeERP"].Description; else branch.BranchCodeERP = "";
						if (inventorConfigs.ContainsKey("BranchCodeLocal") == true) branch.BranchCodeLocal = inventorConfigs["BranchCodeLocal"].Description; else branch.BranchCodeLocal = "";
						if (inventorConfigs.ContainsKey("ContactPerson") == true) branch.ContactPerson = inventorConfigs["ContactPerson"].Description; else branch.ContactPerson = "";
						if (inventorConfigs.ContainsKey("ExportCatalogAdapterCode") == true) branch.ExportCatalogAdapterCode = inventorConfigs["ExportCatalogAdapterCode"].Description; else branch.ExportCatalogAdapterCode = "";
						if (inventorConfigs.ContainsKey("ExportIturAdapterCode") == true) branch.ExportIturAdapterCode = inventorConfigs["ExportIturAdapterCode"].Description; else branch.ExportIturAdapterCode = "";
						if (inventorConfigs.ContainsKey("Mail") == true) branch.Mail = inventorConfigs["Mail"].Description; else branch.Mail = "";
						if (inventorConfigs.ContainsKey("Fax") == true) branch.Fax = inventorConfigs["Fax"].Description; else branch.Fax = "";
						if (inventorConfigs.ContainsKey("ImportCatalogAdapterParms") == true) branch.ImportCatalogAdapterParms = inventorConfigs["ImportCatalogAdapterParms"].Description; else branch.ImportCatalogAdapterParms = "";
						if (inventorConfigs.ContainsKey("ImportCatalogProviderCode") == true) branch.ImportCatalogProviderCode = inventorConfigs["ImportCatalogProviderCode"].Description; else branch.ImportCatalogProviderCode = "";
						if (inventorConfigs.ContainsKey("ImportIturAdapterParms") == true) branch.ImportIturAdapterParms = inventorConfigs["ImportIturAdapterParms"].Description; else branch.ImportIturAdapterParms = "";
						if (inventorConfigs.ContainsKey("ImportIturProviderCode") == true) branch.ImportIturProviderCode = inventorConfigs["ImportIturProviderCode"].Description; else branch.ImportIturProviderCode = "";
						if (inventorConfigs.ContainsKey("ImportLocationAdapterParms") == true) branch.ImportLocationAdapterParms = inventorConfigs["ImportLocationAdapterParms"].Description; else branch.ImportLocationAdapterParms = "";
						if (inventorConfigs.ContainsKey("ImportLocationProviderCode") == true) branch.ImportLocationProviderCode = inventorConfigs["ImportLocationProviderCode"].Description; else branch.ImportLocationProviderCode = "";
						if (inventorConfigs.ContainsKey("ImportPDAAdapterParms") == true) branch.ImportPDAAdapterParms = inventorConfigs["ImportPDAAdapterParms"].Description; else branch.ImportPDAAdapterParms = "";
						if (inventorConfigs.ContainsKey("ImportPDAProviderCode") == true) branch.ImportPDAProviderCode = inventorConfigs["ImportPDAProviderCode"].Description; else branch.ImportPDAProviderCode = "";
						if (inventorConfigs.ContainsKey("LogoFile") == true) branch.LogoFile = inventorConfigs["LogoFile"].Description; else branch.LogoFile = "";
						if (inventorConfigs.ContainsKey("ImportSectionAdapterCode") == true) branch.ImportSectionAdapterCode = inventorConfigs["ImportSectionAdapterCode"].Description; else branch.ImportSectionAdapterCode = "";
						if (inventorConfigs.ContainsKey("ExportSectionAdapterCode") == true) branch.ExportSectionAdapterCode = inventorConfigs["ExportSectionAdapterCode"].Description; else branch.ExportSectionAdapterCode = "";
						if (inventorConfigs.ContainsKey("UpdateCatalogAdapterCode") == true) branch.UpdateCatalogAdapterCode = inventorConfigs["UpdateCatalogAdapterCode"].Description; else branch.UpdateCatalogAdapterCode = "";
						if (inventorConfigs.ContainsKey("ExportERPAdapterCode") == true) branch.ExportERPAdapterCode = inventorConfigs["ExportERPAdapterCode"].Description; else branch.ExportERPAdapterCode = "";
						if (inventorConfigs.ContainsKey("ImportSupplierAdapterCode") == true) branch.ImportSupplierAdapterCode = inventorConfigs["ImportSupplierAdapterCode"].Description; else branch.ImportSupplierAdapterCode = "";
							
						
						if (String.IsNullOrWhiteSpace(branch.CustomerCode) == false)
						{
							Customer customer = this._customerRepository.GetCustomerByCode(branch.CustomerCode);
							if (customer == null)
							{
								if (addParentObject == true)
								{
									this.AddNewCustomer(branch);
								}
							}
						}
					}
				}
			}
			branch.Description = branch.Description;// + branch.Restore;
			return branch;
		}

		public Customer CustomerFromInventorConfigs(string relativePathDB)
		{
			Customer customer = null;

			string codeDomainObject = "";
			customer = new Customer();
			string subFolderDomainObject = PropertiesSettings.FolderCustomer;
			string retPathDB = GetPathDBFromRelativePathDB(relativePathDB, subFolderDomainObject, out codeDomainObject);

			//int len = subFolder.Length;
			//string pathDB = PathDB;
			//pathDB = pathDB.Trim('\\');
			//customer.Code = pathDB.Substring(len).Trim('\\');
			//customer.DBPath = customer.Code;

			customer.Code = codeDomainObject;
			customer.DBPath = retPathDB;
			customer.ModifyDate = DateTime.Now;
			customer.CreateDate = DateTime.Now;
			customer.LastUpdatedCatalog = DateTime.Now;
			customer.Name = "unknownRestore";

			customer.Restore = "Restore";
			customer.RestoreBit = true;

			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(relativePathDB)))
			{
				var inventorConfigList = dc.InventorConfigs.Select(e => e);
				Dictionary<string, App_Data.InventorConfig> inventorConfigs = new Dictionary<string, App_Data.InventorConfig>();
				foreach (var inventorConfig in inventorConfigList)
				{
					inventorConfigs[inventorConfig.TypeObject] = inventorConfig;
				}

				if (inventorConfigs.ContainsKey("DomainType") == true)
				{
					if (inventorConfigs["DomainType"].Description == DomainTypeEnum.Customer.ToString())
					{
						if (inventorConfigs.ContainsKey("Code") == true) customer.Restore = customer.Restore + " Code old: " + inventorConfigs["Code"].Description;
						if (inventorConfigs.ContainsKey("Name") == true) customer.Name = inventorConfigs["Name"].Description; 
						if (inventorConfigs.ContainsKey("Phone") == true) customer.Phone = inventorConfigs["Phone"].Description; else customer.Phone = "";
						if (inventorConfigs.ContainsKey("DBPath") == true) customer.Restore = customer.Restore + " DBPath old: " + inventorConfigs["DBPath"].Description;
						if (inventorConfigs.ContainsKey("Description") == true) customer.Description = inventorConfigs["Description"].Description; else customer.Description = "";
						if (inventorConfigs.ContainsKey("Address") == true) customer.Address = inventorConfigs["Address"].Description; else customer.Address = "";
						if (inventorConfigs.ContainsKey("ContactPerson") == true) customer.ContactPerson = inventorConfigs["ContactPerson"].Description; else customer.ContactPerson = "";
						if (inventorConfigs.ContainsKey("ExportCatalogAdapterCode") == true) customer.ExportCatalogAdapterCode = inventorConfigs["ExportCatalogAdapterCode"].Description; else customer.ExportCatalogAdapterCode = "";
						if (inventorConfigs.ContainsKey("ExportIturAdapterCode") == true) customer.ExportIturAdapterCode = inventorConfigs["ExportIturAdapterCode"].Description; else customer.ExportIturAdapterCode = "";
						if (inventorConfigs.ContainsKey("Mail") == true) customer.Mail = inventorConfigs["Mail"].Description; else customer.Mail = "";
						if (inventorConfigs.ContainsKey("Fax") == true) customer.Fax = inventorConfigs["Fax"].Description; else customer.Fax = "";
						if (inventorConfigs.ContainsKey("ImportCatalogAdapterParms") == true) customer.ImportCatalogAdapterParms = inventorConfigs["ImportCatalogAdapterParms"].Description; else customer.ImportCatalogAdapterParms = "";
						if (inventorConfigs.ContainsKey("ImportCatalogProviderCode") == true) customer.ImportCatalogProviderCode = inventorConfigs["ImportCatalogProviderCode"].Description; else customer.ImportCatalogProviderCode = "";
						if (inventorConfigs.ContainsKey("ImportIturAdapterParms") == true) customer.ImportIturAdapterParms = inventorConfigs["ImportIturAdapterParms"].Description; else customer.ImportIturAdapterParms = "";
						if (inventorConfigs.ContainsKey("ImportIturProviderCode") == true) customer.ImportIturProviderCode = inventorConfigs["ImportIturProviderCode"].Description; else customer.ImportIturProviderCode = "";
						if (inventorConfigs.ContainsKey("ImportLocationAdapterParms") == true) customer.ImportLocationAdapterParms = inventorConfigs["ImportLocationAdapterParms"].Description; else customer.ImportLocationAdapterParms = "";
						if (inventorConfigs.ContainsKey("ImportLocationProviderCode") == true) customer.ImportLocationProviderCode = inventorConfigs["ImportLocationProviderCode"].Description; else customer.ImportLocationProviderCode = "";
						if (inventorConfigs.ContainsKey("ImportPDAAdapterParms") == true) customer.ImportPDAAdapterParms = inventorConfigs["ImportPDAAdapterParms"].Description; else customer.ImportPDAAdapterParms = "";
						if (inventorConfigs.ContainsKey("ImportPDAProviderCode") == true) customer.ImportPDAProviderCode = inventorConfigs["ImportPDAProviderCode"].Description; else customer.ImportPDAProviderCode = "";
						if (inventorConfigs.ContainsKey("LogoPath") == true) customer.LogoPath = inventorConfigs["LogoPath"].Description; else customer.LogoPath = "";
						if (inventorConfigs.ContainsKey("ExportCatalogAdapterCode") == true) customer.ExportCatalogAdapterCode = inventorConfigs["ExportCatalogAdapterCode"].Description; else customer.ExportCatalogAdapterCode = "";
						if (inventorConfigs.ContainsKey("ExportIturAdapterCode") == true) customer.ExportIturAdapterCode = inventorConfigs["ExportIturAdapterCode"].Description; else customer.ExportIturAdapterCode = "";
						if (inventorConfigs.ContainsKey("ImportSectionAdapterCode") == true) customer.ImportSectionAdapterCode = inventorConfigs["ImportSectionAdapterCode"].Description; else customer.ImportSectionAdapterCode = "";
						if (inventorConfigs.ContainsKey("ExportSectionAdapterCode") == true) customer.ExportSectionAdapterCode = inventorConfigs["ExportSectionAdapterCode"].Description; else customer.ExportSectionAdapterCode = "";
						if (inventorConfigs.ContainsKey("UpdateCatalogAdapterCode") == true) customer.UpdateCatalogAdapterCode = inventorConfigs["UpdateCatalogAdapterCode"].Description; else customer.UpdateCatalogAdapterCode = "";
						if (inventorConfigs.ContainsKey("ExportERPAdapterCode") == true) customer.ExportERPAdapterCode = inventorConfigs["ExportERPAdapterCode"].Description; else customer.ExportERPAdapterCode = "";
						if (inventorConfigs.ContainsKey("ImportSupplierAdapterCode") == true) customer.ImportSupplierAdapterCode = inventorConfigs["ImportSupplierAdapterCode"].Description; else customer.ImportSupplierAdapterCode = "";
						if (inventorConfigs.ContainsKey("ImportBranchAdapterCode") == true) customer.ImportBranchAdapterCode = inventorConfigs["ImportBranchAdapterCode"].Description; else customer.ImportBranchAdapterCode = "";
						if (inventorConfigs.ContainsKey("ExportBranchAdapterCode") == true) customer.ExportBranchAdapterCode = inventorConfigs["ExportBranchAdapterCode"].Description; else customer.ExportBranchAdapterCode = "";
					}
				}
			}
			customer.Description = customer.Description;// + customer.Restore;
			return customer;
		}

		public Inventor InventorFromInventorConfigs(string relativePathDB, bool addParentObject = false)
		{
			Inventor inventor = null;
			inventor = new Inventor();
			inventor.InventorDate = DateTime.Now;
			inventor.CustomerCode = "unknown";
			inventor.BranchCode = "unknown";
			inventor.Name = "unknownRestore";

			string subFolderDomainObject = PropertiesSettings.FolderInventor;
			//string relativePathDB = PathDB;
			string codeDomainObject = "";
			string retPathDB = this.GetPathDBFromRelativePathDB(relativePathDB, subFolderDomainObject, out codeDomainObject);

			inventor.DBPath = retPathDB;
			inventor.Code = codeDomainObject;
			inventor.CreateDate = DateTime.Now;
			inventor.CompleteDate = DateTime.Now;
			inventor.LastUpdatedCatalog = DateTime.Now;
			inventor.Restore = "Restore";
			inventor.RestoreBit = true;

			Dictionary<string, App_Data.InventorConfig> inventorConfigs = new Dictionary<string, App_Data.InventorConfig>();
			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(relativePathDB)))
			{
				var inventorConfigList = dc.InventorConfigs.Select(e => e);

				foreach (var inventorConfig in inventorConfigList)
				{
					inventorConfigs[inventorConfig.TypeObject] = inventorConfig;
				}
			}

			if (inventorConfigs.ContainsKey("DomainType") == true)
			{
				if (inventorConfigs["DomainType"].Description == DomainTypeEnum.Inventor.ToString())
				{
					if (inventorConfigs.ContainsKey("Code") == true) inventor.Restore = inventor.Restore + " Code old: " + inventorConfigs["Code"].Description;
					if (inventorConfigs.ContainsKey("Name") == true) inventor.Name = inventorConfigs["Name"].Description;
					if (inventorConfigs.ContainsKey("InventorDate") == true)
					{
						DateTime inventorDate = new DateTime();
						bool ret = DateTime.TryParse(inventorConfigs["InventorDate"].Description, out inventorDate);
						if (ret == true) { inventor.InventorDate = inventorDate; }
					}
					if (inventorConfigs.ContainsKey("CompleteDate") == true)
					{
						DateTime completeDate = new DateTime();
						bool ret = DateTime.TryParse(inventorConfigs["CompleteDate"].Description, out completeDate);
						if (ret == true) inventor.CompleteDate = completeDate;
					}
					if (inventorConfigs.ContainsKey("CustomerCode") == true) inventor.CustomerCode = inventorConfigs["CustomerCode"].Description;
					if (inventorConfigs.ContainsKey("BranchCode") == true) inventor.BranchCode = inventorConfigs["BranchCode"].Description;
					if (inventorConfigs.ContainsKey("DBPath") == true) inventor.Restore = inventor.Restore + " DBPath old: " + inventorConfigs["DBPath"].Description;
					if (inventorConfigs.ContainsKey("Description") == true) inventor.Description = inventorConfigs["Description"].Description; else inventor.Description = "";
					if (inventorConfigs.ContainsKey("ImportCatalogAdapterCode") == true) inventor.ImportCatalogAdapterCode = inventorConfigs["ImportCatalogAdapterCode"].Description; else inventor.ImportCatalogAdapterCode = "";
					if (inventorConfigs.ContainsKey("ImportCatalogParms") == true) inventor.ImportCatalogParms = inventorConfigs["ImportCatalogParms"].Description; else inventor.ImportCatalogParms = "";
					if (inventorConfigs.ContainsKey("ImportIturAdapterCode") == true) inventor.ImportIturAdapterCode = inventorConfigs["ImportIturAdapterCode"].Description; else inventor.ImportIturAdapterCode = "";
					if (inventorConfigs.ContainsKey("ImportIturParms") == true) inventor.ImportIturParms = inventorConfigs["ImportIturParms"].Description; else inventor.ImportIturParms = "";
					if (inventorConfigs.ContainsKey("ImportLocationAdapterCode") == true) inventor.ImportLocationAdapterCode = inventorConfigs["ImportLocationAdapterCode"].Description; else inventor.ImportLocationAdapterCode = "";
					if (inventorConfigs.ContainsKey("ImportLocationParms") == true) inventor.ImportLocationParms = inventorConfigs["ImportLocationParms"].Description; else inventor.ImportLocationParms = "";
					if (inventorConfigs.ContainsKey("StatusCode") == true) inventor.StatusCode = inventorConfigs["StatusCode"].Description; else inventor.StatusCode = "";
					if (inventorConfigs.ContainsKey("ImportSectionAdapterCode") == true) inventor.ImportSectionAdapterCode = inventorConfigs["ImportSectionAdapterCode"].Description; else inventor.ImportSectionAdapterCode = "";
					if (inventorConfigs.ContainsKey("UpdateCatalogAdapterCode") == true) inventor.UpdateCatalogAdapterCode = inventorConfigs["UpdateCatalogAdapterCode"].Description; else inventor.UpdateCatalogAdapterCode = "";
					if (inventorConfigs.ContainsKey("ImportPDAProviderCode") == true) inventor.ImportPDAProviderCode = inventorConfigs["ImportPDAProviderCode"].Description; else inventor.ImportPDAProviderCode = "";
					if (inventorConfigs.ContainsKey("ExportERPAdapterCode") == true) inventor.ExportERPAdapterCode = inventorConfigs["ExportERPAdapterCode"].Description; else inventor.ExportERPAdapterCode = "";
					if (inventorConfigs.ContainsKey("ImportSupplierAdapterCode") == true) inventor.ImportSupplierAdapterCode = inventorConfigs["ImportSupplierAdapterCode"].Description; else inventor.ImportSupplierAdapterCode = "";
					if (String.IsNullOrWhiteSpace(inventor.CustomerCode) == false)
					{
					    Customer customer = this._customerRepository.GetCustomerByCode(inventor.CustomerCode);
					    if (customer == null)
					    {
							if (addParentObject == true)
							{
								this.AddNewCustomer(inventor);
							}
					    }
					}
					if (String.IsNullOrWhiteSpace(inventor.BranchCode) == false)
					{
					    Branch branch = this._branchRepository.GetBranchByCode(inventor.BranchCode);
					    if (branch == null)
					    {
							if (addParentObject == true)
							{
								this.AddNewBranch(inventor);
							}
					    }
					}
				}
			}
			inventor.Description = inventor.Description;// + inventor.Restore;
			return inventor;
		}



		public AuditConfig AuditConfigFromInventorConfigs(string PathDB, bool addParentObject = false)
		{
			AuditConfig auditConfig = null;
			auditConfig = new AuditConfig();
			auditConfig.Code = Guid.NewGuid().ToString();
			auditConfig.InventorDate = DateTime.Now;
			auditConfig.CreateDate = DateTime.Now;
			auditConfig.CustomerCode = "unknown";
			auditConfig.CustomerName = "Unknown";
			auditConfig.BranchCode = "unknown";
			auditConfig.BranchName = "Unknown";
			//auditConfig.Description = "Restore";
			auditConfig.Description = "";
			auditConfig.StatusAuditConfig = StatusAuditConfigEnum.NotCurrent.ToString();

			string subFolder = PropertiesSettings.FolderInventor;
			int len = subFolder.Length;

			string pathDB = PathDB;
			pathDB = pathDB.Trim('\\');
			auditConfig.DBPath = pathDB.Substring(len).Trim('\\');
			while (pathDB.IndexOf('\\') > 0)
			{
				int index = pathDB.IndexOf('\\');
				pathDB = pathDB.Substring(index).Trim('\\');
			}
			auditConfig.InventorCode = pathDB;

			Dictionary<string, App_Data.InventorConfig> inventorConfigs = new Dictionary<string, App_Data.InventorConfig>();
			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(PathDB)))
			{
				var inventorConfigList = dc.InventorConfigs.Select(e => e);
				
				foreach (var inventorConfig in inventorConfigList)
				{
					inventorConfigs[inventorConfig.TypeObject] = inventorConfig;
				}
			}


			if (inventorConfigs.ContainsKey("DomainType") == true)
			{
				if (inventorConfigs["DomainType"].Description == DomainTypeEnum.Inventor.ToString())
				{
					//if (inventorConfigs.ContainsKey("Code") == true) auditConfig.InventorCode = inventorConfigs["Code"].Description;
					if (inventorConfigs.ContainsKey("InventorDate") == true)
					{
						DateTime inventorDate = new DateTime();
						bool ret = DateTime.TryParse(inventorConfigs["InventorDate"].Description, out inventorDate);
						if (ret == true) auditConfig.InventorDate = inventorDate;
					}
					//if (inventorConfigs.ContainsKey("CompleteDate") == true)
					//{
					//    DateTime completeDate = new DateTime();
					//    bool ret = DateTime.TryParse(inventorConfigs["CompleteDate"].Description, out completeDate);
					//    if (ret == true) auditConfig.CompleteDate = completeDate;
					//}
					if (inventorConfigs.ContainsKey("Name") == true) auditConfig.InventorName = inventorConfigs["Name"].Description;

					//if (inventorConfigs.ContainsKey("InventorCode") == true) auditConfig.Description = auditConfig.Description + " InventorCode :" + inventorConfigs["InventorCode"].Description;
					//if (inventorConfigs.ContainsKey("Name") == true) auditConfig.Description = auditConfig.Description + " InventorName :" + inventorConfigs["Name"].Description;
					if (inventorConfigs.ContainsKey("CustomerCode") == true) auditConfig.CustomerCode = inventorConfigs["CustomerCode"].Description;
					if (inventorConfigs.ContainsKey("BranchCode") == true) auditConfig.BranchCode = inventorConfigs["BranchCode"].Description;
					//if (inventorConfigs.ContainsKey("DBPath") == true) auditConfig.Description = auditConfig.Description + " PathDB :" + inventorConfigs["DBPath"].Description;
					if (inventorConfigs.ContainsKey("Description") == true) auditConfig.Description = auditConfig.Description + inventorConfigs["Description"].Description;

					if (String.IsNullOrWhiteSpace(auditConfig.CustomerCode) == false)
					{
						Customer customer = this._customerRepository.GetCustomerByCode(auditConfig.CustomerCode);
						if (customer != null)
						{
							auditConfig.CustomerName = customer.Name;
						}
						else
						{
							if (addParentObject == true)
							{
								this.AddNewCustomer(auditConfig);
							}
						}
					}
					if (String.IsNullOrWhiteSpace(auditConfig.BranchCode) == false)
					{
						Branch branch = this._branchRepository.GetBranchByCode(auditConfig.BranchCode);
						if (branch != null)
						{
							auditConfig.BranchName = branch.Name;
						}
						else
						{
							if (addParentObject == true)
							{
								this.AddNewBranch(auditConfig);
							}
						}
					}

				}
			}

			return auditConfig;
		}

		/// <summary>
		///  PathDB = @"Customer\\CustomerCode1"
		/// </summary>
		/// <param name="pathDB"></param>
		/// <returns></returns>
		public bool IsCustomerCount4UDB(string pathDB, out string customerCode)
		{
			customerCode = "";
			string fromFilePath = base._connection.BuildCount4UDBFilePath(pathDB);
			if (File.Exists(fromFilePath) == false) return false;

			//string subFolder = PropertiesSettings.FolderCustomer;
			//int len = subFolder.Length;
			//string pathDB = PathDB;
			//pathDB = pathDB.Trim('\\');
			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var inventorConfigList = dc.InventorConfigs.Select(e => e);
				Dictionary<string, App_Data.InventorConfig> inventorConfigs = new Dictionary<string, App_Data.InventorConfig>();
				foreach (var inventorConfig in inventorConfigList)
				{
					inventorConfigs[inventorConfig.TypeObject] = inventorConfig;
				}

				if (inventorConfigs.ContainsKey("DomainType") == true)
				{
					if (inventorConfigs.ContainsKey("Code") == true) customerCode = inventorConfigs["Code"].Description;
					if (inventorConfigs["DomainType"].Description == DomainTypeEnum.Customer.ToString())
					{
						if (string.IsNullOrWhiteSpace(customerCode) == true)  //if customerCode пустой восстановить из пути PathDB
						{
							//Localization.Resources.Log_TraceRepositoryResultNN%"Restore Customer Code from Path DB: {0}"
							this._log.Add(MessageTypeEnum.TraceRepository, String.Format("Restore Customer Code from Path DB: {0}", pathDB));
							this.GetPathDBFromRelativePathDB(pathDB, PropertiesSettings.FolderCustomer, out customerCode);
						}
						return true;
					}
				}
			}
			return false;
		}

		public bool IsBranchCount4UDB(string pathDB, out string customerCode, out string branchCode)
		{
			customerCode = "";
			branchCode = "";
			string fromFilePath = base._connection.BuildCount4UDBFilePath(pathDB);
			if (File.Exists(fromFilePath) == false) return false;

			//string subFolder = PropertiesSettings.FolderBranch;
			//int len = subFolder.Length;
			//string pathDB = PathDB;
			//pathDB = pathDB.Trim('\\');
			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var inventorConfigList = dc.InventorConfigs.Select(e => e);
				Dictionary<string, App_Data.InventorConfig> inventorConfigs = new Dictionary<string, App_Data.InventorConfig>();
				foreach (var inventorConfig in inventorConfigList)
				{
					inventorConfigs[inventorConfig.TypeObject] = inventorConfig;
				}

				if (inventorConfigs.ContainsKey("DomainType") == true)
				{
					if (inventorConfigs.ContainsKey("Code") == true) branchCode = inventorConfigs["Code"].Description;
					if (inventorConfigs.ContainsKey("CustomerCode") == true) customerCode = inventorConfigs["CustomerCode"].Description;
					if (inventorConfigs["DomainType"].Description == DomainTypeEnum.Branch.ToString())
					{
						if (string.IsNullOrWhiteSpace(branchCode) == true) //if branchCode пустой восстановить из пути PathDB
						{
							//Localization.Resources.Log_TraceRepositoryResultNN%"Restore Branch Code from Path DB: {0}"
							this._log.Add(MessageTypeEnum.TraceRepository, String.Format("Restore Branch Code from Path DB: {0}", pathDB));
							this.GetPathDBFromRelativePathDB(pathDB, PropertiesSettings.FolderBranch, out branchCode);
						}
						return true;
					}
				}
			}
			return false;
		}

		public bool IsInventorCount4UDB(string pathDB, out string customerCode, out string branchCode, out string inventorCode )
		{
			customerCode = "";
			branchCode = "";
			inventorCode = "";
			string fromFilePath = base._connection.BuildCount4UDBFilePath(pathDB);
			if (File.Exists(fromFilePath) == false) return false;

			//string subFolder = PropertiesSettings.FolderInventor;
			//int len = subFolder.Length;
			//pathDB = pathDB.Trim('\\');
			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var inventorConfigList = dc.InventorConfigs.Select(e => e);
				Dictionary<string, App_Data.InventorConfig> inventorConfigs = new Dictionary<string, App_Data.InventorConfig>();
				foreach (var inventorConfig in inventorConfigList)
				{
					inventorConfigs[inventorConfig.TypeObject] = inventorConfig;
				}

				if (inventorConfigs.ContainsKey("DomainType") == true)
				{
					if (inventorConfigs.ContainsKey("Code") == true) inventorCode = inventorConfigs["Code"].Description;
					if (inventorConfigs.ContainsKey("CustomerCode") == true) customerCode = inventorConfigs["CustomerCode"].Description;
					if (inventorConfigs.ContainsKey("BranchCode") == true) branchCode = inventorConfigs["BranchCode"].Description;
					if (inventorConfigs["DomainType"].Description == DomainTypeEnum.Inventor.ToString())
					{
						if (string.IsNullOrWhiteSpace(inventorCode) == true)
						{
							//восстановить inventorCode из пути PathDB
							//Localization.Resources.Log_TraceRepositoryResultNN%"Restore Inventor Code from Path DB: {0}"
							this._log.Add(MessageTypeEnum.TraceRepository, String.Format("Restore Inventor Code from Path DB: {0}", pathDB));
							this.GetPathDBFromRelativePathDB(pathDB, PropertiesSettings.FolderImport, out inventorCode);
						}
						return true;
					}
				}
			}
			return false;
		}

		public void RestoreDomainObject(string pathDB)
		{
			//Localization.Resources.Log_TraceRepositoryResultNN%"RestoreDomainObject - Path DB: {0}"
			this._log.Add(MessageTypeEnum.TraceRepository, String.Format("RestoreDomainObject - Path DB: {0}", pathDB));

			string fromFilePath = base._connection.BuildCount4UDBFilePath(pathDB);
			if (File.Exists(fromFilePath) == false)
			{
				//Localization.Resources.Log_TraceRepositoryResultNN%"File {0} Not Exists - so don't restore Domain Object"
				this._log.Add(MessageTypeEnum.TraceRepository, String.Format("File {0} Not Exists - so don't restore Domain Object", fromFilePath));
				return;
			}

			string customerCode = "";
			string branchCode = "";
			string inventorCode = "";
			// ========= if pathDB содержит Customer DB ==============
			bool isCustomer = IsCustomerCount4UDB(pathDB, out customerCode);
			if (isCustomer == true)
			{
				Customer customer = this._customerRepository.GetCustomerByCode(customerCode);
				if (customer == null) //if domainObject не существует в MainDB удалить файл fromFilePath
				{
					#if DEBUG
					// в   DEBUG режиме восстанавливать и  domainObject в MainDB
							this.RepairDomainCustomer(pathDB);
					#else     
					//Localization.Resources.Log_TraceRepositoryResultNN%"CustomerCode {0} Not Exists in MainDB - so don't restore Domain Object"
					this._log.Add(MessageTypeEnum.TraceRepository, String.Format("CustomerCode [{0}] Not Exists in MainDB - so don't restore Domain Object", customerCode));
 					if (File.Exists(fromFilePath) == true) 	File.Delete(fromFilePath);
					return;
					#endif
				}
			}

			// ========= if pathDB содержит Branch DB==============
			else if (IsBranchCount4UDB(pathDB, out customerCode, out branchCode) == true)
			{
				Branch branch = this._branchRepository.GetBranchByCode(branchCode);
				Customer customer = this._customerRepository.GetCustomerByCode(customerCode);
				if (branch == null  //if domainObject не существует в MainDB удалить файл fromFilePath
					|| customer == null) //if domainObject не существует в MainDB удалить файл fromFilePath
				{
					#if DEBUG
					// в   DEBUG режиме восстанавливать и  domainObject в MainDB

					if (branch == null)
					{
						this.RepairDomainBranch(pathDB);
					}
					#else     
					//Localization.Resources.Log_TraceRepositoryResultNN%"CustomerCode {0} Not Exists in MainDB - so don't restore Domain Object"
					this._log.Add(MessageTypeEnum.TraceRepository, String.Format("Customer Code [{0}] or Branch Code [{1}] Not Exists in MainDB - so don't restore Domain Object", customerCode, branchCode));
					if (File.Exists(fromFilePath) == true) File.Delete(fromFilePath); 
					return;
					#endif
				}
			}
			//bool isBranch = IsBranchCount4UDB(PathDB, out customerCode);

			// =========  if pathDB содержит  Inventor DB ==============
			if (IsInventorCount4UDB(pathDB, out customerCode, out branchCode, out inventorCode) == true)
			{
				Branch branch = this._branchRepository.GetBranchByCode(branchCode);
				Customer customer = this._customerRepository.GetCustomerByCode(customerCode);
				if (branch == null || customer == null)	   // требование, если нет кастомера или бренча то инвентор не удаляем и не распаковываем
				{
					//скопировать в fromFilePath -> fromFilePath\remove fromFilePath
					//Localization.Resources.Log_TraceRepositoryResultNN%"CustomerCode {0} Not Exists in MainDB - so don't restore Domain Object"
					this._log.Add(MessageTypeEnum.TraceRepository, String.Format("Customer Code [{0}] or Branch Code [{1}] Not Exists in MainDB - so don't restore Domain Object", customerCode, branchCode));
					Inventor inventor = this._inventorRepository.GetInventorByCode(inventorCode);
					if (inventor != null)
					{
						this._inventorRepository.Delete(inventorCode, false);
						this._log.Add(MessageTypeEnum.TraceRepository, String.Format("Delete Inventor with InventorCode [{0}]", inventorCode));
						}
					AuditConfig auditConfig = this._auditConfigRepository.GetAuditConfigByInventorCode(inventorCode, CBIContext.History);
					if (auditConfig != null)
					{
						this._auditConfigRepository.Delete(auditConfig, CBIContext.History);
						this._log.Add(MessageTypeEnum.TraceRepository, String.Format("Delete AuditConfig with InventorCode [{0}]", inventorCode));
					}
					//base._connection.RemoveDB(pathDB, PropertiesSettings.FolderInventor.Trim('\\') + @"\", false);
					return;
				}
				else
				{
					//востановить в БД Аудит, и инвентор 
					Inventor inventor = this._inventorRepository.GetInventorByCode(inventorCode);
					if (inventor == null)
					{
						//Localization.Resources.Log_TraceRepositoryResultNN%"Insert Inventor DomainObject and AuditConfig DomainObject in AuditDB"
						this._log.Add(MessageTypeEnum.TraceRepository, String.Format("Insert Inventor DomainObject and AuditConfig DomainObject in AuditDB"));
						this.RepairDomainInventor(pathDB, false);		  //Insert Inventor & AuditConfig
					}
					else
					{
						//Localization.Resources.Log_TraceRepositoryResultNN%"Update Inventor DomainObject and AuditConfig DomainObject in AuditDB"
						this._log.Add(MessageTypeEnum.TraceRepository, String.Format("Update Inventor DomainObject and AuditConfig DomainObject in AuditDB"));
						this.RepairDomainInventor(pathDB, true);		   //Update Inventor & AuditConfig
					}
				}
			}
			//bool isInventor = IsInventorCount4UDB(PathDB, out customerCode,  out branchCode);
 		}


		private void AddNewBranch(AuditConfig auditConfig)
		{
			Branch branch = new Branch(auditConfig);
			//branch.Code = auditConfig.BranchCode;
			//branch.CustomerCode = auditConfig.CustomerCode;
			//branch.DBPath = auditConfig.BranchCode;
			//branch.Name = auditConfig.BranchCode;
			//branch.CreateDate = DateTime.Now;
			//branch.ModifyDate = DateTime.Now;
			//branch.RestoreBit = true;
			//branch.Restore = "Restore form Inventor Code: " + auditConfig.InventorCode;
			//branch.Description = "Restore form Inventor Code: " + auditConfig.InventorCode;
			//branch.Address = "";
			//branch.ContactPerson = "";
			//branch.Phone = "";
			//branch.Mail = "";
			//branch.Fax = "";
			this._branchRepository.Insert(branch);
		}

		private void AddNewCustomer(AuditConfig auditConfig)
		{
			Customer customer = new Customer(auditConfig);
			//customer.Code = auditConfig.CustomerCode;
			//customer.DBPath = auditConfig.CustomerCode;
			//customer.Name = auditConfig.CustomerCode;
			//customer.CreateDate = DateTime.Now;
			//customer.ModifyDate = DateTime.Now;
			//customer.RestoreBit = true;
			//customer.Restore = "Restore form Inventor Code: " + auditConfig.InventorCode;
			//customer.Description = "Restore form Inventor Code: " + auditConfig.InventorCode;
			//customer.Address = "";
			//customer.ContactPerson = "";
			//customer.Phone = "";
			//customer.Mail = "";
			//customer.Fax = "";
			this._customerRepository.Insert(customer);
		}

		private void AddNewBranch(Inventor inventor)
		{
			Branch branch = new Branch(inventor);
			//branch.Code = inventor.BranchCode;
			//branch.CustomerCode = inventor.CustomerCode;
			//branch.DBPath = inventor.BranchCode;
			//branch.Name = inventor.BranchCode;
			//branch.CreateDate = DateTime.Now;
			//branch.ModifyDate = DateTime.Now;
			//branch.RestoreBit = true;
			//branch.Restore = "Restore form Inventor Code: " + inventor.Code;
			//branch.Description = "Restore form Inventor Code: " + inventor.Code;
			//branch.Address = "";
			//branch.ContactPerson = "";
			//branch.Phone = "";
			//branch.Mail = "";
			//branch.Fax = "";
			this._branchRepository.Insert(branch);
		}

		private void AddNewCustomer(Inventor inventor)
		{
			Customer customer = new Customer(inventor);
			//customer.Code = inventor.CustomerCode;
			//customer.DBPath = inventor.CustomerCode;
			//customer.Name = inventor.CustomerCode;
			//customer.CreateDate = DateTime.Now;
			//customer.ModifyDate = DateTime.Now;
			//customer.RestoreBit = true;
			//customer.Restore = "Restore form Inventor Code: " + inventor.Code;
			//customer.Description = "Restore form Inventor Code: " + inventor.Code;
			//customer.Address = "";
			//customer.ContactPerson = "";
			//customer.Phone = "";
			//customer.Mail = "";
			//customer.Fax = "";
			this._customerRepository.Insert(customer);
		}

		private void AddNewCustomer(Branch branch)
		{
			Customer customer = new Customer(branch);
			//customer.Code = branch.CustomerCode;
			//customer.DBPath = branch.CustomerCode;
			//customer.Name = branch.CustomerCode;
			//customer.CreateDate = DateTime.Now;
			//customer.ModifyDate = DateTime.Now;
			//customer.RestoreBit = true;
			//customer.Restore = "Restore form Customer Code: " + branch.CustomerCode;
			//customer.Description = "Restore form Customer Code: " + branch.CustomerCode;
			//customer.Address = "";
			//customer.ContactPerson = "";
			//customer.Phone = "";
			//customer.Mail = "";
			//customer.Fax = "";
			this._customerRepository.Insert(customer);
		}

		#region private
		private App_Data.InventorConfig GetEntityByCode(App_Data.Count4UDB db, string code)
		{
			var entity = db.InventorConfigs.FirstOrDefault(e => e.Code.CompareTo(code) == 0);
			return entity;
		}

		private InventorConfig GetInventorConfig(string pathDB)
		{
			using (App_Data.Count4UDB dc = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = dc.InventorConfigs.FirstOrDefault();
				return entity.ToDomainObject();
			}
		}

//	relativePathDB=inventor\2014\7\8\f8c419fb-5705-48e0-b8a7-a146b54c98cb
//subFolderDomainObject="inventor"
//возвращает в out codeDomainObject=f8c419fb-5705-48e0-b8a7-a146b54c98cb
//и вся функция возвращает 2014\7\8\f8c419fb-5705-48e0-b8a7-a146b54c98cb
		//this.GetPathDBFromRelativePathDB(pathDB, PropertiesSettings.FolderCustomer, out customerCode);
		//this.GetPathDBFromRelativePathDB(pathDB, PropertiesSettings.FolderBranch, out customerCode);
		//this.GetPathDBFromRelativePathDB(pathDB, PropertiesSettings.FolderInventor, out inventorCode);
		// а бывает и так this.GetPathDBFromRelativePathDB(pathDB, PropertiesSettings.FolderImport, out inventorCode);
		public string GetPathDBFromRelativePathDB(string relativePathDB, string subFolderDomainObject,
	out string codeDomainObject)
		{
			int len = subFolderDomainObject.Length;
			string pathDB = relativePathDB;

			pathDB = pathDB.Trim(@"\".ToCharArray());
			string retDBPath = pathDB.Substring(len).Trim(@"\".ToCharArray());	   //DBPath без папки Inventor\Customer\Branch

			while (pathDB.IndexOf(@"\") > 0)
			{
				int index = pathDB.IndexOf(@"\");
				pathDB = pathDB.Substring(index).Trim(@"\".ToCharArray());   //выбираем код из пути год\\месяц\\день\\InventorCode
			}
			codeDomainObject = pathDB;
			return retDBPath;
		}


		public string GetObjectCodeFromRelativePathDB(string relativePathDB)
		{
			string codeDomainObject = "";
			if (relativePathDB.ToLower().Contains("inventor") == true)
			{
				GetPathDBFromRelativePathDB(relativePathDB, "Inventor", out codeDomainObject);
			}
			else	if (relativePathDB.ToLower().Contains("customer") == true)
			{
				GetPathDBFromRelativePathDB(relativePathDB, "Customer", out codeDomainObject);
			}
			else if (relativePathDB.ToLower().Contains("branch") == true)
			{
				GetPathDBFromRelativePathDB(relativePathDB, "Branch", out codeDomainObject);
			}
			return codeDomainObject;
		}
		#endregion


	
	}
}
