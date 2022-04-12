using System;
using Count4U.Model.Count4U;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Count4U
{
	/// <summary>
	/// Интерфейс репозитория для доступа к коду поставщика - Supplier объектам
	/// </summary>
	public interface ISupplierRepository
	{
		Suppliers GetSuppliers(string pathDB);
		Suppliers GetSuppliers(SelectParams selectParams, string pathDB);
		void Delete(Supplier supplier, string pathDB);
		void DeleteAll(string pathDB);
		void Insert(Supplier supplier, string pathDB);
		void Insert(Dictionary<string, Supplier> dictionarySupplier, string pathDB);
		void Update(Supplier supplier, string pathDB);
		Supplier GetSupplierByName(string name, string pathDB);
		Supplier GetSupplierByCode(string code, string pathDB);
        int GetSuppliersTotal(string pathDB);

		Dictionary<string, Supplier> GetSupplierDictionary(string pathDB, bool refill = false);
		void ClearSupplierDictionary();
		void AddSupplierInDictionary(string code, Supplier location);
		void RemoveSupplierFromDictionary(string code);
		bool IsExistSupplierInDictionary(string code);
		Supplier GetSupplierByCodeFromDictionary(string code);
		List<string> GetSupplierCodeList(string pathDB);
		void FillSupplierDictionary(string pathDB);
		void RepairCodeFromDB(string pathDB);
		void ReCountShilfSum(SelectParams selectParams, string pathDB);
	    bool IsAnyInDb(string pathDB);
	}
}

