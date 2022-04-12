using System;
using Count4U.Model.Count4U;
using Count4U.Model.Audit;
using Count4U.Model.Main;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к InventorConfig объектам
    /// </summary>
	public interface IInventorConfigRepository
	{
		/// <summary>
		void ClearInventorConfigs(string pathDB);
		//void Delete(string inventorConfigCode, string pathDB);
		AuditConfigs GetAuditConfigsFromInventorConfigs(string pathDB);
		InventorConfig GetCurrentConfig(string pathDB);
		//InventorConfig GetInventorConfig(string pathDB);
		string GetInventorStatusCode(string pathDB);
		void Insert(AuditConfig auditConfig, string pathDB);
		void Insert(InventorConfig inventorConfig, string pathDB);
		void SetCurrentConfig(InventorConfig inventorConfig, string pathDB);
		void SetCurrentConfig(AuditConfig auditConfig, string pathDB);
		void SetInventorStatus(InventorConfig inventorConfig, string statusInventorConfigCode, string pathDB);
		void Update(InventorConfig inventorConfig, string pathDB);
		Branch BranchFromInventorConfigs(string pathDB, bool addParentObject = false);
		Customer CustomerFromInventorConfigs(string pathDB);
		Inventor InventorFromInventorConfigs(string pathDB, bool addParentObject = false);
		AuditConfig AuditConfigFromInventorConfigs(string pathDB, bool addParentObject = false);
		void RepairDomainCustomer();
		void RepairDomainCustomer(string pathDB, bool updateByInventorConfig = false);
		void RepairDomainBranch();
		void RepairDomainBranch(string pathDB, bool updateByInventorConfig = false);
		void RepairDomainInventor();
		void RepairDomainInventor(string pathDB, bool updateByInventorConfig = false);
		bool IsCustomerCount4UDB(string PathDB, out string customerCode);
		bool IsBranchCount4UDB(string PathDB, out string customerCode, out string branchCode);
		bool IsInventorCount4UDB(string PathDB, out string customerCode, out string branchCode, out string inventorCode);
		void RestoreDomainObject(string pathDB);
		void TestRestoreDomainObject_Inventor();
		string GetPathDBFromRelativePathDB(string relativePathDB, string subFolderDomainObject, out string codeDomainObject);
		string GetObjectCodeFromRelativePathDB(string relativePathDB);
		//void RepairDomainAuditConfig();
		//void RepairDomainAuditConfig(string pathDB, bool updateByInventorConfig = false);

	}
}
