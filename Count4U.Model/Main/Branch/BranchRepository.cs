using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Main.Mapping;
using Count4U.Model.Interface.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Audit;
using Count4U.Model.Interface;

namespace Count4U.Model.Main
{
    public class BranchRepository : IBranchRepository
    {
        private Branches _brancheList;

        #region IBranchRepository Members

        public Branches GetBranches()
        {
            if (this._brancheList == null)
            {
				this._brancheList = new Branches{
		                   new Branch {CustomerCode = "CustomerCode1", Code = "BranchCode1.1", Name="BranchName1.1", Phone="112-341", ContactPerson = "Contact Person", Address = "Mana street 6/9 \r\n Tel-Aviv, 42444\r\n"},
		                   new Branch {CustomerCode = "CustomerCode2", Code = "BranchCode2.1", Name="BranchName2.1", Phone="112-134", ContactPerson = "Contact Person", Address = "Mana street 6/9 \r\n Tel-Aviv, 42444\r\n"},
		                   new Branch {CustomerCode = "CustomerCode3", Code = "BranchCode3.1", Name="BranchName3.1", Phone="112-334", ContactPerson = "Contact Person", Address = "Mana street 6/9 \r\n Tel-Aviv, 42444\r\n"},
   		                   new Branch {CustomerCode = "CustomerCode1", Code = "BranchCode1.2", Name="BranchName1.2", Phone="112-434", ContactPerson = "Contact Person", Address = "Mana street 6/9 \r\n Tel-Aviv, 42444\r\n"},
		                   new Branch {CustomerCode = "CustomerCode2", Code = "BranchCode2.2", Name="BranchName2.2", Phone="112-634", ContactPerson = "Contact Person", Address = "Mana street 6/9 \r\n Tel-Aviv, 42444\r\n"},
		                   new Branch {CustomerCode = "CustomerCode3", Code = "BranchCode3.2", Name="BranchName3.2", Phone="112-345", ContactPerson = "Contact Person", Address = "Mana street 6/9 \r\n Tel-Aviv, 42444\r\n"},
   		                   new Branch {CustomerCode = "CustomerCode1", Code = "BranchCode1.3", Name="BranchName1.3", Phone="112-534", ContactPerson = "Contact Person", Address = "Mana street 6/9 \r\n Tel-Aviv, 42444\r\n"},
		                   new Branch {CustomerCode = "CustomerCode2", Code = "BranchCode2.3", Name="BranchName2.3", Phone="112-364", ContactPerson = "Contact Person", Address = "Mana street 6/9 \r\n Tel-Aviv, 42444\r\n"},
		                   new Branch {CustomerCode = "CustomerCode3", Code = "BranchCode3.3", Name="BranchName3.3", Phone="112-734", ContactPerson = "Contact Person", Address = "Mana street 6/9 \r\n Tel-Aviv, 42444\r\n"}
		               };
            }
			return this._brancheList;
        }

        public Branches GetBranchesByCustomer(Customer customer)
        {
			Branches branches = this.GetBranchesByCustomerCode(customer.Code, CBIContext.Main);
			return branches;
        }

		public Branches GetBranchesByCustomerCode(string customerCode, CBIContext contextCBI)
        {
			Branches branches = this.GetBranches();
            var branches1 = Branches.FromEnumerable(from c in branches
                            where c.CustomerCode == customerCode
                            select
                                new Branch { CustomerCode = c.CustomerCode, Code = c.Code, Name = c.Name, Phone = c.Phone, ContactPerson = c.ContactPerson, Address = c.Address});
            return branches1;

         }

        public Branches GetBranchesDetailsByCustomer(Customer customer)
        {
			Branches branches = this.GetBranchesByCustomerCode(customer.Code, CBIContext.Main);
			return branches;
        }

		/// <summary>
		/// Присвоить текущему списку  - список branches
		/// </summary>
		/// <returns></returns>
		public void SetBranches(Branches branches)		//TODO:
		{
			this._brancheList = branches;
		}

		public void SetCurrent(Branch currentBranch)
		{
			this._brancheList.CurrentItem = currentBranch;
		}

		public Branch GetCurrent()
		{
			return this._brancheList.CurrentItem;
		}

