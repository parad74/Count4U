using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;
using Count4U.Model.Audit.MappingEF;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using System.IO;
using System.Security.AccessControl;
using Count4U.Model.Count4U;
using NLog;
using System.Text;
using Count4U.Localization;

namespace Count4U.Model.Audit
{
    public class InventorEFRepository : BaseEFRepository, IInventorRepository
    {
        //private Inventor this._current;
        //private Inventors this._inventorList;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public InventorEFRepository(IConnectionDB connection)
            : base(connection)
        {
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

        #region IInventorRepository Members


        public Inventors GetInventors()
        {
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var domainObjects = dc.Inventor.ToList().Select(e => e.ToDomainObject());
                return Inventors.FromEnumerable(domainObjects);
            }
        }

        //public Inventors GetInventors(SelectParams selectParams, AuditConfig auditConfig)
        //{
        //    if (selectParams == null)
        //        return this.GetInventors(auditConfig);

        //    long totalCount = 0;
        //    using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString))
        //    {
        //        // Получение сущностей и общего количества из БД.
        //        // Getting entities and total count from database.
        //        var entities = GetEntities(dc, AsQueryable(dc.Inventor), dc.Inventor.AsQueryable(), selectParams, out totalCount);

        //        // Преобразование сущностей в объекты предметной области.
        //        // Converting entites to domain objects.
        //        var domainObjects = entities.Select(e => e.ToDomainObject());

        //        // Возврат результата.
        //        // Returning result.
        //        Inventors result = Inventors.FromEnumerable(domainObjects);
        //        result.TotalCount = totalCount;
        //        return result;
        //    }
        //}

        public Inventors GetInventors(SelectParams selectParams)
        {
            if (selectParams == null)
                return this.GetInventors();

            long totalCount = 0;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                // Получение сущностей и общего количества из БД.
                // Getting entities and total count from database.
                var entities = this.GetEntities(dc, AsQueryable(dc.Inventor), dc.Inventor.AsQueryable(), selectParams, out totalCount);

                // Преобразование сущностей в объекты предметной области.
                // Converting entites to domain objects.
                var domainObjects = entities.Select(e => e.ToDomainObject());

                // Возврат результата.
                // Returning result.
                Inventors result = Inventors.FromEnumerable(domainObjects);
                result.TotalCount = totalCount;
                return result;
            }
        }

        public void SetInventors(Inventors inventors)	  /// ?? TODO:
        {
            throw new NotImplementedException();
        }

