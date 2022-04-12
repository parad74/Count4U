using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface
{
	public interface IImportPreviousInventorysRepository
	{
		void InsertPreviousInventorys(string fromPathFile, string pathDB,
		   PreviousInventorySQLiteParserEnum previousInventoryParserEnum,
		   Encoding encoding, string[] separators, int countExcludeFirstString,
		   List<ImportDomainEnum> importType,
		   Dictionary<ImportProviderParmEnum, object> parms = null);
		//IEnumerable<PreviousInventory> GetPreviousInventorys(string pathDB);
		void ClearPreviousInventorys(string pathDB);
		//void DropCurrentInventoryAdvanced(string pathDB);
	}
}