		//public Branch Clone(Branch branch)
		//{
		//    var domainObject = branch.Clone();
		//    domainObject.ID = 0;
		//    return domainObject;
		//}

		public void Delete(Branch branch)
		{
			var entity = this.GetEntityByCode(branch.Code);
			if (entity == null) return;
			this.GetBranches().Remove(entity);
		}

	
		public void Delete(string branchCode)
		{
			var entity = this.GetEntityByCode(branchCode);
			if (entity == null) return;
			this.GetBranches().Remove(entity);
		}

		public void Insert(Branch branch, bool copyEmptyDB = true, string inheritFromDBPath = null)
		{
			if (branch == null) return;
			var entity = branch.ToEntity();
			this.GetBranches().Add(entity);
		}

	
		public void Update(Branch branch)
		{
			if (branch == null) return;
			var entity = this.GetEntityByCode(branch.Code);
			if (entity == null) return;
			entity.ApplyChanges(branch);
		}

		public void DeleteAllByCustomerCode(string customerCode)
		{
			Branches branches = this.GetBranchesByCustomerCode(customerCode, CBIContext.Main);
			foreach (Branch branch in branches)
			{
				var entity = this.GetEntityByCode(branch.Code);
				if (entity != null)
				{
					this.GetBranches().Remove(entity);								 // TODO: проверить как отрабатывает
				}
			}
		}

		public void Insert(string customerCode, Branch branch, bool copyEmptyDB = true)
		{
			if (branch == null) return;
			var entity = branch.ToEntity();
			entity.CustomerCode = customerCode;
			this.GetBranches().Add(entity);
		}

		public void Insert(Customer customer, Branch branch, bool copyEmptyDB = true)
		{
			if (customer == null) return;
			if (branch == null) return;
			var entity = branch.ToEntity();
			entity.CustomerCode = customer.Code;
			this.GetBranches().Add(entity);
		}
  
		public Branch GetBranchByCode(string branchCode)
		{
			var entity = this.GetEntityByCode(branchCode);
			if (entity == null) return null;
			return entity.ToDomainObject();
		}

		#endregion

		#region private

		private Branch GetEntityByCode(string branchCode)
		{
			var entity = this.GetBranches().First(e => e.Code.CompareTo(branchCode) == 0);
			return entity;
		}

		#endregion

		#region IBranchRepository Members


		public Branches GetBranches(SelectParams selectParams)
		{
			throw new NotImplementedException();
		}

		public Branches GetBranches(SelectParams selectParams, AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		public void SetCurrent(Branch currentBranch, AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		public Branch GetCurrent(AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		public string GetCurrentCode(AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public void Delete(Branch branch, bool full = false)
		{
			throw new NotImplementedException();
		}

		public void Delete(string branchCode, bool full = false)
		{
			throw new NotImplementedException();
		}

		public void DeleteAllByCustomerCode(string customerCode, bool full = false)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


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

		#region IBranchRepository Members


		public Branches GetBranches(CBIContext contextCBI)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public List<string> GetCodeList()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public void RefillInventorConfigs(Branch branch)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public Dictionary<string, Branch> FillBranchDictionary()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public void Insert(Dictionary<string, Branch> branchToDBDictionary, bool copyEmptyDB = true)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public void Insert(Branches branchs)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public void RefillInventorConfigsAllBranchesInMainDB()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public void InsertDomainBranchFromInventorConfig(Branches branches)
		{
			throw new NotImplementedException();
		}

		public void UpdateDomainBranchByInventorConfig(Branches branches)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public void Delete(List<string> branchCodeList)
		{
			throw new NotImplementedException();
		}

		#endregion


		public void DeleteBranchWithDoubleCode(string description = "Repair")
		{
			throw new NotImplementedException();
		}


		public List<string> GetBranchCodeListByCustomerCode(string customerCode)
		{
			throw new NotImplementedException();
		}

		#region IBranchRepository Members


		public void UpdateBranchName(Branch branch)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members

		public string BuildRelativeDbPath(Branch branche)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public void DeleteDomainObjectOnly(List<string> branchCodeList)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		Interface.IConnectionDB IBranchRepository.Connection
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
	}
}
