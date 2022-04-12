using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Main;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using Count4U.Model.Audit;

namespace Count4U.Model.Main
{
    public class BranchDBRepository : BaseEFRepository, IBranchRepository
    {
        private readonly string _configurationMainDBConnectionString;

  		public BranchDBRepository(ConnectionDB connection)
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
 
		#region IBranchRepository Members

        public Branches _branches;

        public Branches GetBranches()
        {
			//App_Data.MainDB dc = new App_Data.MainDB(Properties.Settings.Default.MainDBConnectionString);
            App_Data.MainDB dc = new App_Data.MainDB(this._configurationMainDBConnectionString);
            if (this._branches == null)
            {
				this._branches = new Branches();
				var brs = from b in dc.Branch
								select new { Code = b.Code, CustomerCode = b.CustomerCode, Name = b.Name, Phone = b.Phone  };

				foreach (var br in brs)
					{
                        Branch newBranch = new Branch { CustomerCode = br.CustomerCode, Code = br.Code, Name = br.Name, Phone = br.Phone };
						this._branches.Add(newBranch);
					}
				}
            return this._branches;
        }

        public Branches GetBranchesByCustomer(Customer customer, CBIContext contextCBI)
        {
            return GetBranchesByCustomerCode(customer.Code, CBIContext.Main);
        }

        public Branches GetBranchesDetailsByCustomer(Customer customer)
        {
            return GetBranchesByCustomerCode(customer.Code, CBIContext.Main);
        }

        public Branches GetBranchesByCustomerCode(string customerCode, CBIContext contextCBI)
        {
            Branches branches = GetBranches();
            var branches1 = Branches.FromEnumerable(from b in branches
                            where b.CustomerCode == customerCode
                            select
                                new Branch { CustomerCode = b.CustomerCode, Code = b.Code, Name = b.Name, Phone = b.Phone });
            return branches1;

          }

   		public Branch Clone(Branch branch)
		{
			throw new NotImplementedException();
		}

		public void Delete(Branch branch)
		{
			throw new NotImplementedException();
		}

		public void Delete(string branchCode)
		{
			throw new NotImplementedException();
		}

		public void Update(Branch branch)
		{
			throw new NotImplementedException();
		}

		public void UpdateBranchName(Branch branch)
		{
			throw new NotImplementedException();
		}

		public void DeleteAllByCustomerCode(string customerCode)
		{
			throw new NotImplementedException();
		}

		

        /// <summary>
        /// ?? TO DO подумать
        /// </summary>
        /// <param name="branches"></param>
        public void SetBranches(Branches branches)
        {
            throw new NotImplementedException();
        }

		public Branch GetBranchByCode(string branchCode)
		{
			throw new NotImplementedException();
		}
        #endregion

		#region IBranchRepository Members


		public void SetCurrent(Branch currentBranch)
		{
			throw new NotImplementedException();
		}

		public Branch GetCurrent()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public Branches GetBranches(SelectParams selectParams)
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


		public Branches GetBranchesByCustomer(Customer customer)
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

		public Dictionary<string, Branch> FillBranchDictionary()
		{
			throw new NotImplementedException();
		}

		#region IBranchRepository Members


		public void Insert(Dictionary<string, Branch> branchToDBDictionary, bool copyEmptyDB = true)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBranchRepository Members


		public void Insert(string customerCode, Branch branch, bool copyEmptyDB = true)
		{
			throw new NotImplementedException();
		}

		public void Insert(Customer customer, Branch branch, bool copyEmptyDB = true)
		{
			throw new NotImplementedException();
		}

		public void Insert(Branch branch, bool copyEmptyDB = true, string inheritFromDBPath = null)
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

		public string BuildRelativeDbPath(Branch branche)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
