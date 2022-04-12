using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Main.MappingEF;
using Count4U.Model.Audit.MappingEF;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.SelectionParams;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using NLog;
using System.IO;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Audit;
using Count4U.Localization;

namespace Count4U.Model.Main
{
    public class BranchEFRepository : BaseEFRepository, IBranchRepository
    {
		//private Branch this._current;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private IServiceLocator _serviceLocator;

		public BranchEFRepository(IConnectionDB connection, IServiceLocator serviceLocator)
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
 
		#region IBranchRepository Members

       // public Branches this._branches;

        public Branches GetBranches()
        {
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = dc.Branch.ToList().Select(e => e.ToDomainObject());
				return Branches.FromEnumerable(domainObjects);
			}
        }

		public Branches GetBranches(SelectParams selectParams)
		{
			if (selectParams == null)
				return GetBranches();

			long totalCount = 0;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				// Получение сущностей и общего количества из БД.
				// Getting entities and total count from database.
				var entities = GetEntities(dc, AsQueryable(dc.Branch), dc.Branch.AsQueryable(), selectParams, out totalCount);

				// Преобразование сущностей в объекты предметной области.
				// Converting entites to domain objects.
				var domainObjects = entities.Select(e => e.ToDomainObject());

				// Возврат результата.
				// Returning result.
				Branches result = Branches.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

        public Branches GetBranchesByCustomer(Customer customer)
        {
			if (customer == null) return null;
			if (string.IsNullOrWhiteSpace(customer.Code) == true) return null;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = dc.Branch.Where(e => e.CustomerCode == customer.Code)
													 .ToList().Select(e => e.ToDomainObject());
				return Branches.FromEnumerable(domainObjects);
			}

        }

