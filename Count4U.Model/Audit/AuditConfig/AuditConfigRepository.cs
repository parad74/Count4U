using System;
using System.Linq;
using Count4U.Model.Interface;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface.Main;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit.Mapping;
using Count4U.Model.App_Data;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Audit
{
	public class AuditConfigRepository : IAuditConfigRepository
	{
		 
		private AuditConfigs _auditConfigList;
		private IServiceLocator _serviceLocator;

		public AuditConfigRepository(IServiceLocator serviceLocator)
        {
			this._serviceLocator = serviceLocator;
        }
	
		#region AuditConfig
		public AuditConfigs GetAuditConfigs(CBIContext contextCBI)
		{
			//Скорее список есть и надо добавить новый элемент?
			if (this._auditConfigList == null)
			{
				this._auditConfigList = new AuditConfigs();
			}
			return this._auditConfigList;
		}

		public AuditConfig GetCurrentConfig(CBIContext contextCBI, bool fromDB = false)
		{
			return this._auditConfigList.CurrentItem;
		}

		public void SetCurrentConfig(CBIContext contextCBI, AuditConfig auditConfig)
		{
			this._auditConfigList.CurrentItem = auditConfig;
		}

		public AuditConfigs GetAuditConfigsLast(int count, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}


		public AuditConfig GetAuditConfigByCode(string code, CBIContext contextCBI)
		{
			AuditConfig auditConfig = this.GetEntityByCode(code, contextCBI);
			return auditConfig;
		}

		public AuditConfigs GetAuditConfigsByCustomerCode(string customerCode, CBIContext contextCBI)
		{
			var domainObjects = this.GetAuditConfigs(contextCBI).Where(e => e.CustomerCode.CompareTo(customerCode) == 0)
										 .ToList().Select(e => e.ToDomainObject());
			return AuditConfigs.FromEnumerable(domainObjects, this._serviceLocator);
		}

		public AuditConfigs GetAuditConfigsByBranchCode(string branchCode, CBIContext contextCBI)
		{
			var domainObjects = this.GetAuditConfigs(contextCBI).Where(e => e.CustomerCode.CompareTo(branchCode) == 0)
								 .ToList().Select(e => e.ToDomainObject());
			return AuditConfigs.FromEnumerable(domainObjects, this._serviceLocator);

		}

		public AuditConfig GetAuditConfigByInventorCode(string inventorCode, CBIContext contextCBI)
		{
			var entity = this.GetAuditConfigs(contextCBI).First(e => e.InventorCode.CompareTo(inventorCode) == 0);
			return entity;
		}

		public AuditConfig Clone(AuditConfig inventorConfig)
        {

			return new AuditConfig();
        }

		public void Delete(AuditConfig inventorConfig, CBIContext contextCBI)
        {
			var entity = this.GetEntityByCode(inventorConfig.DirtyInventorCode, contextCBI);
			if (entity == null) return;
			this.GetAuditConfigs(contextCBI).Remove(entity);
        }

		public void Insert(AuditConfig inventorConfig, CBIContext contextCBI)
        {
			if (inventorConfig == null) return;
			var entity = inventorConfig.ToEntity();
			this.GetAuditConfigs(contextCBI).Add(entity);
        }

		public void Update(AuditConfig inventorConfig, CBIContext contextCBI)
        {
			if (inventorConfig == null) return;
			var entity = this.GetEntityByCode(inventorConfig.Code, contextCBI);
			if (entity == null) return;
			entity.ApplyChanges(inventorConfig);
        }

		public StatusAuditConfigs GetConfigStatus(CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public void SetAuditStatus(AuditConfig auditConfig, StatusInventor statusAudit, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public StatusInventor GetAuditStatus(CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public AuditConfigs GetAuditConfigsByCustomerCodeLast(int count, string customerCode, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public AuditConfigs GetAuditConfigsByBranchCodeLast(int count, string branchCode, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public AuditConfig GetProcessConfig(CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public void SetProcessConfig(CBIContext contextCBI, AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		public Customers GetHistoryCustomerLast()
		{
			throw new NotImplementedException();
		}

		public Branches GetHistoryBranchForCustomerLast(string customerCode, int count)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region private
		private AuditConfig GetEntityByCode(string code, CBIContext contextCBI)
		{
  			var entity = this.GetAuditConfigs(contextCBI).First(e => e.Code.CompareTo(code) == 0);
			return entity;
		}


		#endregion

		#region IAuditConfigRepository Members


		public bool IsExistProcessConfig()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IAuditConfigRepository Members


		public AuditConfigs UpdateBranchNameByCode(string branchCode, string name, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public AuditConfigs UpdateCustomerNameByCode(string customerCode, string name, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IAuditConfigRepository Members


		public void DeleteAllByBranchCode(string branchCode, string name, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public void DeleteAllByCustomerCode(string customerCode, string name, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IAuditConfigRepository Members


		public void DeleteAllByBranchCode(string branchCode, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		public void DeleteAllByCustomerCode(string customerCode, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IAuditConfigRepository Members


		public void DeleteByInventorCode(string inventorCode, CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IAuditConfigRepository Members


		public void Insert(AuditConfigs auditConfigs)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IAuditConfigRepository Members


		public void InsertDomainAuditConfigFromInventorConfig(AuditConfigs auditConfigs)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IAuditConfigRepository Members


		public Inventors GetInventors()
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.List<string> GetInventorCodeList()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
