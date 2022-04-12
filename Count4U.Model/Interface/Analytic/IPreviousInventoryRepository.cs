using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.Count4Mobile;
using System.Data.Entity;

namespace Count4U.Model.Interface.Count4Mobile
{

	public interface IPreviousInventoryRepository
    {
		PreviousInventorys GetPreviousInventorys(string pathDB) ;
       	void DeleteAll(string pathDB) ;
       	void Insert(PreviousInventory previousInventory, string pathDB);
		void Insert(PreviousInventorys previousInventorys, string pathDB);
		Dictionary<string, PreviousInventory> GetDictionaryPreviousInventorys(string pathDB);
		Dictionary<string, PreviousInventory> GetDictionaryPreviousInventorysByUid(string pathDB);
		List<PreviousInventory> GetListBySerialNumberLocal(string serialNumberLocal, string pathDB);
		List<PreviousInventory> GetListBySerialNumberSupplier(string serialNumberSupplier, string pathDB);
		List<PreviousInventory> GetListByItemCode(string itemCode, string pathDB);
		List<PreviousInventory> GetListBySerialNumberSupplierOrSerialNumberLocal(string serialNumberSupplier, string serialNumberLocal, string pathDB);
	}
}