        public Branches GetBranchesDetailsByCustomer(Customer customer)
        {
			if (customer == null) return null;
			if (string.IsNullOrWhiteSpace(customer.Code) == true) return null;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var domainObjects = dc.Branch.Where(e => e.CustomerCode == customer.Code)
													 .ToList().Select(e => e.ToDomainObject());
				return Branches.FromEnumerable(domainObjects);
			}

        }

        public Branches GetBranchesByCustomerCode(string customerCode, CBIContext contextCBI)
        {
			if (string.IsNullOrWhiteSpace(customerCode) == true) return null;

			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
				case CBIContext.Main:
					using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
					{
						var domainObjects = dc.Branch.Where(e => e.CustomerCode == customerCode)
															 .ToList().Select(e => e.ToDomainObject());
						return Branches.FromEnumerable(domainObjects);
					}
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig.Where(e => e.CustomerCode == customerCode)
							.ToList().Select(e => e.ToDomainObject());
						return AuditConfigs.BranchesFromEnumerable(domainObjects, this._serviceLocator);
					}
				default:
					using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
					{
						var domainObjects = dc.Branch.Where(e => e.CustomerCode == customerCode)
															 .ToList().Select(e => e.ToDomainObject());
						return Branches.FromEnumerable(domainObjects);
					}
			}
        }


		public Branches GetBranches(CBIContext contextCBI)
		{
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
				case CBIContext.Main:
					using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
					{
						var domainObjects = dc.Branch.ToList().Select(e => e.ToDomainObject());
						return Branches.FromEnumerable(domainObjects);
					}
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig
							.ToList().Select(e => e.ToDomainObject());
						return AuditConfigs.BranchesFromEnumerable(domainObjects, this._serviceLocator);
					}
				default:
					using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
					{
						var domainObjects = dc.Branch
															 .ToList().Select(e => e.ToDomainObject());
						return Branches.FromEnumerable(domainObjects);
					}
			}
		}

		//public void SetCurrent(Branch currentBranch)
		//{
		//    this._current = currentBranch;
		//}

		//public Branch GetCurrent()
		//{
		//    return this._current;
		//}

		public void SetCurrent(Branch currentBranch, AuditConfig auditConfig)
		{
			if (auditConfig == null) return;
			if (currentBranch == null)
			{
				auditConfig.ClearBranch();
			}
			else
			{
				if (auditConfig.BranchCode != currentBranch.Code)
				{
					auditConfig.ClearBranch();
					auditConfig.BranchCode = currentBranch.Code;
					auditConfig.BranchName = currentBranch.Name;
				}
			}
		}

		public Branch GetCurrent(AuditConfig auditConfig)
		{
			if (auditConfig == null) return null;
			if (string.IsNullOrWhiteSpace(auditConfig.BranchCode) == true) return null;
			Branch branch = this.GetBranchByCode(auditConfig.BranchCode);
			return branch;
		}

		public string GetCurrentCode(AuditConfig auditConfig)
		{
			if (auditConfig == null) return "";
			if (string.IsNullOrWhiteSpace(auditConfig.BranchCode) == true) return "";
			return auditConfig.BranchCode;
		}

		public List<string> GetCodeList()
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = dc.Branch.Select(e => e.Code).Distinct().ToList();
				return entity;
			}
		}

		public List<string> GetBranchCodeListByCustomerCode(string customerCode)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				try
				{
					var entitys = db.Branch.Where(e => e.CustomerCode == customerCode).Select(e => e.Code).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetBranchCodeListByCustomerCode", exp);
				}
			}
			return ret;
		}
		//public Branch Clone(Branch branch)
		//{
		//    if (branch == null) return null;
		//    var domainObject = branch.Clone();
		//    domainObject.ID = 0;
		//    domainObject.Code = "";
		//    return domainObject;
		//}

		public void Delete(Branch branch)
		{
			if (branch == null) return;
			if (string.IsNullOrWhiteSpace(branch.Code) == true) return;
			//if (full == true)
			//{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entity = this.GetEntityByCode(dc, branch.Code);
					if (entity == null) return;
					string folder = PropertiesSettings.FolderBranch.Trim('\\') + @"\";
					this.Connection.RemoveDB(entity.DBPath, folder, false);
					dc.Branch.DeleteObject(entity);
					dc.SaveChanges();
				}
			//}
		}

		public void Delete(string branchCode)
		{
			if (string.IsNullOrWhiteSpace(branchCode) == true) return;
			//if (full == true)
			//{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
				{
					var entity = this.GetEntityByCode(dc, branchCode);
					if (entity == null) return;
					string folder = PropertiesSettings.FolderBranch.Trim('\\') + @"\";
					this.Connection.RemoveDB(entity.DBPath, folder, false);
					dc.Branch.DeleteObject(entity);
					dc.SaveChanges();
				}
			//}
		}

		public void Delete(List<string> branchCodeList)
		{
			IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();

			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (var branchCode in branchCodeList)
				{
					if (string.IsNullOrWhiteSpace(branchCode) == true) continue;
					var entity = this.GetEntityByCode(dc, branchCode);
					if (entity == null) continue;

					if (inventorRepository.GetInventorCountByBranchCode(branchCode) == 0)
					{
						string folder = PropertiesSettings.FolderBranch.Trim('\\') + @"\";
						this.Connection.RemoveDB(entity.DBPath, folder, false);
						dc.Branch.DeleteObject(entity);
					}
				}
				dc.SaveChanges();
			} //dc
		}


		public void DeleteDomainObjectOnly(List<string> branchCodeList)
		{
				using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (var branchCode in branchCodeList)
				{
					if (string.IsNullOrWhiteSpace(branchCode) == true) continue;
					var entity = this.GetEntityByCode(dc, branchCode);
					if (entity == null) continue;
	   				dc.Branch.DeleteObject(entity);
				}
				dc.SaveChanges();
			} //dc
		}

		public List<App_Data.Branch> GetBranchsCountByBranchCode(string branchCode)
		{
			if (string.IsNullOrEmpty(branchCode) == true) return null;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entety = dc.Branch.Where(e => e.Code == branchCode).ToList();
				return entety;
			}
		}

		public void DeleteBranchWithDoubleCode(string description = "RepairBranch")
		{
			//Repair
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				List<string> branchCodeList = dc.Branch.Select(e => e.Code).Distinct().ToList();		

				foreach (var branchCode in branchCodeList)
				{
					if (string.IsNullOrWhiteSpace(branchCode) == true) continue;
					var entity = this.GetEntityByCode(dc, branchCode);
					if (entity == null) return;
					var branchList = this.GetBranchsCountByBranchCode(branchCode);
					if (branchList == null) continue;
					if (branchList.Count > 1)
					{
						var delEntity = branchList.FirstOrDefault(e => e.Description == description);
						dc.Branch.DeleteObject(delEntity);
					}
				}
				dc.SaveChanges();
			} //dc
		}

		public void DeleteAllByCustomerCode(string customerCode, bool full = false)
		{
			if (string.IsNullOrWhiteSpace(customerCode) == true) return;
			//if (full == true)
			//{

			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entities = dc.Branch.Where(e => e.CustomerCode == customerCode).ToList();
				if (entities == null) return;
				string folder = PropertiesSettings.FolderBranch.Trim('\\') + @"\";
				foreach (var entity in entities)
				{
					this.Connection.RemoveDB(entity.DBPath, folder, false);
				}
				entities.ForEach(e => dc.Branch.DeleteObject(e));
				dc.SaveChanges();
			}
		//}
		}

		public void Insert(Dictionary<string, Branch> branchToDBDictionary, bool copyEmptyDB = true)
		{
			foreach (KeyValuePair<string, Branch> keyValuePair in branchToDBDictionary)
			{
				//string key = keyValuePair.Key;
				Branch branch = keyValuePair.Value;
				Insert(branch, copyEmptyDB);
			}
		}

		public void Insert(Branches branchs)
		{
			if (branchs == null) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (Branch branch in branchs)
				{
					if (branch == null) { continue; }
					App_Data.Branch entity = GetEntityByCode(dc, branch.Code);
					if (entity == null)
					{
						entity = branch.ToEntity();
						entity.ModifyDate = DateTime.Now;
						if (entity.CreateDate == DateTime.MinValue)
						{
							entity.CreateDate = DateTime.Now;
						}
						entity.LastUpdatedCatalog = DateTime.Now;
						entity.Restore = DateTime.Now.ToString();
						entity.RestoreBit = true; 
						dc.Branch.AddObject(entity);
					}
				}
				dc.SaveChanges();
			}
		}

		public void Insert(Branch branch, bool copyDB = true, string inheritFromDBPath = null )
		{
			if (branch == null)
			{
				_logger.Trace("Insert", "branch is null");
				return;
			}

			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = branch.ToEntity();
				string folder = PropertiesSettings.FolderBranch.Trim('\\') + @"\";

				if (copyDB == true)
				{
					if (String.IsNullOrEmpty(inheritFromDBPath) == false)
					{
						string sourceCount4UDBPath = this.Connection.BuildCount4UDBFilePath(inheritFromDBPath);
						this.Connection.CopyCount4UDB(branch.DBPath, folder, sourceCount4UDBPath);
						string relativePath = BuildRelativeDbPath(branch);
						this.Connection.CopyEmptyAnaliticDB(relativePath);
					}
					else
					{
						string pathDB = this.Connection.CopyEmptyCount4UAndAnaliticDB(branch.DBPath, folder);
					}
				}

				entity.CreateDate = DateTime.Now;
				entity.ModifyDate = DateTime.Now;
			
				dc.Branch.AddObject(entity);
				dc.SaveChanges();
			}

			
			if (copyDB == true)
			{
				InventorConfigs inventorConfigs = FillInventorProperty(branch);
				string branchDBPath = PropertiesSettings.FolderBranch.Trim('\\') + @"\" + branch.DBPath;
				using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(branchDBPath)))
				{
					var list = db.InventorConfigs.ToList();
					if (list.Count > 0)
					{
						list.ForEach(e => db.InventorConfigs.DeleteObject(e));
						db.SaveChanges();
					}

					var entity = branch.ToInventorConfigEntity();
					db.InventorConfigs.AddObject(entity);

					foreach (var inventorConfig in inventorConfigs)
					{
						db.InventorConfigs.AddObject(inventorConfig.ToEntity());
					}
					db.SaveChanges();
				}

			}
		}

		public void InsertDomainBranchFromInventorConfig(Branches branches)
		{
			if (branches == null) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (Branch branch in branches)
				{
					if (branch == null) { continue; }
					App_Data.Branch entity = GetEntityByCode(dc, branch.Code);
					if (entity == null)
					{
						entity = branch.ToEntity();
						entity.ModifyDate = DateTime.Now;
						if (entity.CreateDate == DateTime.MinValue)
						{
							entity.CreateDate = DateTime.Now;
						}
						entity.LastUpdatedCatalog = DateTime.Now;
						entity.Restore = DateTime.Now.ToString();
						entity.RestoreBit = true;
						dc.Branch.AddObject(entity);
					}
				}
				dc.SaveChanges();
			}
		}



		public void UpdateBranchName(Branch branch)
		{
			if (branch == null) return;
			//branch.CreateDate = DateTime.Now;
			//branch.ModifyDate = DateTime.Now;
			if (string.IsNullOrWhiteSpace(branch.Code) == true) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, branch.Code);
				if (entity == null) return;
				Branch oldBranch = entity.ToDomainObject();
				oldBranch.Name = branch.Name;
				entity.ApplyChanges(oldBranch);
				dc.SaveChanges();
			}

			RefillInventorConfigs(branch);
		}

		public void Update(Branch branch)
		{
			if (branch == null) return;
			//branch.CreateDate = DateTime.Now;
			//branch.ModifyDate = DateTime.Now;
			if (string.IsNullOrWhiteSpace(branch.Code) == true) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, branch.Code);
				if (entity == null) return;
				entity.ApplyChanges(branch);
				dc.SaveChanges();
			}

			//string relativePath = BuildRelativeDbPath(branch);
			//this.Connection.CopyEmptyAnaliticDB(relativePath);

			RefillInventorConfigs(branch);
		}

		public string BuildRelativeDbPath(Branch branch)
		{
				return PropertiesSettings.FolderBranch.Trim('\\') + @"\" + branch.DBPath;
		}

		// Update branch в БД MainDB по InventorConfig  из БД Count4U, по заданному пути  pathDB
		public void UpdateDomainBranchByInventorConfig(Branches branches)
		{
			if (branches == null) return;
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				foreach (Branch branch in branches)
				{
					if (branch == null) { continue; }
					//customer.CreateDate = DateTime.Now;
					//branch.ModifyDate = DateTime.Now;
					var entity = this.GetEntityByCode(dc, branch.Code);
					if (entity == null) continue;
					entity.ApplyChanges(branch);
					//this.RefillInventorConfigs(customer);
				}
				dc.SaveChanges();
			}
		}

		public void RefillInventorConfigs(Branch branch)
		{
			if (branch == null) return;
			string branchDBPath = PropertiesSettings.FolderBranch.Trim('\\') + @"\" + branch.DBPath;
			string fullPath = this.Connection.BuildCount4UDBFilePath(branchDBPath);
			if (String.IsNullOrWhiteSpace(fullPath) == true || File.Exists(fullPath) == false) return;

			InventorConfigs inventorConfigs = FillInventorProperty(branch);
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(branchDBPath)))
			{
				db.InventorConfigs.ToList().ForEach(e => db.InventorConfigs.DeleteObject(e));
				db.SaveChanges();

				var entity = branch.ToInventorConfigEntity();
				db.InventorConfigs.AddObject(entity);

				foreach (var inventorConfig in inventorConfigs)
				{
					db.InventorConfigs.AddObject(inventorConfig.ToEntity());
				}
				db.SaveChanges();
			}

            //CreateInfoFile(branch, branchDBPath);

		}

        private void CreateInfoFile(Branch branch, string branchDBPath, InventorConfigs inventorConfigs = null)
        {
            string branchFullPath = this.Connection.BuildCount4UDBFolderPath(branchDBPath);

            if (Directory.Exists(branchFullPath) == true)
            {
                string[] files = Directory.GetFiles(branchFullPath, "*.c4u", SearchOption.TopDirectoryOnly);
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
                string newFileInfo = DateTime.Now.ToString("ddMMyyyy") + "_Branch_" + branch.CustomerCode + "__" + branch.Code + ".c4u";
                string newFileInfoPath = branchFullPath + newFileInfo;
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

		public void RefillInventorConfigsAllBranchesInMainDB()
		{
			Branches branchs = this.GetBranches();
			foreach (var branch in branchs)
				{
					this.RefillInventorConfigs(branch);
				}
		}

		public Dictionary<string, InventorConfig> GetInventorConfigDictionary(string branchDBPath)
		{
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(branchDBPath)))
			{
				return InventorConfigs.FromEnumerableToDictionary(db.InventorConfigs.Select(e => e.ToDomainObject()));
			}
		}

		public void Insert(string customerCode, Branch branch, bool copyEmptyDB = true)
		{
			if (branch == null)
			{
				_logger.Trace("Insert", "branch is null");
				return;
			}
			branch.CreateDate = DateTime.Now;
			branch.ModifyDate = DateTime.Now;
			branch.LastUpdatedCatalog = DateTime.Now;
			branch.CustomerCode = customerCode;
			this.Insert(branch);
		}

		public void Insert(Customer customer, Branch branch, bool copyEmptyDB = true)
		{
			if (branch == null)
			{
				_logger.Trace("Insert", "branch is null");
				return;
			}
			branch.CreateDate = DateTime.Now;
			branch.ModifyDate = DateTime.Now;
			branch.LastUpdatedCatalog = DateTime.Now;
			branch.CustomerCode = customer.Code;
			this.Insert(branch);
		}

        /// <summary>
        /// ?? TODO: подумать
        /// </summary>
        /// <param name="branches"></param>
        public void SetBranches(Branches branches)
        {
            throw new NotImplementedException();
        }

		public Branch GetBranchByCode(string branchCode)
		{
			if (string.IsNullOrEmpty(branchCode) == true) return null;

			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = this.GetEntityByCode(dc, branchCode);
                if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public Dictionary<string, Branch> FillBranchDictionary()
		{
			Dictionary<string, Branch> branchDictionary = new Dictionary<string, Branch>();

			Branches branchs = this.GetBranches();

			foreach (Branch branch in branchs)
			{
				branchDictionary[branch.Code] = branch;
			}

			return branchDictionary;
		}
        #endregion

		#region private

		private InventorConfigs FillInventorProperty(Branch branch)
		{

			InventorConfigs inventorConfigs = new InventorConfigs();
            if (branch != null)
            {
                inventorConfigs.Add(new InventorConfig { TypeObject = "DomainType", Description = DomainTypeEnum.Branch.ToString(), InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Code", Description = branch.Code, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "BranchCode", Description = branch.Code, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Name", Description = branch.Name, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "CustomerCode", Description = branch.CustomerCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Phone", Description = branch.Phone, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "DBPath", Description = branch.DBPath, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Description", Description = branch.Description, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Address", Description = branch.Address, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "BranchCodeERP", Description = branch.BranchCodeERP, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "BranchCodeLocal", Description = branch.BranchCodeLocal, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ContactPerson", Description = branch.ContactPerson, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportCatalogAdapterCode", Description = branch.ExportCatalogAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportIturAdapterCode", Description = branch.ExportIturAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Mail", Description = branch.Mail, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Fax", Description = branch.Fax, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportCatalogAdapterParms", Description = branch.ImportCatalogAdapterParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportCatalogProviderCode", Description = branch.ImportCatalogProviderCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportIturAdapterParms", Description = branch.ImportIturAdapterParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportIturProviderCode", Description = branch.ImportIturProviderCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportLocationAdapterParms", Description = branch.ImportLocationAdapterParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportLocationProviderCode", Description = branch.ImportLocationProviderCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportPDAAdapterParms", Description = branch.ImportPDAAdapterParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportPDAProviderCode", Description = branch.ImportPDAProviderCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "LogoFile", Description = branch.LogoFile, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportSectionAdapterCode", Description = branch.ImportSectionAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportSectionAdapterCode", Description = branch.ExportSectionAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "UpdateCatalogAdapterCode", Description = branch.UpdateCatalogAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportERPAdapterCode", Description = branch.ExportERPAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportSupplierAdapterCode", Description = branch.ImportSupplierAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
            }

            try
            {
                string branchDBPath = PropertiesSettings.FolderBranch.Trim('\\') + @"\" + branch.DBPath;
                CreateInfoFile(branch, branchDBPath, inventorConfigs);
            }
            catch { }

			return inventorConfigs;
		}

	
		private App_Data.Branch GetEntityByCode(App_Data.MainDB db, string code)
		{
			var entity = db.Branch.FirstOrDefault(e => e.Code.CompareTo(code) == 0);
			return entity;
		}
		#endregion

	
	}
}
