using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Audit;

namespace Count4U.Model.Interface.Main
{
    /// <summary>
    /// Интерфейс репозитория для доступа к магазинам - Branch объектам
    /// </summary>
    public interface IBranchRepository
    {
		string BuildRelativeDbPath(Branch branche);
        Branches GetBranches();

		Branches GetBranches(SelectParams selectParams);
		Branches GetBranches(CBIContext contextCBI);

	     void SetBranches(Branches branches);
		Branch GetBranchByCode(string branchCode);

        Branches GetBranchesByCustomer(Customer customer);

         Branches GetBranchesByCustomerCode(string customerCode, CBIContext contextCBI);
        Branches GetBranchesDetailsByCustomer(Customer customer);
		void SetCurrent(Branch currentBranch, AuditConfig auditConfig);
		
		Branch GetCurrent(AuditConfig auditConfig);

		string GetCurrentCode(AuditConfig auditConfig);

		List<string> GetCodeList();
		List<string> GetBranchCodeListByCustomerCode(string customerCode);
		void Delete(Branch branch);

      	void Delete(string branchCode);
		void Delete(List<string> branchCodeList);
		void DeleteDomainObjectOnly(List<string> branchCodeList);
		
		void DeleteAllByCustomerCode(string customerCode, bool full = false);

    	void Insert(Branches branchs);
		void Insert(string customerCode, Branch branch, bool copyEmptyDB = true);

  		void Insert(Customer customer, Branch branch, bool copyEmptyDB = true);
		void Insert(Branch branch, bool copyEmptyDB = true, string inheritFromDBPath = null);

		void Insert(Dictionary<string, Branch> branchToDBDictionary, bool copyEmptyDB = true);
		void InsertDomainBranchFromInventorConfig(Branches branches);
		void Update(Branch branch);
		void UpdateBranchName(Branch branch);
		void UpdateDomainBranchByInventorConfig(Branches branches);
		IConnectionDB Connection { get; set; }
		void RefillInventorConfigs(Branch branch);
		void RefillInventorConfigsAllBranchesInMainDB();
		Dictionary<string, Branch> FillBranchDictionary();
		void DeleteBranchWithDoubleCode(string description = "Repair");
	}
}
