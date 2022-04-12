using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;
using Count4U.Model.Audit.Mapping;
using Count4U.Model.Interface;

namespace Count4U.Model.Audit
{
    public class InventorRepository : IInventorRepository
	{
        private  Inventors _inventorList;
	
        #region IInventorRepository Members

        public Inventors GetInventors()
        {
			if (this._inventorList == null)
            {
				this._inventorList = new Inventors{
		                   new Inventor {Code = "InventorCode1", CustomerCode = "CustomerCode1",  BranchCode = "BranchCode1.1", Name="InventorName1.1.1"},
		                   new Inventor {Code = "InventorCode2", CustomerCode = "CustomerCode1",  BranchCode = "BranchCode1.1", Name="InventorName1.1.2"},
		                   new Inventor {Code = "InventorCode3", CustomerCode = "CustomerCode2", BranchCode = "BranchCode2.1", Name="InventorName2.1.1"},
		                   new Inventor {Code = "InventorCode4", CustomerCode = "CustomerCode2", BranchCode = "BranchCode2.1", Name="InventorName2.1.2"},
		                   new Inventor {Code = "InventorCode5", CustomerCode = "CustomerCode3", BranchCode = "BranchCode3.1", Name="InventorName3.1.1"},
		                   new Inventor {Code = "InventorCode6", CustomerCode = "CustomerCode3", BranchCode = "BranchCode3.1", Name="InventorName3.1.2"},
   		                   new Inventor {Code = "InventorCode7", CustomerCode = "CustomerCode1", BranchCode = "BranchCode1.2", Name="InventorName1.2.1"},
   		                   new Inventor {Code = "InventorCode8", CustomerCode = "CustomerCode1", BranchCode = "BranchCode1.2", Name="InventorName1.2.2"},
		                   new Inventor {Code = "InventorCode9", CustomerCode = "CustomerCode2", BranchCode = "BranchCode2.2", Name="InventorName2.2.1"},
		                   new Inventor {Code = "InventorCode10", CustomerCode = "CustomerCode2", BranchCode = "BranchCode2.2", Name="InventorName2.2.2"},
		                   new Inventor {Code = "InventorCode11", CustomerCode = "CustomerCode3", BranchCode = "BranchCode3.2", Name="InventorName3.2.1"},
   		                   new Inventor {Code = "InventorCode12", CustomerCode = "CustomerCode1", BranchCode = "BranchCode1.3", Name="InventorName1.3.1"},
		                   new Inventor {Code = "InventorCode13", CustomerCode = "CustomerCode2", BranchCode = "BranchCode2.3", Name="InventorName2.3.1"},
   		                   new Inventor {Code = "InventorCode14", CustomerCode = "CustomerCode1", BranchCode = "BranchCode1.3", Name="InventorName1.3.2"},
		                   new Inventor {Code = "InventorCode15", CustomerCode = "CustomerCode3", BranchCode = "BranchCode3.3", Name="InventorName3.3.1"},
		                   new Inventor {Code = "InventorCode16", CustomerCode = "CustomerCode3", BranchCode = "BranchCode3.3", Name="InventorName3.3.2"},
		                   new Inventor {Code = "InventorCode17", CustomerCode = "CustomerCode3", BranchCode = "BranchCode3.3", Name="InventorName3.3.3"},
		                   new Inventor {Code = "InventorCode18", CustomerCode = "CustomerCode3", BranchCode = "BranchCode3.3", Name="InventorName3.3.4"}
		               };
            }
			return this._inventorList;
       }

        public void SetInventors(Inventors inventors)  //TODO: ?
        {
			this._inventorList = inventors;
        }

		public void SetCurrent(Inventor currentInventor)
		{
			this._inventorList.CurrentItem = currentInventor;
		}

		public  Inventor GetCurrent()
		{
			return this._inventorList.CurrentItem;
		}

		public Inventor GetInventorByCode(string inventorCode)
        {
			var entity = this.GetEntityByCode(inventorCode);
			if (entity == null) return null;
			return entity.ToDomainObject();
        }
 	
		public void SetStatus(Inventor inventor,  string statusCode, string statusName )
		{
			//Inventor inventor = GetInventorByCode(inventor.Code);
			inventor.StatusCode = statusCode;
			inventor.Status = statusName;							// TODO: проверить как отрабатывает
		}

