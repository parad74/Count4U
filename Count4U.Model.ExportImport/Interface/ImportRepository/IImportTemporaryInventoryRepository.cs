using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface
{
	public interface IImportTemporaryInventoryRepository
	{
		void InsertTemporaryInventorys(string fromPathFile, string pathDB,
		   TemporaryInventorySQLiteParserEnum temporaryInventoryParserEnum,
		   Encoding encoding, string[] separators, int countExcludeFirstString,
		   List<ImportDomainEnum> importType,
		   Dictionary<ImportProviderParmEnum, object> parms = null);
		//IEnumerable<TemporaryInventory> GetTemporaryInventorys(string pathDB);
		void ClearTemporaryInventorys(string pathDB);
	}
}
