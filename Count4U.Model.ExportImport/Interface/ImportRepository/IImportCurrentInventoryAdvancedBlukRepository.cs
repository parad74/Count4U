using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using ErikEJ.SqlCe;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface
{
	public interface IImportCurrentInventoryAdvancedBlukRepository
	{
		void InsertCurrentInventoryAdvanced(string pathDB,
			CurrentInventoryAdvancedReaderEnum currentInventoryAdvancedReaderEnum, 
			bool refill = true, 
			bool refillCatalogDictionary = false,
			SelectParams selectParms = null,
			Dictionary<object, object> parmsIn = null, 
			//bool addResult = true, 
			List<ImportDomainEnum> importType = null, 
			List<string[]> ColumnMappings = null);
		void ClearCurrentInventoryAdvanced(string pathDB);
		void DropCurrentInventoryAdvanced(string pathDB);
		

		}
}
