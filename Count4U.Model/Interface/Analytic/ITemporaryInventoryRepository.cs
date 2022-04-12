using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.Count4Mobile;
using System.Data.Entity;

namespace Count4U.Model.Interface.Count4Mobile
{

	public interface ITemporaryInventoryRepository
    {
		TemporaryInventorys GetTemporaryInventorys(string pathDB);
		void DeleteAll(string pathDB) ;
		void Insert(TemporaryInventory temporaryInventory, string pathDB);
		void Insert(TemporaryInventorys temporaryInventorys, string pathDB);
		Dictionary<string, TemporaryInventory> GetDictionaryTemporaryInventorys(string pathDB, string domain = "InventProduct", string operation = "DELETE");
		Dictionary<string, TemporaryInventory> GetDictionaryDeletedByNewUidTemporaryInventorys(string pathDB, string domain = "InventProduct");
		void FillKeysNewUidTemporaryInventorys(string pathDB, string domain = "InventProduct");
		List<TemporaryInventory> GetTemporaryInventorysInventProduct(string pathDB, string domain = "InventProduct", string operation = "DELETE");
		List<TemporaryInventory> GetListDeletedByNewUidTemporaryInventorys(string pathDB, string newID, List<TemporaryInventory> temporaryInventorysFromDB);
		void Update(TemporaryInventorys temporaryInventorys, string pathDB);
		void Update(TemporaryInventory temporaryInventory, string pathDB);
	}
}
