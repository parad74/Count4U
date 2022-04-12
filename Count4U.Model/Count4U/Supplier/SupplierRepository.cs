using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class SupplierRepository : ISupplierRepository
	{
        private Suppliers _list;

        #region ISupplierRepository Members

		public Suppliers GetStatuses(string pathDB)
        {
            if (this._list == null)
            {
                this._list = new Suppliers {
                    new Supplier() { ID = 1, Name = "Supplier1", Description = "Supplier1" },
                    new Supplier() { ID = 2, Name = "Supplier2", Description = "Supplier2" },
                };
            }
            return this._list;
        }

        #endregion

		#region ISupplierRepository Members

		public Suppliers GetSuppliers(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISupplierRepository Members


		public void Delete(Supplier supplier, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void DeleteAll(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Insert(Supplier supplier, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Insert(System.Collections.Generic.Dictionary<string, Supplier> dictionarySupplier, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Update(Supplier supplier, string pathDB)
		{
			throw new NotImplementedException();
		}

		public Supplier GetSupplierByName(string name, string pathDB)
		{
			throw new NotImplementedException();
		}

		public Supplier GetSupplierByCode(string code, string pathDB)
		{
			throw new NotImplementedException();
		}

        public int GetSuppliersTotal(string pathDB)
        {
            throw new NotImplementedException();
        }

        #endregion

		#region ISupplierRepository Members


		public System.Collections.Generic.Dictionary<string, Supplier> GetSupplierDictionary(string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public void ClearSupplierDictionary()
		{
			throw new NotImplementedException();
		}

		public void AddSupplierInDictionary(string code, Supplier location)
		{
			throw new NotImplementedException();
		}

		public void RemoveSupplierFromDictionary(string code)
		{
			throw new NotImplementedException();
		}

		public bool IsExistSupplierInDictionary(string code)
		{
			throw new NotImplementedException();
		}

		public Supplier GetSupplierByCodeFromDictionary(string code)
		{
			throw new NotImplementedException();
		}

		public void FillSupplierDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISupplierRepository Members


		public Suppliers GetSuppliers(SelectionParams.SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISupplierRepository Members


		public System.Collections.Generic.List<string> GetSupplierCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISupplierRepository Members


		public void RepairCodeFromDB(string pathDB)
		{
			throw new NotImplementedException();
		}

        public bool IsAnyInDb(string pathDB)
        {
            throw new NotImplementedException();
        }

        #endregion




		public void ReCountShilfSum(SelectionParams.SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}
	}
}