        public Inventor GetInventorByDirtyCode(string dirtyCode)
        {
			//var entity = this.GetInventors().First(e => e.ImportCatalogAdapterCode.CompareTo(dirtyCode) == 0);
			//return entity;
			throw new NotImplementedException();
        }

  
        public Inventors GetInventorsByBranchCode(string branchCode)
        {
			var entity = this.GetInventors().Where(e => e.BranchCode.CompareTo(branchCode) == 0)
							.ToList().Select(e => e.ToDomainObject());
			return Inventors.FromEnumerable(entity);
        }

        public Inventors GetInventorsByCustomerCode(string customerCode)
        {
			var entity = this.GetInventors().Where(e => e.CustomerCode.CompareTo(customerCode) == 0)
						.ToList().Select(e => e.ToDomainObject());
			return Inventors.FromEnumerable(entity);
        }

        public Inventors GetInventorsByStatus(string status)
        {
			var domainObjects = this.GetInventors().Where(e => e.Status.CompareTo(status) == 0)
								  .ToList().Select(e => e.ToDomainObject());
			return Inventors.FromEnumerable(domainObjects);
        }

		public Inventors GetInventorsByStatusCode(string statusCode)
		{
			var domainObjects = this.GetInventors().Where(e => e.StatusCode.CompareTo(statusCode) == 0)
						  .ToList().Select(e => e.ToDomainObject());
			return Inventors.FromEnumerable(domainObjects);

		}

		//public Inventor Clone(Inventor inventor)
		//{
		//    var domainObject = inventor.Clone();
		//    domainObject.ID = 0;
		//    domainObject.Code = "";
		//    domainObject.ImportCatalogAdapterCode = "";
		//    return domainObject;
		//}

        public void Delete(Inventor inventor)
        {
			var entity = this.GetEntityByCode(inventor.Code);
			if (entity == null) return;
			this.GetInventors().Remove(entity);

        }

		public void Delete(string inventorCode)
		{
			var entity = this.GetEntityByCode(inventorCode);
			if (entity == null) return;
			this.GetInventors().Remove(entity);
		}

        public void Insert(Inventor inventor, string inheritFromDBPath = null)
		{
			if (inventor == null) return;
			var entity = inventor.ToEntity();
			this.GetInventors().Add(entity);
        }

        public void Update(Inventor inventor)
        {
			if (inventor == null) return;
			var entity = this.GetEntityByCode(inventor.Code);
			if (entity == null) return;
			entity.ApplyChanges(inventor);
        }

  
        #endregion

		#region private

		private Inventor GetEntityByCode(string inventorCode)
		{
			var entity = this.GetInventors().FirstOrDefault(e => e.Code.CompareTo(inventorCode) == 0);
			return entity;
		}
	
		public Inventors GetInventors(SelectionParams.SelectParams selectParams)
		{
			throw new NotImplementedException();
		}

		public void SetCurrent(Inventor currentInventor, AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		public Inventor GetCurrent(AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public string GetCurrentCode(AuditConfig auditConfig)
		{
			throw new NotImplementedException();
		}

		//public string CopyEmptyDB(string dbPath)
		//{
		//    throw new NotImplementedException();
		//}

		#endregion

		#region IInventorRepository Members


		public void DeleteAllByBranchCode(string branchCode)
		{
			throw new NotImplementedException();
		}

		public void DeleteAllByCustomerCode(string customerCode)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public void DeleteAllByBranchCode(string branchCode, bool full = true)
		{
			throw new NotImplementedException();
		}

		public void DeleteAllByCustomerCode(string customerCode, bool full = true)
		{
			throw new NotImplementedException();
		}

		public string RemoveDB(string dbPath, bool full = false)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public void Delete(Inventor inventor, bool full = true)
		{
			throw new NotImplementedException();
		}

		public void Delete(string code, bool full = true)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public string CopyEmptyDB(string dbPath, string folder)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


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

		#region IInventorRepository Members


		public List<string> GetCodeList()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public void RefillInventorConfigs(Inventor inventor)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public void Insert(Inventors inventors)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public void RefillInventorConfigsAllInventorsInAuditDB()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public void InsertDomainInventorFromInventorConfig(Inventors inventors)
		{
			throw new NotImplementedException();
		}

		public void UpdateDomainInventorByInventorConfig(Inventors inventors)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public int GetInventorCountByBranchCode(string branchCode)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public List<string> GetInventorCodeListByCustomerCode(string customerCode)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members


		public List<string> GetInventorCodeListByBranchCode(string branchCode)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IInventorRepository Members

		public string BuildRelativeDbPath(Inventor inventor)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
