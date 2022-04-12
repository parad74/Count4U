using System;
using System.Linq;
using Count4U.Model.Interface;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Main;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit.MappingEF;
using System.Data.Entity.Core.Objects;
using Count4U.Model.App_Data;
using System.IO;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;

namespace Count4U.Model.Audit
{
	public class AuditConfigEFRepository : BaseEFRepository, IAuditConfigRepository
	{
		private CBIContext _contextCBI;
		private AuditConfig _currentAuditConfig;
		private IServiceLocator _serviceLocator;
  
		public readonly IStatusAuditConfigRepository _statusAuditConfigRepository;

		public AuditConfigEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator
			//IStatusAuditConfigRepository statusAuditConfigRepository
            ) : base(connection)
        {
			this._serviceLocator = serviceLocator;
			//if (statusAuditConfigRepository == null) throw new ArgumentNullException("statusAuditConfigRepository");

			//this._statusAuditConfigRepository = statusAuditConfigRepository;
        }

		#region BaseEFRepository Members
		public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
		{
			return objectSet.AsQueryable();
		}
		#endregion

		public CBIContext ContextCBI
		{
			get { return this._contextCBI; }
			set { this._contextCBI = value; }
		}
	

		public AuditConfig CurrentAuditConfig
		{
			get { return this._currentAuditConfig; }
			set { this._currentAuditConfig = value; }
		}

