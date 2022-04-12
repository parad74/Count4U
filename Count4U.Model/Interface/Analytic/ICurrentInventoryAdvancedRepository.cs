using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.Count4Mobile;
using System.Data.Entity;

namespace Count4U.Model.Interface.Count4Mobile
{
  
	public interface ICurrentInventoryAdvancedRepository
    {
		CurrentInventoryAdvanceds GetCurrentInventoryAdvanceds(string pathDB);
		IEnumerable<CurrentInventoryAdvanced> GetCurrentInventoryAdvancedEnumerable(string pathDB, bool refill = true);
		List<CurrentInventoryAdvanced> GetCurrentInventoryAdvancedList(string pathDB,
			bool refill = true,
			bool refillCatalogDictionary = false, 
			SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null,
			List<ImportDomainEnum> importType = null);
		//CurrentInventoryAdvanceds GetCurrentInventoryAdvanceds(int topCount, string pathDB);
		//CurrentInventoryAdvanceds GetCurrentInventoryAdvanceds(SelectParams selectParams, string pathDB);

		//void Delete(CurrentInventoryAdvanceds location, string pathDB);
		void DeleteAll(string pathDB);

		void Insert(CurrentInventoryAdvanced currentInventoryAdvanced, string pathDB);
		void Insert(CurrentInventoryAdvanceds currentInventoryAdvanceds, string pathDB);

		
	}
}