        public Inventor GetInventorByCode(string inventorCode)
        {
            if (string.IsNullOrEmpty(inventorCode) == true) return null;

			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var entity = this.GetEntityByCode(dc, inventorCode);
                if (entity == null) return null;
                return entity.ToDomainObject();
            }
        }

        //public void SetCurrent(Inventor currentInventor)
        //{
        //    this._current = currentInventor;
        //}

        public void SetCurrent(Inventor currentInventor, AuditConfig auditConfig)
        {
            if (auditConfig == null) return;
            if (currentInventor == null)
            {
                auditConfig.ClearInventor();
            }
            else
            {
                if (auditConfig.InventorCode != currentInventor.Code)
                {
                    auditConfig.ClearInventor();
                    auditConfig.InventorCode = currentInventor.Code;
                    auditConfig.InventorName = currentInventor.Name;
                    auditConfig.InventorDate = currentInventor.InventorDate;
                }
            }
        }

        public Inventor GetCurrent(AuditConfig auditConfig)
        {
            if (auditConfig == null) return null;
            return this.GetInventorByCode(auditConfig.InventorCode);
        }

        public void SetStatus(Inventor inventor, string statusCode, string statusName)
        {
            if (inventor == null) return;
            inventor.StatusCode = statusCode;
            inventor.Status = statusName;
            this.Update(inventor);
        }

        public Inventors GetInventorsByBranchCode(string branchCode)
        {
            if (string.IsNullOrEmpty(branchCode) == true) return null;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var domainObjects = dc.Inventor.Where(e => e.BranchCode == branchCode)
                                                     .ToList().Select(e => e.ToDomainObject());
                return Inventors.FromEnumerable(domainObjects);
            }
        }

		public int GetInventorCountByBranchCode(string branchCode)
		{
			if (string.IsNullOrEmpty(branchCode) == true) return 0;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				var entety = dc.Inventor.Where(e => e.BranchCode == branchCode).ToList();
				if (entety == null) return 0;
				return entety.Count;
			}
		}

        public Inventors GetInventorsByCustomerCode(string customerCode)
        {
            if (string.IsNullOrEmpty(customerCode) == true) return null;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var domainObjects = dc.Inventor.Where(e => e.CustomerCode == customerCode)
                                                     .ToList().Select(e => e.ToDomainObject());
                return Inventors.FromEnumerable(domainObjects);
            }
        }

        public Inventors GetInventorsByStatus(string statusCode)
        {
            if (string.IsNullOrEmpty(statusCode) == true) return null;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var domainObjects = dc.Inventor.Where(e => e.StatusInventorCode == statusCode)
                                                     .ToList().Select(e => e.ToDomainObject());
                return Inventors.FromEnumerable(domainObjects);
            }
        }

        public Inventors GetInventorsByStatusCode(string statusCode)
        {
            if (string.IsNullOrEmpty(statusCode) == true) return null;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var domainObjects = dc.Inventor.Where(e => e.StatusInventorCode == statusCode)
                                                     .ToList().Select(e => e.ToDomainObject());
                return Inventors.FromEnumerable(domainObjects);
            }
        }

		public List<string> GetCodeList()
		{
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				var entity = dc.Inventor.Select(e => e.Code).Distinct().ToList();
				return entity;
			}
		}

		public List<string> GetInventorCodeListByCustomerCode(string customerCode)
		{
			if (string.IsNullOrEmpty(customerCode) == true) return new List<string>();
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				var entity = dc.Inventor.Where(e => e.CustomerCode == customerCode).Select(e => e.Code).Distinct().ToList();
				return entity;
			}
		}

		public List<string> GetInventorCodeListByBranchCode(string branchCode)
		{
			if (string.IsNullOrEmpty(branchCode) == true) return new List<string>();
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				var entity = dc.Inventor.Where(e => e.BranchCode == branchCode).Select(e => e.Code).Distinct().ToList();
				return entity;
			}
		}				 
	
        public void DeleteAllByBranchCode(string branchCode, bool full = true)
        {
            if (full == false) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var entities = dc.Inventor.Where(e => e.BranchCode == branchCode).ToList();
                if (entities == null) return;
                string folder = PropertiesSettings.FolderInventor.Trim('\\') + @"\";
                foreach (var entity in entities)
                {
                    this.Connection.RemoveDB(entity.DBPath, folder, false);
                }
                entities.ForEach(e => dc.Inventor.DeleteObject(e));
                dc.SaveChanges();
            }
        }

        public void DeleteAllByCustomerCode(string customerCode, bool full = true)
        {
            if (full == false) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var entities = dc.Inventor.Where(e => e.CustomerCode == customerCode).ToList();
                if (entities == null) return;
                string folder = PropertiesSettings.FolderInventor.Trim('\\') + @"\";

                foreach (var entity in entities)
                {
                    this.Connection.RemoveDB(entity.DBPath, folder, false);
                }
                entities.ForEach(e => dc.Inventor.DeleteObject(e));
                dc.SaveChanges();
            }
        }

        public void Delete(Inventor inventor, bool full = true)		  //TODO: удалить / перенести файл БД
        {
            if (inventor == null) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var entity = this.GetEntityByCode(dc, inventor.Code);
                if (entity == null) return;
                string folder = PropertiesSettings.FolderInventor.Trim('\\') + @"\";
                this.Connection.RemoveDB(entity.DBPath, folder, false);
                dc.Inventor.DeleteObject(entity);
                dc.SaveChanges();
            }
        }

        public void Delete(string code, bool full = true)	 			  //TODO: удалить / перенести файл БД
        {
            if (string.IsNullOrEmpty(code) == true) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var entity = this.GetEntityByCode(dc, code);
                if (entity == null) return;
                string folder = PropertiesSettings.FolderInventor.Trim('\\') + @"\";
                this.Connection.RemoveDB(entity.DBPath, folder, false);
                dc.Inventor.DeleteObject(entity);
                dc.SaveChanges();
            }
        }
     

        public void Insert(Inventor inventor, string inheritFromDBPath = null)
        {
  			if (inventor == null)
			{
				_logger.Trace("Insert", "inventor is null");
				return;
			}

			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var entity = inventor.ToEntity();
                string folder = PropertiesSettings.FolderInventor.Trim('\\') + @"\";

                if (!String.IsNullOrEmpty(inheritFromDBPath))
                {
					string sourceCount4UDPath = this.Connection.BuildCount4UDBFilePath(inheritFromDBPath);
					this.Connection.CopyCount4UDB(inventor.DBPath, folder, sourceCount4UDPath);

					string sourceAnaliticDBPath = this.Connection.BuildAnalyticDBFilePath(inheritFromDBPath);
					this.Connection.CopyAnaliticDB(inventor.DBPath, folder, sourceAnaliticDBPath);
					// если не было    AnaliticDB, то копируем пустую
					string relativePath = this.BuildRelativeDbPath(inventor);
					this.Connection.CopyEmptyAnaliticDB(relativePath);
                }
                else
                {
                    string pathDB = this.Connection.CopyEmptyCount4UAndAnaliticDB(inventor.DBPath, folder);
                }

                dc.Inventor.AddObject(entity);
                dc.SaveChanges();
            }

			InventorConfigs inventorConfigs = FillInventorProperty(inventor);

			string inventorDBPath = PropertiesSettings.FolderInventor.Trim('\\') + @"\" + inventor.DBPath;
			using (App_Data.Count4UDB db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(inventorDBPath)))
			{
					var list = db.InventorConfigs.ToList();
					if (list.Count > 0)
					{
						list.ForEach(e => db.InventorConfigs.DeleteObject(e));
						db.SaveChanges();
					}

				var entity = inventor.ToInventorConfigEntity();
				db.InventorConfigs.AddObject(entity);

				foreach (var inventorConfig in inventorConfigs)
				{
					db.InventorConfigs.AddObject(inventorConfig.ToEntity());
				}
				db.SaveChanges();
			}
        }

		public void Insert(Inventors inventors)
		{
			if (inventors == null) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				foreach (Inventor inventor in inventors)
				{
					if (inventor == null) { continue; }
					App_Data.Inventor entity = GetEntityByCode(dc, inventor.Code);
					// если нет объекта с таким кодом, только тогда добавляем
					if (entity == null)
					{
						entity = inventor.ToEntity();
						if (entity.InventorDate == DateTime.MinValue)	entity.InventorDate = DateTime.Now;
						if (entity.CreateDate == DateTime.MinValue)	entity.CreateDate = DateTime.Now;
						entity.LastUpdatedCatalog = DateTime.Now;
						entity.Restore = DateTime.Now.ToString();
						entity.RestoreBit = true;
						dc.Inventor.AddObject(entity);
					}
				}
				dc.SaveChanges();
			}
		}

		public string BuildRelativeDbPath(Inventor inventor)
		{
			return PropertiesSettings.FolderInventor.Trim('\\') + @"\" + inventor.DBPath;
	}

		public void InsertDomainInventorFromInventorConfig(Inventors inventors)
		{
			if (inventors == null) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				foreach (Inventor inventor in inventors)
				{
					if (inventor == null) { continue; }
					App_Data.Inventor entity = GetEntityByCode(dc, inventor.Code);
					if (entity == null)
					{
						entity = inventor.ToEntity();
						//entity.ModifyDate = DateTime.Now;
						if (entity.InventorDate == DateTime.MinValue) entity.InventorDate = DateTime.Now;
						if (entity.CreateDate == DateTime.MinValue) entity.CreateDate = DateTime.Now;
						entity.LastUpdatedCatalog = DateTime.Now;
						entity.Restore = DateTime.Now.ToString();
						entity.RestoreBit = true;
						dc.Inventor.AddObject(entity);
					}
				}
				dc.SaveChanges();
			}
 		}
	    
        public void Update(Inventor inventor)
        {
            if (inventor == null) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
            {
                var entity = this.GetEntityByCode(dc, inventor.Code);
                if (entity == null) return;
				//inventor.InventorDate = DateTime.Now; //test - убрать
                entity.ApplyChanges(inventor);
               dc.SaveChanges();
				
				var entityAuditConfig = dc.AuditConfig.FirstOrDefault(e => e.InventorCode.CompareTo(inventor.Code) == 0);
				if (entityAuditConfig == null) return;
				entityAuditConfig.InventorDate = entity.InventorDate != null ? Convert.ToDateTime(entity.InventorDate) : DateTime.Now;
				entityAuditConfig.InventorName = entity.Name;
				entityAuditConfig.DBPath = entity.DBPath;
				entityAuditConfig.Description = entity.Description;
				dc.SaveChanges();
            }

			string inventorRelativePath = BuildRelativeDbPath(inventor);
			this.Connection.CopyEmptyAnaliticDB(inventorRelativePath);

		this.RefillInventorConfigs(inventor);
        }

		// Update Inventor в БД AuditDB по InventorConfig  из БД Count4U, по заданному пути  pathDB
		public void UpdateDomainInventorByInventorConfig(Inventors inventors)
		{
			if (inventors == null) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				foreach (Inventor inventor in inventors)
				{
					if (inventor == null) { continue; }
					
					var entity = this.GetEntityByCode(dc, inventor.Code);
					if (entity == null) continue;
					entity.ApplyChanges(inventor);

					var entityAuditConfig = dc.AuditConfig.FirstOrDefault(e => e.InventorCode.CompareTo(inventor.Code) == 0);
					if (entityAuditConfig == null) return;
					entityAuditConfig.InventorDate = inventor.InventorDate;
					entityAuditConfig.InventorName = inventor.Name;
					entityAuditConfig.DBPath = inventor.DBPath;
					//entityAuditConfig.Description = inventor.Description;
					entityAuditConfig.CustomerCode = inventor.CustomerCode;
					entityAuditConfig.BranchCode = inventor.BranchCode;
					
					//this.RefillInventorConfigs(customer);
				}
				dc.SaveChanges();
			}
		}

		public void RefillInventorConfigs(Inventor inventor)
		{
			if (inventor == null) return;
			string inventorDBPath = PropertiesSettings.FolderInventor.Trim('\\') + @"\" + inventor.DBPath;
			string fullPath = this.Connection.BuildCount4UDBFilePath(inventorDBPath);
      
			if (String.IsNullOrWhiteSpace(fullPath) == true || File.Exists(fullPath) == false) return;

			InventorConfigs inventorConfigs = FillInventorProperty(inventor);

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(inventorDBPath)))
			{
				db.InventorConfigs.ToList().ForEach(e => db.InventorConfigs.DeleteObject(e));
				db.SaveChanges();

				var entity = inventor.ToInventorConfigEntity();
				db.InventorConfigs.AddObject(entity);

				foreach (var inventorConfig in inventorConfigs)
				{
					db.InventorConfigs.AddObject(inventorConfig.ToEntity());
				}

				db.SaveChanges();
			}


            //CreateInfoFile(inventor, inventorDBPath, inventorConfigs);
		}

        private void CreateInfoFile(Inventor inventor, string inventorDBPath, InventorConfigs inventorConfigs = null)
        {
            string inventorFullPath = this.Connection.BuildCount4UDBFolderPath(inventorDBPath);

            if (Directory.Exists(inventorFullPath) == true)
            {
                string[] files = Directory.GetFiles(inventorFullPath, "*.c4u", SearchOption.TopDirectoryOnly);
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
                string newFileInfo = DateTime.Now.ToString("ddMMyyyy") + "_Inventor_" + inventor.CustomerCode + "__" + inventor.BranchCode + ".c4u";
                string newFileInfoPath = inventorFullPath + newFileInfo;
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

		public void RefillInventorConfigsAllInventorsInAuditDB()
		{
			Inventors inventors = this.GetInventors();
			foreach (var inventor in inventors)
			{
				this.RefillInventorConfigs(inventor);
			}
		}
        #endregion

        #region private

		private InventorConfigs FillInventorProperty(Inventor inventor)
		{
			InventorConfigs inventorConfigs = new InventorConfigs();
            if (inventor != null)
            {
                inventorConfigs.Add(new InventorConfig { TypeObject = "DomainType", Description = DomainTypeEnum.Inventor.ToString(), InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Code", Description = inventor.Code, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "InventorCode", Description = inventor.Code, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Name", Description = inventor.Name, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "InventorDate", Description = inventor.InventorDate.ToString(), InventorDate = inventor.InventorDate, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "CustomerCode", Description = inventor.CustomerCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "BranchCode", Description = inventor.BranchCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "DBPath", Description = inventor.DBPath, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "Description", Description = inventor.Description, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportCatalogAdapterCode", Description = inventor.ImportCatalogAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportCatalogParms", Description = inventor.ImportCatalogParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportIturAdapterCode", Description = inventor.ImportIturAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportIturParms", Description = inventor.ImportIturParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportLocationAdapterCode", Description = inventor.ImportLocationAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportLocationParms", Description = inventor.ImportLocationParms, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "StatusCode", Description = inventor.StatusCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportSectionAdapterCode", Description = inventor.ImportSectionAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "UpdateCatalogAdapterCode", Description = inventor.UpdateCatalogAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportPDAProviderCode", Description = inventor.ImportPDAProviderCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
               // inventorConfigs.Add(new InventorConfig { TypeObject = "CompleteDate", Description = inventor.CompleteDate.ToString(), InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ExportERPAdapterCode", Description = inventor.ExportERPAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
                inventorConfigs.Add(new InventorConfig { TypeObject = "ImportSupplierAdapterCode", Description = inventor.ImportSupplierAdapterCode, InventorDate = DateTime.Now, ModifyDate = DateTime.Now, CreateDate = DateTime.Now });
            }

            try
            {
                string inventorDBPath = PropertiesSettings.FolderInventor.Trim('\\') + @"\" + inventor.DBPath;
                CreateInfoFile(inventor, inventorDBPath, inventorConfigs);
            }
            catch { }

			return inventorConfigs;
		}

        private App_Data.Inventor GetEntityByCode(App_Data.AuditDB dc, string code)
        {
            var entity = dc.Inventor.FirstOrDefault(e => e.Code.CompareTo(code) == 0);
            return entity;
        }


        #endregion


        public string GetCurrentCode(AuditConfig auditConfig)
        {
            if (auditConfig == null) return "";
            if (string.IsNullOrWhiteSpace(auditConfig.InventorCode) == true) return "";
            return auditConfig.InventorCode;

        }




    }
}