		#region AuditConfig
		public AuditConfigs GetAuditConfigs(CBIContext contextCBI)			// TODO:
		{
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;					//TODO:
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig.ToList().Select(e => e.ToDomainObject());
						return AuditConfigs.FromEnumerable(domainObjects, this._serviceLocator);
					}
				default:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig.ToList().Select(e => e.ToDomainObject());
						return AuditConfigs.FromEnumerable(domainObjects, this._serviceLocator);
					}
			}
			
		}

		
		public AuditConfig GetProcessConfig(CBIContext contextCBI)		
		{
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return this._currentAuditConfig;			
				case CBIContext.Main:
					return this._currentAuditConfig;
				case CBIContext.History:
					//if (this._currentAuditConfig != null) return this._currentAuditConfig;

					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var entity = this.GetAuditConfigWithInProcessStatus(dc);
						if (entity == null) return null;

						var customerEntity = this.GetCustomerByCode(entity.CustomerCode);
						if (customerEntity != null) entity.CustomerName = customerEntity.Name;

						var branchEntity = this.GetBranchByCode(entity.BranchCode);
						if (branchEntity != null) entity.BranchName = branchEntity.Name;

						return entity.ToDomainObject();
						//this._currentAuditConfig = entity.ToDomainObject();
						//return this._currentAuditConfig;
					}
			default:
					return this._currentAuditConfig;
			
			}
		}

		public bool IsExistProcessConfig()
		{
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				var entity = this.GetAuditConfigWithInProcessStatus(dc);
				if (entity == null) return false;
				else return true;
			}
		}

		//get { return FileSystem.IsAppRedactionOffice() ? "OFFICE" : "LAPTOP";  }
		public void SetProcessConfig(CBIContext contextCBI, AuditConfig auditConfig)
		{
			this._contextCBI = contextCBI;
			this._currentAuditConfig = auditConfig;

			IIturAnalyzesRepository iturAnalyzesRepository = this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
			iturAnalyzesRepository.ClearProductDictionary();

			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					this._currentAuditConfig = auditConfig;
					break;
				case CBIContext.Main:
					this._currentAuditConfig = auditConfig;
					break;
				case CBIContext.History:
					{
						// Сбрасываем в БД статус текущего
						using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
						{
							this.StatusAuditConfigToNotCurrent(dc);
						}

						// Устанавливаем в БД текущий
						auditConfig.StatusAuditConfig = StatusAuditConfigEnum.InProcess.ToString();
						this.Update(auditConfig, contextCBI);

						//Устанавливаем текущий в программе
						//this._currentAuditConfig = auditConfig;
						break;
					}
				default:
					this._currentAuditConfig = auditConfig;
					break;
			}
		}

		public AuditConfig GetCurrentConfig(CBIContext contextCBI, bool fromDB = false)		
		{
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return this._currentAuditConfig;			
				case CBIContext.Main:
					return this._currentAuditConfig;
				case CBIContext.History:
					//2 вариант
					//return this._currentAuditConfig;
					//1 вариант
					if (fromDB == false)
					{
						if (this._currentAuditConfig == null)
						{
							AuditConfigs auditConfigs = this.GetAuditConfigs(contextCBI);
							if (auditConfigs.Count > 0)
							{
								this._currentAuditConfig = auditConfigs[0];
							}
						}
						return this._currentAuditConfig;
					}
					else
					{
						using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
						{
							if (this._currentAuditConfig != null)
							{
								this._currentAuditConfig = this.GetAuditConfigByCode(this._currentAuditConfig.Code,
									CBIContext.History);
							}
							return this._currentAuditConfig;
						}
					}
					//3 вариант
					//if (this._currentAuditConfig == null) throw new ArgumentNullException("_currentAuditConfig");
					//this._currentAuditConfig = this.GetAuditConfigByCode(this._currentAuditConfig.Code,
					//    CBIContext.History);
					//return this._currentAuditConfig;
			default:
					return this._currentAuditConfig;
			
			}
		}

		public void SetCurrentConfig(CBIContext contextCBI, AuditConfig auditConfig)	  //TODO: через БД
		{
			this._contextCBI = contextCBI;
			this._currentAuditConfig = auditConfig;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					this._currentAuditConfig = auditConfig;			
					break;
				case CBIContext.Main:
					this._currentAuditConfig = auditConfig;
					break;
				case CBIContext.History:
					this._currentAuditConfig = auditConfig;
					break;
					//{
					//    // Сбрасываем в БД статус текущего
					//    using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString))
					//    {
					//        this.StatusAuditConfigToNotCurrent(dc);
					//    }

					//    // Устанавливаем в БД текущий
					//    auditConfig.StatusAuditConfig = StatusAuditConfigEnum.Current.ToString();
					//    this.Update(auditConfig, contextCBI);

					//    //Устанавливаем текущий в программе
					//    this._currentAuditConfig = auditConfig;			  	
					//    break;
					//}
					
				default:
					this._currentAuditConfig = auditConfig;
					break;
			}
		}


		public AuditConfigs GetAuditConfigsLast(int count, CBIContext contextCBI)	
		{
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig.ToList()
							.OrderByDescending(e => e.InventorDate)
							.Select(e => e.ToDomainObject()).Skip(0).Take(count);
						return AuditConfigs.FromEnumerable(domainObjects, this._serviceLocator);
					}
				default:
					return null;
			}
		}

		public Customers GetHistoryCustomerLast()
		{
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				var domainObjects = dc.AuditConfig.ToList()
					.OrderByDescending(e => e.InventorDate)
					.Select(e => e.ToDomainObject());//.Skip(0).Take(count); ;
               Customers castomers =  AuditConfigs.CustomersFromEnumerable(domainObjects, this._serviceLocator);
               return castomers;
			}
		}
		public Branches GetHistoryBranchForCustomerLast(string customerCode, int count)
		{
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				var domainObjects = dc.AuditConfig.ToList()
						.Where(e => e.CustomerCode == customerCode)
						.OrderByDescending(e => e.InventorDate)
						.Select(e => e.ToDomainObject()).Skip(0).Take(count);
				return AuditConfigs.BranchesFromEnumerable(domainObjects, this._serviceLocator);
			}
		}


		public AuditConfigs GetAuditConfigsByCustomerCodeLast(int count, string customerCode,
			CBIContext contextCBI)
		{
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig.ToList()
							.Where(e => e.CustomerCode == customerCode)
							.OrderByDescending(e => e.InventorDate)
							.Select(e => e.ToDomainObject()).Skip(0).Take(count);
						return AuditConfigs.FromEnumerable(domainObjects, this._serviceLocator);
					}
				default:
					return null;
			}
		}

		public AuditConfigs GetAuditConfigsByBranchCodeLast(int count, string branchCode,
			CBIContext contextCBI)
		{
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig.ToList()
							.Where(e => e.BranchCode == branchCode)
							.OrderByDescending(e => e.InventorDate)
							.Select(e => e.ToDomainObject()).Skip(0).Take(count);
						return AuditConfigs.FromEnumerable(domainObjects, this._serviceLocator);
					}
				default:
					return null;
			}
		}

		public AuditConfig GetAuditConfigByCode(string code, CBIContext contextCBI)
		{
			if (string.IsNullOrEmpty(code) == true) return null;

			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var entity = this.GetEntityByCode(dc, code, contextCBI);
						if (entity == null) return null;

						var customerEntity = this.GetCustomerByCode(entity.CustomerCode);
						if (customerEntity != null) entity.CustomerName = customerEntity.Name;

						var branchEntity = this.GetBranchByCode(entity.BranchCode);
						if (branchEntity != null) entity.BranchName = branchEntity.Name;

						return entity.ToDomainObject();					
					}
				default:
					return null;
			}

		
		}

	
		public AuditConfigs GetAuditConfigsByCustomerCode(string customerCode, CBIContext contextCBI)
		{
			if (string.IsNullOrEmpty(customerCode) == true) return null;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig.Where(e => e.CustomerCode == customerCode)
															 .ToList().Select(e => e.ToDomainObject());
						return AuditConfigs.FromEnumerable(domainObjects, this._serviceLocator);
					}
				default:
					return null;
			}
	 	}

		public AuditConfigs GetAuditConfigsByBranchCode(string branchCode, CBIContext contextCBI)
		{
			if (string.IsNullOrEmpty(branchCode) == true) return null;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var domainObjects = dc.AuditConfig.Where(e => e.BranchCode == branchCode)
															 .ToList().Select(e => e.ToDomainObject());
						return AuditConfigs.FromEnumerable(domainObjects, this._serviceLocator);
					}
				default:
					return null;
			}
		}



		public AuditConfig GetAuditConfigByInventorCode(string inventorCode, CBIContext contextCBI)
		{
			if (string.IsNullOrEmpty(inventorCode) == true) return null;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var entity = dc.AuditConfig.FirstOrDefault(e => e.InventorCode.CompareTo(inventorCode) == 0);
						if (entity == null) return null;

						var customerEntity = this.GetCustomerByCode(entity.CustomerCode);
						if (customerEntity != null) entity.CustomerName = customerEntity.Name;

						var branchEntity = this.GetBranchByCode(entity.BranchCode);
						if (branchEntity != null) entity.BranchName = branchEntity.Name;

						return entity.ToDomainObject(); 
						//var domainObjects = dc.AuditConfig.Where(e => e.InventorCode == inventorCode)
						//                                     .ToList().Select(e => e.ToDomainObject());
						//return AuditConfigs.FromEnumerable(domainObjects);
					}	
				default:
					return null;
			}
	
		}

		public Inventors GetInventors()
		{
			Inventors inventors = new Inventors();
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				List<string> list = dc.AuditConfig.Select(e => e.InventorCode).Distinct().ToList();

				foreach (string inventorCode in list)
				{
					 var entety = dc.Inventor.FirstOrDefault(e => e.Code == inventorCode );
					if (entety == null) continue;
					inventors.Add(entety.ToDomainObject());
				}
				return inventors;
			}
		}

		public List<string> GetInventorCodeList()
		{
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				List<string> list = dc.AuditConfig.Select(e => e.InventorCode).Distinct().ToList();
				return list;
			}
		}

		public AuditConfig Clone(AuditConfig auditConfig)
        {
			AuditConfig cloneAuditConfig = new AuditConfig(auditConfig);
			string newCode = Guid.NewGuid().ToString();
			cloneAuditConfig.Code = newCode;
			return cloneAuditConfig;
        }

		public void Delete(AuditConfig auditConfig, CBIContext contextCBI)
        {
			if (auditConfig == null) return;
			if (string.IsNullOrEmpty(auditConfig.Code) == true) return;

			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					break;
				case CBIContext.Main:
					break;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var entity = this.GetEntityByCode(dc, auditConfig.Code, contextCBI);
						if (entity == null) return;
						dc.AuditConfig.DeleteObject(entity);
						dc.SaveChanges();
					}
					break;
				default:
					break;
			}
		}

		public void DeleteByInventorCode(string inventorCode, CBIContext contextCBI)
		{
			if (string.IsNullOrEmpty(inventorCode) == true) return;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return;
				case CBIContext.Main:
					return;
				case CBIContext.History:
					{
						using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
						{
							var entities = dc.AuditConfig.Where(e => e.InventorCode == inventorCode).ToList();
							if (entities == null) return;
							entities.ForEach(e => dc.AuditConfig.DeleteObject(e));
							dc.SaveChanges();
						}
						return;
					}
				default:
					return;
			}
		}

		public void DeleteAllByBranchCode(string branchCode, CBIContext contextCBI)
		{
			if (string.IsNullOrEmpty(branchCode) == true) return;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return;
				case CBIContext.Main:
					return;
				case CBIContext.History:
					{
						using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
						{
							var entities = dc.AuditConfig.Where(e => e.BranchCode == branchCode).ToList();
							if (entities == null) return;
							entities.ForEach(e => dc.AuditConfig.DeleteObject(e));
							dc.SaveChanges();
						}
						return;
					}
				default:
					return;
			}
		}

		public void DeleteAllByCustomerCode(string customerCode, CBIContext contextCBI)
		{
			if (string.IsNullOrEmpty(customerCode) == true) return;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return;
				case CBIContext.Main:
					return;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var entities = dc.AuditConfig.Where(e => e.CustomerCode == customerCode).ToList();
						if (entities == null) return;
						entities.ForEach(e => dc.AuditConfig.DeleteObject(e));
						dc.SaveChanges();
					}
				return;
				default:
					return;
			}
		}

		public void Insert(AuditConfig auditConfig, CBIContext contextCBI)
        {
			if (auditConfig == null) return;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						auditConfig.CreateDate = DateTime.Now;

						var customerEntity = this.GetCustomerByCode(auditConfig.CustomerCode);
						if (customerEntity != null) auditConfig.CustomerName = customerEntity.Name;

						var branchEntity = this.GetBranchByCode(auditConfig.BranchCode);
						if (branchEntity != null) auditConfig.BranchName = branchEntity.Name;

						var entity = auditConfig.ToEntity();
						dc.AuditConfig.AddObject(entity);
						dc.SaveChanges();
					}
					break;
				case CBIContext.Main:
					break;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						auditConfig.CreateDate = DateTime.Now;

						var customerEntity = this.GetCustomerByCode(auditConfig.CustomerCode);
						if (customerEntity != null) auditConfig.CustomerName = customerEntity.Name;

						var branchEntity = this.GetBranchByCode(auditConfig.BranchCode);
						if (branchEntity != null) auditConfig.BranchName = branchEntity.Name;

						var entity = auditConfig.ToEntity();
						dc.AuditConfig.AddObject(entity);
						dc.SaveChanges();
					}
					break;
				default:
					break;
			}
		
	    }

		public void Insert(AuditConfigs auditConfigs)
		{
			if (auditConfigs == null) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				foreach (AuditConfig auditConfig in auditConfigs)
				{
					if (auditConfig == null) { continue; }
					App_Data.AuditConfig entity = GetAuditConfigByInventorCode(dc, auditConfig.InventorCode);
					if (entity == null)
					{
						entity = auditConfig.ToEntity();
						if (entity.InventorDate == DateTime.MinValue) entity.InventorDate = DateTime.Now;
						if (entity.CreateDate == DateTime.MinValue) entity.CreateDate = DateTime.Now;

						var customerEntity = this.GetCustomerByCode(entity.CustomerCode);
						if (customerEntity != null) entity.CustomerName = customerEntity.Name;

						var branchEntity = this.GetBranchByCode(entity.BranchCode);
						if (branchEntity != null) entity.BranchName = branchEntity.Name;

						//auditConfigEntity.Restore = DateTime.Now.ToString();
						//auditConfigEntity.RestoreBit = true;
						dc.AuditConfig.AddObject(entity);
					}
				}
				dc.SaveChanges();
			}
		}

		public void InsertDomainAuditConfigFromInventorConfig(AuditConfigs auditConfigs)
		{
			if (auditConfigs == null) return;
			using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
			{
				foreach (AuditConfig auditConfig in auditConfigs)
				{
					if (auditConfig == null) { continue; }
					App_Data.AuditConfig entity = GetAuditConfigByInventorCode(dc, auditConfig.InventorCode);
					if (entity == null)
					{
						entity = auditConfig.ToEntity();
						//entity.ModifyDate = DateTime.Now;
						if (entity.InventorDate == DateTime.MinValue) entity.InventorDate = DateTime.Now;
						if (entity.CreateDate == DateTime.MinValue) entity.CreateDate = DateTime.Now;
						dc.AuditConfig.AddObject(entity);
					}
				}
				dc.SaveChanges();
			}
		}

		public AuditConfigs UpdateBranchNameByCode(string branchCode, string name, CBIContext contextCBI)
		{
			if (string.IsNullOrEmpty(branchCode) == true) return null;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					{

						AuditConfigs auditConfigs = this.GetAuditConfigsByBranchCode(branchCode, contextCBI);
						foreach (AuditConfig auditConfig in auditConfigs)
						{
							auditConfig.BranchName = name;
							this.Update(auditConfig, contextCBI);
						}
						return auditConfigs;
					}
				default:
					return null;
			}
		}

		public AuditConfigs UpdateCustomerNameByCode(string customerCode, string name, CBIContext contextCBI)
		{
			if (string.IsNullOrEmpty(customerCode) == true) return null;
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					return null;
				case CBIContext.Main:
					return null;
				case CBIContext.History:
					{
						AuditConfigs auditConfigs = this.GetAuditConfigsByCustomerCode(customerCode, contextCBI);
						foreach (AuditConfig auditConfig in auditConfigs)
						{
							auditConfig.CustomerName = name;
							this.Update(auditConfig, contextCBI);
						}
						return auditConfigs;
					}
				default:
					return null;
			}
		}

		public void Update(AuditConfig auditConfig, CBIContext contextCBI)
        {
			if (auditConfig == null) return;
			if (string.IsNullOrEmpty(auditConfig.Code) == true) return;
			
			switch (contextCBI)
			{
				case CBIContext.CreateInventor:
					break;
				case CBIContext.Main:
					break;
				case CBIContext.History:
					using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString()))
					{
						var entity = this.GetEntityByCode(dc, auditConfig.Code, contextCBI);
						if (entity == null) return;

						var customerEntity = this.GetCustomerByCode(auditConfig.CustomerCode);
						if (customerEntity != null) auditConfig.CustomerName = customerEntity.Name;

						var branchEntity = this.GetBranchByCode(auditConfig.BranchCode);
						if (branchEntity != null) auditConfig.BranchName = branchEntity.Name;

						entity.ApplyChanges(auditConfig);
						dc.SaveChanges();
					}
					break;
				default:
					break;
			}
	
        }

		public StatusAuditConfigs GetConfigStatus(CBIContext contextCBI)
		{
			//using (App_Data.AuditDB dc = new App_Data.AuditDB(this.AuditConnectionString))
			//{
			//    var domainObjects = dc.StatusAuditConfig.OrderBy(e => e.Code).Select(e => e.ToDomainObject());
			//    return StatusAuditConfigs.FromEnumerable(domainObjects);
			//}
			throw new NotImplementedException();
 		}

		public void SetAuditStatus(AuditConfig auditConfig, StatusInventor statusInventor, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public StatusInventor GetAuditStatus(CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region private
		private App_Data.AuditConfig GetEntityByCode(App_Data.AuditDB dc, string code, 
			CBIContext contextCBI)
		{
			var entity = dc.AuditConfig.FirstOrDefault(e => e.Code.CompareTo(code) == 0);
			return entity;
		}

		private App_Data.Inventor GetInventorByCode(App_Data.AuditDB dc, string inventorCode)
		{
			var entity = dc.Inventor.FirstOrDefault(e => e.Code.CompareTo(inventorCode) == 0);
			return entity;
		}

		private App_Data.Customer GetCustomerByCode(string customerCode)
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = dc.Customer.FirstOrDefault(e => e.Code.CompareTo(customerCode) == 0);
				return entity;
			}
		}

		private App_Data.Branch GetBranchByCode( string branchCode)
		{
			using (App_Data.MainDB dc = new App_Data.MainDB(this.MainDBConnectionString()))
			{
				var entity = dc.Branch.FirstOrDefault(e => e.Code.CompareTo(branchCode) == 0);
				return entity;
			}
		}

		private App_Data.AuditConfig GetAuditConfigByInventorCode(App_Data.AuditDB dc, string inventorCode)
		{
			var entity = dc.AuditConfig.FirstOrDefault(e => e.InventorCode.CompareTo(inventorCode) == 0);
			return entity;
		}

		private void StatusAuditConfigToNotCurrent(App_Data.AuditDB dc)
		{
			string notCurrent = StatusAuditConfigEnum.NotCurrent.ToString();
			var entitys = dc.AuditConfig.Where(e => e.StatusAuditConfig != notCurrent);
			foreach (var ac in entitys)
			{
				ac.StatusAuditConfig = notCurrent;
			}
			dc.SaveChanges();
		}

		//get { return FileSystem.IsAppRedactionOffice() ? "OFFICE" : "LAPTOP";  }
		private App_Data.AuditConfig GetAuditConfigWithInProcessStatus(App_Data.AuditDB dc)
		{
			string inProcess = StatusAuditConfigEnum.InProcess.ToString();
			try
			{
				var entity = dc.AuditConfig.FirstOrDefault(e => e.StatusAuditConfig.CompareTo(inProcess) == 0);
				return entity;
			}
			catch { return null; }
		}

	
			#endregion

	  }
}
